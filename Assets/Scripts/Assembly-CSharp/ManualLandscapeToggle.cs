using UnityEngine;

public class ManualLandscapeToggle : MonoBehaviour
{
	public UILabel LandscapeName;

	public UISprite LandscapeArt;

	public UISprite LandscapeFrame;

	public int LaneID;

	private void Start()
	{
	}

	public void OnEnable()
	{
		ManualLandscapeScript instance = ManualLandscapeScript.GetInstance();
		if (instance != null)
		{
			instance.Init(this);
		}
	}

	private void OnClick()
	{
		ManualLandscapeScript instance = ManualLandscapeScript.GetInstance();
		if (instance != null)
		{
			instance.ToggleLandscape(this);
		}
	}
}
