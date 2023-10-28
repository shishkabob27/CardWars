using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("NIS/TriggerFMV")]
public class NisTriggerFmv : NisComponent
{
	public float playDelaySecs;

	public MoviePlayerController target;

	public bool stopMusicOnStart = true;

	public bool restoreMusicOnStop = true;

	private bool playing;

	public NisTriggerFmv()
		: base(false)
	{
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
		MoviePlayerController targetApplied = ((!(target != null)) ? GetComponent<MoviePlayerController>() : target);
		if (targetApplied == null)
		{
			yield break;
		}
		playing = true;
		SLOTAudioManager audioMan = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		AudioSource lastPlayingMusic2 = null;
		if (stopMusicOnStart)
		{
			lastPlayingMusic2 = audioMan.LastMusicAudioSource;
			audioMan.StopMusic(lastPlayingMusic2);
		}
		if (playDelaySecs > 0f)
		{
			yield return new WaitForSeconds(playDelaySecs);
		}
		Action<MoviePlayerController> playbackCompleteCb2 = null;
		playbackCompleteCb2 = delegate(MoviePlayerController moviePlayer)
		{
			moviePlayer.onComplete -= playbackCompleteCb2;
			if (playing)
			{
				if (restoreMusicOnStop && lastPlayingMusic2 != null)
				{
					audioMan.PlayMusic(lastPlayingMusic2, lastPlayingMusic2.clip, 0f, 1f);
				}
				playing = false;
				SetComplete();
			}
		};
		targetApplied.onComplete += playbackCompleteCb2;
		targetApplied.PlayMovie();
	}
}
