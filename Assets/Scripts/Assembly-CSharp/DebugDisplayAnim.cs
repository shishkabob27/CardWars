using UnityEngine;

public class DebugDisplayAnim : MonoBehaviour
{
	public GameObject displayAnimObj;

	public GameObject displayAnimParent;

	private DebugFlagsScript debugFlag;

	private DebugFPS debugFPS;

	public int player;

	private bool setFlag;

	private GameObject obj;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (displayAnimParent == null)
		{
			displayAnimParent = debugFlag.GetParentObj();
		}
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.animationDisplay && !setFlag)
		{
			if (obj == null)
			{
				if (displayAnimParent != null)
				{
					obj = debugFlag.SpawnFPSObject(displayAnimObj, displayAnimParent);
				}
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				obj.SetActive(true);
				DebugDisplayAnimValue component = obj.GetComponent<DebugDisplayAnimValue>();
				component.player = player;
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.animationDisplay && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
	}
}
