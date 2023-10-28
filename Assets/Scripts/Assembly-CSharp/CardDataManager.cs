using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataManager : ILoadable
{
	private const string CreaturesFileName = "db_Creatures.json";

	private const string BuildingsFileName = "db_Buildings.json";

	private const string SpellsFileName = "db_Spells.json";

	private const string DweebsFileName = "db_Dweeb.json";

	private const string DefaultRewardCardID = "Creature_Pig";

	private static CardDataManager instance;

	private Dictionary<string, CardForm> Cards = new Dictionary<string, CardForm>();

	private Dictionary<string, CardForm>[] CardsByType = new Dictionary<string, CardForm>[4]
	{
		new Dictionary<string, CardForm>(),
		new Dictionary<string, CardForm>(),
		new Dictionary<string, CardForm>(),
		new Dictionary<string, CardForm>()
	};

	public bool Loaded;

	public static CardDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CardDataManager();
			}
			return instance;
		}
	}

	private void FillCardData(Dictionary<string, object> dict, CardForm form)
	{
		form.ID = TFUtils.LoadString(dict, "ID");
		form.Name = TFUtils.LoadLocalizedString(dict, "Name");
		form.RawDescription = TFUtils.LoadLocalizedString(dict, "Desc");
		form.BaseVal1 = TFUtils.LoadInt(dict, "val1", 0);
		form.BaseVal2 = TFUtils.LoadInt(dict, "val2", 0);
		form.BaseSalePrice = TFUtils.LoadInt(dict, "BaseSalePrice", 0);
		form.IconAtlas = TFUtils.LoadString(dict, "IconAtlas");
		form.FrameAtlas = TFUtils.LoadString(dict, "FrameAtlas");
		form.SpriteName = TFUtils.LoadString(dict, "SpriteName");
		form.FrameSpriteName = TFUtils.LoadString(dict, "FrameSpriteName");
		form.ScriptName = TFUtils.LoadString(dict, "ScriptName");
		form.ScriptVizName = TFUtils.LoadString(dict, "VizOverride", string.Empty).Trim();
		if (form.ScriptVizName.Length <= 0)
		{
			form.ScriptVizName = form.ScriptName;
		}
		try
		{
			form.AbilityType = (AbilityType)(int)Enum.Parse(typeof(AbilityType), TFUtils.LoadString(dict, "AbilityType"), true);
		}
		catch
		{
			form.AbilityType = AbilityType.None;
		}
		form.Faction = (Faction)(int)Enum.Parse(typeof(Faction), TFUtils.LoadString(dict, "Faction"), true);
		try
		{
			form.Quality = (Quality)(int)Enum.Parse(typeof(Quality), TFUtils.LoadString(dict, "Quality"), true);
		}
		catch
		{
			form.Quality = Quality.Standard;
		}
		form.Rarity = TFUtils.LoadInt(dict, "Rarity");
		form.CanFuse = TFUtils.LoadBool(dict, "CanFuse", true);
		form.Cost = TFUtils.LoadInt(dict, "Cost");
		form.FloopCost = TFUtils.LoadInt(dict, "FloopCost", 0);
		form.CostDescription = KFFLocalization.Get("!!CARD_FLOOPCOST").Replace("<cost>", form.FloopCost.ToString());
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data4 = SQUtils.ReadJSONData("db_Creatures.json");
		Dictionary<string, object>[] array = data4;
		foreach (Dictionary<string, object> dict in array)
		{
			CreatureCard CurrentCard = new CreatureCard();
			FillCardData(dict, CurrentCard);
			CurrentCard.ObjectName = TFUtils.LoadString(dict, "ObjectName");
			CurrentCard.BaseATK = TFUtils.LoadInt(dict, "ATK");
			CurrentCard.BaseDEF = TFUtils.LoadInt(dict, "DEF");
			try
			{
				CurrentCard.ShortHand = TFUtils.LoadLocalizedString(dict, "ShortHandName");
			}
			catch (KeyNotFoundException)
			{
				CurrentCard.ShortHand = CurrentCard.Name;
			}
			Cards.Add(CurrentCard.ID, CurrentCard);
			CardsByType[0].Add(CurrentCard.ID, CurrentCard);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		data4 = SQUtils.ReadJSONData("db_Buildings.json");
		Dictionary<string, object>[] array2 = data4;
		foreach (Dictionary<string, object> dict2 in array2)
		{
			BuildingCard CurrentCard2 = new BuildingCard();
			FillCardData(dict2, CurrentCard2);
			CurrentCard2.ObjectName = TFUtils.LoadString(dict2, "ObjectName");
			Cards.Add(CurrentCard2.ID, CurrentCard2);
			CardsByType[1].Add(CurrentCard2.ID, CurrentCard2);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		data4 = SQUtils.ReadJSONData("db_Spells.json");
		Dictionary<string, object>[] array3 = data4;
		foreach (Dictionary<string, object> dict3 in array3)
		{
			SpellCard CurrentCard3 = new SpellCard();
			FillCardData(dict3, CurrentCard3);
			CurrentCard3.ParticleName = TFUtils.LoadString(dict3, "Particles");
			Cards.Add(CurrentCard3.ID, CurrentCard3);
			CardsByType[2].Add(CurrentCard3.ID, CurrentCard3);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		data4 = SQUtils.ReadJSONData("db_Dweeb.json");
		Dictionary<string, object>[] array4 = data4;
		foreach (Dictionary<string, object> dict4 in array4)
		{
			DweebCard CurrentCard4 = new DweebCard
			{
				ID = TFUtils.LoadString(dict4, "ID"),
				Name = TFUtils.LoadLocalizedString(dict4, "Name"),
				RawDescription = TFUtils.LoadLocalizedString(dict4, "Desc"),
				SpriteName = TFUtils.LoadString(dict4, "SpriteName")
			};
			CardsByType[3].Add(CurrentCard4.ID, CurrentCard4);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Loaded = true;
	}

	private void VerifyCards()
	{
		foreach (KeyValuePair<string, CardForm> card in Cards)
		{
			switch (card.Value.Type)
			{
			case CardType.Creature:
			{
				GameObject gameObject = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Summons/" + card.Value.ObjectName) as GameObject;
				if (!(gameObject == null))
				{
				}
				break;
			}
			case CardType.Building:
			{
				GameObject gameObject = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Building/" + card.Value.ObjectName) as GameObject;
				if (!(gameObject == null))
				{
				}
				break;
			}
			}
		}
	}

	public void Destroy()
	{
		instance = null;
	}

	public CardForm GetCard(CardType type, string id)
	{
		return CardsByType[(int)type][id];
	}

	public CardForm GetCard(string id, bool backwards_compatibility = true)
	{
		CardForm value;
		if (Cards.TryGetValue(id, out value))
		{
			return value;
		}
		if (backwards_compatibility)
		{
			foreach (KeyValuePair<string, CardForm> card in Cards)
			{
				CardForm value2 = card.Value;
				if (value2.Name == id)
				{
					return value2;
				}
			}
			if (Cards.TryGetValue("Creature_Pig", out value))
			{
				return value;
			}
		}
		return null;
	}

	public List<CardForm> GetCards(CardType type)
	{
		Dictionary<string, CardForm> dictionary = CardsByType[(int)type];
		return new List<CardForm>(dictionary.Values);
	}

	public List<CardForm> GetCards()
	{
		return new List<CardForm>(Cards.Values);
	}
}
