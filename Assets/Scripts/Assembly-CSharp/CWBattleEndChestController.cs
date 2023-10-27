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
	public CWResultTapDelegate battleEndTapDelegate;
	public float lootCameraOffsetX;
	public float lootCameraOffsetY;
	public float lootCameraTargetOffsetY;
}
