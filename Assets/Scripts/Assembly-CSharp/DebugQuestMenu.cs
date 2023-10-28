using UnityEngine;

public class DebugQuestMenu : MonoBehaviour
{
	public DebugPopupQuestSelector questSelectorPrefab;

	public Canvas canvasRoot;

	private DebugFlagsScript debugFlag;

	private GameObject selectorInst;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
	}

	private void Update()
	{
		if (debugFlag.mapDebug.questMenu && !(canvasRoot == null) && !(questSelectorPrefab == null))
		{
			debugFlag.mapDebug.questMenu = false;
			if (selectorInst == null)
			{
				selectorInst = UnityUtils.InstantiatePrefab(questSelectorPrefab.gameObject, canvasRoot.gameObject);
			}
			SetDebugGuiEnabled(false);
			selectorInst.SetActive(true);
		}
	}

	private void SetDebugGuiEnabled(bool enabled)
	{
		DebugGUI component = debugFlag.GetComponent<DebugGUI>();
		if (component != null)
		{
			component.enabled = enabled;
		}
	}
}
