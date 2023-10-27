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
}
