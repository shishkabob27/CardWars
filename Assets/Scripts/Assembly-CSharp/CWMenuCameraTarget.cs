using UnityEngine;

public class CWMenuCameraTarget : MonoBehaviour
{
	public Camera gameCamera;

	public bool followFlag = true;

	public void setFollowFlagTrue()
	{
		followFlag = true;
	}

	private void LateUpdate()
	{
		if (followFlag)
		{
			gameCamera.transform.LookAt(base.transform);
		}
	}
}
