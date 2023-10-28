using UnityEngine;

public class KFFSoundPlayer
{
	public delegate AudioSource PlayOneShotFunction(AudioClip clip, float volume, float pitch);

	private static PlayOneShotFunction playOneShotFunction;

	public static void SetPlayOneShotFunction(PlayOneShotFunction f)
	{
		playOneShotFunction = f;
	}

	public static PlayOneShotFunction GetPlayOneShotFunction()
	{
		return playOneShotFunction;
	}

	public static AudioSource PlayOneShot(AudioClip clip)
	{
		return PlayOneShot(clip, 1f, 1f);
	}

	public static AudioSource PlayOneShot(AudioClip clip, float volume)
	{
		return PlayOneShot(clip, volume, 1f);
	}

	public static AudioSource PlayOneShot(AudioClip clip, float volume, float pitch)
	{
		if (playOneShotFunction != null)
		{
			return playOneShotFunction(clip, volume, pitch);
		}
		return null;
	}
}
