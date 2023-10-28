using System.Collections;
using UnityEngine;

public class CWFXSoundSequencer : MonoBehaviour
{
	public float[] timer;

	public AudioClip[] sounds;

	private void Start()
	{
		for (int i = 0; i < sounds.Length; i++)
		{
			StartCoroutine(playSoundWithDelay(timer[i], sounds[i]));
		}
	}

	private IEnumerator playSoundWithDelay(float waitTime, AudioClip sound)
	{
		yield return new WaitForSeconds(waitTime);
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), sound, true, false, SLOTAudioManager.AudioType.SFX);
	}

	private void Update()
	{
	}
}
