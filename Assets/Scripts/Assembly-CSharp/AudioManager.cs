using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip PlaceCard1;

	public AudioClip PlaceCard2;

	public AudioClip PlaceCard3;

	public AudioClip PlaceCard4;

	public AudioClip PlaceCard5;

	public AudioClip DrawCard;

	private static AudioManager manager;

	private void Awake()
	{
		manager = this;
	}

	public static AudioManager GetInstance()
	{
		return manager;
	}

	public void PlayClip(AudioClip sound)
	{
		AudioSource component = base.gameObject.GetComponent<AudioSource>();
		if (sound != null && component != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(component, sound, false, false, SLOTAudioManager.AudioType.SFX);
		}
	}

	public void PlaceCardClip()
	{
		AudioClip audioClip = null;
		bool flag = PlaceCard1 == null && PlaceCard2 == null && PlaceCard4 == null && PlaceCard4 == null && PlaceCard5 == null;
		while (audioClip == null && !flag)
		{
			int num = new System.Random().Next(1, 6);
			if (num == 1)
			{
				audioClip = PlaceCard1;
			}
			if (num == 2)
			{
				audioClip = PlaceCard2;
			}
			if (num == 3)
			{
				audioClip = PlaceCard3;
			}
			if (num == 4)
			{
				audioClip = PlaceCard4;
			}
			if (num == 5)
			{
				audioClip = PlaceCard5;
			}
		}
		if (audioClip != null)
		{
			PlayClip(audioClip);
		}
	}

	public void PlayDrawCardClip()
	{
		if (DrawCard != null)
		{
			PlayClip(DrawCard);
		}
	}
}
