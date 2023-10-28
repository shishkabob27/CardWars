using System;
using UnityEngine;

public class SLOTMusic : MonoBehaviour
{
	public AudioSource[] musicAudioSources;

	public int playOnStartIndex = -1;

	public float volume = 1f;

	private static AudioSource audiosourcetomatch;

	private static bool MatchAudioSource(AudioSource source)
	{
		return source == audiosourcetomatch;
	}

	private void Start()
	{
		SLOTAudioManager instance = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		if (instance != null)
		{
			instance.UpdateAudioVolumes();
			if (musicAudioSources != null && playOnStartIndex >= 0 && playOnStartIndex < musicAudioSources.Length && musicAudioSources[playOnStartIndex] != null && musicAudioSources[playOnStartIndex].clip != null)
			{
				instance.PlayMusic(musicAudioSources[playOnStartIndex], musicAudioSources[playOnStartIndex].clip, 0f, 1f);
			}
		}
	}

	public bool FindAudioSource(AudioSource source)
	{
		audiosourcetomatch = source;
		bool result = Array.Find(musicAudioSources, MatchAudioSource) != null;
		audiosourcetomatch = null;
		return result;
	}
}
