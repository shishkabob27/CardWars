#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class AIDeckManager : ILoadable
{
	private const string DecksFileName = "db_Decks.json";

	private static AIDeckManager instance;

	private Dictionary<string, Deck> Decks = new Dictionary<string, Deck>();

	public bool Loaded;

	public static AIDeckManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AIDeckManager();
			}
			return instance;
		}
	}

	public Dictionary<string, object>[] LoadDeckData()
	{
		string text = Path.Combine("Blueprints", "db_Decks.json");
		TFUtils.DebugLog("CardDataScript DeckData path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		Deck CurrentDeck = null;
		while (!CardDataManager.Instance.Loaded)
		{
			yield return null;
		}
		Dictionary<string, object>[] array = LoadDeckData();
		foreach (Dictionary<string, object> dict in array)
		{
			string DeckID = TFUtils.LoadString(dict, "DeckID");
			if (DeckID.Length > 0)
			{
				CurrentDeck = new Deck
				{
					Name = DeckID
				};
				if (!Decks.ContainsKey(DeckID))
				{
					Decks.Add(DeckID, CurrentDeck);
				}
			}
			if (CurrentDeck != null)
			{
				if (CurrentDeck.GetLandscapeCount() < 4)
				{
					LandscapeType i = (LandscapeType)(int)Enum.Parse(typeof(LandscapeType), TFUtils.LoadString(dict, "Landscapes"), true);
					CurrentDeck.AddLandscape(i);
				}
				string Name = TFUtils.LoadString(dict, "Cards");
				CardForm Form = CardDataManager.Instance.GetCard(Name);
				if (Form == null)
				{
					string replacementCard = ParametersManager.Instance.AIDeck_Bad_Card_Swap;
					Name = replacementCard;
					Form = CardDataManager.Instance.GetCard(Name);
				}
				int Level = TFUtils.LoadInt(dict, "Level", 1);
				float DropRate = TFUtils.LoadFloat(dict, "DropChance", 0f);
				float CoinRate = TFUtils.LoadFloat(dict, "CoinChance", 0f);
				int DropLevel = TFUtils.LoadInt(dict, "DropLevel", 1);
				int CoinReward = TFUtils.LoadInt(dict, "CoinReward", 0);
				int StaticDrop = TFUtils.LoadInt(dict, "StaticWeight", 0);
				TFUtils.Assert(Form != null, "Can't find card \"" + Name + "\"");
				CurrentDeck.AddCard(new CardItem(Form, Level)
				{
					DropRate = DropRate,
					DropLevel = DropLevel,
					CoinDropRate = CoinRate,
					CoinReward = CoinReward,
					StaticDropWeight = StaticDrop
				});
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Loaded = true;
	}

	public void Destroy()
	{
		instance = null;
	}

	public Dictionary<string, Deck>.KeyCollection GetDeckNames()
	{
		return Decks.Keys;
	}

	public Deck GetDeck(string ID)
	{
		Deck result = null;
		try
		{
			switch (Deck.DeckTypeFromID(ID))
			{
			case DeckType.NORMAL:
				result = Decks[ID];
				break;
			case DeckType.PLAYER_CLONE_DECK:
			{
				PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
				result = playerInfoScript.GetSelectedDeck();
				break;
			}
			}
		}
		catch
		{
		}
		return result;
	}

	public Deck GetMPDeck(string[] aLandscapes, string[] aCards, string aLeader, int aRank)
	{
		Deck deck = new Deck();
		deck.Name = "MP_Deck";
		deck.Leader = LeaderManager.Instance.CreateLeader(aLeader, aRank);
		int num = aCards.Length;
		for (int i = 0; i < num; i++)
		{
			CardForm card = CardDataManager.Instance.GetCard(aCards[i]);
			CardItem newCard = new CardItem(card);
			deck.AddCard(newCard);
		}
		num = aLandscapes.Length;
		for (int j = 0; j < num; j++)
		{
			LandscapeType l = (LandscapeType)(int)Enum.Parse(typeof(LandscapeType), aLandscapes[j]);
			deck.AddLandscape(l);
		}
		return deck;
	}

	public Deck GetDeckCopy(string ID)
	{
		Deck deck = GetDeck(ID);
		if (deck != null)
		{
			return deck.Clone();
		}
		return null;
	}
}
