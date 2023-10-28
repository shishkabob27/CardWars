using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CWFloopActionManager : MonoBehaviour
{
	private GameState GameInstance;

	private CreatureManagerScript creatureMgr;

	private PanelManagerBattle panelMgrBattle;

	private BattlePhaseManager phaseMgr;

	public List<CreatureScript> targetScripts = new List<CreatureScript>();

	public int player;

	public int lane;

	public GameObject numberDisplayObj;

	public CardItem card;

	public Animation anim;

	public AudioClip floopSound;

	public GameObject floopActionCamTarget;

	public GameObject floopActionCamLookAtTarget;

	public UIButtonTween opponentFloopPanelTween;

	public GameObject[] heroTargetObjeccts;

	public GameObject[] magicTargetObjects;

	public GameObject[] costTargetObjects;

	public GameObject spawnFXCameraCenter;

	public float heroFxWaitTime1 = 1f;

	public float heroFxWaitTime2 = 2f;

	public GameObject floopFX;

	public float detailCameraOffsetX = 15f;

	public float detailCameraOffsetY = 15f;

	public float floopCameraOffsetX = 10f;

	public float floopCameraOffsetY = 10f;

	public float floopCameraTargetOffsetY = 3f;

	public GameObject floopButtonFXIdle;

	public GameObject floopButtonFXAction;

	public GameObject floopPanel;

	public GameObject spellPanel;

	public Transform[] PlayerNeutralPoints;

	public Transform[] OpponentNeutralPoints;

	private Dictionary<CardScript, GameObject> persistantFX;

	private GameObject[,] persistantFXList = new GameObject[2, 4];

	private Dictionary<CardScript, string> persistantContext;

	private static CWFloopActionManager g_floopManager;

	public Camera BattleCamera;

	public Transform BattleCameraTarget;

	public CWFloopPrompt floopPrompt;

	public Transform[] PlayerStatusEffects;

	public Transform[] OpponentStatusEffects;

	public GameObject DeckTarget;

	public bool usingSpellCamera;

	public Camera SpellCamera;

	public Transform BannerEffect;

	private void Awake()
	{
		g_floopManager = this;
	}

	public static CWFloopActionManager GetInstance()
	{
		return g_floopManager;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		creatureMgr = CreatureManagerScript.GetInstance();
		panelMgrBattle = PanelManagerBattle.GetInstance();
		persistantFX = new Dictionary<CardScript, GameObject>();
		persistantContext = new Dictionary<CardScript, string>();
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public IEnumerator PlayFloopAction()
	{
		GameObject target = creatureMgr.Spawn_Points[player, lane, 0].gameObject;
		FillFloopInfo();
		FloopCameraTrigger(target, false);
		yield return StartCoroutine(DoWaitThenTrigger(1f, anim, "Floop"));
	}

	private void FloopPanelInfoSet()
	{
		CWCommandCardFill component = floopPanel.GetComponent<CWCommandCardFill>();
		component.lane = lane;
		component.playerType = player;
		component.creatureFlag = true;
	}

	private void AlignCameraTargets(int lane)
	{
	}

	private IEnumerator DoWaitThenTrigger(float waitTime, Animation anim, string animName)
	{
		PlayAnimOnce(anim, animName);
		UICamera.useInputEnabler = true;
		FloopFX();
		yield return new WaitForSeconds(waitTime);
		GameInstance.FloopCard(player, lane, CardType.Creature);
	}

	private void FloopFX()
	{
		GameObject gameObject = creatureMgr.Spawn_Points[player, lane, 0].gameObject;
		SLOTGame.InstantiateFX(floopFX, gameObject.transform.position, Quaternion.identity);
	}

	private void PlayAnimOnce(Animation anim, string animName)
	{
		if (anim != null && animName != string.Empty)
		{
			string animName2 = GetAnimName(anim, animName);
			if (animName2 != string.Empty)
			{
				anim.Play(animName2);
			}
			SummonScript component = anim.gameObject.GetComponent<SummonScript>();
			if (component.Idle != string.Empty)
			{
				anim.CrossFadeQueued(component.Idle);
			}
		}
	}

	private string GetAnimName(Animation anim, string str)
	{
		string empty = string.Empty;
		foreach (AnimationState item in anim)
		{
			if (item.name == str || item.name.EndsWith(str))
			{
				empty = item.name;
			}
		}
		return empty;
	}

	public void DoEffect(CardScript source, CardScript[] targets)
	{
		UICamera.useInputEnabler = true;
		if (source is LeaderScript)
		{
			phaseMgr.Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2LeaderAbilityAction : BattlePhase.P1LeaderAbilityAction);
		}
		else if (source.Data.Form.Type != CardType.Spell)
		{
			phaseMgr.Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2FloopAction : BattlePhase.P1FloopAction);
		}
		else
		{
			phaseMgr.Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2SpellCreature : BattlePhase.P1SpellCreature);
		}
		string stringFromJson = GetStringFromJson(source.Data.Form.ScriptVizName, "TargetCreatureAnimDelay");
		float waitTime = ((!(stringFromJson != string.Empty)) ? 0f : float.Parse(stringFromJson));
		StartCoroutine(DoSequenceSpawnCreature(waitTime, source, targets));
	}

	public void DoEffect(CardScript source, PlayerType player)
	{
		if (source is LeaderScript)
		{
			phaseMgr.Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2LeaderAbilityActionHero : BattlePhase.P1LeaderAbilityActionHero);
		}
		else if (source.Data.Form.Type != CardType.Spell)
		{
			phaseMgr.Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2FloopActionHero : BattlePhase.P1FloopActionHero);
		}
		else
		{
			phaseMgr.Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2SpellHero : BattlePhase.P1SpellHero);
		}
		StartCoroutine(DoSequenceSpawnHero(heroFxWaitTime1, heroFxWaitTime2, source, player));
	}

	public void DoEffectNeutral(CardScript source, CardScript[] targets)
	{
		BattlePhaseManager.GetInstance().Phase = ((player != (int)PlayerType.User) ? BattlePhase.P2FloopAction : BattlePhase.P1FloopAction);
		string stringFromJson = GetStringFromJson(source.Data.Form.ScriptVizName, "TargetCreatureAnimDelay");
		float waitTime = ((!(stringFromJson != string.Empty)) ? 0f : float.Parse(stringFromJson));
		StartCoroutine(DoSequenceSpawnNeutral(waitTime, source, targets));
	}

	public void DoEffect(CardScript source, int lane)
	{
		BattlePhaseManager.GetInstance().Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2FloopAction : BattlePhase.P1FloopAction);
		StartCoroutine(DoSequenceSpawnPersistent(source, lane));
	}

	private void TriggerSpawnFX(string scriptVizName, GameObject target, string colName, CardScript source)
	{
		TriggerSpawnFX(scriptVizName, target, colName, source, -1);
	}

	private void TriggerSpawnFX(string scriptVizName, GameObject target, string colName, CardScript source, int lane)
	{
		string stringFromJson = GetStringFromJson(scriptVizName, colName);
		string stringFromJson2 = GetStringFromJson(scriptVizName, "PersistFX");
		string stringFromJson3 = GetStringFromJson(scriptVizName, "ContextFX");
		bool persist = stringFromJson2 != null && stringFromJson2 != string.Empty;
		GameObject spawnedFXPersist = GetSpawnedFXPersist(stringFromJson, target, persist, source, stringFromJson3, lane);
		if (spawnedFXPersist != null)
		{
			Camera componentInChildren = spawnedFXPersist.GetComponentInChildren<Camera>();
			if (componentInChildren != null)
			{
				usingSpellCamera = true;
				SpellCamera = componentInChildren;
			}
		}
	}

	private void TriggerSpawnFXWithRotation(string scriptVizName, GameObject target, string colName, CardScript source, Quaternion rotation)
	{
		string stringFromJson = GetStringFromJson(scriptVizName, colName);
		string stringFromJson2 = GetStringFromJson(scriptVizName, "PersistFX");
		string stringFromJson3 = GetStringFromJson(scriptVizName, "ContextFX");
		bool persist = stringFromJson2 != null && stringFromJson2 != string.Empty;
		GameObject spawnedFXPersist = GetSpawnedFXPersist(stringFromJson, target, persist, source, stringFromJson3);
		if (spawnedFXPersist != null)
		{
			spawnedFXPersist.transform.rotation = rotation;
			Camera componentInChildren = spawnedFXPersist.GetComponentInChildren<Camera>();
			if (componentInChildren != null)
			{
				usingSpellCamera = true;
				SpellCamera = componentInChildren;
			}
		}
	}

	private void TriggerTrailFX(string scriptVizName, GameObject startObj, GameObject endObj, CardScript source)
	{
		string stringFromJson = GetStringFromJson(scriptVizName, "TrailFX");
		string stringFromJson2 = GetStringFromJson(scriptVizName, "PersistFX");
		string stringFromJson3 = GetStringFromJson(scriptVizName, "ContextFX");
		bool persist = stringFromJson2 != null && stringFromJson2 != string.Empty;
		GameObject spawnedFXPersist = GetSpawnedFXPersist(stringFromJson, startObj, persist, source, stringFromJson3);
		if (!(spawnedFXPersist == null))
		{
			Camera componentInChildren = spawnedFXPersist.GetComponentInChildren<Camera>();
			if (componentInChildren != null)
			{
				usingSpellCamera = true;
				SpellCamera = componentInChildren;
			}
			TweenPosition tweenPosition = spawnedFXPersist.GetComponent<TweenPosition>();
			if (tweenPosition == null)
			{
				tweenPosition = spawnedFXPersist.AddComponent<TweenPosition>();
			}
			tweenPosition.from = new Vector3(startObj.transform.position.x * 0.1f, startObj.transform.position.y * 0.1f, startObj.transform.position.z * 0.1f);
			tweenPosition.to = new Vector3(endObj.transform.position.x * 0.1f, endObj.transform.position.y * 0.1f, endObj.transform.position.z * 0.1f);
			tweenPosition.duration = heroFxWaitTime2;
			tweenPosition.Play(true);
			Object.Destroy(spawnedFXPersist, heroFxWaitTime2);
		}
	}

	private GameObject TriggerImpactFX(string scriptVizName, GameObject target, string colName, CardScript source)
	{
		string stringFromJson = GetStringFromJson(scriptVizName, colName);
		string stringFromJson2 = GetStringFromJson(scriptVizName, "PersistFX");
		string stringFromJson3 = GetStringFromJson(scriptVizName, "ContextFX");
		bool persist = stringFromJson2 != null && stringFromJson2 != string.Empty;
		GameObject spawnedFXPersist = GetSpawnedFXPersist(stringFromJson, target, persist, source, stringFromJson3);
		if (spawnedFXPersist == null)
		{
			return null;
		}
		Camera componentInChildren = spawnedFXPersist.GetComponentInChildren<Camera>();
		if (componentInChildren != null)
		{
			usingSpellCamera = true;
			SpellCamera = componentInChildren;
		}
		return spawnedFXPersist;
	}

	private IEnumerator DoSequenceSpawnHero(float waitTime1, float waitTime2, CardScript source, PlayerType player)
	{
		string magicFX = GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXMagic");
		string costFX = GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXCost");
		string cardFX = GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXCard");
		string bannerFX = GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXBanner");
		string statusFX = GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXEnemyStatus");
		float waitTime3 = ((!(GetStringFromJson(source.Data.Form.ScriptVizName, "DelayAfterAnimation") == string.Empty)) ? float.Parse(GetStringFromJson(source.Data.Form.ScriptName, "DelayAfterAnimation")) : 0f);
		if (bannerFX != string.Empty && player == PlayerType.User && DeckTarget != null)
		{
			TriggerImpactFX(source.Data.Form.ScriptVizName, BannerEffect.gameObject, "SpawnFXBanner", source);
		}
		if (magicFX != string.Empty)
		{
			GameObject magicTarget = magicTargetObjects[(int)player];
			yield return new WaitForSeconds(waitTime3);
			TriggerImpactFX(source.Data.Form.ScriptVizName, magicTarget, "SpawnFXMagic", source);
		}
		else if (costFX != string.Empty && player == PlayerType.User)
		{
			int count = 0;
			GameObject[] array = costTargetObjects;
			foreach (GameObject target2 in array)
			{
				if (count < GameInstance.GetHand(PlayerType.User).Count)
				{
					TriggerImpactFX(source.Data.Form.ScriptVizName, target2, "SpawnFXCost", source);
					count++;
				}
			}
		}
		else if (cardFX != string.Empty && player == PlayerType.User)
		{
			if (DeckTarget != null)
			{
				TriggerImpactFX(source.Data.Form.ScriptVizName, DeckTarget, "SpawnFXCard", source);
			}
		}
		else if (statusFX != string.Empty)
		{
			if (player == PlayerType.Opponent)
			{
				for (int ii2 = 0; ii2 < OpponentStatusEffects.Length; ii2++)
				{
					if (OpponentStatusEffects[ii2] != null && OpponentStatusEffects[ii2].childCount <= 0)
					{
						GameObject obj2 = TriggerImpactFX(source.Data.Form.ScriptVizName, OpponentStatusEffects[ii2].gameObject, "SpawnFXEnemyStatus", source);
						obj2.transform.parent = OpponentStatusEffects[ii2];
						break;
					}
				}
			}
			else if (player == PlayerType.User)
			{
				for (int ii = 0; ii < PlayerStatusEffects.Length; ii++)
				{
					if (PlayerStatusEffects[ii] != null && PlayerStatusEffects[ii].childCount <= 0)
					{
						GameObject obj = TriggerImpactFX(source.Data.Form.ScriptVizName, PlayerStatusEffects[ii].gameObject, "SpawnFXEnemyStatus", source);
						obj.transform.parent = PlayerStatusEffects[ii];
						break;
					}
				}
			}
		}
		else
		{
			TriggerSpawnFX(source.Data.Form.ScriptVizName, spawnFXCameraCenter, "SpawnFX", source);
			yield return new WaitForSeconds(waitTime1);
			GameObject target = heroTargetObjeccts[(int)player];
			TriggerTrailFX(source.Data.Form.ScriptVizName, spawnFXCameraCenter, target, source);
			yield return new WaitForSeconds(waitTime2);
			TriggerImpactFX(source.Data.Form.ScriptVizName, target, "ImpactFX", source);
		}
		bool resumeOnComplete = source.DoResult(null);
		CWPlayerHandsController.GetInstance().UpdateCards(GameInstance.GetHand(PlayerType.User));
		if (resumeOnComplete)
		{
			string cam = GetStringFromJson(source.Data.Form.ScriptVizName, "FloopCameraDelay");
			float delayCamera = ((cam != null && !(cam == string.Empty)) ? float.Parse(cam) : 1f);
			yield return new WaitForSeconds(delayCamera);
			if (CWLootingSequencer.GetInstance().chestLanes.Count != 0)
			{
				BattlePhaseManager.GetInstance().Phase = BattlePhase.LootAfterP2Battle;
			}
			else
			{
				BattlePhaseManager.GetInstance().Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2Setup : BattlePhase.P1Setup);
			}
			CWOpponentActionSequencer.GetInstance().resumeFlag = true;
			UICamera.useInputEnabler = false;
		}
	}

	private void FloopCameraTrigger(GameObject target, bool targetOffset)
	{
		if (targetOffset)
		{
			floopActionCamLookAtTarget.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + floopCameraTargetOffsetY, target.transform.position.z);
		}
		else
		{
			floopActionCamLookAtTarget.transform.position = target.transform.position;
		}
		floopActionCamTarget.transform.position = new Vector3(target.transform.position.x - floopCameraOffsetX, target.transform.position.y + floopCameraOffsetY, target.transform.position.z);
		CWiTweenCamTrigger component = GetComponent<CWiTweenCamTrigger>();
		component.tweenName = "ToFloopAction";
		component.PlayCam();
	}

	private IEnumerator DoSequenceSpawnCreature(float waitTime, CardScript source, CardScript[] targets)
	{
		bool resumeOnComplete = true;
		foreach (CardScript sc in targets)
		{
			int type = ((sc.Data.Form.Type != 0) ? 1 : 0);
			GameObject target = creatureMgr.Spawn_Points[(int)sc.Owner, sc.CurrentLane.Index, type].gameObject;
			source.TargetScript = sc;
			if (sc.Owner != source.Owner)
			{
				TriggerSpawnFX(source.Data.Form.ScriptVizName, target, "SpawnFX", source);
			}
			else
			{
				TriggerSpawnFX(source.Data.Form.ScriptVizName, target, "SpawnFXOwner", source);
			}
			if (GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXATK") != string.Empty)
			{
				UILabel atkLabel = null;
				UILabel[] labels = target.transform.GetComponentsInChildren<UILabel>();
				UILabel[] array = labels;
				foreach (UILabel lb2 in array)
				{
					if (lb2.name == "ATKLabel")
					{
						atkLabel = lb2;
						break;
					}
				}
				if (atkLabel != null)
				{
					TriggerSpawnFXWithRotation(source.Data.Form.ScriptVizName, atkLabel.gameObject, "SpawnFXATK", source, Quaternion.identity);
				}
				else
				{
					TriggerSpawnFXWithRotation(source.Data.Form.ScriptVizName, target, "SpawnFXATK", source, Quaternion.identity);
				}
			}
			if (GetStringFromJson(source.Data.Form.ScriptVizName, "SpawnFXDEF") != string.Empty)
			{
				UILabel defLabel = null;
				UILabel[] labels2 = target.transform.GetComponentsInChildren<UILabel>();
				UILabel[] array2 = labels2;
				foreach (UILabel lb in array2)
				{
					if (lb.name == "Slash")
					{
						defLabel = lb;
						break;
					}
				}
				if (defLabel != null)
				{
					TriggerSpawnFXWithRotation(source.Data.Form.ScriptVizName, defLabel.gameObject, "SpawnFXDEF", source, Quaternion.identity);
				}
				else
				{
					TriggerSpawnFXWithRotation(source.Data.Form.ScriptVizName, target, "SpawnFXDEF", source, Quaternion.identity);
				}
			}
			FloopCameraTrigger(target, true);
			if (waitTime > 0f)
			{
				yield return new WaitForSeconds(waitTime);
			}
			string num = GetStringFromJson(source.Data.Form.ScriptVizName, "DelayAfterAnimation");
			string animName2 = ((sc.Owner != source.Owner) ? GetStringFromJson(source.Data.Form.ScriptVizName, "TargetCreatureAnim") : GetStringFromJson(source.Data.Form.ScriptVizName, "OwnerCreatureAnim"));
			if (animName2 != string.Empty && sc.Data.Form.Type == CardType.Creature)
			{
				Animation anim2 = creatureMgr.Instances[(int)sc.Owner, sc.CurrentLane.Index, 0].gameObject.GetComponent<Animation>();
				PlayAnimOnce(anim2, animName2);
				if (num == string.Empty)
				{
					num = anim2[animName2].length.ToString();
				}
			}
			animName2 = ((sc.Owner != source.Owner) ? GetStringFromJson(source.Data.Form.ScriptVizName, "TargetBuildingAnim") : GetStringFromJson(source.Data.Form.ScriptVizName, "OwnerBuildingAnim"));
			if (animName2 != string.Empty && sc.Data.Form.Type == CardType.Building)
			{
				Animation anim = creatureMgr.Instances[(int)sc.Owner, sc.CurrentLane.Index, 1].gameObject.GetComponent<Animation>();
				PlayAnimOnce(anim, animName2);
				if (num == string.Empty)
				{
					num = anim[animName2].length.ToString();
				}
			}
			float delayAfterAnim = ((!(num == string.Empty)) ? float.Parse(num) : 0f);
			yield return new WaitForSeconds(delayAfterAnim);
			resumeOnComplete = source.DoResult(sc) && resumeOnComplete;
			yield return new WaitForSeconds(1f - waitTime - delayAfterAnim);
		}
		if (resumeOnComplete)
		{
			string cam = GetStringFromJson(source.Data.Form.ScriptVizName, "FloopCameraDelay");
			float delayCamera = ((cam != null && !(cam == string.Empty)) ? float.Parse(cam) : 1f);
			yield return new WaitForSeconds(delayCamera);
			if (CWLootingSequencer.GetInstance().chestLanes.Count != 0)
			{
				BattlePhaseManager.GetInstance().Phase = BattlePhase.LootAfterP2Battle;
			}
			else
			{
				BattlePhaseManager.GetInstance().Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2Setup : BattlePhase.P1Setup);
			}
			CWOpponentActionSequencer.GetInstance().resumeFlag = true;
			UICamera.useInputEnabler = false;
		}
	}

	private IEnumerator DoSequenceSpawnPersistent(CardScript source, int lane)
	{
		GameObject target = PlayerNeutralPoints[lane].gameObject;
		if (source.Owner == PlayerType.User)
		{
			target = OpponentNeutralPoints[lane].gameObject;
		}
		TriggerSpawnFX(source.Data.Form.ScriptVizName, target, "SpawnFXNeutral", source, lane);
		FloopCameraTrigger(target, true);
		FlippedLandscapeScript flipScript = LandscapeManagerScript.GetInstance().FlippedScripts[(!source.Owner).IntValue, lane];
		flipScript.doNotStopHighlightFlag = true;
		SetColorForLaneBlock(true, flipScript.gameObject);
		string cam = GetStringFromJson(source.Data.Form.ScriptVizName, "FloopCameraDelay");
		float delayCamera = ((cam != null && !(cam == string.Empty)) ? float.Parse(cam) : 1f);
		yield return new WaitForSeconds(delayCamera);
		if (CWLootingSequencer.GetInstance().chestLanes.Count != 0)
		{
			BattlePhaseManager.GetInstance().Phase = BattlePhase.LootAfterP2Battle;
		}
		else
		{
			BattlePhaseManager.GetInstance().Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2Setup : BattlePhase.P1Setup);
		}
		CWOpponentActionSequencer.GetInstance().resumeFlag = true;
		UICamera.useInputEnabler = false;
	}

	private IEnumerator DoSequenceSpawnNeutral(float waitTime, CardScript source, CardScript[] targets)
	{
		GameObject target = null;
		if (targets.Length >= 1)
		{
			target = ((source.Owner == PlayerType.User) ? OpponentNeutralPoints[targets[0].CurrentLane.Index].gameObject : PlayerNeutralPoints[targets[0].CurrentLane.Index].gameObject);
			TriggerSpawnFX(source.Data.Form.ScriptVizName, target, "SpawnFXNeutral", source);
			FloopCameraTrigger(target, true);
		}
		if (targets.Length <= 0)
		{
			target = ((source.Owner == PlayerType.User) ? OpponentNeutralPoints[source.CurrentLane.Index].gameObject : PlayerNeutralPoints[source.CurrentLane.Index].gameObject);
			TriggerSpawnFX(source.Data.Form.ScriptVizName, target, "SpawnFXNeutral", source);
			FloopCameraTrigger(target, true);
		}
		bool resumeOnComplete = true;
		foreach (CardScript sc in targets)
		{
			int type = ((sc.Data.Form.Type != 0) ? 1 : 0);
			target = creatureMgr.Spawn_Points[(int)sc.Owner, sc.CurrentLane.Index, type].gameObject;
			FloopCameraTrigger(target, true);
			if (waitTime > 0f)
			{
				yield return new WaitForSeconds(waitTime);
			}
			string animName2 = ((sc.Owner != source.Owner) ? GetStringFromJson(source.Data.Form.ScriptVizName, "TargetCreatureAnim") : GetStringFromJson(source.Data.Form.ScriptVizName, "OwnerCreatureAnim"));
			if (animName2 != string.Empty && sc.Data.Form.Type == CardType.Creature)
			{
				Animation anim2 = creatureMgr.Instances[(int)sc.Owner, sc.CurrentLane.Index, 0].gameObject.GetComponent<Animation>();
				PlayAnimOnce(anim2, animName2);
			}
			animName2 = ((sc.Owner != source.Owner) ? GetStringFromJson(source.Data.Form.ScriptVizName, "TargetBuildingAnim") : GetStringFromJson(source.Data.Form.ScriptVizName, "OwnerBuildingAnim"));
			if (animName2 != string.Empty && sc.Data.Form.Type == CardType.Building)
			{
				Animation anim = creatureMgr.Instances[(int)sc.Owner, sc.CurrentLane.Index, 1].gameObject.GetComponent<Animation>();
				PlayAnimOnce(anim, animName2);
			}
			string num = GetStringFromJson(source.Data.Form.ScriptVizName, "DelayAfterAnimation");
			float delayAfterAnim = ((!(num == string.Empty)) ? float.Parse(num) : 0f);
			yield return new WaitForSeconds(delayAfterAnim);
			resumeOnComplete = source.DoResult(sc) && resumeOnComplete;
			yield return new WaitForSeconds(1f - waitTime - delayAfterAnim);
		}
		if (resumeOnComplete)
		{
			string cam = GetStringFromJson(source.Data.Form.ScriptVizName, "FloopCameraDelay");
			float delayCamera = ((cam != null && !(cam == string.Empty)) ? float.Parse(cam) : 1f);
			yield return new WaitForSeconds(delayCamera);
			if (CWLootingSequencer.GetInstance().chestLanes.Count != 0)
			{
				BattlePhaseManager.GetInstance().Phase = BattlePhase.LootAfterP2Battle;
			}
			else
			{
				BattlePhaseManager.GetInstance().Phase = ((source.Owner != PlayerType.User) ? BattlePhase.P2Setup : BattlePhase.P1Setup);
			}
			CWOpponentActionSequencer.GetInstance().resumeFlag = true;
			UICamera.useInputEnabler = false;
		}
	}

	public void FillFloopInfo()
	{
		FloopActionType actionType = FloopActionType.Floop;
		if (card.Form.Type == CardType.Spell)
		{
			actionType = FloopActionType.Spell;
			PanelManagerBattle.FillCardInfo(spellPanel, card, actionType);
		}
		else
		{
			PanelManagerBattle.FillCardInfo(floopPanel, card, actionType);
		}
	}

	private string GetStringFromJson(string str, string colName)
	{
		string result = string.Empty;
		Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_FloopActions.json");
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			if ((string)dictionary["FloopActionID"] == str)
			{
				result = (string)dictionary[colName];
				break;
			}
		}
		return result;
	}

	public GameObject GetSpawnedFX(string resourceName, GameObject target, bool parentFlag)
	{
		GameObject gameObject = null;
		gameObject = GetSpawnedFX(resourceName, target);
		if (parentFlag)
		{
			gameObject.transform.parent = target.transform;
		}
		return gameObject;
	}

	public GameObject GetSpawnedFX(string resourceName, GameObject target)
	{
		GameObject gameObject = null;
        //Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Props/" + resourceName);
		GameObject original = Resources.Load("Props/" + resourceName, typeof(GameObject)) as GameObject;
        if (original == null)
		{
            //original = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Particles/" + resourceName);
            original = Resources.Load("Particles/" + resourceName, typeof(GameObject)) as GameObject;
        }
		if (original == null)
		{
            //original = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("FloopActions/" + resourceName);
            original = Resources.Load("FloopActions/" + resourceName, typeof(GameObject)) as GameObject;
        }
		if (original != null)
		{
			gameObject = Instantiate(original, target.transform.position, target.transform.rotation) as GameObject;
			if (gameObject != null)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().UpdateAudioVolumes(gameObject);
			}
		}
		return gameObject;
	}

	public GameObject GetSpawnedFXPersist(string resourceName, GameObject target, bool persist, CardScript source, string context)
	{
		return GetSpawnedFXPersist(resourceName, target, persist, source, context, -1);
	}

	public GameObject GetSpawnedFXPersist(string resourceName, GameObject target, bool persist, CardScript source, string context, int lane)
	{
		GameObject gameObject = null;
		gameObject = GetSpawnedFX(resourceName, target);
		if (gameObject != null && persist)
		{
			SelfDestructScript component = gameObject.GetComponent<SelfDestructScript>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			if (source.TargetScript != null)
			{
				if (!source.TargetScript.FXList.ContainsKey(context))
				{
					source.TargetScript.FXList.Add(context, gameObject);
				}
			}
			else if (lane == -1)
			{
				GameState.Instance.CharacterFXList[(int)(!source.Owner)].Add(context, gameObject);
			}
			if (lane != -1)
			{
				if (persistantFXList[(int)(!source.Owner), lane] != null)
				{
					Object.Destroy(persistantFXList[(int)(!source.Owner), lane]);
				}
				persistantFXList[(int)(!source.Owner), lane] = gameObject;
			}
		}
		return gameObject;
	}

	public void RemoveVFX(GameObject vfx)
	{
		if (vfx != null)
		{
			Object.Destroy(vfx);
		}
	}

	public void RemovePersistantVFX(PlayerType player, int lane)
	{
		if (persistantFXList[player.IntValue, lane] != null)
		{
			Object.Destroy(persistantFXList[player.IntValue, lane]);
			StartCoroutine(DelayColorChangeOnLane(player, lane));
		}
	}

	private IEnumerator DelayColorChangeOnLane(PlayerType player, int lane)
	{
		FlippedLandscapeScript flipScript = LandscapeManagerScript.GetInstance().FlippedScripts[player.IntValue, lane];
		iTween.Stop(flipScript.gameObject);
		SetColorForLaneBlock(false, flipScript.gameObject);
		yield return new WaitForSeconds(0.5f);
		iTween.Stop(flipScript.gameObject);
		flipScript.gameObject.GetComponent<Renderer>().material.color = Color.clear;
		flipScript.doNotStopHighlightFlag = false;
	}

	public void RemovePersistantVFX(CardScript source, string context)
	{
		if (source != null && persistantFX.ContainsKey(source) && persistantContext.ContainsKey(source) && persistantContext[source] == context)
		{
			GameObject obj = persistantFX[source];
			persistantFX.Remove(source);
			persistantContext.Remove(source);
			Object.Destroy(obj);
		}
	}

	public void RemovePersistantVFX(string context)
	{
		if (context == null || !persistantContext.ContainsValue(context))
		{
			return;
		}
		CardScript cardScript = null;
		foreach (CardScript key in persistantContext.Keys)
		{
			if (persistantContext[key] == context)
			{
				cardScript = key;
				break;
			}
		}
		if (cardScript != null)
		{
			RemovePersistantVFX(cardScript, context);
		}
	}

	public void SetCameraPosition(Camera floopCam, Transform location, Transform target)
	{
		if (BattleCamera != null)
		{
			BattleCamera.transform.position = location.position;
			if (floopCam != null)
			{
				floopCam.gameObject.SetActive(false);
			}
			BattleCamera.gameObject.SetActive(true);
			usingSpellCamera = false;
			SpellCamera = null;
		}
		if (BattleCameraTarget != null)
		{
			BattleCameraTarget.position = target.position;
		}
	}

	public void TriggerLeader(PlayerType player)
	{
		StartCoroutine(DelayTriggerLeader(player));
	}

	private IEnumerator DelayTriggerLeader(PlayerType player)
	{
		yield return new WaitForSeconds(2f);
		GameState.Instance.UseLeaderAbility(player);
	}

	private void SetColorForLaneBlock(bool blocked, GameObject laneObj)
	{
		Material material = laneObj.GetComponent<Renderer>().material;
		Color color = ((!blocked) ? material.color : Color.red);
		material.color = color;
		if (blocked)
		{
			iTween.FadeTo(laneObj, iTween.Hash("alpha", 0f, "time", 0.5f, "looptype", iTween.LoopType.pingPong));
		}
		else
		{
			iTween.FadeTo(laneObj, iTween.Hash("alpha", 0f, "time", 1f, "looptype", iTween.LoopType.none));
		}
	}
}
