using UnityEngine;

public class HeartAnimation : MonoBehaviour
{
	public GameObject heartTweenDoneTarget;
	public string heartTweenDoneFunctionName;
	public Collider[] collidersToDisable;
	public GameObject heart;
	public string heartTweenBeginTarget;
	public float heartTweenBeginTargetXOffset;
	public float heartTweenBeginTargetYOffset;
	public Vector3 heartTweenEndPosOffset;
	public AudioClip heartBeginSFX;
	public AudioClip heartEndSFX;
	public GameObject heartVFXPrefab;
	public float heartSpawnDelay;
	public UITweener blinkTween;
	public int heartCount;
	public UILabel heartCountLabel;
}
