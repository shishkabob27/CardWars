using System.Collections;
using UnityEngine;

[AddComponentMenu("NIS/TriggerSFX")]
public class NisTriggerSfx : NisComponent
{
	public float playDelaySecs;

	public AudioClip clip;

	public bool autoPlay;

	private bool playing;

	private AudioSource audioSource;

	private void Start()
	{
		if (autoPlay)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		if (audioSource != null)
		{
			audioSource.Stop();
		}
	}

	protected virtual void OnNisPlay()
	{
		Play();
	}

	private void Play()
	{
		if (base.isActiveAndEnabled)
		{
			StartCoroutine(CoroutinePlay());
		}
	}

	private IEnumerator CoroutinePlay()
	{
		if (playing)
		{
			yield break;
		}
		SLOTAudioManager audioMan = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		AudioClip targetClip = clip;
		playing = true;
		if (playDelaySecs > 0f)
		{
			yield return new WaitForSeconds(playDelaySecs);
		}
		if (audioMan == null)
		{
			SetComplete();
			yield break;
		}
		if (targetClip != null)
		{
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
			}
			if (audioSource == null)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			audioMan.PlayOneShot(audioSource, targetClip);
		}
		playing = false;
		SetComplete();
	}
}
