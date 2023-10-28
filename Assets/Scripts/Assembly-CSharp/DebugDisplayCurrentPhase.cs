using UnityEngine;

public class DebugDisplayCurrentPhase : MonoBehaviour
{
	public UILabel currentPhaseLabel;

	public GameObject currentPhaseObj;

	public GameObject currentPhaseParent;

	private DebugFlagsScript debugFlag;

	private BattlePhaseManager phaseMgr;

	private bool setFlag;

	public GameObject obj;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
		if (currentPhaseParent == null)
		{
			currentPhaseParent = GameObject.Find("debugParentObj");
			if (currentPhaseParent == null)
			{
				currentPhaseParent = new GameObject("debugParentObj");
			}
			currentPhaseParent.transform.localScale = Vector3.one;
			currentPhaseParent.transform.localPosition = new Vector3(600f, 0f, 0f);
			GameObject gameObject = GameObject.Find("Battle UI Panel");
			if (gameObject == null)
			{
				gameObject = GameObject.Find("Menu UI Panel");
			}
			if (gameObject != null)
			{
				currentPhaseParent.transform.parent = gameObject.transform;
			}
		}
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.displayCurrentPhase && !setFlag)
		{
			if (obj == null && currentPhaseParent != null)
			{
				obj = debugFlag.SpawnFPSObject(currentPhaseObj, currentPhaseParent);
			}
			if (obj != null)
			{
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.displayCurrentPhase && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
		if (obj != null)
		{
			if (currentPhaseLabel == null)
			{
				currentPhaseLabel = obj.GetComponentInChildren<UILabel>();
			}
			currentPhaseLabel.text = phaseMgr.Phase.ToString();
		}
	}
}
