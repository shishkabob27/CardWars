using UnityEngine;

public class PlayQuestButton : AsyncData<string>
{
	public GameObject heartTweenDoneEvents;
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
	public UIButtonTween FailedToGetDeck;
	public UIButtonTween LoadingActivityShow;
	public UIButtonTween LoadingActivityHide;
	public RefreshMatch RefreshMatchScript;
}
