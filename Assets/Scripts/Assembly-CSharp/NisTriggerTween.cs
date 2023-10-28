using System.Collections;
using UnityEngine;

[AddComponentMenu("NIS/TriggerTween")]
public class NisTriggerTween : NisComponent
{
	public float playDelaySecs;

	public float completionDelaySecs;

	public float skipPromptDelaySecs;

	public bool reverse;

	public bool waitForClick;

	public GameObject target;

	private bool playing;

	private bool clicked;

	protected virtual void OnNisPlay()
	{
		Play();
	}

	protected virtual void OnNisClick()
	{
		if (playing)
		{
			clicked = true;
		}
	}

	private void Play()
	{
		if (base.isActiveAndEnabled)
		{
			StopCoroutine("CoroutineSkipPromptDelay");
			StartCoroutine("CoroutineSkipPromptDelay");
			StartCoroutine(CoroutinePlay());
		}
	}

	protected override void SetComplete()
	{
		StopCoroutine("CoroutineSkipPromptDelay");
		SetShowSkipPrompt(true);
		base.SetComplete();
	}

	private IEnumerator CoroutineSkipPromptDelay()
	{
		if (!(skipPromptDelaySecs <= 0f))
		{
			SetShowSkipPrompt(false);
			yield return new WaitForSeconds(skipPromptDelaySecs);
			SetShowSkipPrompt(true);
		}
	}

	private IEnumerator CoroutinePlay()
	{
		if (!playing)
		{
			playing = true;
			if (playDelaySecs > 0f)
			{
				yield return new WaitForSeconds(playDelaySecs);
			}
			PlayTween((!(target != null)) ? base.gameObject : target);
			if (completionDelaySecs > 0f)
			{
				yield return new WaitForSeconds(completionDelaySecs);
			}
			clicked = false;
			while (playing && waitForClick && !clicked)
			{
				yield return null;
			}
			if (playing)
			{
				playing = false;
				SetComplete();
			}
		}
	}

	private void PlayTween(GameObject target)
	{
		if (!(target == null))
		{
			UIButtonTween[] componentsInChildren = target.GetComponentsInChildren<UIButtonTween>();
			UIButtonTween[] array = componentsInChildren;
			foreach (UIButtonTween uIButtonTween in array)
			{
				uIButtonTween.Play(true);
			}
		}
	}
}
