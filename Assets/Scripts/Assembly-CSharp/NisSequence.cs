using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NIS/Sequence")]
public class NisSequence : NisComponent
{
	public List<NisComponent> segments = new List<NisComponent>();

	public NisInput inputProxy;

	public bool manageInputProxy;

	public float skipPromptDelaySecs;

	private bool playing;

	private int activeSegmentIdx = -1;

	private List<NisComponent> skipPromptInactives = new List<NisComponent>();

	public virtual bool isPlaying
	{
		get
		{
			return playing;
		}
	}

	public virtual int playingSegmentIndex
	{
		get
		{
			return (!playing) ? (-1) : activeSegmentIdx;
		}
	}

	private void Awake()
	{
		if (inputProxy != null)
		{
			inputProxy.onClick += OnNisClick;
			if (manageInputProxy)
			{
				EnableInputProxy(false);
			}
		}
	}

	private void OnDestroy()
	{
		if (inputProxy != null)
		{
			inputProxy.onClick -= OnNisClick;
		}
	}

	public virtual void Play()
	{
		if (base.isActiveAndEnabled && !playing)
		{
			playing = true;
			activeSegmentIdx = -1;
			if (manageInputProxy)
			{
				EnableInputProxy(true);
			}
			ResetSkipPromptTracking();
			StartCoroutine("CoroutineSkipPromptDelay");
			PlayNextSegment();
		}
	}

	protected override void SetComplete()
	{
		ResetSkipPromptTracking();
		base.SetComplete();
	}

	protected virtual void OnNisPlay()
	{
		Play();
	}

	protected virtual void OnNisClick()
	{
		if (playing)
		{
			NisComponent nisComponent = segments[activeSegmentIdx];
			nisComponent.SendMessage("OnNisClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	protected virtual void PlayNextSegment()
	{
		if (!playing)
		{
			return;
		}
		int num = ((segments.Count > 0) ? (segments.Count - 1) : (-1));
		while (activeSegmentIdx < num)
		{
			activeSegmentIdx++;
			NisComponent nisComponent = segments[activeSegmentIdx];
			if (nisComponent != null)
			{
				nisComponent.gameObject.SetActive(true);
				PlaySegment(nisComponent.GetComponents<NisComponent>());
				return;
			}
		}
		playing = false;
		activeSegmentIdx = -1;
		if (manageInputProxy)
		{
			EnableInputProxy(false);
		}
		SetComplete();
	}

	private void PlaySegment(NisComponent[] nisComponents)
	{
		int segmentCount = nisComponents.Length;
		Action<NisComponent> nisCallback = null;
		nisCallback = delegate(NisComponent src)
		{
			src.onComplete -= nisCallback;
			OnSkipPromptStatusChange(src, true);
			segmentCount--;
			if (segmentCount <= 0)
			{
				PlayNextSegment();
			}
		};
		foreach (NisComponent nisComponent in nisComponents)
		{
			nisComponent.onComplete += nisCallback;
			nisComponent.onShowSkipPrompt += OnSkipPromptStatusChange;
			OnSkipPromptStatusChange(nisComponent, nisComponent.ShowSkipPrompt);
			nisComponent.SendMessage("OnNisPlay", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void EnableInputProxy(bool isEnabled)
	{
		if (!(inputProxy == null))
		{
			inputProxy.enabled = isEnabled;
			inputProxy.gameObject.SetActive(isEnabled);
		}
	}

	private IEnumerator CoroutineSkipPromptDelay()
	{
		if (!(skipPromptDelaySecs <= 0f))
		{
			OnSkipPromptStatusChange(this, false);
			yield return new WaitForSeconds(skipPromptDelaySecs);
			OnSkipPromptStatusChange(this, true);
		}
	}

	private void ResetSkipPromptTracking()
	{
		StopCoroutine("CoroutineSkipPromptDelay");
		skipPromptInactives.Clear();
		SetShowSkipPrompt(true);
	}

	private void OnSkipPromptStatusChange(NisComponent target, bool showPrompt)
	{
		if (showPrompt)
		{
			if (skipPromptInactives.Remove(target) && skipPromptInactives.Count <= 0)
			{
				SetShowSkipPrompt(true);
			}
		}
		else if (!skipPromptInactives.Contains(target))
		{
			skipPromptInactives.Add(target);
			SetShowSkipPrompt(false);
		}
	}
}
