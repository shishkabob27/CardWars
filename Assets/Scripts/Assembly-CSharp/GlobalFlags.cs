using UnityEngine;

public class GlobalFlags
{
	private static GlobalFlags instance;

	public bool ReturnToMainMenu;

	public bool ReturnToBuildDeck;

	public bool ReturningFromGame;

	public bool NewlyCleared;

	public bool Cleared;

	public bool SkipAnims;

	public int lastQuestConditionStatus;

	public int lastStaminaMax;

	public int lastStamina;

	public int lastGem;

	public Vector3 lastQuestMapCameraIdealPos;

	public float lastQuestMapCameraFOV;

	public bool stopTutorial;

	public bool disableDungeonTimeLock;

	public bool InMPMode;

	public BattleResult BattleResult;

	public bool enableMapDrag = true;

	public static GlobalFlags Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GlobalFlags();
			}
			return instance;
		}
	}

	public static void FullReset()
	{
		instance = null;
	}
}
