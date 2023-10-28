using UnityEngine;

internal class CameraManager
{
	public static void ActivateCamera(Camera cam)
	{
		SetCameraActive(cam, true, true);
	}

	public static void DeactivateCamera(Camera cam)
	{
		SetCameraActive(cam, false, false);
	}

	public static void SetCameraActive(Camera cam, bool enable)
	{
		SetCameraActive(cam, enable, enable);
	}

	public static void SetCameraActive(Camera cam, bool enable, bool uiCameraEnable)
	{
		if (cam != null)
		{
			cam.gameObject.SetActive(enable);
			cam.enabled = enable;
			UICamera component = cam.GetComponent<UICamera>();
			if ((bool)component)
			{
				component.enabled = uiCameraEnable;
			}
		}
	}
}
