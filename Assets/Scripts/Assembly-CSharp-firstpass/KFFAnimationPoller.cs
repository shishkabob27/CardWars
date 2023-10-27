using UnityEngine;

public class KFFAnimationPoller : MonoBehaviour
{
	public enum PollType
	{
		Playing = 0,
		NotPlaying = 1,
		WaitForTime = 2,
	}

	public Animation anim;
	public PollType pollType;
	public string animName;
	public float targetTime;
	public string message;
	public GameObject messageTarget;
	public bool destroyWhenDone;
}
