using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("NIS/TriggerMusic")]
public class NisTriggerMusic : NisComponent
{
	public float playDelaySecs;

	public bool toggleMusicOnPlay = true;

	public AudioSource target;

	public bool autoPlay;

	private bool playing;

	private WeakReference lastMusicSourceRef = new WeakReference(null);

	private void Start()
	{
		if (autoPlay)
		{
			Play();
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
		AudioSource targetAudio = ((!(target == null)) ? target : GetComponent<AudioSource>());
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
		if (toggleMusicOnPlay)
		{
			AudioSource lastMusicSource = lastMusicSourceRef.Target as AudioSource;
			if (lastMusicSource != null)
			{
				if (targetAudio != null)
				{
					audioMan.StopMusic(targetAudio);
				}
				audioMan.PlayMusic(lastMusicSource, lastMusicSource.clip, 0f, 1f);
				lastMusicSourceRef.Target = null;
			}
			else
			{
				lastMusicSourceRef.Target = audioMan.LastMusicAudioSource;
				audioMan.StopMusic(audioMan.LastMusicAudioSource);
				if (targetAudio != null)
				{
					audioMan.PlayMusic(targetAudio, targetAudio.clip, 0f, 1f);
				}
			}
		}
		else if (targetAudio != null && !targetAudio.isPlaying)
		{
			audioMan.StopMusic(audioMan.LastMusicAudioSource);
			audioMan.PlayMusic(targetAudio, targetAudio.clip, 0f, 1f);
			lastMusicSourceRef.Target = null;
		}
		playing = false;
		SetComplete();
	}
}
