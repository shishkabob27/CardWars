using UnityEngine;

public class MapInfo : MonoBehaviour
{
	public string dbMapJsonFileName;
	public GameObject MainCameraObj;
	public BoxCollider MainCameraCollider;
	public TBPan MainCameraPanScript;
	public GameObject MPCameraObj;
	public GameObject[] RegionCameraPos;
	public GameObject[] HiddenPaths;
	public AudioSource MapMusic;
	public Texture LockedRoadTexture;
	public Texture[] RegionRoadTextures;
	public GameObject DefaultNodePrefab;
	public GameObject[] RegionNodePrefabs;
}
