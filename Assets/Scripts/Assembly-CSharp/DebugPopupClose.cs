using UnityEngine;

public class DebugPopupClose : MonoBehaviour
{
	private void OnClick()
	{
		if (DebugPopupScript.NumPopups > 0)
		{
			DebugPopupScript.NumPopups--;
		}
		Object.Destroy(base.transform.parent.gameObject);
	}
}
