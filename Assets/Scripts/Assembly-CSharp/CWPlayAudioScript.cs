using UnityEngine;

public class CWPlayAudioScript : MonoBehaviour
{
	public AudioClip[] clips;

	public int playOnStartIndex = -1;

	private void Start()
	{
		PlayAudio(playOnStartIndex);
	}

	public void PlayAudio(int clipIndex)
	{
		if (base.enabled && clips != null && clipIndex >= 0 && clipIndex < clips.Length)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(base.gameObject, clips[clipIndex]);
		}
	}
}
