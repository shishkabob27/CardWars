using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Deck
{
	public const string SPECIAL_OPP_DECK_PREFIX = "_";

	private List<CardItem> Cards = new List<CardItem>();

	private LandscapeType[] Landscapes;

	private int LandscapeCount;

	private int PlayersLeaderIndex;

	private int CreatureCount;

	private int SpellCount;

	private int BuildingCount;

	public string Name { get; set; }

	public LeaderItem Leader { get; set; }

	public Deck()
	{
		LandscapeCount = 0;
		Landscapes = new LandscapeType[4];
		for (int i = 0; i < 4; i++)
		{
			Landscapes[i] = LandscapeType.None;
		}
		ClearLeader();
	}

	public Deck Clone()
	{
		Deck deck = new Deck();
		deck.Name = Name;
		deck.Leader = Leader;
		int num = CardCount();
		for (int i = 0; i < num; i++)
		{
			deck.AddCard(GetCard(i));
		}
		num = GetLandscapeCount();
		for (int j = 0; j < num; j++)
		{
			deck.AddLandscape(GetLandscape(j));
		}
		return deck;
	}

	public void SetLeaderForPlayer(int leaderIndex)
	{
		if (leaderIndex < 0 || leaderIndex >= LeaderManager.Instance.leaders.Count)
		{
			leaderIndex = 0;
		}
		PlayersLeaderIndex = leaderIndex;
		Leader = LeaderManager.Instance.leaders[leaderIndex];
	}

	public void SetLeaderForPlayer(LeaderItem leader)
	{
		Leader = leader;
		PlayersLeaderIndex = LeaderManager.Instance.leaders.IndexOf(leader);
	}

	public void SetLeaderForPlayer(string leaderId)
	{
		LeaderItem leaderItem = LeaderManager.Instance.leaders.Find((LeaderItem x) => string.Equals(x.Form.ID, leaderId, StringComparison.InvariantCultureIgnoreCase));
		if (leaderItem != null)
		{
			SetLeaderForPlayer(leaderItem);
		}
	}

	public int GetLeaderIndex()
	{
		return PlayersLeaderIndex;
	}

	public void ClearLeader()
	{
		PlayersLeaderIndex = -1;
		Leader = null;
	}

	public void AddLandscape(LandscapeType l)
	{
		Landscapes[LandscapeCount] = l;
		LandscapeCount++;
	}

	public void SetLandscape(int idx, LandscapeType l)
	{
		Landscapes[idx] = l;
	}

	public LandscapeType GetLandscape(int idx)
	{
		return Landscapes[idx];
	}

	public int GetLandscapeCount()
	{
		return LandscapeCount;
	}

	public int GetUniqueLandscapeCount()
	{
		bool[] array = new bool[Enum.GetNames(typeof(LandscapeType)).Length];
		for (int i = 0; i < 4; i++)
		{
			array[(int)Landscapes[i]] = true;
		}
		int num = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j])
			{
				num++;
			}
		}
		return num;
	}

	public int SumOfAllLevels()
	{
		int num = 0;
		foreach (CardItem card in Cards)
		{
			num += card.Level;
		}
		return num;
	}

	public void Sort(Comparison<CardItem> comparison)
	{
		Cards.Sort(comparison);
	}

	public void AddCard(CardItem NewCard)
	{
		Cards.Add(NewCard);
	}

	public void RemoveCard(CardItem card)
	{
		Cards.Remove(card);
	}

	public int CardCount()
	{
		return Cards.Count;
	}

	public int GetCreatureCount()
	{
		return CreatureCount;
	}

	public int GetSpellCount()
	{
		return SpellCount;
	}

	public int GetBuildingCount()
	{
		return BuildingCount;
	}

	public void PrecacheCounts()
	{
		CreatureCount = Cards.Count((CardItem c) => c.Form.Type == CardType.Creature);
		SpellCount = Cards.Count((CardItem c) => c.Form.Type == CardType.Spell);
		BuildingCount = Cards.Count((CardItem c) => c.Form.Type == CardType.Building);
	}

	public string ExceedsMaxDuplicates(int maxDups)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (CardItem card in Cards)
		{
			try
			{
				int num = dictionary[card.Form.ID];
				if (num >= maxDups)
				{
					return card.Form.Name;
				}
				dictionary[card.Form.ID] = num + 1;
			}
			catch (KeyNotFoundException)
			{
				dictionary.Add(card.Form.ID, 1);
			}
		}
		return null;
	}

	public List<CardItem> GetCards()
	{
		return Cards;
	}

	public CardItem GetCard(int idx)
	{
		return Cards[idx];
	}

	public void Empty()
	{
		Cards.Clear();
	}

	public bool IsEmpty()
	{
		return Cards.Count <= 0;
	}

	public void Shuffle()
	{
		for (int num = Cards.Count * 7; num > 0; num--)
		{
			int index = UnityEngine.Random.Range(0, Cards.Count);
			int index2 = UnityEngine.Random.Range(0, Cards.Count);
			CardItem value = Cards[index];
			Cards[index] = Cards[index2];
			Cards[index2] = value;
		}
	}

	public void ShuffleLandscapes()
	{
		for (int num = LandscapeCount * 7; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, LandscapeCount);
			int num3 = UnityEngine.Random.Range(0, LandscapeCount);
			LandscapeType landscapeType = Landscapes[num2];
			Landscapes[num2] = Landscapes[num3];
			Landscapes[num3] = landscapeType;
		}
	}

	public CardItem DealCard()
	{
		CardItem result = null;
		if (Cards.Count > 0)
		{
			result = Cards[0];
			Cards.RemoveAt(0);
		}
		return result;
	}

	public void PlaceCard(CardItem item)
	{
		Cards.Insert(0, item);
	}

	public CardItem GetWeightedCard()
	{
		WeightedList<CardItem> weightedList = new WeightedList<CardItem>();
		foreach (CardItem card in Cards)
		{
			weightedList.Add(card, card.StaticDropWeight);
		}
		return weightedList.RandomItem();
	}

	public string MPDeckSerialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append("\"cards\":[");
		bool flag = true;
		foreach (CardItem card in Cards)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("\"" + card.Form.ID + "\"");
		}
		stringBuilder.Append(']');
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public string MPLandscapeSerialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append("\"landscape\":[");
		bool flag = true;
		for (int i = 0; i < LandscapeCount; i++)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append(string.Concat("\"", Landscapes[i], "\""));
		}
		stringBuilder.Append("],");
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public static DeckType DeckTypeFromID(string ID)
	{
		DeckType result = DeckType.NORMAL;
		if (ID.StartsWith("_"))
		{
			string value = ID.Substring("_".Length);
			try
			{
				DeckType deckType = (DeckType)(int)Enum.Parse(typeof(DeckType), value);
				DeckType deckType2 = deckType;
				if (deckType2 == DeckType.PLAYER_CLONE_DECK)
				{
					result = DeckType.PLAYER_CLONE_DECK;
				}
				else
				{
					result = DeckType.UNKNOWN;
					TFUtils.WarnLog("Unknown special deck: '" + ID + "'");
				}
			}
			catch
			{
			}
		}
		return result;
	}
}
