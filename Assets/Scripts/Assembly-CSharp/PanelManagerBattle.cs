using UnityEngine;
using System.Collections.Generic;

public class PanelManagerBattle : MonoBehaviour
{
	public Camera uiCamera;
	public GameObject newCamera;
	public GameObject newCameraTarget;
	public GameObject hexGrid;
	public GameObject debugWindow;
	public GameObject zoomCard;
	public GameObject tweenP1Damage;
	public GameObject tweenP2Damage;
	public CWCreatureStatsFloorDisplay[] P1FloorDisplays;
	public CWCreatureStatsFloorDisplay[] P2FloorDisplays;
	public GameObject blackPanel;
	public GameObject[] lootObjects;
	public GameObject coinLootObject;
	public GameObject[] tombstoneObjs;
	public GameObject flyingCard;
	public GameObject flyingCardDestination;
	public CWDisableLaneCollider battleLaneColController;
	public GameObject floopPanelCameraTarget;
	public GameObject floopPanelCameraLookAtTarget;
	public GameObject currentCardObj;
	public UILabel QuestStatus;
	public UILabel QuestInfo;
	public UILabel QuestCondition;
	public List<UISlicedSprite> Stars;
	public bool hpBarOnTheGround;
}
