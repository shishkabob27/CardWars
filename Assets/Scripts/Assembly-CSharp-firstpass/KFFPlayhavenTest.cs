using UnityEngine;

public class KFFPlayhavenTest : MonoBehaviour
{
	public bool DisplayTest;

	public void OnGUI()
	{
		if (DisplayTest)
		{
			if (GUI.Button(new Rect(3 * (Screen.width / 8), 0f, Screen.width / 4, Screen.height / 8), "PlayHaven Test Placement 1"))
			{
				KFFRequestorController.GetInstance().RequestContent("test_placement");
			}
			if (GUI.Button(new Rect(3 * (Screen.width / 8), Screen.height / 8, Screen.width / 4, Screen.height / 8), "PlayHaven Test Placement 2"))
			{
				KFFRequestorController.GetInstance().RequestContent("test_placement_2");
			}
		}
	}
}
