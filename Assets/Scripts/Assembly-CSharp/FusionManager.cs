using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FusionManager : ILoadable
{
	public Dictionary<CardForm, RecipeData> recipes = new Dictionary<CardForm, RecipeData>();

	public HashSet<CardForm> Viewed = new HashSet<CardForm>();

	private static FusionSortType primarySort;

	private static FusionManager instance;

	public static FusionManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new FusionManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Fusion.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		recipes.Clear();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			RecipeData newRecipe = new RecipeData
			{
				cost = TFUtils.LoadInt(dict, "Cost", 0),
				questUnlock = TFUtils.LoadInt(dict, "Quest_Unlock", 0)
			};
			for (int ix = 0; ix < 3; ix++)
			{
				RecipeIngredientData ingredient = ReadIngredient(dict, ix + 1);
				if (ingredient != null)
				{
					newRecipe.ingredients.Add(ingredient);
				}
			}
			if (newRecipe.ingredients.Count > 0)
			{
				string keyName = TFUtils.LoadString(dict, "Card_ID", null);
				if (keyName != null && keyName.Length > 0)
				{
					CardForm key = CardDataManager.Instance.GetCard(keyName);
					if (key != null)
					{
						recipes.Add(key, newRecipe);
					}
				}
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public void Destroy()
	{
		instance = null;
		primarySort = FusionSortType.NEW;
	}

	private RecipeIngredientData ReadIngredient(Dictionary<string, object> dict, int num)
	{
		string text = "C" + num + "_";
		CardForm cardForm = null;
		string text2 = TFUtils.LoadString(dict, text + "Name", null);
		if (text2 != null && text2.Length > 0)
		{
			cardForm = CardDataManager.Instance.GetCard(text2);
		}
		if (cardForm == null)
		{
			return null;
		}
		RecipeIngredientData recipeIngredientData = new RecipeIngredientData();
		recipeIngredientData.Form = cardForm;
		recipeIngredientData.Count = TFUtils.LoadInt(dict, text + "Count", 1);
		return recipeIngredientData;
	}

	public RecipeData GetRecipe(CardForm form)
	{
		if (!recipes.ContainsKey(form))
		{
			return null;
		}
		return recipes[form];
	}

	public CardForm GetCardFormByQuestUnlock(string questID)
	{
		int num = int.Parse(questID);
		foreach (KeyValuePair<CardForm, RecipeData> recipe in Instance.recipes)
		{
			if (recipe.Value.questUnlock == num)
			{
				return recipe.Key;
			}
		}
		return null;
	}

	public bool CanFuse(CardForm form)
	{
		if (!form.CanFuse)
		{
			return false;
		}
		RecipeData recipe = GetRecipe(form);
		if (recipe == null || recipe.ingredients.Count == 0)
		{
			return false;
		}
		PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
		PlayerDeckManager deckManager = playerInfoScript.DeckManager;
		if (playerInfoScript.Coins < recipe.cost)
		{
			return false;
		}
		int num = 0;
		int num2 = 0;
		foreach (RecipeIngredientData ingredient in recipe.ingredients)
		{
			if (deckManager.CardCount(ingredient.Form) < ingredient.Count)
			{
				return false;
			}
			num += ingredient.Count;
			if (ingredient.Form.Type == CardType.Creature)
			{
				num2 += ingredient.Count;
			}
		}
		return true;
	}

	public bool MarkAsViewed(CardForm form)
	{
		if (Viewed.Contains(form))
		{
			return false;
		}
		Viewed.Add(form);
		return true;
	}

	public bool HasBeenViewed(CardForm form)
	{
		return Viewed.Contains(form);
	}

	public static FusionSortType GetPrimarySort()
	{
		return primarySort;
	}

	public static void SetSort(FusionSortType pri)
	{
		primarySort = pri;
	}

	public static int PDMCompare(KeyValuePair<CardForm, RecipeData> x, KeyValuePair<CardForm, RecipeData> y)
	{
		int num = 0;
		switch (primarySort)
		{
		case FusionSortType.NEW:
		case FusionSortType.SPELL:
		case FusionSortType.CRET:
		case FusionSortType.BLDG:
			return y.Value.questUnlock.CompareTo(x.Value.questUnlock);
		case FusionSortType.ABC:
			num = x.Key.Name.CompareTo(y.Key.Name);
			break;
		case FusionSortType.TYPE:
			num = x.Key.Type.CompareTo(y.Key.Type);
			break;
		case FusionSortType.ATK:
			num = y.Key.BaseATK.CompareTo(x.Key.BaseATK);
			break;
		case FusionSortType.DEF:
			num = y.Key.BaseDEF.CompareTo(x.Key.BaseDEF);
			break;
		case FusionSortType.CRAFT:
			if (Instance.CanFuse(y.Key) && !Instance.CanFuse(x.Key))
			{
				return 1;
			}
			if (!Instance.CanFuse(y.Key) && Instance.CanFuse(x.Key))
			{
				return -1;
			}
			return 0;
		}
		if (num == 0)
		{
			num = y.Value.questUnlock.CompareTo(x.Value.questUnlock);
		}
		return num;
	}

	public List<KeyValuePair<CardForm, RecipeData>> GetSortedRecipeList()
	{
		IEnumerable<KeyValuePair<CardForm, RecipeData>> enumerable = recipes;
		switch (primarySort)
		{
		case FusionSortType.SPELL:
			enumerable = recipes.Where((KeyValuePair<CardForm, RecipeData> p) => p.Key.Type == CardType.Spell);
			break;
		case FusionSortType.CRET:
			enumerable = recipes.Where((KeyValuePair<CardForm, RecipeData> p) => p.Key.Type == CardType.Creature);
			break;
		case FusionSortType.BLDG:
			enumerable = recipes.Where((KeyValuePair<CardForm, RecipeData> p) => p.Key.Type == CardType.Building);
			break;
		}
		List<KeyValuePair<CardForm, RecipeData>> list = new List<KeyValuePair<CardForm, RecipeData>>();
		foreach (KeyValuePair<CardForm, RecipeData> item in enumerable)
		{
			list.Add(item);
		}
		list.Sort(PDMCompare);
		if (primarySort == FusionSortType.NEW)
		{
			List<KeyValuePair<CardForm, RecipeData>> list2 = new List<KeyValuePair<CardForm, RecipeData>>();
			List<KeyValuePair<CardForm, RecipeData>> list3 = new List<KeyValuePair<CardForm, RecipeData>>();
			foreach (KeyValuePair<CardForm, RecipeData> item2 in list)
			{
				if (!HasBeenViewed(item2.Key))
				{
					list2.Add(item2);
				}
				else
				{
					list3.Add(item2);
				}
			}
			list = list2;
			list.AddRange(list3);
		}
		return list;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append("\"viewed\":[");
		bool flag = true;
		foreach (CardForm item in Viewed)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("\"" + item.ID + "\"");
		}
		stringBuilder.Append(']');
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public void InventoryFromDict(Dictionary<string, object> dict)
	{
		if (dict.ContainsKey("viewed"))
		{
			object[] array = (object[])dict["viewed"];
			object[] array2 = array;
			foreach (object obj in array2)
			{
				string id = (string)obj;
				CardForm card = CardDataManager.Instance.GetCard(id);
				MarkAsViewed(card);
			}
		}
	}
}
