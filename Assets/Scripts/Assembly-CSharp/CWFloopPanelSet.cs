using UnityEngine;

public class CWFloopPanelSet : MonoBehaviour
{
	public GameObject floopDisplayPanel;

	public int playerType;

	public int lane;

	public bool creatureFlag;

	public CWCommandCardSet commandCardSet;

	public GameObject creatureObj;

	private GameState GameInstance;

	private CWFloopActionManager floopActionMgr;

	private PanelManagerBattle panelMgrBattle;

	private CreatureManagerScript creatureMgr;

	public bool floopSetFlag = true;

	public GameObject lowerFloopDisplayPanel;

	private void Start()
	{
		GameInstance = GameState.Instance;
		floopActionMgr = CWFloopActionManager.GetInstance();
		panelMgrBattle = PanelManagerBattle.GetInstance();
		creatureMgr = CreatureManagerScript.GetInstance();
	}

	private void OnClick()
	{
		UICamera.useInputEnabler = true;
		CreatureScript creatureScript = null;
		if (floopSetFlag)
		{
			floopActionMgr.player = playerType;
			floopActionMgr.lane = lane - 1;
			creatureScript = GameInstance.GetCreature(playerType, lane - 1);
			if (creatureScript != null)
			{
				floopActionMgr.card = creatureScript.Data;
			}
			floopActionMgr.anim = commandCardSet.creatureObj.GetComponent<Animation>();
		}
		creatureScript = GameInstance.GetCreature(floopActionMgr.player, floopActionMgr.lane);
		if (creatureScript != null)
		{
			PanelManagerBattle.FillCardInfo(floopDisplayPanel, creatureScript.Data, PlayerType.User);
			PanelManagerBattle.FillCardInfo(floopDisplayPanel, creatureScript.Data, FloopActionType.Floop);
			if (floopActionMgr.floopPrompt != null)
			{
				floopActionMgr.floopPrompt.SetFloopPrompt(creatureScript.Data);
			}
		}
		if (lowerFloopDisplayPanel != null)
		{
			PanelManagerBattle.FillCardInfo(lowerFloopDisplayPanel, creatureScript, PlayerType.User);
		}
		Transform transform = creatureMgr.Spawn_Points[floopActionMgr.player, floopActionMgr.lane, 0];
		panelMgrBattle.floopPanelCameraLookAtTarget.transform.position = new Vector3(transform.position.x, transform.position.y + floopActionMgr.floopCameraTargetOffsetY, transform.position.z);
		panelMgrBattle.floopPanelCameraTarget.transform.position = new Vector3(transform.position.x - floopActionMgr.floopCameraOffsetX, transform.position.y + floopActionMgr.floopCameraOffsetY, transform.position.z);
	}

	private void Update()
	{
	}
}
