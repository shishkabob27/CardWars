using System.Collections.Generic;
using UnityEngine;

public class CWDeckInventoryController : MonoBehaviour
{
	private const int MAX_DECKS = 5;

	private static CWDeckInventoryController g_deckController;

	public CWDeckInventory Table;

	public GameObject labelPanel;

	public UITweener creaturesTweener;

	public UITweener spellsTweener;

	public UITweener buildingsTweener;

	public UITweener heroesTweener;

	private bool initialized;

	public CardType currentTable;

	public int currentDeck
	{
		get
		{
			return PlayerInfoScript.GetInstance().SelectedDeck;
		}
		set
		{
			PlayerInfoScript.GetInstance().SelectedDeck = value;
		}
	}

	private void Awake()
	{
		if (g_deckController == null)
		{
			g_deckController = this;
		}
	}

	private void OnEnable()
	{
		if (creaturesTweener == null)
		{
			creaturesTweener = FindTweener("1_Creature");
		}
		if (spellsTweener == null)
		{
			spellsTweener = FindTweener("2_Spell");
		}
		if (buildingsTweener == null)
		{
			buildingsTweener = FindTweener("3_Building");
		}
		if (heroesTweener == null)
		{
			heroesTweener = FindTweener("5_Hero");
		}
		UpdateUI();
	}

	private UITweener FindTweener(string name)
	{
		Transform transform = base.transform.Find("TopTabs/Tabs/UIGrid/" + name + "/ScaleRoot");
		UITweener uITweener = null;
		if (transform != null)
		{
			uITweener = transform.gameObject.GetComponent(typeof(UITweener)) as UITweener;
			if (uITweener == null)
			{
				TweenScale tweenScale = transform.gameObject.AddComponent(typeof(TweenScale)) as TweenScale;
				if (tweenScale != null)
				{
					tweenScale.style = UITweener.Style.PingPong;
					tweenScale.duration = 0.25f;
					tweenScale.tweenGroup = 123;
					tweenScale.from = Vector3.one;
					tweenScale.to = new Vector3(1.1f, 1.1f, 1.1f);
					uITweener = tweenScale;
				}
			}
		}
		return uITweener;
	}

	public static CWDeckInventoryController GetInstance()
	{
		return g_deckController;
	}

	public void UpdateUI()
	{
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		List<CardItem> sortedInventory = deckManager.GetSortedInventory();
		if (Table != null)
		{
			Table.OnEnable();
		}
		if (labelPanel != null)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (CardItem item in sortedInventory)
			{
				if (item.Form.Type == CardType.Creature)
				{
					num++;
				}
				else if (item.Form.Type == CardType.Building)
				{
					num2++;
				}
				else if (item.Form.Type == CardType.Spell)
				{
					num3++;
				}
			}
			int count = LeaderManager.Instance.leaders.Count;
			UILabel[] componentsInChildren = labelPanel.GetComponentsInChildren<UILabel>();
			UILabel[] array = componentsInChildren;
			foreach (UILabel uILabel in array)
			{
				if (uILabel.name == "Building_Count")
				{
					uILabel.text = num2.ToString();
				}
				if (uILabel.name == "Creature_Count")
				{
					uILabel.text = num.ToString();
				}
				if (uILabel.name == "Spell_Count")
				{
					uILabel.text = num3.ToString();
				}
				if (uILabel.name == "Hero_Count")
				{
					uILabel.text = count.ToString();
				}
				if (uILabel.name == "Card_Count")
				{
					uILabel.text = (num3 + num + num2 + count).ToString();
				}
			}
		}
		if (creaturesTweener != null)
		{
			SLOTUI.EnableTweener(creaturesTweener, !Table.GetShowHeroes() && Table.panelType == CardType.Creature);
		}
		if (spellsTweener != null)
		{
			SLOTUI.EnableTweener(spellsTweener, !Table.GetShowHeroes() && Table.panelType == CardType.Spell);
		}
		if (buildingsTweener != null)
		{
			SLOTUI.EnableTweener(buildingsTweener, !Table.GetShowHeroes() && Table.panelType == CardType.Building);
		}
		if (heroesTweener != null)
		{
			SLOTUI.EnableTweener(heroesTweener, Table.GetShowHeroes());
		}
	}

	private void ChangeCategory(CardType type)
	{
		if (Table != null)
		{
			Table.panelType = type;
		}
		UpdateUI();
	}

	public void ChangeCategoryToCreatures()
	{
		Table.ShowCards();
		currentTable = CardType.Creature;
		ChangeCategory(CardType.Creature);
	}

	public void ChangeCategoryToBuildings()
	{
		Table.ShowCards();
		currentTable = CardType.Building;
		ChangeCategory(CardType.Building);
	}

	public void ChangeCategoryToSpells()
	{
		Table.ShowCards();
		currentTable = CardType.Spell;
		ChangeCategory(CardType.Spell);
	}

	public void ChangeCategoryToHeroes()
	{
		Table.ShowHeroes();
		UpdateUI();
	}

	public void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (instance.IsReady())
			{
				UpdateUI();
				initialized = true;
			}
		}
	}
}
