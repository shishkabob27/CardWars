using UnityEngine;

public class NisTriggerTween : NisComponent
{
	public NisTriggerTween() : base(default(bool))
	{
	}

	public float playDelaySecs;
	public float completionDelaySecs;
	public float skipPromptDelaySecs;
	public bool reverse;
	public bool waitForClick;
	public GameObject target;
}
