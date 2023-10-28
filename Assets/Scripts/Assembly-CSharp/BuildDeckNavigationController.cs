using UnityEngine;

public class BuildDeckNavigationController : MonoBehaviour
{
	private static BuildDeckNavigationController g_instance;

	private void Start()
	{
		g_instance = this;
	}

	public static BuildDeckNavigationController GetInstance()
	{
		return g_instance;
	}

	public void LaunchBuildDeck()
	{
		base.gameObject.SendMessage("OnClick");
	}
}
