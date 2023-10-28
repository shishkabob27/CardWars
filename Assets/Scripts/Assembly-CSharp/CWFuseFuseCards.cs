using System.Collections.Generic;
using UnityEngine;

public class CWFuseFuseCards : MonoBehaviour
{
	public GameObject FuseCardPrefab;

	public GameObject LockedCardPrefab;

	public CWFuseShowRecipe showRecipeScript;

	public CardType panelType;

	private string selRecipeName;

	private string curSelRecipeName;

	public void OnEnable()
	{
		FusionManager.SetSort(FusionSortType.NEW);
		curSelRecipeName = null;
		Populate(null);
	}

	public void Populate(string defaultRecipe)
	{
		selRecipeName = defaultRecipe;
		FillTable();
	}

	public static GameObject AddLockedCard(GameObject parent, GameObject prefab, int curItemNum)
	{
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		SQUtils.SetLayer(gameObject, parent.layer);
		gameObject.name = string.Format("RecipeCard{0:D3}", curItemNum);
		gameObject.GetComponent<Collider>().enabled = true;
		UIPanel component = gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		return gameObject;
	}

	private void FillTable()
	{
		UIFastGrid component = base.gameObject.GetComponent<UIFastGrid>();
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		PlayerDeckManager deckManager = instance.DeckManager;
		FusionManager instance2 = FusionManager.Instance;
		deckManager.PrecacheCounts();
		List<object> list = new List<object>();
		List<KeyValuePair<CardForm, RecipeData>> sortedRecipeList = instance2.GetSortedRecipeList();
		QuestData lastClearedQuest = instance.GetLastClearedQuest("main");
		foreach (KeyValuePair<CardForm, RecipeData> item in sortedRecipeList)
		{
			if (item.Value.questUnlock <= 0)
			{
				list.Add(item);
				continue;
			}
			QuestData quest = QuestManager.Instance.GetQuest(item.Value.questUnlock);
			if (instance.GetQuestProgress(quest) > 0)
			{
				list.Add(item);
			}
		}
		if (selRecipeName == null && list.Count > 0)
		{
			int num = 0;
			if (list[num] == null)
			{
				num++;
			}
			selRecipeName = ((KeyValuePair<CardForm, RecipeData>)list[num]).Key.ID;
		}
		if (selRecipeName != null && selRecipeName != curSelRecipeName)
		{
			curSelRecipeName = selRecipeName;
			showRecipeScript.ShowRecipe(selRecipeName, false);
		}
		component.Initialize(list, pickPrefab, fillCard);
	}

	private GameObject pickPrefab(object data)
	{
		if (data == null)
		{
			return LockedCardPrefab;
		}
		return FuseCardPrefab;
	}

	private void fillCard(GameObject itemObj, object data)
	{
		FusionManager instance = FusionManager.Instance;
		if (data == null)
		{
			return;
		}
		CardForm key = ((KeyValuePair<CardForm, RecipeData>)data).Key;
		UITexture[] componentsInChildren = itemObj.GetComponentsInChildren<UITexture>(true);
		UITexture[] array = componentsInChildren;
		foreach (UITexture uITexture in array)
		{
			switch (uITexture.name)
			{
			case "Card_Glimmer":
				if (key != null && key.HasGlimmer())
				{
					uITexture.gameObject.SetActive(true);
				}
				else
				{
					uITexture.gameObject.SetActive(false);
				}
				break;
			}
		}
		Transform transform = itemObj.transform;
		SQUtils.SetIcon(transform, "Panel/Panel/Card_Art", key.IconAtlas, key.SpriteName);
		string value = null;
		switch (FusionManager.GetPrimarySort())
		{
		case FusionSortType.NEW:
		case FusionSortType.TYPE:
			value = KFFLocalization.Get("!!" + key.Type.ToString().ToUpper());
			break;
		case FusionSortType.CRAFT:
		case FusionSortType.ABC:
		case FusionSortType.SPELL:
		case FusionSortType.CRET:
		case FusionSortType.BLDG:
			value = key.Name;
			break;
		case FusionSortType.ATK:
			value = "ATK " + key.BaseATK;
			break;
		case FusionSortType.DEF:
			value = "DEF " + key.BaseDEF;
			break;
		}
		SQUtils.SetLabel(transform, "Panel/Labels/Type_Label", value);
		GameObject go = transform.Find("Panel/Labels").gameObject;
		GameObject go2 = transform.Find("Panel/Panel/New").gameObject;
		CWFuseRecipeClicked component = itemObj.GetComponent<CWFuseRecipeClicked>();
		component.recipeName = key.ID;
		component.showScript = showRecipeScript;
		if (!instance.HasBeenViewed(key))
		{
			NGUITools.SetActive(go, true);
			NGUITools.SetActive(go2, true);
		}
		else
		{
			NGUITools.SetActive(go, true);
			NGUITools.SetActive(go2, false);
		}
		SQUtils.SetGray(itemObj, (!instance.CanFuse(key)) ? 0.4f : 1f, null, "New");
	}
}
