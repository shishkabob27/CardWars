using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManagerScript : MonoBehaviour
{
	public GameObject Coin;

	private GameObject NewCoin;

	public Transform[,,] Spawn_Points = new Transform[2, 4, 2];

	public GameObject[,,] Instances = new GameObject[2, 4, 2];

	public AudioClip CreatureDeath;

	public GameObject creatureHPBar;

	public bool OpponentOneHitKills;

	public bool PlayerOneHitKills;

	private GameState GameInstance;

	private CWiTweenVantageCam vcam;

	private KFFAnimationPoller animationPoller;

	private static CreatureManagerScript g_creatureManager;

	public Dictionary<string, int> creaturePrefabList = new Dictionary<string, int>();

	public Dictionary<string, int> BuildingPprefabList = new Dictionary<string, int>();

	private void Awake()
	{
		g_creatureManager = this;
	}

	public static CreatureManagerScript GetInstance()
	{
		return g_creatureManager;
	}

	private void Start()
	{
		vcam = CWiTweenVantageCam.GetInstance();
		CreaturesSetup();
	}

	private void CreaturesSetup()
	{
		GameInstance = GameState.Instance;
		GameInstance.CreatureManager = this;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					string[] array = new string[2] { "Creature", "Building" };
					string text = "P" + (i + 1) + "Lane" + (j + 1) + array[k];
					Spawn_Points[i, j, k] = GameObject.Find(text).transform;
				}
			}
		}
	}

	public void Summon(int player, int lane, CardForm data, CardScript cardScript)
	{
		GameObject gameObject = ((player != (int)PlayerType.User) ? BattlePhaseManager.GetInstance().tweenToP2SetupAction : BattlePhaseManager.GetInstance().tweenToP1SetupAction);
		CWCommandCardSet component = gameObject.GetComponent<CWCommandCardSet>();
		component.lane = lane + 1;
		component.playerType = player;
		switch (data.Type)
		{
		case CardType.Creature:
		{
			GameObject gameObject3 = (Instances[player, lane, 0] = SLOTGame.InstantiateGO(data.ObjectName, Spawn_Points[player, lane, 0].position, Spawn_Points[player, lane, 0].rotation));
			gameObject3.transform.parent = Spawn_Points[player, lane, 0];
			SummonScript componentInChildren2 = gameObject3.GetComponentInChildren<SummonScript>();
			componentInChildren2.cardScript = cardScript;
			CWHaloFaceCam componentInChildren3 = gameObject3.GetComponentInChildren<CWHaloFaceCam>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.player = player;
			}
			BattlePhaseManager.GetInstance().currentCreature = gameObject3.gameObject;
			CreatureBattleScript component4 = gameObject3.GetComponent<CreatureBattleScript>();
			component4.Player = player;
			component4.LaneIndex = lane;
			CWCreatureHPBarFaceCam componentInChildren4 = gameObject3.GetComponentInChildren<CWCreatureHPBarFaceCam>();
			GameObject gameObject4 = null;
			gameObject4 = ((!(componentInChildren4 != null)) ? (SLOTGame.InstantiateFX(creatureHPBar, Spawn_Points[player, lane, 0].position, Quaternion.identity) as GameObject) : componentInChildren4.gameObject);
			gameObject4.name = gameObject3.name + "_hpBar";
			componentInChildren2.hpBar = gameObject4;
			gameObject4.SetActive(false);
			CWCreatureHPBarFaceCam cWCreatureHPBarFaceCam = gameObject4.GetComponent(typeof(CWCreatureHPBarFaceCam)) as CWCreatureHPBarFaceCam;
			if (cWCreatureHPBarFaceCam != null)
			{
				cWCreatureHPBarFaceCam.target = gameObject3.gameObject;
				cWCreatureHPBarFaceCam.player = player;
				cWCreatureHPBarFaceCam.lane = lane;
			}
			SkinnedMeshRenderer[] componentsInChildren = gameObject3.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				skinnedMeshRenderer.updateWhenOffscreen = true;
			}
			vcam.target = gameObject3;
			UnityEngine.Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Particles/FX_RezIn_Creature");
			if (@object != null)
			{
				SLOTGame.InstantiateFX(@object, Spawn_Points[player, lane, 0].transform.position, Quaternion.identity);
			}
			CWCreatureStatsFloorDisplay cWCreatureStatsFloorDisplay = ((player != (int)PlayerType.User) ? PanelManagerBattle.GetInstance().P2FloorDisplays[lane] : PanelManagerBattle.GetInstance().P1FloorDisplays[lane]);
			NGUITools.SetActive(cWCreatureStatsFloorDisplay.gameObject, true);
			if (PanelManagerBattle.GetInstance().hpBarOnTheGround)
			{
				gameObject4.transform.parent = cWCreatureStatsFloorDisplay.gameObject.transform;
			}
			else
			{
				gameObject4.transform.parent = gameObject3.transform;
			}
			CWCommandCardSet component5 = Spawn_Points[player, lane, 0].GetComponent<CWCommandCardSet>();
			component5.creatureObj = gameObject3;
			component5.creatureFlag = true;
			component.creatureFlag = true;
			component.creatureObj = gameObject3;
			string text = null;
			if (componentInChildren2 != null)
			{
				text = componentInChildren2.Intro;
			}
			Animation componentInChildren5 = gameObject3.GetComponentInChildren<Animation>();
			if (!(componentInChildren5 != null))
			{
				break;
			}
			AnimationState animationState = null;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					animationState = componentInChildren5[text];
				}
				catch (Exception)
				{
					animationState = null;
				}
			}
			KFFAnimationPoller kFFAnimationPoller = gameObject3.AddComponent(typeof(KFFAnimationPoller)) as KFFAnimationPoller;
			if (kFFAnimationPoller != null)
			{
				animationPoller = kFFAnimationPoller;
				kFFAnimationPoller.anim = componentInChildren5;
				kFFAnimationPoller.pollType = KFFAnimationPoller.PollType.Playing;
				kFFAnimationPoller.animName = text;
				kFFAnimationPoller.targetTime = ((!(animationState != null)) ? 0f : (animationState.length - 0.5f));
				kFFAnimationPoller.message = "CreatureAnimationPlaying";
				kFFAnimationPoller.messageTarget = base.gameObject;
				kFFAnimationPoller.destroyWhenDone = false;
			}
			break;
		}
		case CardType.Building:
		{
			GameObject gameObject2 = (Instances[player, lane, 1] = SLOTGame.InstantiateGO(data.ObjectName, Spawn_Points[player, lane, 1].position, Spawn_Points[player, lane, 1].rotation));
			BattlePhaseManager.GetInstance().currentCreature = gameObject2.gameObject;
			gameObject2.transform.parent = Spawn_Points[player, lane, 1];
			SummonScript component2 = gameObject2.GetComponent<SummonScript>();
			component2.cardScript = cardScript;
			CWHaloFaceCam componentInChildren = gameObject2.GetComponentInChildren<CWHaloFaceCam>();
			if (componentInChildren != null)
			{
				componentInChildren.player = player;
			}
			CWCommandCardSet component3 = Spawn_Points[player, lane, 1].GetComponent<CWCommandCardSet>();
			component3.creatureObj = gameObject2;
			component.creatureFlag = false;
			component.creatureObj = gameObject2;
			vcam.target = gameObject2;
			break;
		}
		}
	}

	public void RemoveInstance(int player, int lane, CardType type)
	{
		GameObject gameObject = Instances[player, lane, (int)type];
		if ((bool)gameObject)
		{
			PoolManager.Release(Instances[player, lane, (int)type]);
		}
		Instances[player, lane, (int)type] = null;
	}

	public void SetupUniqueListForPool()
	{
		for (int i = 0; i <= 1; i++)
		{
			List<CardItem> cardsInDeck = GameInstance.GetCardsInDeck(i);
			foreach (CardItem item in cardsInDeck)
			{
				if (item.Form.Type == CardType.Creature)
				{
					if (!creaturePrefabList.ContainsKey(item.Form.ObjectName))
					{
						creaturePrefabList.Add(item.Form.ObjectName, 1);
						continue;
					}
					Dictionary<string, int> dictionary;
					Dictionary<string, int> dictionary2 = (dictionary = creaturePrefabList);
					string objectName;
					string key = (objectName = item.Form.ObjectName);
					int num = dictionary[objectName];
					dictionary2[key] = num + 1;
				}
				else if (item.Form.Type == CardType.Building)
				{
					if (!BuildingPprefabList.ContainsKey(item.Form.ObjectName))
					{
						BuildingPprefabList.Add(item.Form.ObjectName, 1);
						continue;
					}
					Dictionary<string, int> buildingPprefabList;
					Dictionary<string, int> dictionary3 = (buildingPprefabList = BuildingPprefabList);
					string objectName;
					string key2 = (objectName = item.Form.ObjectName);
					int num = buildingPprefabList[objectName];
					dictionary3[key2] = num + 1;
				}
			}
		}
		foreach (KeyValuePair<string, int> creaturePrefab in creaturePrefabList)
		{
            //GameObject prefab = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Summons/" + creaturePrefab.Key) as GameObject;
            GameObject original = Resources.Load("Summons/" + creaturePrefab.Key, typeof(GameObject)) as GameObject;
            PoolManager.PopulateStore(original, creaturePrefab.Key, creaturePrefab.Value);
		}
		foreach (KeyValuePair<string, int> buildingPprefab in BuildingPprefabList)
		{
            //GameObject prefab2 = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Building/" + buildingPprefab.Key) as GameObject;
            GameObject prefab2 = Resources.Load("Building/" + buildingPprefab.Key, typeof(GameObject)) as GameObject;
            PoolManager.PopulateStore(prefab2, buildingPprefab.Key, buildingPprefab.Value);
		}
	}

	private void Update()
	{
		GameInstance.Update();
		if (Input.GetKeyDown("f"))
		{
		}
		if (Input.GetKeyDown("e"))
		{
		}
		if (Input.GetKeyDown("c"))
		{
		}
		if (OpponentOneHitKills)
		{
		}
		if (!PlayerOneHitKills)
		{
		}
	}

	public void FinishSummoning(PlayerType PlayerID)
	{
		Time.timeScale = 1f;
		if (PlayerID == PlayerType.User)
		{
			BattlePhaseManager instance = BattlePhaseManager.GetInstance();
			if (instance.Phase != BattlePhase.P1SetupActionRareCard)
			{
				BattlePhaseManager.GetInstance().Phase = BattlePhase.P1Setup;
			}
		}
		else
		{
			CWOpponentActionSequencer.GetInstance().resumeFlag = true;
		}
	}

	public void HideCreatures()
	{
	}

	public void MoveInstance(PlayerType player, int source, int dest, CardType type)
	{
		GameObject gameObject = Instances[(int)player, source, (int)type];
		GameObject gameObject2 = Instances[(int)player, dest, (int)type];
		Instances[(int)player, source, (int)type] = gameObject2;
		Instances[(int)player, dest, (int)type] = gameObject;
		if (gameObject != null)
		{
			gameObject.transform.parent = Spawn_Points[(int)player, dest, (int)type];
			gameObject.transform.position = Spawn_Points[(int)player, dest, (int)type].position;
			if (type == CardType.Creature)
			{
				CreatureBattleScript component = gameObject.GetComponent<CreatureBattleScript>();
				component.LaneIndex = dest;
			}
			SummonScript component2 = gameObject.GetComponent<SummonScript>();
			if (component2 != null)
			{
				component2.Summon();
			}
		}
		if (gameObject2 != null)
		{
			gameObject2.transform.parent = Spawn_Points[(int)player, source, (int)type];
			gameObject2.transform.position = Spawn_Points[(int)player, source, (int)type].position;
			if (type == CardType.Creature)
			{
				CreatureBattleScript component3 = gameObject2.GetComponent<CreatureBattleScript>();
				component3.LaneIndex = source;
			}
			SummonScript component4 = gameObject2.GetComponent<SummonScript>();
			if (component4 != null)
			{
				component4.Summon();
			}
		}
	}

	public void MoveInstance(PlayerType victim, int source, PlayerType thief, int dest, CardType type)
	{
		GameObject gameObject = Instances[(int)victim, source, (int)type];
		GameObject gameObject2 = Instances[(int)thief, dest, (int)type];
		Instances[(int)victim, source, (int)type] = gameObject2;
		Instances[(int)thief, dest, (int)type] = gameObject;
		if (gameObject != null)
		{
			gameObject.transform.parent = Spawn_Points[(int)thief, dest, (int)type];
			gameObject.transform.position = Spawn_Points[(int)thief, dest, (int)type].position;
			if (type == CardType.Creature)
			{
				CreatureBattleScript component = gameObject.GetComponent<CreatureBattleScript>();
				component.Player = thief;
				component.LaneIndex = dest;
			}
			SummonScript component2 = gameObject.GetComponent<SummonScript>();
			if (component2 != null)
			{
				component2.Summon();
			}
		}
		if (gameObject2 != null)
		{
			gameObject2.transform.parent = Spawn_Points[(int)victim, source, (int)type];
			gameObject2.transform.position = Spawn_Points[(int)victim, source, (int)type].position;
			if (type == CardType.Creature)
			{
				CreatureBattleScript component3 = gameObject2.GetComponent<CreatureBattleScript>();
				component3.Player = victim;
				component3.LaneIndex = source;
			}
			SummonScript component4 = gameObject2.GetComponent<SummonScript>();
			if (component4 != null)
			{
				component4.Summon();
			}
		}
	}

	public void SpawnCoins(int Location)
	{
	}

	private void CreatureAnimationPlaying()
	{
		if (animationPoller != null)
		{
			animationPoller.Reset();
			animationPoller.pollType = KFFAnimationPoller.PollType.WaitForTime;
			animationPoller.message = "TriggerCreaturePanelTutorial";
			animationPoller.destroyWhenDone = true;
		}
	}

	private void TriggerCreaturePanelTutorial()
	{
		if (TutorialManager.Instance != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.CreaturePanelShown);
		}
	}
}
