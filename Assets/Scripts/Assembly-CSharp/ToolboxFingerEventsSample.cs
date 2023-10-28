using UnityEngine;

public class ToolboxFingerEventsSample : SampleBase
{
	public Light light1;

	public Light light2;

	private void ToggleLight1()
	{
		light1.enabled = !light1.enabled;
	}

	private void ToggleLight2()
	{
		light2.enabled = !light2.enabled;
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates the use of the toolbox scripts TBFingerDown and TBFingerUp. It also shows how you can use the message target property to turn the light on & off.";
	}
}
