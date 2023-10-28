using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
	private enum DebugTab
	{
		Cheats,
		Inventory,
		Quests,
		NUM_TABS
	}

	public int typeWidth = 120;

	public int buttonWidth = 60;

	public int defaultHeight = 30;

	public int boxHeight;

	public int boxWidth;

	public int numButtonsWidth = 5;

	public int numButtonsHeight = 10;

	public int smallButtonWidth = 30;

	public int posX;

	public int posY;

	private static DebugGUI g_debugGUI;

	private DebugCardScript helperScript;

	private DebugTab currentTab;

	private void Awake()
	{
		g_debugGUI = this;
		helperScript = new DebugCardScript();
		helperScript.Owner = PlayerType.User;
	}

	public static DebugGUI GetInstance()
	{
		return g_debugGUI;
	}

	private void OnEnable()
	{
		Time.timeScale = 0f;
	}

	private void OnDisable()
	{
		Time.timeScale = 1f;
	}

	private void AddTabCheats()
	{
		int num = defaultHeight;
		int num2 = buttonWidth;
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Unlock Leaders", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			LeaderManager instance2 = LeaderManager.Instance;
			foreach (KeyValuePair<string, LeaderForm> leaderForm in instance2.leaderForms)
			{
				string key = leaderForm.Key;
				if (!key.Contains("_Dumb_"))
				{
					instance2.AddNewLeaderIfUnique(key);
				}
			}
		}
		if (GUILayout.Button("Unlock Cards", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript.GetInstance().DeckManager.DebugAddAllCards();
		}
		if (GUILayout.Button("Select Card", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			instance.cardSelection = true;
			helperScript.OpenDiscardPile();
		}
		if (GUILayout.Button("+ Calendar Day", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			DateTime lastCalendarTimestamp = PlayerInfoScript.GetInstance().LastCalendarTimestamp;
			DateTime lastCalendarTimestamp2 = ((lastCalendarTimestamp.Day <= 1) ? DateTime.MinValue : new DateTime(lastCalendarTimestamp.Year, lastCalendarTimestamp.Month, lastCalendarTimestamp.Day - 1));
			PlayerInfoScript.GetInstance().LastCalendarTimestamp = lastCalendarTimestamp2;
			MenuController instance3 = MenuController.GetInstance();
			if (instance3.MenuState == MenuController.MenuStates.MainMenu)
			{
				instance3.ActivateCalendar();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Restore Stamina", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			if (instance == null)
			{
				instance = DebugFlagsScript.GetInstance();
			}
			PlayerInfoScript instance4 = PlayerInfoScript.GetInstance();
			instance4.Stamina = Mathf.Max(instance4.Stamina_Max, instance4.Stamina);
		}
		if (GUILayout.Button("Clear stamina", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance5 = PlayerInfoScript.GetInstance();
			instance5.Stamina = 0;
		}
		if (GUILayout.Button("+10 max stamina", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance6 = PlayerInfoScript.GetInstance();
			instance6.Stamina_Max += 10;
		}
		if (GUILayout.Button("+10 stamina", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance7 = PlayerInfoScript.GetInstance();
			instance7.Stamina += 10;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+1000 XP", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance8 = PlayerInfoScript.GetInstance();
			Deck selectedDeck = instance8.GetSelectedDeck();
			selectedDeck.Leader.XP += 1000;
			PlayerInfoScript.GetInstance().Save();
		}
		GUILayout.EndHorizontal();
	}

	private void AddTabQuests()
	{
		int num = defaultHeight;
		int num2 = buttonWidth;
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		GUILayout.BeginHorizontal();
		string text = "El Fisto " + instance.elFistoMode;
		if (GUILayout.Button(text, GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			int elFistoMode = (int)instance.elFistoMode;
			int num3 = ++elFistoMode;
			if (num3 >= 3)
			{
				num3 = 0;
			}
			instance.elFistoMode = (ElFistoController.ElFistoModes)num3;
		}
		GUILayout.EndHorizontal();
		GameState instance2 = GameState.Instance;
		LandscapeManagerScript instance3 = LandscapeManagerScript.GetInstance();
		if (instance2 != null && instance3 != null)
		{
			int num4 = Enum.GetNames(typeof(LandscapeType)).Length;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Lanes:", GUILayout.Width(60f), GUILayout.Height(defaultHeight));
			for (int i = 0; i < 4; i++)
			{
				LandscapeType landscapeType = instance2.GetLandscapeType(PlayerType.User, i);
				if (GUILayout.Button(landscapeType.ToString(), GUILayout.Width(buttonWidth), GUILayout.Height(defaultHeight)))
				{
					LandscapeType type = (LandscapeType)((int)(landscapeType + 1) % num4);
					instance3.RemoveLandscape(PlayerType.User, i);
					instance2.SetLandscape(PlayerType.User, i, type);
					instance3.UpdateLandscapes();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Auto Win", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
			{
				instance.autoWin = true;
			}
			if (GUILayout.Button("Auto Lose", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
			{
				instance.autoLose = true;
			}
			GUILayout.EndHorizontal();
		}
		SideQuestDebugButtons();
		GUILayout.BeginHorizontal();
		if (MapControllerBase.GetInstance() != null)
		{
			MapControllerBase instance4 = MapControllerBase.GetInstance();
		}
		instance.battleDisplay.autoCrit = GUILayout.Toggle(instance.battleDisplay.autoCrit, "Auto-crit", GUILayout.Width(num2), GUILayout.Height(num));
		instance.InfiniteMagic = GUILayout.Toggle(instance.InfiniteMagic, "Infinite Magic", GUILayout.Width(num2), GUILayout.Height(num));
		GUILayout.EndHorizontal();
		MapControllerBase instance5 = MapControllerBase.GetInstance();
		if (instance5 != null)
		{
			PlayerInfoScript instance6 = PlayerInfoScript.GetInstance();
			QuestManager instance7 = QuestManager.Instance;
			GUILayout.BeginHorizontal();
			string mapQuestType = instance5.MapQuestType;
			if (GUILayout.Button("Unlock " + mapQuestType + " Quests", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
			{
				foreach (QuestData item in instance7.GetQuestsByType(mapQuestType))
				{
					instance6.SetQuestProgress(item, 3);
				}
				instance7.InitializeQuestStates(mapQuestType);
				if (instance5.QuestMapInfo != null)
				{
					instance5.QuestMapInfo.ShowAllPaths();
					foreach (MapRegionInfo region in instance5.QuestMapInfo.Regions)
					{
						if (region != null)
						{
							instance6.UnlockRegion(mapQuestType, region.RegionID);
						}
					}
				}
				instance5.QuestMapRefresh();
			}
			GUILayout.EndHorizontal();
		}
		if (!(instance5 != null) || !(instance5.QuestMapInfo != null))
		{
			return;
		}
		PlayerInfoScript instance8 = PlayerInfoScript.GetInstance();
		QuestManager instance9 = QuestManager.Instance;
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Quest +1 Star", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			QuestData currentQuest = instance8.GetCurrentQuest();
			if (currentQuest != null)
			{
				instance8.SetQuestProgress(currentQuest, instance8.GetQuestProgress(currentQuest) + 1);
				instance9.InitializeQuestStates(currentQuest.QuestType);
				instance5.QuestMapRefresh();
			}
			instance8.Save();
		}
		if (GUILayout.Button("Quest -1 Star", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			QuestData currentQuest2 = instance8.GetCurrentQuest();
			if (currentQuest2 != null)
			{
				instance8.SetQuestProgress(currentQuest2, instance8.GetQuestProgress(currentQuest2) - 1);
				instance9.InitializeQuestStates(currentQuest2.QuestType);
				instance5.QuestMapRefresh();
			}
			instance8.Save();
		}
		GUILayout.EndHorizontal();
	}

	private void AddTabInventory()
	{
		int num = defaultHeight;
		int num2 = buttonWidth;
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("0 Gems", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
			instance2.Gems = 0;
		}
		if (GUILayout.Button("+10 Gems", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript.GetInstance().Gems += 10;
		}
		if (GUILayout.Button("-10 Gems", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance3 = PlayerInfoScript.GetInstance();
			instance3.Gems -= 10;
			if (instance3.Gems < 0)
			{
				instance3.Gems = 0;
			}
		}
		if (GUILayout.Button("Purchase VG", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			KFFUpsightVGController.GetInstance().DebugRedeemVirtualGoods();
			DebugGUIDisplay.GetInstance().OnClick();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("+100000 Coins", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript.GetInstance().Coins += 100000;
		}
		if (GUILayout.Button("-100000 Coins", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance4 = PlayerInfoScript.GetInstance();
			instance4.Coins -= 100000;
			if (instance4.Coins < 0)
			{
				instance4.Coins = 0;
			}
		}
		if (GUILayout.Button("CardBox +10", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript.GetInstance().MaxInventory += 10;
		}
		if (GUILayout.Button("CardBox -10", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			PlayerInfoScript instance5 = PlayerInfoScript.GetInstance();
			instance5.MaxInventory = Math.Max(0, instance5.MaxInventory - 10);
		}
		GUILayout.EndHorizontal();
	}

	private void AddTabButtons(DebugTab tab)
	{
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		switch (tab)
		{
		case DebugTab.Cheats:
			AddTabCheats();
			break;
		case DebugTab.Inventory:
			AddTabInventory();
			break;
		case DebugTab.Quests:
			AddTabQuests();
			break;
		}
	}

	private void OnGUI()
	{
		boxHeight = Screen.height;
		boxWidth = Screen.width - posX;
		buttonWidth = (Screen.width - posX) / numButtonsWidth;
		defaultHeight = (Screen.height - posY) / numButtonsHeight;
		typeWidth = buttonWidth - 4;
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		GUI.contentColor = Color.white;
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		GUI.Box(new Rect(0f, 0f, boxWidth, boxHeight), string.Empty);
		GUI.Box(new Rect(0f, 0f, boxWidth, boxHeight), string.Empty);
		GUI.Box(new Rect(0f, 0f, boxWidth, boxHeight), string.Empty);
		GUI.Box(new Rect(0f, 0f, boxWidth, boxHeight), string.Empty);
		int num = defaultHeight;
		int num2 = buttonWidth;
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Hide Debug", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			DebugFlagsScript instance2 = DebugFlagsScript.GetInstance();
			DebugGUI component = instance2.gameObject.GetComponent<DebugGUI>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			Time.timeScale = 1f;
		}
		if (GUILayout.Button("Disable Debug", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			DebugFlagsScript instance3 = DebugFlagsScript.GetInstance();
			DebugGUI component2 = instance3.gameObject.GetComponent<DebugGUI>();
			if ((bool)component2)
			{
				component2.enabled = false;
			}
			Time.timeScale = 1f;
			instance3.DebugInBuild = false;
		}
		if (GUILayout.Button("Reset Account", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			SessionManager.GetInstance().DeleteLocal();
			SessionManager.GetInstance().DeleteFromServer();
			PlayerInfoScript.ResetPlayerName();
			PlayerPrefs.DeleteAll();
			Application.Quit();
		}
		SocialManager instance4 = SocialManager.Instance;
		if (!instance4.IsPlayerAuthenticated() && GUILayout.Button("Social Login", GUILayout.Width(typeWidth), GUILayout.Height(defaultHeight)))
		{
			instance4.AuthenticatePlayer(true);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		int num3 = 4;
		foreach (object value in Enum.GetValues(typeof(DebugTab)))
		{
			DebugTab debugTab = (DebugTab)(int)value;
			if (debugTab == DebugTab.NUM_TABS)
			{
				break;
			}
			if (GUILayout.Button(debugTab.ToString(), GUILayout.Width(Screen.width / num3), GUILayout.Height(defaultHeight)))
			{
				currentTab = debugTab;
			}
		}
		GUILayout.EndHorizontal();
		AddTabButtons(currentTab);
		GUILayout.BeginHorizontal();
		instance.mapDebug.unlockQuest = GUILayout.Toggle(instance.mapDebug.unlockQuest, "Lock/Unlock Quests", GUILayout.Width(num2), GUILayout.Height(num));
		instance.mapDebug.questMenu = GUILayout.Toggle(instance.mapDebug.questMenu, "Quest Menu", GUILayout.Width(num2), GUILayout.Height(num));
		instance.mapDebug.compQuest89 = GUILayout.Toggle(instance.mapDebug.compQuest89, "Comp Quest 89", GUILayout.Width(num2), GUILayout.Height(num));
		instance.failIAP = GUILayout.Toggle(instance.failIAP, "Fail IAP", GUILayout.Width(num2), GUILayout.Height(num));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		instance.disableDungeonTimeLock = !GUILayout.Toggle(!instance.disableDungeonTimeLock, "Dungeon time lock", GUILayout.Width(num2), GUILayout.Height(num));
		instance.FCCompleteDemo = GUILayout.Toggle(instance.FCCompleteDemo, "Complete FC Demo", GUILayout.Width(num2), GUILayout.Height(num));
		instance.stopTutorial = GUILayout.Toggle(instance.stopTutorial, "SkipTutorial", GUILayout.Width(num2), GUILayout.Height(num));
		instance.battleDisplay.FPSDisplay = GUILayout.Toggle(instance.battleDisplay.FPSDisplay, "FPS", GUILayout.Width(num2), GUILayout.Height(num));
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void SideQuestDebugButtons()
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		if (!(instance != null) || string.IsNullOrEmpty(instance.MapQuestType))
		{
			return;
		}
		PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
		SideQuestManager instance3 = SideQuestManager.Instance;
		string mapQuestType = instance.MapQuestType;
		List<SideQuestData> activeSideQuests = instance3.GetActiveSideQuests(mapQuestType);
		List<SideQuestData> sideQuests = instance3.GetSideQuests(mapQuestType);
		bool flag = false;
		List<string> sideQuestGroups = instance3.GetSideQuestGroups(mapQuestType);
		sideQuestGroups.Sort();
		string groupName;
		foreach (string item in sideQuestGroups)
		{
			groupName = item;
			GUILayout.BeginHorizontal();
			GUILayout.Label("SQ:" + mapQuestType + "-" + groupName + ":", GUILayout.Width(100f), GUILayout.Height(defaultHeight));
			List<SideQuestData> list = sideQuests.FindAll((SideQuestData s) => s.Group == groupName);
			SideQuestData sideQuestData = activeSideQuests.Find((SideQuestData s) => s.Group == groupName);
			string text = "None";
			if (sideQuestData != null)
			{
				text = sideQuestData.iQuestID.ToString();
			}
			SideQuestProgress.SideQuestState sideQuestState = instance2.GetSideQuestState(sideQuestData);
			string text2 = text;
			text = string.Concat(text2, " (", sideQuestState, ")");
			if (GUILayout.Button(text + ": ", GUILayout.Width(100f), GUILayout.Height(defaultHeight)))
			{
				SideQuestData sideQuestData2 = list[0];
				if (sideQuestData != null)
				{
					instance3.DeactivateSideQuest(sideQuestData);
					int num = list.IndexOf(sideQuestData) + 1;
					sideQuestData2 = ((num < list.Count) ? list[num] : null);
				}
				instance3.ActivateSideQuest(sideQuestData2);
				sideQuestData = sideQuestData2;
				if (sideQuestData2 != null)
				{
					flag = true;
				}
			}
			if (sideQuestData != null)
			{
				SideQuestProgress sideQuestProgress = instance2.GetSideQuestProgress(sideQuestData);
				int num2 = 0;
				if (sideQuestProgress != null)
				{
					num2 = sideQuestProgress.Collected;
				}
				GUILayout.Label(string.Format("{0}/{1}:", num2, sideQuestData.NumCollectibles), GUILayout.Width(40f), GUILayout.Height(defaultHeight));
				if (GUILayout.Button("+1", GUILayout.Width(smallButtonWidth), GUILayout.Height(defaultHeight)))
				{
					instance2.IncSideQuestProgress(sideQuestData, 1, null);
					flag = true;
				}
				if (GUILayout.Button("-1", GUILayout.Width(smallButtonWidth), GUILayout.Height(defaultHeight)))
				{
					instance2.IncSideQuestProgress(sideQuestData, -1, null);
					flag = true;
				}
				if (sideQuestData.EventStartDate.HasValue)
				{
					GUILayout.Label("Start-" + sideQuestData.EventStartDate.Value.ToShortDateString() + ":", GUILayout.Width(100f), GUILayout.Height(defaultHeight));
					if (GUILayout.Button("+1d", GUILayout.Width(smallButtonWidth), GUILayout.Height(defaultHeight)))
					{
						sideQuestData.IncStartDate(TimeSpan.FromDays(1.0));
						flag = true;
					}
					if (GUILayout.Button("-1d", GUILayout.Width(smallButtonWidth), GUILayout.Height(defaultHeight)))
					{
						sideQuestData.DecStartDate(TimeSpan.FromDays(1.0));
						flag = true;
					}
				}
				if (sideQuestData.EventEndDate.HasValue)
				{
					GUILayout.Label("End-" + sideQuestData.EventEndDate.Value.ToShortDateString() + ":", GUILayout.Width(100f), GUILayout.Height(defaultHeight));
					if (GUILayout.Button("+1d", GUILayout.Width(smallButtonWidth), GUILayout.Height(defaultHeight)))
					{
						sideQuestData.IncEndDate(TimeSpan.FromDays(1.0));
						flag = true;
					}
					if (GUILayout.Button("-1d", GUILayout.Width(smallButtonWidth), GUILayout.Height(defaultHeight)))
					{
						sideQuestData.DecEndDate(TimeSpan.FromDays(1.0));
						flag = true;
					}
				}
			}
			if (flag)
			{
				if (sideQuestData != null)
				{
					SideQuestProgress sideQuestProgress2 = instance2.GetSideQuestProgress(sideQuestData, true);
					if (sideQuestData.IsEventEnded)
					{
						instance3.ExpireSideQuest(sideQuestData);
					}
					else if (!sideQuestData.IsEventStarted)
					{
						instance3.SetSideQuestState(sideQuestData, SideQuestProgress.SideQuestState.Inactive);
					}
					else if (sideQuestProgress2.Collected == 0)
					{
						instance3.SetSideQuestState(sideQuestData, SideQuestProgress.SideQuestState.Pending);
					}
					else if (sideQuestProgress2.Collected == sideQuestData.NumCollectibles)
					{
						instance3.SetSideQuestState(sideQuestData, SideQuestProgress.SideQuestState.Accomplished);
					}
					else if (sideQuestProgress2.Collected < sideQuestData.NumCollectibles)
					{
						instance3.SetSideQuestState(sideQuestData, SideQuestProgress.SideQuestState.InProgress);
					}
				}
				if (instance.QuestMapRoot != null)
				{
					SideQuestController[] components = instance.GetComponents<SideQuestController>();
					if (components != null && components.Length > 0)
					{
						for (int i = 0; i < components.Length; i++)
						{
							if (components[i].SideQuestGroup == groupName)
							{
								components[i].UpdateActiveSideQuest();
							}
						}
					}
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}
