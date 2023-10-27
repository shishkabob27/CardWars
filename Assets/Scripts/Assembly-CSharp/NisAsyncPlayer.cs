using UnityEngine;

public class NisAsyncPlayer : NisSequence
{
	public GameObject nisRoot;
	public GameObject skipPromptGO;
	public string[] sequencesPath;
	public bool keepLastSequenceOnComplete;
	public bool noCache;
	public bool overrideLayer;
	public CanvasRenderer scrimPrefab;
	public float fadeSecs;
	public UIButtonTween busyTweenShow;
	public UIButtonTween busyTweenHide;
}
