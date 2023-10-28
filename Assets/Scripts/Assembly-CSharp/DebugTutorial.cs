using UnityEngine;

public class DebugTutorial : MonoBehaviour
{
	private DebugFlagsScript displayPanel;

	public GameObject debugTutorialPrefab;

	public GameObject debugObj;

	private PanelManager pMgr;

	private PanelManagerBattle pMgrBattle;

	private PanelManagerDeck pMgrDeck;

	private bool displayed;

	private void Start()
	{
		displayPanel = DebugFlagsScript.GetInstance();
		pMgrBattle = PanelManagerBattle.GetInstance();
	}

	private void Update()
	{
		if (displayPanel.tutorialDebug)
		{
			if (!displayed)
			{
				displayed = true;
				if (debugObj == null)
				{
					debugObj = Object.Instantiate(debugTutorialPrefab);
				}
				if (pMgrBattle != null)
				{
					debugObj.transform.position = new Vector3(-6f, -3f, -100f);
				}
				else if (MapControllerBase.GetInstance() != null)
				{
					debugObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
					debugObj.transform.position = new Vector3(-7f, -3f, 0f);
				}
				else
				{
					debugObj.transform.position = new Vector3(-6f, -3f, 0f);
				}
			}
		}
		else if (displayed)
		{
			displayed = false;
			if (debugObj != null)
			{
				debugObj.transform.position = new Vector3(-20f, -3f, -9.5f);
			}
		}
	}
}
