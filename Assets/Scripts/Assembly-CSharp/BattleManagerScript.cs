using UnityEngine;

public class BattleManagerScript : MonoBehaviour
{
	private PulsatingScript OpponentPortrait;

	private PulsatingScript PlayerPortrait;

	private CreatureManagerScript CreatureManager;

	private VoiceoverScript Voiceover;

	private GameDataScript GameData;

	public bool FirstRound = true;

	public int SpellID;

	private GameState GameInstance;

	private BattlePhaseManager phaseMgr;

	private CWBattleSequenceController battleSqController;

	private static BattleManagerScript g_battleManager;

	public bool battleStarted;

	public int LaneIndex { get; set; }

	public float Timer { get; set; }

	public bool BattlePaused { get; set; }

	private void Awake()
	{
		g_battleManager = this;
		Singleton<AnalyticsManager>.Instance.IncBattleCount();
	}

	public static BattleManagerScript GetInstance()
	{
		return g_battleManager;
	}

	private void Start()
	{
		battleSqController = CWBattleSequenceController.GetInstance();
		CreatureManager = CreatureManagerScript.GetInstance();
		Voiceover = VoiceoverScript.GetInstance();
		GameData = GameDataScript.GetInstance();
		GameInstance = GameState.Instance;
		phaseMgr = BattlePhaseManager.GetInstance();
		LaneIndex = 0;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!instance.isInitialized)
		{
			instance.Login();
		}
		if (GameState.Instance.ActiveQuest != null)
		{
			int iQuestID = GameState.Instance.ActiveQuest.iQuestID;
			QuestData quest = QuestManager.Instance.GetQuest(iQuestID);
			GameObject gameObject = quest.LoadCustomPrefab();
			if (null != gameObject)
			{
				UnityUtils.InstantiatePrefab(gameObject, base.gameObject);
				TFUtils.DebugLog("Instantiated custom prefab: " + quest.CustomPrefab);
			}
		}
	}

	private void Update()
	{
		if (phaseMgr.Phase == BattlePhase.P1Battle || phaseMgr.Phase == BattlePhase.P2Battle)
		{
			if (GameInstance.GetHealth(PlayerType.User) > 0 && GameInstance.GetHealth(PlayerType.Opponent) > 0 && GameData.Turn != 1)
			{
				Timer += Time.deltaTime;
				if ((double)Timer > 0.5 && !battleStarted)
				{
					battleSqController.BattleSequence();
					battleStarted = true;
				}
			}
		}
		else
		{
			battleStarted = false;
		}
	}

	public void P1BattleFinished()
	{
		if (!GameData.GameOver)
		{
			if (CWLootingSequencer.GetInstance().chestLanes.Count != 0)
			{
				phaseMgr.Phase = BattlePhase.LootAfterP1Battle;
			}
			else
			{
				phaseMgr.Phase = BattlePhase.P2SetupBanner;
				CWiTweenBattleCam.GetInstance().SetCamBackAfterBattle();
			}
			GameData.ActivePlayer = 2;
			GameData.Turn++;
			Timer = 0f;
			LaneIndex = 0;
			Voiceover.P2SetupPhase();
			GameData.UpdateText();
		}
		GameInstance.HitAreaModifier = 0f;
		GameInstance.CritAreaModifier = 0f;
	}

	public void P2BattleFinished()
	{
		if (!GameData.GameOver)
		{
			if (CWLootingSequencer.GetInstance().chestLanes.Count != 0)
			{
				phaseMgr.Phase = BattlePhase.LootAfterP2Battle;
			}
			else
			{
				phaseMgr.Phase = BattlePhase.P1SetupBanner;
				CWiTweenBattleCam.GetInstance().SetCamBackAfterBattle();
			}
			GameData.ActivePlayer = 1;
			GameData.Turn++;
			Timer = 0f;
			LaneIndex = 0;
			Voiceover.P1SetupPhase();
			GameData.UpdateText();
		}
		GameInstance.DefenseAreaModifier = 0f;
		GameInstance.DefenseAreaCritModifier = 0f;
	}

	public void CheckForDefeat()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (GameInstance.LaneHasCreature(i, j))
				{
					CreatureBattleScript component = CreatureManager.Instances[i, j, 0].GetComponent<CreatureBattleScript>();
					component.CheckForDefeat();
				}
			}
		}
	}
}
