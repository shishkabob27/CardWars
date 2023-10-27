using UnityEngine;

public class CWHeartAnimSequencer : MonoBehaviour
{
	public float enableTimer;
	public GameObject heartObj;
	public UILabel heartCountLabel;
	public int questNo;
	public string[] animNames;
	public float[] animTimers;
	public AudioClip[] animSounds;
	public GameObject[] heartFX;
	public Transform heartStartPosition;
	public GameObject vfx;
	public CWiTweenTrigger tweenTrigger;
}
