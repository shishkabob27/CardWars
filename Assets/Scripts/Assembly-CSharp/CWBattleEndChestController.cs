using System.Collections;
using UnityEngine;

public class CWBattleEndChestController : MonoBehaviour
{
	public Transform[] chestSpawnPoints;

	public GameObject[] resultLootChests;

	public GameObject[] vfxRarity;

	public GameObject normalChestFX;

	public GameObject premiumChestFX;

	public GameObject[] bannerObjects;

	public GameObject resultCard;

	public AudioClip revealCardSound;

	public float chestSpawnInterval;

	public float[] fxTimes;

	public float[] cardTimes;

	public Transform bannerParentTr;

	public int openedChestCount;

	public int chestCount;

	public GameObject tweenTarget;

	public TweenPosition earningHeader;

	public GameObject tapAnywherePanel;

	public GameObject hasCardTween;

	public CWDisableLaneCollider battleLaneCollider;

	private QuestEarningManager earningMgr;

	private CWLootingSequencer lootingSqcr;

	public CWResultTapDelegate battleEndTapDelegate;

	public float lootCameraOffsetX = 11f;

	public float lootCameraOffsetY = 5f;

	public float lootCameraTargetOffsetY = 3f;

	private static CWBattleEndChestController g_battleEndChestCtrlr;

	private bool awarded;

	private object awardLock = new object();

	private bool buttonDisplayed = true;

	private void Awake()
	{
		g_battleEndChestCtrlr = this;
	}

	public static CWBattleEndChestController GetInstance()
	{
		return g_battleEndChestCtrlr;
	}

	private void Start()
	{
		earningMgr = QuestEarningManager.GetInstance();
		lootingSqcr = CWLootingSequencer.GetInstance();
	}

	public void ChestPlacement()
	{
		bool flag;
		lock (awardLock)
		{
			flag = !awarded;
			awarded = true;
		}
		if (flag)
		{
			StartCoroutine(SpawnChests());
		}
	}

	private IEnumerator SpawnChests()
	{
		chestCount = ((earningMgr.earnedCards.Count <= 5) ? earningMgr.earnedCards.Count : 5);
		buttonDisplayed = false;
		for (int i = 0; i < chestCount; i++)
		{
			CardItem card = earningMgr.earnedCards[i];
			GameObject spawnObj2 = null;
			Transform parentTr = chestSpawnPoints[i];
			QuestData curQuest = PlayerInfoScript.GetInstance().GetCurrentQuest();
			GameObject chestPrefab = null;
			if (!string.IsNullOrEmpty(curQuest.ResultLootChestPrefab))
			{
				chestPrefab = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(curQuest.ResultLootChestPrefab) as GameObject;
			}
			if (chestPrefab == null)
			{
				chestPrefab = resultLootChests[card.Form.Rarity - 1];
			}
			spawnObj2 = SLOTGame.InstantiateFX(chestPrefab, parentTr.position, parentTr.rotation) as GameObject;
			spawnObj2.transform.parent = parentTr;
			CWResultChestTrigger chestTrigger = spawnObj2.GetComponentInChildren<CWResultChestTrigger>();
			if (chestTrigger != null)
			{
				chestTrigger.card = card;
			}
			yield return new WaitForSeconds(chestSpawnInterval);
		}
		if (earningHeader != null)
		{
			earningHeader.gameObject.SetActive(chestCount > 0);
			UIPanel p = earningHeader.gameObject.GetComponent(typeof(UIPanel)) as UIPanel;
			if (p != null)
			{
				p.enabled = chestCount > 0;
			}
		}
		lock (awardLock)
		{
			awarded = false;
		}
	}

	public void PlaceCamForLoot(Vector3 pos)
	{
		lootingSqcr.lootingCamLookAtTarget.transform.position = new Vector3(pos.x, pos.y + lootCameraTargetOffsetY, pos.z);
		lootingSqcr.lootingCamTarget.transform.position = new Vector3(pos.x - lootCameraOffsetX, pos.y + lootCameraOffsetY, pos.z);
		CWiTweenCamTrigger component = GetComponent<CWiTweenCamTrigger>();
		component.tweenName = "ToLootAfterBattle";
		component.PlayCam();
	}

	public void SetCameraBack()
	{
		CWiTweenCamTrigger component = GetComponent<CWiTweenCamTrigger>();
		component.tweenName = "ToP1WinChest";
		component.PlayCam();
	}

	private IEnumerator DelayButtonDisplay(float delay)
	{
		yield return new WaitForSeconds(delay);
		tweenTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
	}

	private void Update()
	{
		if (openedChestCount == chestCount && !buttonDisplayed)
		{
			float delay = 0f;
			if (chestCount != 0)
			{
				delay = 1f;
			}
			earningHeader.gameObject.SetActive(false);
			buttonDisplayed = true;
			StartCoroutine(DelayButtonDisplay(delay));
		}
	}
}
