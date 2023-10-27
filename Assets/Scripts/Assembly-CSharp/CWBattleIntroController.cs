using UnityEngine;

public class CWBattleIntroController : MonoBehaviour
{
	public NisAsyncPlayer nisPlayerPrefab;
	public UIButtonPlayAnimation CameraAnim;
	public Animation CharacterP1;
	public Animation CharacterP2;
	public GameObject ContinueButton;
	public CWMenuCameraTarget CameraTarget;
	public UILabel NameP1;
	public UILabel NameP2;
	public UISprite bgP1;
	public UISprite bgP2;
	public UILabel Arena;
	public BattleJukeboxScript jukebox;
	public GameObject Logos;
	public AnimationClip[] introCameraClips;
	public GameObject[] tweenIn;
	public GameObject[] tweenOut;
	public float[] tweenInDelay;
}
