using UnityEngine;

public class CrossSceneNavScript : MonoBehaviour
{
	private static CrossSceneNavScript crossSceneNav;

	public GameObject OptionTweenController;

	public bool useTweenController;

	private void Awake()
	{
		crossSceneNav = this;
	}

	public static CrossSceneNavScript GetInstance()
	{
		return crossSceneNav;
	}

	private void OnClick()
	{
		if (useTweenController && OptionTweenController != null)
		{
			OptionTweenController.SendMessage("OnClick");
		}
	}
}
