using UnityEngine;

public class BusyIcon : MonoBehaviour
{
	public float rotationSpeed = -400f;

	public Animation anim;

	public string clipName;

	private float lastTime;

	private float animElapsed;

	private void Start()
	{
		if (anim != null)
		{
			anim.Play();
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float num = 0f;
		if (lastTime != 0f)
		{
			num = Time.realtimeSinceStartup - lastTime;
			if (num > Time.fixedDeltaTime)
			{
				num = Time.fixedDeltaTime;
			}
		}
		lastTime = Time.realtimeSinceStartup;
		if (rotationSpeed != 0f)
		{
			base.transform.Rotate(new Vector3(0f, rotationSpeed * num, 0f));
		}
		if (anim != null)
		{
			AnimationState animationState = anim[clipName];
			if (animationState != null)
			{
				animationState.time = animElapsed;
				anim.Sample();
				animElapsed += num;
				animElapsed = Mathf.Repeat(animElapsed, animationState.length);
			}
		}
	}
}
