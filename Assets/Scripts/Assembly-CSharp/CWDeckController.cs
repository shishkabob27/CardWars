using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CWDeckController : MonoBehaviour
{
	public List<GameObject> Tabs;

	private List<UICheckbox> TabCheckboxes = new List<UICheckbox>();

	public List<GameObject> Tables;

	private List<CWDeckDeckCards> TableScripts = new List<CWDeckDeckCards>();

	public List<GameObject> Bars;

	private List<CWDeckOpenPanel> BarScripts = new List<CWDeckOpenPanel>();

	public CWDeckHeroPanel heroPanel;

	public CWDeckNameplate nameplate;

	private bool initialized;

	private static CWDeckController g_deckController;

	public BuildingPos buildingBarPos = BuildingPos.Down;

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

	public int currentMPDeck
	{
		get
		{
			return PlayerInfoScript.GetInstance().SelectedMPDeck;
		}
		set
		{
			PlayerInfoScript.GetInstance().SelectedMPDeck = value;
		}
	}

	private void Awake()
	{
		if (g_deckController == null)
		{
			g_deckController = this;
		}
	}

	public static CWDeckController GetInstance()
	{
		return g_deckController;
	}

	public void Start()
	{
		foreach (GameObject tab in Tabs)
		{
			TabCheckboxes.Add(tab.GetComponent<UICheckbox>());
		}
		foreach (GameObject table in Tables)
		{
			TableScripts.Add(table.GetComponent<CWDeckDeckCards>());
		}
		foreach (GameObject bar in Bars)
		{
			BarScripts.Add(bar.GetComponent<CWDeckOpenPanel>());
		}
	}

	public void OnDeckChanged()
	{
		int num = currentDeck;
		if (num != currentDeck)
		{
			ChangeDeck();
			if (heroPanel != null)
			{
				heroPanel.Refresh();
			}
			if (nameplate != null)
			{
				nameplate.Refresh();
			}
			LandscapePreviewController instance = LandscapePreviewController.GetInstance();
			if (instance != null)
			{
				instance.UpdatePreview();
			}
		}
	}

	public void SetLandscapes()
	{
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		Deck deck = deckManager.GetDeck(currentDeck);
		Dictionary<Faction, int> dictionary = new Dictionary<Faction, int>();
		Faction[] array = (Faction[])Enum.GetValues(typeof(Faction));
		foreach (Faction key in array)
		{
			dictionary.Add(key, 0);
		}
		for (int j = 0; j < deck.CardCount(); j++)
		{
			CardForm form = deck.GetCard(j).Form;
			Dictionary<Faction, int> dictionary2;
			Dictionary<Faction, int> dictionary3 = (dictionary2 = dictionary);
			Faction faction;
			Faction key2 = (faction = form.Faction);
			int num = dictionary2[faction];
			dictionary3[key2] = num + 1;
		}
		dictionary.Remove(Faction.Universal);
		List<KeyValuePair<LandscapeType, int>> list = new List<KeyValuePair<LandscapeType, int>>();
		foreach (KeyValuePair<Faction, int> item2 in dictionary)
		{
			LandscapeType key3 = (LandscapeType)(int)Enum.Parse(typeof(LandscapeType), item2.Key.ToString());
			int value = item2.Value;
			KeyValuePair<LandscapeType, int> item = new KeyValuePair<LandscapeType, int>(key3, value);
			list.Add(item);
		}
		list.Sort((KeyValuePair<LandscapeType, int> a, KeyValuePair<LandscapeType, int> b) => -a.Value.CompareTo(b.Value));
		int num2 = list.Count((KeyValuePair<LandscapeType, int> a) => a.Value > 0);
		if (num2 >= 4)
		{
			deck.SetLandscape(0, list[0].Key);
			deck.SetLandscape(1, list[1].Key);
			deck.SetLandscape(2, list[2].Key);
			deck.SetLandscape(3, list[3].Key);
		}
		else if (num2 == 3)
		{
			float num3 = list[0].Value;
			float num4 = list[1].Value;
			float num5 = list[2].Value;
			if (num4 / (num3 + num4 + num5) >= 0.5f)
			{
				deck.SetLandscape(0, list[0].Key);
				deck.SetLandscape(1, list[1].Key);
				deck.SetLandscape(2, list[1].Key);
				deck.SetLandscape(3, list[2].Key);
			}
			else if (num5 / (num3 + num4 + num5) >= 0.5f)
			{
				deck.SetLandscape(0, list[0].Key);
				deck.SetLandscape(1, list[1].Key);
				deck.SetLandscape(2, list[2].Key);
				deck.SetLandscape(3, list[2].Key);
			}
			else
			{
				deck.SetLandscape(0, list[0].Key);
				deck.SetLandscape(1, list[0].Key);
				deck.SetLandscape(2, list[1].Key);
				deck.SetLandscape(3, list[2].Key);
			}
		}
		else if (num2 == 2)
		{
			float num6 = list[0].Value;
			float num7 = list[1].Value;
			if (num6 / (num6 + num7) >= 0.65f)
			{
				deck.SetLandscape(0, list[0].Key);
				deck.SetLandscape(1, list[0].Key);
				deck.SetLandscape(2, list[0].Key);
				deck.SetLandscape(3, list[1].Key);
			}
			else if (num7 / (num6 + num7) >= 0.65f)
			{
				deck.SetLandscape(0, list[0].Key);
				deck.SetLandscape(1, list[1].Key);
				deck.SetLandscape(2, list[1].Key);
				deck.SetLandscape(3, list[1].Key);
			}
			else
			{
				deck.SetLandscape(0, list[0].Key);
				deck.SetLandscape(1, list[0].Key);
				deck.SetLandscape(2, list[1].Key);
				deck.SetLandscape(3, list[1].Key);
			}
		}
		else if (num2 <= 1)
		{
			deck.SetLandscape(0, list[0].Key);
			deck.SetLandscape(1, list[0].Key);
			deck.SetLandscape(2, list[0].Key);
			deck.SetLandscape(3, list[0].Key);
		}
		LandscapePreviewController instance = LandscapePreviewController.GetInstance();
		if (instance != null)
		{
			instance.UpdatePreview();
		}
	}

	private void FillInCounts()
	{
		Deck deck = PlayerInfoScript.GetInstance().DeckManager.GetDeck(currentDeck);
		if (deck == null)
		{
			return;
		}
		int num = deck.CardCount();
		int[] array = new int[16];
		int i;
		for (i = 0; i < num; i++)
		{
			CardItem card = deck.GetCard(i);
			array[(int)card.Form.Type]++;
		}
		i = 0;
		foreach (GameObject bar in Bars)
		{
			SQUtils.SetLabel(bar.transform, "Count", array[i].ToString());
			i++;
		}
	}

	public void ChangeDeck()
	{
		FillInCounts();
		foreach (CWDeckDeckCards tableScript in TableScripts)
		{
			if (NGUITools.GetActive(tableScript.gameObject))
			{
				tableScript.OnEnable();
			}
		}
	}

	public void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (instance != null && instance.IsReady())
			{
				FillInCounts();
				BarScripts[0].OpenMyself();
				initialized = true;
			}
		}
	}
}
