using UnityEngine;

public class CWiTweenVantageCam : MonoBehaviour
{
	public float CAMERA_Y_OFFSET;
	public bool CAMERA_Y_OFFSET_FlAG;
	public float CAMERA_DISTANCE_OFFSET;
	public bool CAMERA_DISTANCE_OFFSET_FLAG;
	public GameObject gameCamera;
	public GameObject gameCameraTarget;
	public Transform transformTo;
	public Transform transformTargetTo;
	public string theLastTweenEvent;
	public float time;
	public Transform[] creatureVantagePoints;
	public Transform[] buildingVantagePoints;
	public float summonTimer;
	public GameObject target;
	public Vector3 targetOffset;
	public Vector3 camOffset;
}
