using System.Collections.Generic;
using UnityEngine;

public class CWLootingSequencer : MonoBehaviour
{
	public GameObject lootingCamTarget;

	public GameObject lootingCamLookAtTarget;

	public List<CardScript> chestLanes = new List<CardScript>();

	private CreatureManagerScript creatureMgr;

	private BattlePhaseManager phaseMgr;

	public List<int> playedChest = new List<int>();

	public bool holdLootFlag;

	public UILabel lootCountLabel;

	public GameObject fxPrefab;

	public float lootCameraOffsetX = 11f;

	public float lootCameraOffsetY = 5f;

	public bool LootInBattle;

	private static CWLootingSequencer g_lootingSequencer;

	private void Awake()
	{
		g_lootingSequencer = this;
	}

	public static CWLootingSequencer GetInstance()
	{
		return g_lootingSequencer;
	}

	private void Start()
	{
		creatureMgr = CreatureManagerScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public void PlayLootingSequence()
	{
		if (playedChest.Count == chestLanes.Count)
		{
			if (LootInBattle)
			{
				phaseMgr.Phase = ((phaseMgr.Phase != BattlePhase.LootAfterP1Battle) ? BattlePhase.P1SetupBanner : BattlePhase.P2SetupBanner);
			}
			else
			{
				phaseMgr.Phase = BattlePhase.P1Setup;
			}
			holdLootFlag = false;
			playedChest.Clear();
			chestLanes.Clear();
			return;
		}
		foreach (CardScript chestLane in chestLanes)
		{
			if (!playedChest.Contains(chestLane.CurrentLane.Index))
			{
				PlaceCamForLoot(chestLane);
				break;
			}
		}
	}

	public void PlaceCamForLoot(CardScript sc)
	{
		GameObject gameObject = creatureMgr.Spawn_Points[(int)sc.Owner, sc.CurrentLane.Index, 0].gameObject;
		BoxCollider[] componentsInChildren = gameObject.GetComponentsInChildren<BoxCollider>(true);
		BoxCollider[] array = componentsInChildren;
		foreach (BoxCollider boxCollider in array)
		{
			if (boxCollider.gameObject.name.Contains("Loot"))
			{
				boxCollider.enabled = true;
			}
		}
		lootingCamLookAtTarget.transform.position = gameObject.transform.position;
		lootingCamTarget.transform.position = new Vector3(gameObject.transform.position.x - lootCameraOffsetX, gameObject.transform.position.y + lootCameraOffsetY, gameObject.transform.position.z);
		CWiTweenCamTrigger component = GetComponent<CWiTweenCamTrigger>();
		component.tweenName = "ToLootAfterBattle";
		component.PlayCam();
	}

	private void Update()
	{
	}
}
