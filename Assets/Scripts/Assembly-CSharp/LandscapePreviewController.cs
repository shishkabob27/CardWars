using UnityEngine;

public class LandscapePreviewController : MonoBehaviour
{
	public LandscapePreviewScript Lane1;

	public LandscapePreviewScript Lane2;

	public LandscapePreviewScript Lane3;

	public LandscapePreviewScript Lane4;

	private static LandscapePreviewController lp_controller;

	private void Awake()
	{
		lp_controller = this;
	}

	public static LandscapePreviewController GetInstance()
	{
		return lp_controller;
	}

	public void UpdatePreview()
	{
		if (Lane1 != null)
		{
			Lane1.UpdatePreview();
		}
		if (Lane2 != null)
		{
			Lane2.UpdatePreview();
		}
		if (Lane3 != null)
		{
			Lane3.UpdatePreview();
		}
		if (Lane4 != null)
		{
			Lane4.UpdatePreview();
		}
	}
}
