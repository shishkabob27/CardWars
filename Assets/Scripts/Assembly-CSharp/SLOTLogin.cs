using UnityEngine;

public class SLOTLogin : MonoBehaviour
{
	public NextScreenScript nextScript;

	public GameObject name_label;

	public GameObject continue_button;

	public GameObject back_button;

	public GameObject status_text;

	public GameObject newPlayerDestination;

	public GameObject oldPlayerDestination;

	public UILabel StatusLabel;

	private void OnClick()
	{
		SessionManager instance = SessionManager.GetInstance();
		if (!instance.IsLoggedIn())
		{
			UILabel component = name_label.GetComponent<UILabel>();
			if (component != null)
			{
				PlayerInfoScript.GetInstance().PlayerName = component.text;
				instance.OnReadyCallback = OnReady;
				instance.Login(false, null, component.text);
			}
			base.transform.localPosition = new Vector3(0f, 1000f, 0f);
			back_button.transform.localPosition = new Vector3(0f, 1000f, 0f);
			StatusLabel.text = KFFLocalization.Get("!!PLEASE_WAIT");
		}
	}

	private void OnReady()
	{
	}
}
