using UnityEngine;

public class SimpleErrorPopup : MonoBehaviour
{
	public delegate void ErrorPopupButtonClickedCallback(int buttonIndex);

	public UILabel titleLabel;

	public UILabel messageLabel;

	public GameObject[] buttons;

	public bool destroyWhenDone = true;

	private bool done;

	private ErrorPopupButtonClickedCallback callback;

	private void Awake()
	{
		GameObject[] array = buttons;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				UIButtonMessage uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
				if (uIButtonMessage != null)
				{
					uIButtonMessage.target = base.gameObject;
					uIButtonMessage.functionName = "OnButtonClick";
				}
			}
		}
	}

	private void HideTweenDone()
	{
		if (destroyWhenDone)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnButtonClick(GameObject button)
	{
		if (done)
		{
			return;
		}
		done = true;
		if (callback == null)
		{
			return;
		}
		int buttonIndex = -1;
		for (int i = 0; i < buttons.Length; i++)
		{
			if (button == buttons[i])
			{
				buttonIndex = i;
				break;
			}
		}
		callback(buttonIndex);
	}

	public void Setup(string title, string message, int buttonCount, string[] buttonStrings, ErrorPopupButtonClickedCallback cb)
	{
		callback = cb;
		if (titleLabel != null)
		{
			titleLabel.text = title;
		}
		if (messageLabel != null)
		{
			messageLabel.text = message;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			GameObject gameObject = buttons[i];
			if (!(gameObject != null))
			{
				continue;
			}
			gameObject.SetActive(i < buttonCount && (buttonStrings == null || (i < buttonStrings.Length && buttonStrings[i] != null)));
			if (buttonStrings != null && i < buttonStrings.Length)
			{
				UILabel componentInChildren = SLOTGame.GetComponentInChildren<UILabel>(gameObject, false);
				if (componentInChildren != null && !string.IsNullOrEmpty(buttonStrings[i]))
				{
					componentInChildren.text = buttonStrings[i];
				}
			}
		}
	}
}
