using UnityEngine;

public class DebugAnimButtonDisplay : MonoBehaviour
{
	public GameObject animButtonObj;

	public GameObject animButtonParent;

	private DebugFlagsScript debugFlag;

	private bool setFlag;

	private GameObject obj;

	private CharAnimDebug currentDebug;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (animButtonParent == null)
		{
			animButtonParent = debugFlag.GetParentObj();
		}
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.debugFacial == currentDebug)
		{
			return;
		}
		if (debugFlag.battleDisplay.debugFacial == CharAnimDebug.None)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			PanelManagerBattle.GetInstance().uiCamera.GetComponent<Camera>().enabled = true;
		}
		else
		{
			SetPlayer(debugFlag.battleDisplay.debugFacial);
		}
		currentDebug = debugFlag.battleDisplay.debugFacial;
	}

	private void SetPlayer(CharAnimDebug debug)
	{
		PanelManagerBattle.GetInstance().uiCamera.GetComponent<Camera>().enabled = false;
		if (obj == null)
		{
			if (animButtonParent != null)
			{
				obj = debugFlag.SpawnFPSObject(animButtonObj, animButtonParent);
			}
			obj.SetActive(true);
			obj.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			obj.SetActive(true);
		}
		int num = ((debug != CharAnimDebug.Me) ? 1 : 0);
		Transform transform = obj.transform.Find("DebugCam_0");
		Transform transform2 = obj.transform.Find("DebugCam_1");
		CWPlayCharacterAnimation[] componentsInChildren = obj.GetComponentsInChildren<CWPlayCharacterAnimation>(true);
		CWPlayCharacterAnimation[] array = componentsInChildren;
		foreach (CWPlayCharacterAnimation cWPlayCharacterAnimation in array)
		{
			cWPlayCharacterAnimation.player = num;
		}
		transform.GetComponent<Camera>().enabled = debug == CharAnimDebug.Me;
		transform2.GetComponent<Camera>().enabled = debug == CharAnimDebug.Them;
		Transform transform3 = obj.transform.Find("Panel");
		transform3.transform.localPosition = new Vector3(-2100f * (float)num, 100f, 0f);
	}
}
