using UnityEngine;
using System.Collections.Generic;

public class PlayerInfoScript : MonoBehaviour
{
	public string PlayerName;
	public string Avatar;
	public string LastSaveTimeStamp;
	public int CampaignProgress;
	public int MatchProgress;
	public int OpponentCostumeID;
	public int OpponentID;
	public int PlayerCostumeID;
	public int PlayerID;
	public int PlayerAge;
	public int DefaultAgeGate;
	public int DefaultAgeGateUK;
	public int TOSCurrentVersion;
	public int PPCurrentVersion;
	public int MatchID;
	public bool UsePresetDeck;
	public bool Tutorial;
	public int DeckID;
	public int NumMPGamesPlayed;
	public int CurrentMPQuest;
	public int Stamina;
	public int Stamina_Max;
	public string LastTimestamp;
	public int NumUsedFreeGifts;
	public int SelectedMPDeck;
	public string MPDeckLeaderID;
	public List<string> Tags;
	public bool NotificationEnabled;
	public List<string> tutorialsCompleted;
	public bool AutoBattleSetting;
	public string Party;
	public int PartyExpiration;
	public bool LoginAttempted;
	public bool isInitialized;
}
