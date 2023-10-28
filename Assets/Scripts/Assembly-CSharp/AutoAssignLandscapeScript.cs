using UnityEngine;

public class AutoAssignLandscapeScript : MonoBehaviour
{
	private void OnClick()
	{
		CWDeckController.GetInstance().SetLandscapes();
		LandscapePreviewController instance = LandscapePreviewController.GetInstance();
		if (instance != null)
		{
			instance.UpdatePreview();
		}
	}
}
