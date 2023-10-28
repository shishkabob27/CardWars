using UnityEngine;

public class DisplayConnectionStatus : MonoBehaviour
{
	public GameObject showConnecting;

	public GameObject hideConnecting;

	private static DisplayConnectionStatus g_displayConnectionStatus;

	private void Awake()
	{
		if (g_displayConnectionStatus == null)
		{
			g_displayConnectionStatus = this;
		}
	}

	public static DisplayConnectionStatus GetInstance()
	{
		return g_displayConnectionStatus;
	}

	public void ShowConnectingPanel()
	{
		if (hideConnecting != null)
		{
			UIButtonTween component = hideConnecting.GetComponent<UIButtonTween>();
			component.enabled = false;
		}
		if (!(showConnecting == null))
		{
			UIButtonTween component = showConnecting.GetComponent<UIButtonTween>();
			component.Play(true);
		}
	}

	public void HideConnectingPanel()
	{
		if (showConnecting != null)
		{
			UIButtonTween component = showConnecting.GetComponent<UIButtonTween>();
			component.enabled = false;
		}
		if (!(hideConnecting == null))
		{
			UIButtonTween component = hideConnecting.GetComponent<UIButtonTween>();
			component.Play(true);
		}
	}
}
