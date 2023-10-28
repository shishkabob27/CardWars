using UnityEngine;

public class CWBattleSequenceCam : MonoBehaviour
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

	private CWiTweenCamTrigger triggerScript;

	private void Start()
	{
		triggerScript = GetComponent<CWiTweenCamTrigger>();
		GameInstance = GameState.Instance;
		if (SLOTGame.IsLowEndDevice())
		{
			Transform[] array = battleCameraTargetPoints;
			foreach (Transform transform in array)
			{
				transform.position = new Vector3(0f, transform.position.y, transform.position.z);
			}
			Transform[] array2 = battleCameraPoints;
			foreach (Transform transform2 in array2)
			{
				transform2.position = new Vector3(0f, 6f, transform2.position.z + 2f);
			}
		}
	}

	public void MoveBattleCameraSplitScreen(int laneIndex)
	{
		transformTo = null;
		transformTargetTo = null;
		transformTo = battleCameraPoints[laneIndex];
		transformTargetTo = battleCameraTargetPoints[laneIndex];
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
}
