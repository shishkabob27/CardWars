using UnityEngine;

public class DebugFPS : MonoBehaviour
{
	private DebugFlagsScript debugFlag;

	public GameObject fpsObject;

	public GameObject fpsParent;

	private GameObject obj;

	private bool setFlag;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (!(fpsParent == null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Battle UI Panel");
		if (gameObject == null)
		{
			gameObject = GameObject.Find("Menu UI Panel");
		}
		if (gameObject != null)
		{
			fpsParent = GameObject.Find("debugParentObj");
			if (fpsParent == null)
			{
				fpsParent = new GameObject("debugParentObj");
			}
			fpsParent.transform.parent = gameObject.transform;
			fpsParent.transform.localScale = Vector3.one;
			fpsParent.transform.localPosition = new Vector3(600f, 0f, 0f);
		}
	}

	public GameObject SpawnFPSObject(GameObject parent)
	{
		obj = Object.Instantiate(fpsObject);
		if (parent != null)
		{
			obj.transform.parent = parent.transform;
		}
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
		return obj;
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.FPSDisplay && !setFlag)
		{
			if (obj == null)
			{
				if (fpsParent != null)
				{
					obj = debugFlag.SpawnFPSObject(fpsObject, fpsParent);
				}
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
				obj.layer = obj.transform.parent.gameObject.layer;
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.FPSDisplay && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
	}
}
