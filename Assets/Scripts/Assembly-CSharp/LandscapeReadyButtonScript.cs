using UnityEngine;

public class LandscapeReadyButtonScript : MonoBehaviour
{
	private LandscapeManagerScript LandscapeManager;

	private void Start()
	{
		LandscapeManager = LandscapeManagerScript.GetInstance();
	}

	private void OnClick()
	{
		LandscapeManager.AssignOpponentLandscapes();
		LandscapeManager.UpdateLandscapes();
	}
}
