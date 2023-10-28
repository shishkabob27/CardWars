using UnityEngine;

public class DebugTimeScale : MonoBehaviour
{
	public UILabel TimeScaleLabel;

	public GameObject timeScaleObj;

	public GameObject timeScaleParent;

	private DebugFlagsScript debugFlag;

	private DebugFPS debugFPS;

	private bool setFlag;

	private GameObject obj;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (!(timeScaleParent == null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Battle UI Panel");
		if (gameObject == null)
		{
			gameObject = GameObject.Find("Menu UI Panel");
		}
		if (gameObject == null)
		{
			gameObject = GameObject.Find("QuestMap_UI_Panel");
		}
		if (gameObject != null)
		{
			timeScaleParent = GameObject.Find("debugParentObj");
			if (timeScaleParent == null)
			{
				timeScaleParent = new GameObject("debugParentObj");
			}
			timeScaleParent.transform.parent = gameObject.transform;
			timeScaleParent.transform.localScale = Vector3.one;
			timeScaleParent.transform.localPosition = new Vector3(600f, 0f, 0f);
		}
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.TimeScaleDisplay && !setFlag)
		{
			if (obj == null)
			{
				if (timeScaleParent != null)
				{
					obj = debugFlag.SpawnFPSObject(timeScaleObj, timeScaleParent);
				}
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
				obj.transform.parent.localRotation = Quaternion.Euler(Vector3.zero);
				Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
				Transform[] array = componentsInChildren;
				foreach (Transform transform in array)
				{
					transform.gameObject.layer = timeScaleParent.layer;
				}
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.TimeScaleDisplay && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
		if (obj != null)
		{
			if (TimeScaleLabel == null)
			{
				TimeScaleLabel = obj.GetComponentInChildren<UILabel>();
			}
			TimeScaleLabel.text = Time.timeScale.ToString();
		}
	}
}
