using UnityEngine;

public class CWiTweenBattleCam : MonoBehaviour
{
	public GameObject gameCamera;

	public GameObject gameCameraTarget;

	public Transform transformTo;

	public Transform transformTargetTo;

	public string theLastTweenEvent;

	public float transitionTime;

	public Transform[] battleCameraPoints;

	public Transform[] battleCameraTargetPoints;

	public float summonTimer;

	private GameState GameInstance;

	private BattlePhaseManager phaseMgr;

	private CWiTweenCamTrigger triggerScript;

	private BattleManagerScript battleManager;

	private static CWiTweenBattleCam g_battleCam;

	private void Awake()
	{
		g_battleCam = this;
	}

	public static CWiTweenBattleCam GetInstance()
	{
		return g_battleCam;
	}

	private void Start()
	{
		battleManager = BattleManagerScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
		triggerScript = GetComponent<CWiTweenCamTrigger>();
		GameInstance = GameState.Instance;
		PanelManagerBattle instance = PanelManagerBattle.GetInstance();
		if (instance != null)
		{
			gameCamera = instance.newCamera;
			gameCameraTarget = instance.newCameraTarget;
		}
	}

	private void NotCutToBattleCamera()
	{
		iTween.Stop(gameCamera);
		iTween.Stop(gameCameraTarget);
		PlayerType playerType = ((phaseMgr.Phase != BattlePhase.P1Battle) ? PlayerType.Opponent : PlayerType.User);
		int num = ((playerType != PlayerType.User) ? 3 : 0);
		int num2 = 0;
		if (battleManager.LaneIndex == 0)
		{
			num2 = GetLaneForBattleCamera(playerType);
		}
		transformTo = battleCameraPoints[Mathf.Abs(num2 - num)];
		transformTargetTo = battleCameraTargetPoints[Mathf.Abs(num2 - num)];
		if (transformTo != null)
		{
			iTween.MoveTo(gameCamera, iTween.Hash("position", transformTo.transform.position, "time", transitionTime));
		}
		if (transformTargetTo != null)
		{
			iTween.MoveTo(gameCameraTarget, iTween.Hash("position", transformTargetTo.transform.position, "time", transitionTime));
		}
		triggerScript.tweenName = ((phaseMgr.Phase != BattlePhase.P1Battle) ? "ToP1Setup" : "ToP2Setup");
	}

	private void CutToBattleCamera()
	{
		iTween.Stop(gameCamera);
		iTween.Stop(gameCameraTarget);
		PlayerType playerType = ((phaseMgr.Phase != BattlePhase.P1Battle) ? PlayerType.Opponent : PlayerType.User);
		int num = ((playerType != PlayerType.User) ? 3 : 0);
		int num2 = 0;
		if (battleManager.LaneIndex == 0)
		{
			num2 = GetLaneForBattleCamera(playerType);
		}
		gameCamera.transform.position = battleCameraPoints[Mathf.Abs(num2 - num)].position;
		gameCameraTarget.transform.position = battleCameraTargetPoints[Mathf.Abs(num2 - num)].position;
		triggerScript.tweenName = ((phaseMgr.Phase != BattlePhase.P1Battle) ? "ToP1Setup" : "ToP2Setup");
	}

	public void MoveBattleCamera(int laneIndex)
	{
		transformTo = null;
		transformTargetTo = null;
		PlayerType playerType = ((phaseMgr.Phase != BattlePhase.P1Battle) ? PlayerType.Opponent : PlayerType.User);
		if (laneIndex <= 4 && GameInstance.LaneHasCreature(playerType, laneIndex))
		{
			int num = ((playerType != PlayerType.User) ? (3 - laneIndex) : laneIndex);
			transformTo = battleCameraPoints[num];
			transformTargetTo = battleCameraTargetPoints[num];
		}
		if (transformTo != null)
		{
			iTween.MoveTo(gameCamera, iTween.Hash("position", transformTo.transform.position, "time", transitionTime));
		}
		if (transformTargetTo != null)
		{
			iTween.MoveTo(gameCameraTarget, iTween.Hash("position", transformTargetTo.transform.position, "time", transitionTime));
		}
	}

	private int GetLaneForBattleCamera(PlayerType player)
	{
		int result = 0;
		int num = ((player != PlayerType.User) ? 3 : 0);
		for (int i = 0; i < 4; i++)
		{
			if (GameInstance.LaneHasCreature(player, Mathf.Abs(i - num)))
			{
				result = Mathf.Abs(i - num);
				break;
			}
		}
		return result;
	}

	public void SetCamBackAfterBattle()
	{
		triggerScript.PlayCam();
	}

	private void Update()
	{
	}
}
