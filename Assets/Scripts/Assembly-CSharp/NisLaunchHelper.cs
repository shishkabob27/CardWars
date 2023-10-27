using UnityEngine;

public class NisLaunchHelper : MonoBehaviour
{
	public GameObject nisRoot;
	public NisAsyncPlayer playerPrefab;
	public bool overrideLayer;
	public UIButtonTween busyTweenShow;
	public UIButtonTween busyTweenHide;
	public float busyTweenWaitSeconds;
	public bool cleanupPlayerOnFinish;
	public float endFadeSecsOverride;
}
