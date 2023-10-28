using System.Collections;
using UnityEngine;

public class CWBattleSequenceController : MonoBehaviour
{
	public Camera mainGameCam;

	public Transform mainGameCamTarget;

	public Camera battleCamP1;

	public Transform battleCamP1Target;

	public Camera battleCamP2;

	public Transform battleCamP2Target;

	public GameObject battleCamP1ResetPos;

	public GameObject battleCamP2ResetPos;

	public UILabel tapToAttack;

	public UISprite barSprite;

	public UISprite baseSprite;

	public UISprite baseSpriteOutline;

	public UISprite baseSpriteDupFX;

	public UISprite undefendedSprite;

	public UISprite critAreaSprite;

	public UISprite hitAreaSprite;

	public UISprite hitAreaFXSprite;

	public GameObject hitAreaEdgeStart;

	public GameObject hitAreaEdgeEnd;

	public GameObject critAreaEdgeEnd;

	public bool animateFlag;

	public float totalTime = 2f;

	public float damageModifierCrit;

	public float critAreaStart;

	public float critAreaEnd;

	public float hitAreaStart;

	public float hitAreaEnd;

	public GameObject critTweenTarget;

	public GameObject hitTweenTarget;

	public GameObject missTweenTarget;

	public GameObject awayTweenTarget;

	public GameObject ringStartTweenTarget;

	public GameObject[] critDamageTweenTargets;

	public GameObject[] hitDamageTweenTargets;

	public GameObject[] missDamageTweenTargets;

	public GameObject missNoCreatureDamageTweenTarget;

	public PlayerType player;

	public int lane;

	private GameObject currentCreature;

	private GameObject opponentCreature;

	private GameState GameInstance;

	private LeaderForm leaderForm;

	private CreatureManagerScript creatureMgr;

	private BattleManagerScript battleMgr;

	private BattlePhaseManager phaseMgr;

	private CWBattleSequenceCam battleCamScriptP1;

	private CWBattleSequenceCam battleCamScriptP2;

	private DebugFlagsScript debugFlag;

	private static CWBattleSequenceController g_battleSqController;

	public bool camAlignFlag;

	private float lowResFov = 72f;

	public bool undefendedFlag;

	public bool noAttackFlag;

	private bool _keyPressed;

	public string result;

	public bool forceMiss;

	public bool forceCrit;

	public bool timeOutFlag;

	public GameObject tweenTargetPr;

	public float currentAngle;

	private float time;

	public bool ignoreTimeScale = true;

	private float mRt;

	private float mTimeStart;

	private float mTimeDelta;

	private float mActual;

	private bool mTimeStarted;

	private bool mStarted;

	private float mStartTime;

	private float debugTotalDelay;

	private float deltaTime;

	public int round;

	public int roundLimit = 1000000;

	public float realTime
	{
		get
		{
			return mRt;
		}
	}

	private void Awake()
	{
		g_battleSqController = this;
	}

	public static CWBattleSequenceController GetInstance()
	{
		return g_battleSqController;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		creatureMgr = CreatureManagerScript.GetInstance();
		battleMgr = BattleManagerScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
		battleCamScriptP1 = battleCamP1.GetComponent<CWBattleSequenceCam>();
		battleCamScriptP2 = battleCamP2.GetComponent<CWBattleSequenceCam>();
		debugFlag = DebugFlagsScript.GetInstance();
	}

	private void SplitCameraEnable(bool enable)
	{
		camAlignFlag = enable;
		Camera component = battleCamP1.GetComponent<Camera>();
		Camera component2 = battleCamP2.GetComponent<Camera>();
		component.enabled = enable;
		component2.enabled = enable;
		battleCamP1.rect = new Rect(0f, 0f, 0.5f, 1f);
		battleCamP2.rect = new Rect(0.5f, 0f, 0.5f, 1f);
		if (enable && SLOTGame.IsLowEndDevice())
		{
			battleCamP1.rect = new Rect(0f, 0f, 1f, 1f);
			battleCamP1.fieldOfView = lowResFov;
			component2.enabled = false;
		}
		mainGameCam.GetComponent<Camera>().enabled = !enable;
	}

	private void HandleBattlePerformanceThrottle(bool enterBattle)
	{
		if (SLOTGame.IsLowEndDevice())
		{
			PerfThrottleManager.HandlePerfThrottleEvent(PerfThrottleManager.PerfEvents.BATTLE_SEQUENCE, !enterBattle);
			for (int i = 0; i < 4; i++)
			{
				PerfThrottleManager.PerfEvents perfEvent = (PerfThrottleManager.PerfEvents)(2 + i);
				PerfThrottleManager.HandlePerfThrottleEvent(perfEvent, !enterBattle);
			}
		}
	}

	public void BattleSequence()
	{
		player = ((phaseMgr.Phase != BattlePhase.P1Battle) ? PlayerType.Opponent : PlayerType.User);
		StartCoroutine(BattleSequencePlay());
	}

	private IEnumerator BattleSequencePlay()
	{
		bool skipFlag = false;
		int creatureCount = 0;
		for (int i = 0; i < 4; i++)
		{
			if (GameInstance.LaneHasCreature(player, i))
			{
				CreatureScript creatureScript = GameInstance.GetCreature(player, i);
				creatureScript.DamageLastTurn = 0;
				creatureCount++;
			}
		}
		if (creatureCount == 0)
		{
			skipFlag = true;
		}
		if (!skipFlag)
		{
			HandleBattlePerformanceThrottle(true);
			SplitCameraEnable(true);
			PerfThrottleManager.PerfEvents prevPerfEvent = PerfThrottleManager.PerfEvents.NONE;
			for (int j = 0; j < 4; j++)
			{
				while (BattleManagerScript.GetInstance().BattlePaused)
				{
					yield return null;
				}
				lane = ((phaseMgr.Phase != BattlePhase.P1Battle) ? (3 - j) : j);
				if (prevPerfEvent != 0)
				{
					PerfThrottleManager.HandlePerfThrottleEvent(prevPerfEvent, false);
				}
				PerfThrottleManager.PerfEvents perfEvent = (PerfThrottleManager.PerfEvents)(2 + j);
				PerfThrottleManager.HandlePerfThrottleEvent(perfEvent, true);
				prevPerfEvent = perfEvent;
				if (!GameInstance.LaneHasCreature(player, lane))
				{
					continue;
				}
				undefendedFlag = false;
				noAttackFlag = false;
				CreatureScript ActiveScript = GameInstance.GetCreature(player, lane);
				int OppositeIndex = ActiveScript.CurrentLane.OpponentLane.Index;
				if (!GameInstance.LaneHasCreature(!player, OppositeIndex) && !player == PlayerType.User)
				{
					forceMiss = true;
					undefendedFlag = true;
				}
				else if (ActiveScript.ATK == 0)
				{
					forceMiss = true;
					noAttackFlag = true;
				}
				else
				{
					forceMiss = false;
					if (debugFlag.battleDisplay.autoCrit)
					{
						forceCrit = true;
					}
					else
					{
						forceCrit = false;
					}
				}
				if (PlayerInfoScript.GetInstance().AutoBattleSetting)
				{
					result = ((phaseMgr.Phase != BattlePhase.P1Battle) ? "Miss" : "Hit");
					UpdateBattleCamera();
					yield return StartCoroutine(BattleAction());
				}
				else
				{
					ringStartTweenTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
					_keyPressed = false;
					yield return StartCoroutine(RingStart());
				}
			}
		}
		yield return StartCoroutine(BattleEnd(false));
	}

	private void DebugRingStart()
	{
		StartCoroutine(RingStart());
	}

	private IEnumerator RingStart()
	{
		while (BattleManagerScript.GetInstance().BattlePaused)
		{
			yield return null;
		}
		timeOutFlag = false;
		ResetTimerVariables();
		UpdateBattleCamera();
		leaderForm = GameInstance.GetDeck(PlayerType.User).Leader.Form;
		SetUpRing(leaderForm, lane);
		yield return new WaitForSeconds(1f);
		if (forceMiss || forceCrit)
		{
			yield return StartCoroutine(StopRing());
		}
		else
		{
			animateFlag = true;
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>());
			Time.timeScale = 0f;
			yield return StartCoroutine(WaitForKeyPress());
		}
		CWBattleRingTapDelegate tapDelegate = GetComponent<CWBattleRingTapDelegate>();
		if ((bool)tapDelegate)
		{
			tapDelegate.disableFlag = false;
		}
	}

	private IEnumerator WaitForKeyPress()
	{
		while (!_keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				yield return StartCoroutine(StopRing());
				break;
			}
			yield return 0;
			if (round == roundLimit)
			{
				yield return StartCoroutine(StopRing());
			}
		}
	}

	public IEnumerator BattleEnd(bool GameOver)
	{
		yield return null;
		Time.timeScale = 1f;
		StopAllCoroutines();
		HandleBattlePerformanceThrottle(false);
		if (!GameOver)
		{
			SplitCameraEnable(false);
		}
		if (phaseMgr.Phase == BattlePhase.P1Battle)
		{
			battleMgr.P1BattleFinished();
		}
		else
		{
			battleMgr.P2BattleFinished();
		}
	}

	private void ResetTimerVariables()
	{
		barSprite.transform.localRotation = Quaternion.Euler(Vector3.zero);
		deltaTime = 0f;
		mStarted = false;
		animateFlag = false;
		round = 0;
		_keyPressed = false;
	}

	private void SetUpRing(LeaderForm leader, int playerLane)
	{
		tapToAttack.text = ((phaseMgr.Phase != BattlePhase.P1Battle) ? KFFLocalization.Get("!!Q_5_TAPTOBLOCK") : KFFLocalization.Get("!!Q_5_TAPTOATTACK"));
		string colorValue = ((phaseMgr.Phase != BattlePhase.P1Battle) ? leader.Ring_P2_HitColor : leader.Ring_P1_HitColor);
		Color colorRGBA = GetColorRGBA(colorValue);
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		float num;
		if (phaseMgr.Phase == BattlePhase.P1Battle)
		{
			float? ring_P1_HitAreaRange = leader.Ring_P1_HitAreaRange;
			num = ring_P1_HitAreaRange.Value + activeQuest.P1_HitAreaMod;
		}
		else
		{
			float? ring_P2_HitAreaRange = leader.Ring_P2_HitAreaRange;
			num = ring_P2_HitAreaRange.Value + activeQuest.P2_HitAreaMod;
		}
		float num2 = num;
		float num3;
		if (phaseMgr.Phase == BattlePhase.P1Battle)
		{
			float? ring_P1_CritAreaRange = leader.Ring_P1_CritAreaRange;
			num3 = ring_P1_CritAreaRange.Value + activeQuest.P1_CritAreaMod;
		}
		else
		{
			float? ring_P2_CritAreaRange = leader.Ring_P2_CritAreaRange;
			num3 = ring_P2_CritAreaRange.Value + activeQuest.P2_CritAreaMod;
		}
		float num4 = num3;
		if (phaseMgr.Phase == BattlePhase.P1Battle)
		{
			num2 += num2 * GameInstance.HitAreaModifier;
			num4 += num4 * GameInstance.CritAreaModifier;
		}
		else
		{
			num2 += num2 * GameInstance.DefenseAreaModifier;
			num4 += num4 * GameInstance.DefenseAreaCritModifier;
		}
		if (num2 + num4 > 1f)
		{
			num2 = 1f - num4;
		}
		if (phaseMgr.Phase == BattlePhase.P1Battle && !GameInstance.LaneHasCreature(PlayerType.Opponent, 3 - playerLane))
		{
			num2 = 1f - num4;
		}
		hitAreaStart = Random.Range(0f, 1f - num2 - num4);
		hitAreaEnd = hitAreaStart + num2;
		SetAreaObj(hitAreaStart, num2, hitAreaSprite, colorRGBA);
		SetAreaObj(hitAreaStart, num2, hitAreaFXSprite, colorRGBA);
		baseSprite.spriteName = ((phaseMgr.Phase != BattlePhase.P1Battle) ? leaderForm.Ring_P2_BGSprite : leaderForm.Ring_P1_BGSprite);
		baseSpriteDupFX.spriteName = ((phaseMgr.Phase != BattlePhase.P1Battle) ? leaderForm.Ring_P2_BGSprite : leaderForm.Ring_P1_BGSprite);
		colorValue = ((phaseMgr.Phase != BattlePhase.P1Battle) ? leader.Ring_P2_BGColor : leader.Ring_P1_BGColor);
		colorRGBA = GetColorRGBA(colorValue);
		SetAreaObj(hitAreaStart, 1f, baseSprite, colorRGBA);
		SetAreaObj(hitAreaStart, 1f, baseSpriteDupFX, colorRGBA);
		SetAreaObj(hitAreaStart, 1f, baseSpriteOutline, Color.white);
		colorValue = ((phaseMgr.Phase != BattlePhase.P1Battle) ? leader.Ring_P2_CritColor : leader.Ring_P1_CritColor);
		colorRGBA = GetColorRGBA(colorValue);
		critAreaStart = hitAreaEnd;
		critAreaEnd = hitAreaEnd + num4;
		num2 = critAreaEnd - hitAreaStart;
		SetAreaObj(hitAreaStart, num2, critAreaSprite, colorRGBA);
		RotateRingObj(hitAreaStart, hitAreaEdgeStart);
		RotateRingObj(hitAreaEnd, hitAreaEdgeEnd);
		RotateRingObj(critAreaEnd, critAreaEdgeEnd);
		barSprite.spriteName = ((phaseMgr.Phase != BattlePhase.P1Battle) ? leaderForm.Ring_P2_BarSprite : leaderForm.Ring_P1_BarSprite);
		float? timeFor1SpinMin = leader.TimeFor1SpinMin;
		float min = timeFor1SpinMin.Value * activeQuest.SpinFactor;
		float? timeFor1SpinMax = leader.TimeFor1SpinMax;
		totalTime = Random.Range(min, timeFor1SpinMax.Value * activeQuest.SpinFactor);
		if (debugFlag.battleDisplay.superSlowBattleRing)
		{
			totalTime = 6f;
		}
		float? critDamageMod = leader.CritDamageMod;
		damageModifierCrit = critDamageMod.Value;
		TweenColor component = hitAreaFXSprite.GetComponent<TweenColor>();
		component.to = new Color(hitAreaFXSprite.color.r, hitAreaFXSprite.color.g, hitAreaFXSprite.color.b, 0f);
		hitAreaFXSprite.gameObject.SetActive(false);
	}

	private void RotateRingObj(float f, GameObject obj)
	{
		float z = -360f * f;
		obj.transform.localRotation = Quaternion.Euler(0f, 0f, z);
	}

	private void SetAreaObj(float start, float range, UISprite sp, Color color)
	{
		RotateRingObj(start, sp.gameObject);
		sp.fillAmount = range;
		sp.color = color;
	}

	private Color GetColorRGBA(string colorValue)
	{
		Color color = new Color(255f, 255f, 255f, 255f);
		string[] array = colorValue.Split(':');
		float r = float.Parse(array[0]) / 255f;
		float g = float.Parse(array[1]) / 255f;
		float b = float.Parse(array[2]) / 255f;
		float a = float.Parse(array[3]) / 255f;
		return new Color(r, g, b, a);
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
		}
	}

	private IEnumerator StopRing()
	{
		Time.timeScale = 1f;
		_keyPressed = true;
		if (timeOutFlag)
		{
			result = "Miss";
		}
		else if (forceMiss)
		{
			result = "Miss";
		}
		else if (forceCrit)
		{
			result = "Crit";
		}
		else if (currentAngle >= hitAreaStart && currentAngle <= hitAreaEnd)
		{
			result = "Hit";
		}
		else if (currentAngle >= critAreaStart && currentAngle <= critAreaEnd)
		{
			result = "Crit";
		}
		else
		{
			result = "Miss";
		}
		GetTweenTarget(result).SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		animateFlag = false;
		yield return new WaitForSeconds(0.3f);
		GetComponent<AudioSource>().Stop();
		awayTweenTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds(0.2f);
		yield return StartCoroutine(BattleAction());
	}

	private IEnumerator BattleAction()
	{
		while (BattleManagerScript.GetInstance().BattlePaused)
		{
			yield return null;
		}
		if (!(creatureMgr != null))
		{
			yield break;
		}
		currentCreature = creatureMgr.Instances[(int)player, lane, 0];
		opponentCreature = creatureMgr.Instances[1 - (int)player, 3 - lane, 0];
		CreatureScript ActiveScript = GameInstance.GetCreature(player, lane);
		GameObject damageTweenTarget = GetTweenTarget(result + "Damage");
		int OppositeIndex = ActiveScript.CurrentLane.OpponentLane.Index;
		bool stop = true;
		if (GameInstance.LaneHasCreature(!player, OppositeIndex))
		{
			CreatureScript OpponentScript = GameInstance.GetCreature(!player, OppositeIndex);
			if (!OpponentScript.Protected && !ActiveScript.CantAttack)
			{
				stop = false;
			}
			if (player == PlayerType.User && result == "Miss")
			{
				damageTweenTarget = GetTweenTarget(result + "DamageNoCreature");
			}
		}
		else
		{
			if (!ActiveScript.CantAttack)
			{
				stop = false;
			}
			if (player == PlayerType.User && result == "Miss")
			{
				damageTweenTarget = GetTweenTarget(result + "DamageNoCreature");
			}
		}
		if (!stop)
		{
			SetCreatureBattleActionTarget();
			CWBattleTweenDamageSq seqArray = damageTweenTarget.GetComponent<CWBattleTweenDamageSq>();
			GameObject[] tweenSequence = seqArray.tweenSequence;
			foreach (GameObject tw in tweenSequence)
			{
				tw.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				yield return new WaitForSeconds(0.2f);
			}
		}
		yield return new WaitForSeconds(0.5f);
		if (GameInstance.GetHealth(!player) == 0)
		{
			yield return StartCoroutine(BattleEnd(true));
		}
	}

	private void SetCreatureBattleActionTarget()
	{
		GameObject gameObject = ((phaseMgr.Phase != BattlePhase.P1Battle) ? opponentCreature : currentCreature);
		GameObject gameObject2 = ((phaseMgr.Phase != BattlePhase.P1Battle) ? currentCreature : opponentCreature);
		UIButtonMessage[] componentsInChildren = tweenTargetPr.GetComponentsInChildren<UIButtonMessage>();
		UIButtonMessage[] array = componentsInChildren;
		foreach (UIButtonMessage uIButtonMessage in array)
		{
			uIButtonMessage.target = ((!uIButtonMessage.name.Contains("_P1_")) ? gameObject2 : gameObject);
		}
		CWBattleRingPlayAnim[] componentsInChildren2 = tweenTargetPr.GetComponentsInChildren<CWBattleRingPlayAnim>();
		CWBattleRingPlayAnim[] array2 = componentsInChildren2;
		foreach (CWBattleRingPlayAnim cWBattleRingPlayAnim in array2)
		{
			cWBattleRingPlayAnim.animTarget = ((!cWBattleRingPlayAnim.name.Contains("_P1_")) ? gameObject2 : gameObject);
		}
	}

	private GameObject GetTweenTarget(string result)
	{
		GameObject gameObject = null;
		switch (result)
		{
		case "Crit":
			gameObject = critTweenTarget;
			if (BattlePhaseManager.GetInstance().Phase == BattlePhase.P1Battle)
			{
				Singleton<AnalyticsManager>.Instance.IncBR_CritCount();
			}
			else if (BattlePhaseManager.GetInstance().Phase == BattlePhase.P2Battle)
			{
				Singleton<AnalyticsManager>.Instance.IncBR_CounterCount();
			}
			break;
		case "Hit":
			gameObject = hitTweenTarget;
			if (BattlePhaseManager.GetInstance().Phase == BattlePhase.P1Battle)
			{
				Singleton<AnalyticsManager>.Instance.IncBR_HitCount();
			}
			else if (BattlePhaseManager.GetInstance().Phase == BattlePhase.P2Battle)
			{
				Singleton<AnalyticsManager>.Instance.IncBR_BlockCount();
			}
			break;
		case "Miss":
			gameObject = missTweenTarget;
			Singleton<AnalyticsManager>.Instance.IncBR_MissCount();
			break;
		case "CritDamage":
			gameObject = critDamageTweenTargets[(int)player];
			break;
		case "HitDamage":
			gameObject = hitDamageTweenTargets[(int)player];
			break;
		case "MissDamage":
			gameObject = missDamageTweenTargets[(int)player];
			break;
		case "MissDamageNoCreature":
			gameObject = missNoCreatureDamageTweenTarget;
			break;
		}
		return gameObject;
	}

	private void Update()
	{
		if (animateFlag)
		{
			if (ignoreTimeScale)
			{
				UpdateRealTimeDelta();
			}
			time = ((!ignoreTimeScale) ? Time.time : realTime);
			if (!mStarted)
			{
				mStarted = true;
				mStartTime = time;
			}
			if (!(time < mStartTime))
			{
				PlayRingTimer();
			}
		}
	}

	private void PlayRingTimer()
	{
		deltaTime = time - mStartTime;
		round = (int)(deltaTime / totalTime);
		currentAngle = deltaTime / totalTime - (float)round;
		barSprite.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle * -360f);
		if (deltaTime >= totalTime * (float)roundLimit)
		{
			animateFlag = false;
			timeOutFlag = true;
		}
	}

	protected float UpdateRealTimeDelta()
	{
		mRt = Time.realtimeSinceStartup;
		if (mTimeStarted)
		{
			float b = mRt - mTimeStart;
			mActual += Mathf.Max(0f, b);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			if (mTimeDelta > 1f)
			{
				mTimeDelta = 1f;
			}
			mTimeStart = mRt;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = mRt;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}

	private void UpdateBattleCamera()
	{
		int laneIndex = ((player != PlayerType.User) ? (3 - lane) : lane);
		battleCamScriptP1.MoveBattleCameraSplitScreen(laneIndex);
		battleCamScriptP2.MoveBattleCameraSplitScreen(laneIndex);
	}
}
