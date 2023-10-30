using UnityEngine;

public class AndroidQuit : MonoBehaviour
{
	private void Confirm()
	{
		base.gameObject.SetActive(false);
		Application.Quit();
		UnityEditor.EditorApplication.isPlaying = false;
	}

	public void Dismiss()
	{
		base.gameObject.SetActive(false);
	}
}
