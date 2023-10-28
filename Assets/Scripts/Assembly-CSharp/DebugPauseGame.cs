using UnityEngine;

public class DebugPauseGame : MonoBehaviour
{
	public GameObject pauseGameObj;

	public GameObject pauseGameParent;

	private DebugFlagsScript debugFlag;

	private DebugFPS debugFPS;

	private bool setFlag;

	private GameObject obj;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (pauseGameParent == null)
		{
			pauseGameParent = debugFlag.GetParentObj();
		}
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.DebugPauseGame && !setFlag)
		{
			if (obj == null)
			{
				if (pauseGameParent != null)
				{
					obj = debugFlag.SpawnFPSObject(pauseGameObj, pauseGameParent);
				}
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
				Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
				Transform[] array = componentsInChildren;
				foreach (Transform transform in array)
				{
					transform.gameObject.layer = pauseGameParent.layer;
				}
			}
			else
			{
				obj.SetActive(true);
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.DebugPauseGame && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
	}
}
