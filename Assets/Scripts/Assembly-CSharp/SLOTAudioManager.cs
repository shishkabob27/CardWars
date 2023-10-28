using System;
using System.Collections.Generic;
using UnityEngine;

public class SLOTAudioManager : SLOTGameSingleton<SLOTAudioManager>
{
	public enum AudioType
	{
		SFX,
		VO,
		Music
	}

	private class MusicFadeOutInfo
	{
		public AudioSource audioSource;

		public AudioClip audioClip;

		public float fadeOutSpeed;

		public float musicVolume;

		public MusicFadeOutInfo(AudioSource a, AudioClip c, float fadeoutspeed, float musicvolume)
		{
			audioSource = a;
			audioClip = c;
			fadeOutSpeed = fadeoutspeed;
			musicVolume = musicvolume;
		}
	}

	public const bool disableMusic = false;

	public AudioSource gui_audiosource;

	private AudioListener listener;

	public float soundVolume = 1f;

	public float voVolume = 1f;

	public float musicVolume = 1f;

	private float prevSoundVolume = -1234f;

	private float prevVOVolume = -1234f;

	private float prevMusicVolume = -1234f;

	private List<MusicFadeOutInfo> musicFadeOutList = new List<MusicFadeOutInfo>();

	private WeakReference _lastMusicReference = new WeakReference(null);

	private static bool s_isOnPhoneCall;

	private static float s_lastPhoneCallCheckTime;

	public AudioSource LastMusicAudioSource
	{
		get
		{
			return _lastMusicReference.Target as AudioSource;
		}
	}

	public float SoundVolume
	{
		get
		{
			return soundVolume;
		}
		set
		{
			soundVolume = value;
			Serialize();
		}
	}

	public float VOVolume
	{
		get
		{
			return voVolume;
		}
		set
		{
			voVolume = value;
			Serialize();
		}
	}

	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			musicVolume = value;
			Serialize();
		}
	}

	public float GetSoundVolume()
	{
		return SoundVolume;
	}

	public float GetVOVolume()
	{
		return VOVolume;
	}

	public float GetMusicVolume()
	{
		return MusicVolume;
	}

	public void SetSoundVolume(float v)
	{
		SoundVolume = v;
		UpdateAudioVolumes();
	}

	public void SetVOVolume(float v)
	{
		VOVolume = v;
		UpdateAudioVolumes();
	}

	public void SetMusicVolume(float v)
	{
		MusicVolume = v;
		UpdateAudioVolumes();
	}

	private void Awake()
	{
		Deserialize();
		KFFSoundPlayer.SetPlayOneShotFunction(PlayOneShot);
	}

	private void Start()
	{
		base.transform.position = new Vector3(0f, 0f, 0f);
	}

	public AudioSource PlaySound(AudioSource audiosource)
	{
		if (audiosource != null)
		{
			return PlaySound(audiosource, audiosource.clip);
		}
		return null;
	}

	public AudioSource PlaySound(AudioSource audiosource, AudioClip audioclip)
	{
		if (audiosource != null)
		{
			return PlaySound(audiosource, audioclip, true, false, AudioType.SFX);
		}
		return null;
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip)
	{
		return PlaySound(obj, audioclip, true);
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip, bool oneshot)
	{
		return PlaySound(obj, audioclip, oneshot, false);
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip, bool oneshot, bool createnewaudiosource)
	{
		return PlaySound(obj, audioclip, oneshot, createnewaudiosource, false);
	}

	public AudioSource PlaySound(GameObject obj, AudioClip audioclip, bool oneshot, bool createnewaudiosource, bool NoStacking)
	{
		AudioSource audioSource = obj.GetComponent(typeof(AudioSource)) as AudioSource;
		if (audioSource == null || createnewaudiosource)
		{
			audioSource = obj.AddComponent(typeof(AudioSource)) as AudioSource;
		}
		return PlaySound(audioSource, audioclip, oneshot, NoStacking);
	}

	public AudioSource PlaySound(AudioSource audiosource, AudioClip audioclip, bool oneshot, bool NoStacking)
	{
		return PlaySound(audiosource, audioclip, oneshot, NoStacking, AudioType.SFX);
	}

	public AudioSource PlaySound(AudioSource audiosource, AudioClip audioclip, bool oneshot, bool NoStacking, AudioType audioType)
	{
		if (audiosource == null)
		{
			return null;
		}
		switch (audioType)
		{
		case AudioType.SFX:
			audiosource.volume = soundVolume;
			break;
		case AudioType.VO:
			audiosource.volume = voVolume;
			break;
		case AudioType.Music:
			audiosource.volume = musicVolume;
			break;
		}
		audiosource.enabled = true;
		if (oneshot)
		{
			audiosource.PlayOneShot(audioclip);
		}
		else if (!NoStacking || !audiosource.isPlaying)
		{
			audiosource.clip = audioclip;
			audiosource.Play();
		}
		return audiosource;
	}

	public void PlayRandomSound(GameObject obj, AudioClip[] clips)
	{
		if (clips != null && clips.Length > 0)
		{
			PlaySound(obj, clips[KFFRandom.GetRandomIndex(clips.Length)], true, false, false);
		}
	}

	public AudioSource PlayMusic(GameObject obj, AudioClip audioclip, float fadeOutSpeed = 0f, float musicvolume = 1f)
	{
		AudioSource audioSource = obj.GetComponent(typeof(AudioSource)) as AudioSource;
		if (audioSource == null)
		{
			audioSource = obj.AddComponent(typeof(AudioSource)) as AudioSource;
		}
		return PlayMusic(audioSource, audioclip, fadeOutSpeed, 1f);
	}

	public AudioSource PlayMusic(AudioSource audiosource, AudioClip audioclip, float fadeOutSpeed = 0f, float musicvolume = 1f)
	{
		if (audiosource == null)
		{
			return null;
		}
		if (audiosource.clip != audioclip || !audiosource.isPlaying)
		{
			if (fadeOutSpeed > 0f && audiosource.isPlaying)
			{
				FadeOutAndPlay(audiosource, audioclip, fadeOutSpeed, musicvolume);
			}
			else
			{
				audiosource.volume = musicVolume * musicvolume;
				audiosource.clip = audioclip;
				audiosource.Play();
				SLOTMusic sLOTMusic = audiosource.gameObject.GetComponent(typeof(SLOTMusic)) as SLOTMusic;
				if (sLOTMusic != null)
				{
					sLOTMusic.volume = musicvolume;
				}
			}
			_lastMusicReference.Target = audiosource;
		}
		return audiosource;
	}

	public void StopMusic(AudioSource audiosource)
	{
		if (audiosource != null)
		{
			_lastMusicReference.Target = null;
			audiosource.Stop();
		}
	}

	public void FadeOutAndPlay(AudioSource audiosource, AudioClip audioclip, float fadeOutSpeed, float musicVolume = 1f)
	{
		if (audiosource != null)
		{
			MusicFadeOutInfo item = new MusicFadeOutInfo(audiosource, audioclip, fadeOutSpeed, musicVolume);
			musicFadeOutList.Add(item);
			_lastMusicReference.Target = audiosource;
		}
	}

	public void FadeOut(AudioSource audiosource, float fadeOutSpeed)
	{
		FadeOutAndPlay(audiosource, null, fadeOutSpeed, 1f);
	}

	public AudioSource PlayVO(AudioSource audiosource, AudioClip audioclip)
	{
		return PlaySound(audiosource, audioclip, false, false, AudioType.VO);
	}

	public AudioSource PlayOneShot(AudioClip audioclip)
	{
		return PlayOneShot(audioclip, 1f);
	}

	public AudioSource PlayOneShot(AudioClip audioclip, float volume)
	{
		return PlayOneShot(audioclip, volume, 1f);
	}

	public AudioSource PlayOneShot(AudioClip audioclip, float volume, float pitch)
	{
		AudioSource audioSource = PlayGUISound(audioclip, true);
		if (audioSource != null)
		{
			audioSource.volume = soundVolume * volume;
			audioSource.pitch = pitch;
		}
		return audioSource;
	}

	public AudioSource PlayOneShot(AudioSource audiosource, AudioClip audioclip)
	{
		return PlaySound(audiosource, audioclip, true, false, AudioType.SFX);
	}

	public AudioSource PlayGUISound(AudioClip audioclip)
	{
		return PlayGUISound(audioclip, true);
	}

	public AudioSource PlayGUISound(AudioClip audioclip, bool oneshot)
	{
		CreateListener();
		if ((bool)gui_audiosource)
		{
			return PlaySound(gui_audiosource, audioclip, oneshot, true);
		}
		gui_audiosource = PlaySound(base.gameObject, audioclip, oneshot, true, false);
		return gui_audiosource;
	}

	public void Serialize()
	{
		PlayerPrefs.SetFloat("Options_Sound_Volume", soundVolume);
		PlayerPrefs.SetFloat("Options_VO_Volume", voVolume);
		PlayerPrefs.SetFloat("Options_Music_Volume", musicVolume);
	}

	public void Deserialize()
	{
		soundVolume = PlayerPrefs.GetFloat("Options_Sound_Volume", 1f);
		voVolume = PlayerPrefs.GetFloat("Options_VO_Volume", 1f);
		musicVolume = PlayerPrefs.GetFloat("Options_Music_Volume", 1f);
	}

	private void Update()
	{
		UpdateVolumes();
		UpdateMusicFadeOut();
	}

	private void UpdateVolumes()
	{
		if (s_isOnPhoneCall && Time.time - s_lastPhoneCallCheckTime > 1f)
		{
			s_isOnPhoneCall = KFFCSUtils.IsOnPhoneCall();
			s_lastPhoneCallCheckTime = Time.time;
		}
		float num = ((!s_isOnPhoneCall) ? soundVolume : 0f);
		float num2 = ((!s_isOnPhoneCall) ? voVolume : 0f);
		float num3 = ((!s_isOnPhoneCall) ? musicVolume : 0f);
		if (num != prevSoundVolume || num2 != prevVOVolume || num3 != prevMusicVolume)
		{
			UpdateAudioVolumes(s_isOnPhoneCall ? 1 : 0);
			prevSoundVolume = num;
			prevVOVolume = num2;
			prevMusicVolume = num3;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		s_isOnPhoneCall = KFFCSUtils.IsOnPhoneCall();
		s_lastPhoneCallCheckTime = Time.time;
		UpdateVolumes();
	}

	public void UpdateAudioVolumes(int isonphonecall = -1)
	{
		UpdateAudioVolumes(null, isonphonecall);
	}

	public void UpdateAudioVolumes(GameObject root, int isonphonecall = -1)
	{
		bool flag = ((isonphonecall < 0) ? KFFCSUtils.IsOnPhoneCall() : (isonphonecall != 0));
		float num = ((!flag) ? soundVolume : 0f);
		float num2 = ((!flag) ? voVolume : 0f);
		float num3 = ((!flag) ? musicVolume : 0f);
		CreateListener();
		UnityEngine.Object[] array = null;
		array = ((!(root != null)) ? UnityEngine.Object.FindObjectsOfType(typeof(AudioSource)) : root.GetComponentsInChildren(typeof(AudioSource)));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Component component = (Component)array2[i];
			AudioSource audioSource = component as AudioSource;
			if (audioSource == null)
			{
				continue;
			}
			SLOTMusic sLOTMusic = audioSource.gameObject.GetComponent(typeof(SLOTMusic)) as SLOTMusic;
			if (sLOTMusic != null && sLOTMusic.FindAudioSource(audioSource))
			{
				audioSource.volume = num3 * sLOTMusic.volume;
				continue;
			}
			bool flag2 = false;
			VOController vOController = audioSource.gameObject.GetComponent(typeof(VOController)) as VOController;
			if (vOController != null)
			{
				flag2 = true;
			}
			else
			{
				VoiceoverScript voiceoverScript = audioSource.gameObject.GetComponent(typeof(VoiceoverScript)) as VoiceoverScript;
				if (voiceoverScript != null)
				{
					flag2 = true;
				}
				else
				{
					TutorialMonitor tutorialMonitor = audioSource.gameObject.GetComponent(typeof(TutorialMonitor)) as TutorialMonitor;
					if (tutorialMonitor != null)
					{
						flag2 = true;
					}
				}
			}
			audioSource.volume = ((!flag2) ? num : num2);
		}
	}

	private void CreateListener()
	{
		if (!(listener == null))
		{
			return;
		}
		listener = UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener;
		if (listener == null)
		{
			Camera camera = Camera.main;
			if (camera == null)
			{
				camera = UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera;
			}
			if (camera != null)
			{
				listener = camera.gameObject.AddComponent(typeof(AudioListener)) as AudioListener;
			}
		}
	}

	private void UpdateMusicFadeOut()
	{
		for (int num = musicFadeOutList.Count - 1; num >= 0; num--)
		{
			MusicFadeOutInfo musicFadeOutInfo = musicFadeOutList[num];
			if (musicFadeOutInfo != null)
			{
				if (musicFadeOutInfo.audioSource != null && musicFadeOutInfo.audioSource.volume > 0f && musicFadeOutInfo.fadeOutSpeed > 0f)
				{
					float num2 = musicFadeOutInfo.fadeOutSpeed * Time.deltaTime;
					if (num2 >= musicFadeOutInfo.audioSource.volume)
					{
						musicFadeOutInfo.audioSource.volume = 0f;
					}
					else
					{
						musicFadeOutInfo.audioSource.volume -= num2;
					}
				}
				else
				{
					musicFadeOutList.RemoveAt(num);
					if (musicFadeOutInfo.audioClip != null)
					{
						PlayMusic(musicFadeOutInfo.audioSource, musicFadeOutInfo.audioClip, 0f, musicFadeOutInfo.musicVolume);
					}
					else if (musicFadeOutInfo.audioSource != null)
					{
						musicFadeOutInfo.audioSource.clip = null;
					}
				}
			}
			else
			{
				musicFadeOutList.RemoveAt(num);
			}
		}
	}
}
