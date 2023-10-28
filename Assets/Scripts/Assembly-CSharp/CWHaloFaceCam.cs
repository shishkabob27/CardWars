using UnityEngine;

public class CWHaloFaceCam : MonoBehaviour
{
	private PanelManagerBattle panelMgrBattle;

	private BattlePhaseManager phaseMgr;

	private CWBattleSequenceController battleCtlr;

	public int player;

	private GameObject gameCamera;

	private GameObject battleCamera;

	private void Start()
	{
		panelMgrBattle = PanelManagerBattle.GetInstance();
		gameCamera = panelMgrBattle.newCamera;
		phaseMgr = BattlePhaseManager.GetInstance();
		battleCtlr = CWBattleSequenceController.GetInstance();
		battleCamera = ((player != 0) ? battleCtlr.battleCamP2.gameObject : battleCtlr.battleCamP1.gameObject);
	}

	private void Update()
	{
		base.transform.LookAt(base.transform.position + gameCamera.transform.rotation * Vector3.back, gameCamera.transform.rotation * Vector3.up);
		if (phaseMgr.Phase == BattlePhase.P1Battle || phaseMgr.Phase == BattlePhase.P2Battle)
		{
			gameCamera = battleCamera;
		}
		else
		{
			gameCamera = panelMgrBattle.newCamera;
		}
	}
}
