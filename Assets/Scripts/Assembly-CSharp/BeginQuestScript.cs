using UnityEngine;

public class BeginQuestScript : MonoBehaviour
{
	public GameObject MainMenuPanel;

	public GameObject FiveReelSlotMachine;

	public bool LeaveScreen;

	public float TransitionSpeed = 0.1f;

	private void Start()
	{
		MainMenuPanel = GameObject.Find("MainMenu_Panel(Clone)");
		if (MainMenuPanel == null)
		{
			MainMenuPanel = GameObject.Find("MainMenu_Panel");
		}
	}

	private void OnClick()
	{
		LeaveScreen = true;
	}

	private void Update()
	{
		if (LeaveScreen)
		{
			base.transform.parent.gameObject.transform.parent.gameObject.transform.localScale = new Vector3(Mathf.Max(base.transform.parent.gameObject.transform.parent.gameObject.transform.localScale.x - Time.deltaTime * TransitionSpeed, 0.01f), Mathf.Max(base.transform.parent.gameObject.transform.parent.gameObject.transform.localScale.y - Time.deltaTime * TransitionSpeed, 0.01f), Mathf.Max(base.transform.parent.gameObject.transform.parent.gameObject.transform.localScale.z - Time.deltaTime * TransitionSpeed, 0.01f));
			MainMenuPanel.transform.localScale = new Vector3(Mathf.Max(MainMenuPanel.transform.localScale.x - Time.deltaTime * TransitionSpeed, 0.01f), Mathf.Max(MainMenuPanel.transform.localScale.y - Time.deltaTime * TransitionSpeed, 0.01f), Mathf.Max(MainMenuPanel.transform.localScale.z - Time.deltaTime * TransitionSpeed, 0.01f));
			if (base.transform.parent.gameObject.transform.parent.gameObject.transform.localScale.x <= 0.01f)
			{
				SLOTGame.InstantiateFX(FiveReelSlotMachine);
				Object.Destroy(MainMenuPanel);
				Object.Destroy(base.transform.parent.gameObject.transform.parent.gameObject);
			}
		}
	}
}
