#define ASSERTS_ON
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QuestLaunchHelper))]
public class FCDemoSequenceController : MonoBehaviour
{
	private QuestLaunchHelper questLaunchHelper;

	private GameState gameState;

	private BattlePhaseManager phaseMan;

	private CWBattleSequenceController sequenceController;

	private QuestData activeQuest;

	private bool launchingNextScene;

	public int Phase1BossHealthMinimum = 1;

	public float Phase2BossHealthThreshold = 0.5f;

	public float Phase2PlayerHealthThreshold = 0.5f;

	public int Phase2BossHealthMinimum = 1;

	private int cakeHealthLastFrame = -1;

	private bool phase1SetupComplete;

	private bool phase2SetupComplete;

	private void Start()
	{
		questLaunchHelper = base.gameObject.GetComponent<QuestLaunchHelper>();
		TFUtils.Assert(null != questLaunchHelper, "Couldn't get QuestLaunchHelper");
		NisLaunchHelper component = base.gameObject.GetComponent<NisLaunchHelper>();
		if (null != component)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("UGUIMenu");
			if (null != gameObject)
			{
				component.nisRoot = gameObject;
			}
		}
		gameState = GameState.Instance;
		phaseMan = BattlePhaseManager.GetInstance();
		activeQuest = gameState.ActiveQuest;
		sequenceController = base.gameObject.GetComponentInParent<CWBattleSequenceController>();
		TFUtils.Assert(null != sequenceController, "Couldn't get CWBattleSequenceController");
		launchingNextScene = false;
		phase1SetupComplete = false;
		phase2SetupComplete = false;
	}

	private void Update()
	{
		if (!launchingNextScene && activeQuest != null && activeQuest.IsQuestType("fc_demo"))
		{
			if (activeQuest.iQuestIndex == 0)
			{
				UpdatePhase1();
			}
			else
			{
				UpdatePhase2();
			}
		}
	}

	private void UpdatePhase1()
	{
		if (!phase1SetupComplete)
		{
			int user = 0;
			int phase1BossHealthMinimum = Phase1BossHealthMinimum;
			gameState.SetMinHealth(user, phase1BossHealthMinimum);
			phase1SetupComplete = true;
		}
		if (launchingNextScene)
		{
			return;
		}
		if (phaseMan.Phase == BattlePhase.Result_P1Defeated)
		{
			launchingNextScene = true;
			if (gameState.BattleResolver != null)
			{
				gameState.BattleResolver.SetResult(PlayerType.Opponent);
			}
			StartCoroutine(ReturnToMainMenu(4f));
		}
		int health = gameState.GetHealth(PlayerType.Opponent);
		if (health <= Phase1BossHealthMinimum && (phaseMan.Phase == BattlePhase.P1Battle || phaseMan.Phase == BattlePhase.P2Battle))
		{
			launchingNextScene = true;
			TFUtils.DebugLog("Opponent's health is at " + health + ", ending Phase 1...", GetType().ToString());
			StartCoroutine(EndPhase1());
		}
	}

	private void UpdatePhase2()
	{
		if (!phase2SetupComplete)
		{
			gameState.SetMinHealth(1, Phase2BossHealthMinimum);
			phase2SetupComplete = true;
		}
		if (launchingNextScene)
		{
			return;
		}
		int health = gameState.GetHealth(PlayerType.User);
		int health2 = gameState.GetHealth(PlayerType.Opponent);
		int num = (int)((float)gameState.GetMaxHealth(PlayerType.Opponent) * Phase2BossHealthThreshold);
		int num2 = (int)((float)gameState.GetMaxHealth(PlayerType.User) * Phase2PlayerHealthThreshold);
		if (cakeHealthLastFrame == -1 || health >= cakeHealthLastFrame)
		{
			cakeHealthLastFrame = health;
			return;
		}
		if (health <= num2 || health2 <= num)
		{
			if (phaseMan.Phase == BattlePhase.P1Battle)
			{
				if (health2 <= Phase2BossHealthMinimum)
				{
					launchingNextScene = true;
					TFUtils.DebugLog("Wow, boss is almost dead!", GetType().ToString());
					StartCoroutine(EndPhase2());
				}
			}
			else if (phaseMan.Phase == BattlePhase.P2Battle)
			{
				gameState.SetHealth(PlayerType.User, 1);
				launchingNextScene = true;
				TFUtils.DebugLog("Cake is down!", GetType().ToString());
				StartCoroutine(EndPhase2());
			}
		}
		cakeHealthLastFrame = health;
	}

	private IEnumerator EndPhase1()
	{
		yield return new WaitForSeconds(0.5f);
		PrepForNISHack();
		List<QuestData> fc_quests = QuestManager.Instance.GetQuestsByType("fc_demo");
		QuestData quest = fc_quests[1];
		GlobalFlags.Instance.InMPMode = false;
		questLaunchHelper.LaunchQuest(quest.QuestID, PlayerInfoScript.GetInstance().GetSelectedDeckCopy(), null, new FCDemoBattleResolver());
	}

	private IEnumerator EndPhase2()
	{
		yield return new WaitForSeconds(0.5f);
		PrepForNISHack();
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		if (null != pinfo)
		{
			pinfo.SetHasCompletedFCDemo();
			Singleton<AnalyticsManager>.Instance.LogFCDemoComplete();
		}
		if (GameState.Instance.BattleResolver != null)
		{
			GameState.Instance.BattleResolver.SetResult(PlayerType.User);
		}
		if (activeQuest.NisWinPostBattle != null)
		{
			bool nisComplete = false;
			NisLaunchHelper nisLauncher = GetComponent<NisLaunchHelper>();
			nisLauncher.OnceComplete(delegate
			{
				nisComplete = true;
			});
			nisLauncher.LaunchNis(activeQuest.NisWinPostBattle);
			while (!nisComplete)
			{
				yield return null;
			}
		}
		yield return StartCoroutine(ReturnToMainMenu(0f));
	}

	private IEnumerator ReturnToMainMenu(float delay)
	{
		UICamera.useInputEnabler = true;
		AsyncOperation resUnloadYield = Resources.UnloadUnusedAssets();
		YieldInstruction waitYield = null;
		if (delay > 0f)
		{
			waitYield = new WaitForSeconds(delay);
		}
		if (waitYield != null)
		{
			yield return waitYield;
		}
		float savedTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		while (!resUnloadYield.isDone)
		{
			yield return null;
		}
		Time.timeScale = savedTimeScale;
		UICamera.useInputEnabler = false;
		GlobalFlags.Instance.ReturnToMainMenu = true;
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("AdventureTime");
	}

	private void PrepForNISHack()
	{
		CameraHack();
		AudioHack();
		PauseBattle();
	}

	private void CameraHack()
	{
		List<string> list = new List<string>();
		list.Add("Battle UI Camera");
		list.Add("Battle BattleCamera P1");
		list.Add("Battle BattleCamera P2");
		list.Add("Battle GameCamera");
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			camera.enabled = list.Contains(camera.gameObject.name);
		}
	}

	private void AudioHack()
	{
		AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
		AudioSource[] array2 = array;
		foreach (AudioSource audioSource in array2)
		{
			if (null != audioSource)
			{
				audioSource.Stop();
			}
		}
	}

	private void PauseBattle()
	{
		BattleManagerScript.GetInstance().BattlePaused = true;
	}
}
