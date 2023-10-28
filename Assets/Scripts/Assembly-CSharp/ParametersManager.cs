using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametersManager : ILoadable
{
	public int Min_Cards_In_Inventory;

	public int Min_Creatures_In_Inventory;

	public int Max_Duplicates_In_Deck;

	public int Min_Cards_In_Deck;

	public int New_Player_Max_Inventory;

	public int New_Player_Max_Stamina;

	public int New_Player_Coins;

	public int New_Player_Gems;

	public int Stamina_Restoration_Rate;

	public int Max_Leader_Level;

	public int Starting_Magic_Points = 2;

	public int Max_Magic_Points = 9;

	public int Restart_OnResume_Time;

	public int Min_Dungeon_Level;

	public string Calendar_Unlock_Tutorial = string.Empty;

	public string GatchaKey_Unlock_Tutorial = string.Empty;

	public string AIDeck_Bad_Card_Swap = string.Empty;

	public int BonusQuest_firstquest_FC;

	public int BonusQuest_timelapse_FC;

	public int BonusQuest_matchlapse_FC;

	public string FC_CardReward_FirstTime = string.Empty;

	public int SideQuest_firstquest_FC = 5001;

	public int SideQuest_matchlapse_FC;

	public int SideQuest_firstquest_Main = 1;

	public int SideQuest_matchlapse_Main;

	private static ParametersManager instance;

	public static ParametersManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ParametersManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		float stamina_minutes = 1f;
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Parameters.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			switch (TFUtils.LoadString(dict, "ID"))
			{
			case "Min_Cards_In_Inventory":
				Min_Cards_In_Inventory = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "Min_Creatures_In_Inventory":
				Min_Creatures_In_Inventory = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "Max_Duplicates_In_Deck":
				Max_Duplicates_In_Deck = TFUtils.LoadInt(dict, "Value", 1);
				break;
			case "Min_Cards_In_Deck":
				Min_Cards_In_Deck = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "New_Player_Max_Inventory":
				New_Player_Max_Inventory = TFUtils.LoadInt(dict, "Value", 1);
				break;
			case "New_Player_Max_Stamina":
				New_Player_Max_Stamina = TFUtils.LoadInt(dict, "Value", 1);
				break;
			case "New_Player_Coins":
				New_Player_Coins = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "New_Player_Gems":
				New_Player_Gems = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "Stamina_Restoration_Rate":
				stamina_minutes = TFUtils.LoadFloat(dict, "Value", 1f);
				break;
			case "Max_Leader_Level":
				Max_Leader_Level = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "Starting_Magic_Points":
				Starting_Magic_Points = TFUtils.LoadInt(dict, "Value", 2);
				break;
			case "Max_Magic_Points":
				Max_Magic_Points = TFUtils.LoadInt(dict, "Value", 9);
				break;
			case "Restart_OnResume_Time":
				Restart_OnResume_Time = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "Dungeon_Min_Level":
				Min_Dungeon_Level = TFUtils.LoadInt(dict, "Value", 0);
				break;
			case "Calendar_Unlock_Tutorial":
				Calendar_Unlock_Tutorial = TFUtils.LoadString(dict, "Value", string.Empty);
				break;
			case "GatchaKey_Unlock_Tutorial":
				GatchaKey_Unlock_Tutorial = TFUtils.LoadString(dict, "Value", string.Empty);
				break;
			case "AIDeck_Bad_Card_Swap":
				AIDeck_Bad_Card_Swap = TFUtils.LoadString(dict, "Value", string.Empty);
				break;
			case "BonusQuest_firstquest_FC":
				BonusQuest_firstquest_FC = TFUtils.LoadInt(dict, "Value", BonusQuest_firstquest_FC);
				break;
			case "BonusQuest_timelapse_FC":
				BonusQuest_timelapse_FC = TFUtils.LoadInt(dict, "Value", BonusQuest_timelapse_FC);
				break;
			case "BonusQuest_matchlapse_FC":
				BonusQuest_matchlapse_FC = TFUtils.LoadInt(dict, "Value", BonusQuest_matchlapse_FC);
				break;
			case "FC_CardReward_FirstTime":
				FC_CardReward_FirstTime = TFUtils.LoadString(dict, "Value", FC_CardReward_FirstTime);
				break;
			case "SideQuest_firstquest_FC":
				SideQuest_firstquest_FC = TFUtils.LoadInt(dict, "Value", SideQuest_firstquest_FC);
				break;
			case "SideQuest_matchlapse_FC":
				SideQuest_matchlapse_FC = TFUtils.LoadInt(dict, "Value", SideQuest_matchlapse_FC);
				break;
			case "SideQuest_firstquest_Main":
				SideQuest_firstquest_Main = TFUtils.LoadInt(dict, "Value", SideQuest_firstquest_Main);
				break;
			case "SideQuest_matchlapse_Main":
				SideQuest_matchlapse_Main = TFUtils.LoadInt(dict, "Value", SideQuest_matchlapse_Main);
				break;
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Stamina_Restoration_Rate = Mathf.FloorToInt(stamina_minutes * 60000f);
	}

	public void Destroy()
	{
		instance = null;
	}
}
