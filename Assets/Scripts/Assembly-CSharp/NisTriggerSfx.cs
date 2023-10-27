using UnityEngine;

public class NisTriggerSfx : NisComponent
{
	public NisTriggerSfx() : base(default(bool))
	{
	}

	public float playDelaySecs;
	public AudioClip clip;
	public bool autoPlay;
}
