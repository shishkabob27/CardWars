using UnityEngine;

public class DebugFlagsScript : MonoBehaviour
{
	public enum ForceStartingPlayer
	{
		None = 0,
		Me = 1,
		Them = 2,
	}

	public bool AddDiamonds;
	public bool InfiniteMagic;
	public bool AttackBonus;
	public bool QuickWin;
	public bool quickLoose;
	public bool DebugInBuild;
	public bool UseLocalJsonFiles;
	public bool unlockLeaders;
	public bool autoWin;
	public bool autoLose;
	public bool recoverPlayerHP;
	public bool recoverOpponentHP;
	public bool cardSelection;
	public bool fingerGesture;
	public bool tutorialDebug;
	public bool stopTutorial;
	public bool failIAP;
	public bool disableDungeonTimeLock;
	public bool FCCompleteDemo;
	public ForceStartingPlayer forceStartingPlayer;
	public bool resetAchievements;
	public bool resetCalendar;
	public string specifyCalendarDate;
	public bool autoIncrementCalendar;
	public ElFistoController.ElFistoModes elFistoMode;
	public bool plainTextGameJson;
	public BattleDebug battleDisplay;
	public MapDebug mapDebug;
}
