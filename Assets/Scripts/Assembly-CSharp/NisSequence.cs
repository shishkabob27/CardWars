using System.Collections.Generic;

public class NisSequence : NisComponent
{
	public NisSequence() : base(default(bool))
	{
	}

	public List<NisComponent> segments;
	public NisInput inputProxy;
	public bool manageInputProxy;
	public float skipPromptDelaySecs;
}
