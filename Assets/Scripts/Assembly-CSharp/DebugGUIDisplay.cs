using UnityEngine;

public class DebugGUIDisplay : MonoBehaviour
{
	private DebugFlagsScript debugFlag;

	private static DebugGUIDisplay instance;

	private bool displayFlag;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		instance = this;
	}

	public static DebugGUIDisplay GetInstance()
	{
		return instance;
	}

	public void OnClick()
	{
		DebugGUI component = debugFlag.gameObject.GetComponent<DebugGUI>();
		if (component != null)
		{
			component.enabled = !component.enabled;
		}
	}

	private void Update()
	{
		if (debugFlag.DebugInBuild && !displayFlag)
		{
			if ((bool)GetComponent<Collider>())
			{
				GetComponent<Collider>().enabled = true;
			}
			displayFlag = true;
		}
		if (!debugFlag.DebugInBuild && displayFlag)
		{
			if ((bool)GetComponent<Collider>())
			{
				GetComponent<Collider>().enabled = false;
			}
			displayFlag = false;
		}
	}
}
