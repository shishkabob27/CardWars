using UnityEngine;

public class BusyIconController : MonoBehaviour
{
	public GameObject busyIcon;

	public Camera busyIconCamera;

	private bool busy;

	protected virtual void Start()
	{
		ShowBusyIcon(busy);
	}

	public void ShowBusyIcon(bool b)
	{
		busy = b;
		if (busyIcon != null)
		{
			busyIcon.SetActive(b);
		}
		if (busyIconCamera != null)
		{
			busyIconCamera.enabled = b;
		}
	}
}
