using UnityEngine;

public class MovingDweebCardScript : MonoBehaviour
{
	public DweebCupManagerScript DweebCupManager;
	public GameObject Splash;
	public UITexture CardFrame;
	public UITexture CardArt;
	public UILabel CardName;
	public UILabel CardDesc;
	public Transform AboveCup;
	public Transform Cup;
	public AudioClip SplashSound;
	public AudioClip CardMove;
	public Vector3 StartPosition;
	public Vector3 TouchStart;
	public Vector3 TouchCurrent;
	public Vector3 TouchLast;
	public bool JustReleased;
	public bool FollowFinger;
	public bool FingerDown;
	public bool Scrolling;
	public bool EnterCup;
	public bool FloatUp;
	public bool Grow;
	public bool Bob;
	public bool Up;
	public float ParentStart;
	public float TouchDelta;
	public float Rotation;
	public float Float;
	public Camera uiCamera;
}
