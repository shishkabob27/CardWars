using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer;

public class PlayerDeckManager
{
	public const int MAX_NUM_DECKS = 5;

	public List<CardItem> Inventory = new List<CardItem>();

	public List<Deck> Decks = new List<Deck>();

	private static SortType primarySort;

	private static SortType secondarySort;

	private int numCreatures;

	private Dictionary<CardForm, int> cardCounts = new Dictionary<CardForm, int>();

	private bool CardsRemoved;

	public bool IsCardsRemoved()
	{
		return CardsRemoved;
	}

	public void ResetCardsRemoved()
	{
		CardsRemoved = false;
	}

	public void InitializeNewPlayer()
	{
		AIDeckManager instance = AIDeckManager.Instance;
		Dictionary<string, Deck>.KeyCollection deckNames = instance.GetDeckNames();
		foreach (string item in deckNames)
		{
			if (item.StartsWith("player", StringComparison.InvariantCultureIgnoreCase))
			{
				Deck deck = instance.GetDeck(item);
				Deck deck2 = new Deck();
				int landscapeCount = deck.GetLandscapeCount();
				for (int i = 0; i < landscapeCount; i++)
				{
					deck2.AddLandscape(deck.GetLandscape(i));
				}
				landscapeCount = deck.CardCount();
				for (int j = 0; j < landscapeCount; j++)
				{
					CardItem card = deck.GetCard(j);
					CardItem cardItem = new CardItem(card.Form, card.Level, false);
					AddCard(cardItem);
					deck2.AddCard(cardItem);
				}
				Decks.Add(deck2);
			}
		}
		FillInEmptyDecks();
	}

	private void FillInEmptyDecks()
	{
		for (int i = Decks.Count; i < 5; i++)
		{
			Deck deck = new Deck();
			for (int j = 0; j < 4; j++)
			{
				deck.AddLandscape(LandscapeType.None);
			}
			Decks.Add(deck);
		}
	}

	public static SortType GetPrimarySort()
	{
		return primarySort;
	}

	public static SortType GetSecondarySort()
	{
		return secondarySort;
	}

	public static void SetSort(SortType pri, SortType sec)
	{
		primarySort = pri;
		secondarySort = sec;
	}

	public static void ResetSort()
	{
		SetSort(SortType.NONE, SortType.NONE);
	}

	public static int PDMCompare(CardItem x, CardItem y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		switch (primarySort)
		{
		case SortType.ATK:
			if (y.ATK < x.ATK)
			{
				return -1;
			}
			if (y.ATK > x.ATK)
			{
				return 1;
			}
			break;
		case SortType.DEF:
			if (y.DEF < x.DEF)
			{
				return -1;
			}
			if (y.DEF > x.DEF)
			{
				return 1;
			}
			break;
		case SortType.TYPE:
			if (x.Form.Type < y.Form.Type)
			{
				return -1;
			}
			if (x.Form.Type > y.Form.Type)
			{
				return 1;
			}
			break;
		case SortType.NAME:
		{
			int num = x.Form.Name.CompareTo(y.Form.Name);
			if (num != 0)
			{
				return num;
			}
			break;
		}
		case SortType.FACT:
			if (x.Form.Faction < y.Form.Faction)
			{
				return -1;
			}
			if (x.Form.Faction > y.Form.Faction)
			{
				return 1;
			}
			break;
		case SortType.RARE:
			if (y.Form.Rarity < x.Form.Rarity)
			{
				return -1;
			}
			if (y.Form.Rarity > x.Form.Rarity)
			{
				return 1;
			}
			break;
		case SortType.MP:
			if (y.Form.Cost < x.Form.Cost)
			{
				return -1;
			}
			if (y.Form.Cost > x.Form.Cost)
			{
				return 1;
			}
			break;
		}
		switch (secondarySort)
		{
		case SortType.ATK:
			return y.ATK - x.ATK;
		case SortType.DEF:
			return y.DEF - x.DEF;
		case SortType.TYPE:
			return x.Form.Type - y.Form.Type;
		case SortType.NAME:
			return x.Form.Name.CompareTo(y.Form.Name);
		case SortType.FACT:
			return x.Form.Faction - y.Form.Faction;
		case SortType.RARE:
			return y.Form.Rarity - x.Form.Rarity;
		case SortType.MP:
			return y.Form.Cost - x.Form.Cost;
		default:
			return 0;
		}
	}

	public List<CardItem> GetSortedInventory()
	{
		List<CardItem> list = new List<CardItem>(Inventory);
		list.Sort(PDMCompare);
		return list;
	}

	public Deck GetSortedDeck(int ID)
	{
		Deck deckCopy = GetDeckCopy(ID);
		if (deckCopy == null)
		{
			return null;
		}
		deckCopy.Sort(PDMCompare);
		return deckCopy;
	}

	public Deck GetDeck(int ID)
	{
		if (ID < 0 || ID >= Decks.Count)
		{
			return null;
		}
		return Decks[ID];
	}

	public void SetDeck(int ID, Deck newDeck)
	{
		if (ID >= 0 && ID < Decks.Count)
		{
			Decks[ID] = newDeck;
		}
	}

	public int GetHighestLeaderRank()
	{
		int num = 0;
		foreach (Deck deck in Decks)
		{
			if (deck.Leader.Rank > num)
			{
				num = deck.Leader.Rank;
			}
		}
		return num;
	}

	public int GetNumValidDecks()
	{
		int num = 0;
		for (int i = 0; i < Decks.Count; i++)
		{
			if (Decks[i].CardCount() > ParametersManager.Instance.Min_Cards_In_Deck)
			{
				num++;
			}
		}
		return num;
	}

	public Deck GetDeckCopy(int ID)
	{
		if (ID < 0 || ID >= Decks.Count)
		{
			return null;
		}
		return Decks[ID].Clone();
	}

	public bool RemoveCard(string cardID)
	{
		CardItem card = GetCard(cardID);
		if (card != null)
		{
			RemoveCard(card);
			return true;
		}
		return false;
	}

	public void RemoveCard(CardItem card)
	{
		CardsRemoved = true;
		Inventory.Remove(card);
		foreach (Deck deck in Decks)
		{
			deck.RemoveCard(card);
		}
	}

	public void RemoveCards(CardForm form, int count)
	{
		CardsRemoved = true;
		List<CardItem> list = new List<CardItem>();
		foreach (CardItem item in Inventory)
		{
			if (list.Count >= count)
			{
				break;
			}
			if (item.Form == form)
			{
				list.Add(item);
			}
		}
		foreach (CardItem item2 in list)
		{
			RemoveCard(item2);
		}
	}

	public void AddCards(IEnumerable<CardItem> cards)
	{
		Inventory.AddRange(cards);
	}

	public void AddCard(CardItem card)
	{
		Inventory.Add(card);
	}

	public CardItem AddCardAward(string cardID)
	{
		switch (cardID)
		{
		case "normal":
			cardID = GachaManager.Instance.PickColumn("DailyGiftStandard");
			break;
		case "gold":
			cardID = GachaManager.Instance.PickColumn("DailyGiftGold");
			break;
		case "obsidian":
			cardID = GachaManager.Instance.PickColumn("DailyGiftObsidian");
			break;
		case "halloween":
			cardID = GachaManager.Instance.PickColumn("DailyGiftHalloween");
			break;
		}
		CardForm card = CardDataManager.Instance.GetCard(cardID);
		if (card == null)
		{
			return null;
		}
		CardItem cardItem = new CardItem(card);
		AddCard(cardItem);
		return cardItem;
	}

	public bool HasCard(string cardID)
	{
		return Inventory.Exists((CardItem c) => c.Form.ID == cardID);
	}

	public bool HasCard(CardType t)
	{
		return Inventory.Exists((CardItem c) => c.Form.Type == t);
	}

	public CardItem GetCard(string cardID)
	{
		return Inventory.Find((CardItem c) => c.Form.ID == cardID);
	}

	public void DetermineMembership()
	{
		foreach (CardItem item in Inventory)
		{
			item.membership = null;
		}
		int num = 0;
		foreach (Deck deck in Decks)
		{
			int num2 = deck.CardCount();
			for (int i = 0; i < num2; i++)
			{
				CardItem card = deck.GetCard(i);
				if (card.membership == null)
				{
					card.membership = new List<int>();
				}
				card.membership.Add(num);
			}
			num++;
		}
	}

	public int CardCount(CardForm form)
	{
		if (!cardCounts.ContainsKey(form))
		{
			return 0;
		}
		return cardCounts[form];
	}

	public int CardCount()
	{
		return Inventory.Count;
	}

	public int CreatureCount()
	{
		return numCreatures;
	}

	public void PrecacheCounts()
	{
		numCreatures = Inventory.Count((CardItem c) => c.Form.Type == CardType.Creature);
		foreach (Deck deck in Decks)
		{
			deck.PrecacheCounts();
		}
		cardCounts.Clear();
		foreach (CardItem item in Inventory)
		{
			if (cardCounts.ContainsKey(item.Form))
			{
				cardCounts[item.Form] = cardCounts[item.Form] + 1;
			}
			else
			{
				cardCounts.Add(item.Form, 1);
			}
		}
	}

	public string CheckValidity()
	{
		PrecacheCounts();
		int num = 0;
		foreach (Deck deck in Decks)
		{
			num++;
			LeaderItem leader = deck.Leader;
			if (deck.CardCount() != 0)
			{
				if (deck.CardCount() < ParametersManager.Instance.Min_Cards_In_Deck)
				{
					return string.Format(KFFLocalization.Get("!!FORMAT_DECK_CONTAINS_LESS_THAN"), num, ParametersManager.Instance.Min_Cards_In_Deck);
				}
				if (deck.CardCount() > leader.RankValues.DeckMaxSize)
				{
					return string.Format(KFFLocalization.Get("!!FORMAT_DECK_CAN_ONLY_HOLD"), num, leader.Form.Name, leader.RankValues.DeckMaxSize);
				}
				string text = deck.ExceedsMaxDuplicates(ParametersManager.Instance.Max_Duplicates_In_Deck);
				if (text != null)
				{
					return string.Format(KFFLocalization.Get("!!FORMAT_DECK_CONTAINS_MORE_THAN"), num, ParametersManager.Instance.Max_Duplicates_In_Deck, text);
				}
			}
		}
		return null;
	}

	public void DebugAddAllCards()
	{
		List<CardForm> cards = CardDataManager.Instance.GetCards();
		foreach (CardForm item in cards)
		{
			CardItem card = new CardItem(item);
			AddCard(card);
		}
		PlayerInfoScript.GetInstance().MaxInventory = Inventory.Count + 25;
	}

	private string SerializeDeck(int ix)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Deck deck = Decks[ix];
		stringBuilder.Append("[");
		bool flag = true;
		int landscapeCount = deck.GetLandscapeCount();
		for (int i = 0; i < landscapeCount; i++)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append("\"" + deck.GetLandscape(i).ToString() + "\"");
			flag = false;
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	private string SerializeCard(CardItem card)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("form", card.Form.ID) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("level", card.Level) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("isNew", card.IsNew) + ",");
		stringBuilder.Append("\"membership\":[");
		bool flag = true;
		if (card.membership != null)
		{
			foreach (int item in card.membership)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				flag = false;
				stringBuilder.Append(item.ToString());
			}
		}
		stringBuilder.Append("]");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public string Serialize()
	{
		DetermineMembership();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append("\"decks\":[");
		bool flag = true;
		for (int i = 0; i < Decks.Count; i++)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append(SerializeDeck(i));
		}
		stringBuilder.Append("],");
		stringBuilder.Append("\"cards\":[");
		flag = true;
		foreach (CardItem item in Inventory)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append(SerializeCard(item));
		}
		stringBuilder.Append(']');
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public void InventoryFromDict(Dictionary<string, object> dict)
	{
		if (!dict.ContainsKey("decks") || !dict.ContainsKey("cards"))
		{
			return;
		}
		Decks.Clear();
		Inventory.Clear();
		object[] array = (object[])dict["decks"];
		object[] array2 = array;
		foreach (object obj in array2)
		{
			string[] array3 = (string[])obj;
			Deck deck = new Deck();
			string[] array4 = array3;
			foreach (string value in array4)
			{
				LandscapeType l = (LandscapeType)(int)Enum.Parse(typeof(LandscapeType), value);
				deck.AddLandscape(l);
			}
			Decks.Add(deck);
		}
		FillInEmptyDecks();
		object[] array5 = (object[])dict["cards"];
		object[] array6 = array5;
		foreach (object obj2 in array6)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)obj2;
			string id = (string)dictionary["form"];
			int num = (int)dictionary["level"];
			if (num > 1)
			{
				num = 1;
			}
			bool flag = false;
			try
			{
				flag = (int)dictionary["isNew"] != 0;
			}
			catch
			{
				flag = false;
			}
			CardForm card = CardDataManager.Instance.GetCard(id);
			if (card == null)
			{
				continue;
			}
			CardItem cardItem = new CardItem(card, num, flag);
			Inventory.Add(cardItem);
			try
			{
				int[] array7 = (int[])dictionary["membership"];
				int[] array8 = array7;
				foreach (int index in array8)
				{
					Decks[index].AddCard(cardItem);
				}
			}
			catch (InvalidCastException)
			{
			}
		}
	}

	public void UpdateMPDeck(SuccessCallback callback)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck deck = instance.DeckManager.Decks[instance.SelectedMPDeck];
		LeaderItem leader = deck.Leader;
		LeaderItem leaderItem = LeaderManager.Instance.GetLeader(instance.MPDeckLeaderID);
		if (leaderItem == null)
		{
			leaderItem = leader;
		}
		deck.Leader = leaderItem;
		string deck2 = deck.MPDeckSerialize();
		string landscapes = deck.MPLandscapeSerialize();
		int num = deck.SumOfAllLevels();
		deck.Leader = leader;
		global::Multiplayer.Multiplayer.UpdateDeck(SessionManager.GetInstance().theSession, deck2, num, landscapes, leaderItem.Form.ID, leaderItem.Rank, callback);
	}
}
