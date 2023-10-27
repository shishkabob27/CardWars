using UnityEngine;
using System.Collections.Generic;

public class CWLootingSequencer : MonoBehaviour
{
	public GameObject lootingCamTarget;
	public GameObject lootingCamLookAtTarget;
	public List<int> playedChest;
	public bool holdLootFlag;
	public UILabel lootCountLabel;
	public GameObject fxPrefab;
	public float lootCameraOffsetX;
	public float lootCameraOffsetY;
	public bool LootInBattle;
}
