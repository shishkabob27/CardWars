using UnityEngine;

public class AndroidQuit : MonoBehaviour
{
	private void Confirm()
	{
		base.gameObject.SetActive(false);
		Application.Quit();
	}

	public void Dismiss()
	{
		base.gameObject.SetActive(false);
	}
}
