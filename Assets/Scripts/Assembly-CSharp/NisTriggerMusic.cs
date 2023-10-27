using UnityEngine;

public class NisTriggerMusic : NisComponent
{
	public NisTriggerMusic() : base(default(bool))
	{
	}

	public float playDelaySecs;
	public bool toggleMusicOnPlay;
	public AudioSource target;
	public bool autoPlay;
}
