using UnityEngine;

public class AndroidQuitDetection : MonoBehaviour, IAndroidBackActivator
{
	public GameObject popup;

	public MenuController controller;

	public bool TryActivate()
	{
		if (popup == null)
		{
			KFFAndroidPlugin.ConfirmQuit();
		}
		else if (!popup.activeSelf)
		{
			if (controller != null)
			{
				controller.SwitchToQuit();
			}
			popup.SetActive(true);
		}
		return true;
	}

	public void OnEnable()
	{
		AndroidUiStack.Instance.Add(this);
	}

	public void OnDisable()
	{
		AndroidUiStack.Instance.Remove(this);
	}
}
