using UnityEngine;

public class OnlineStatusPopup : MonoBehaviour
{
	private void OnEnable()
	{
		UICamera.useInputEnabler = true;
	}

	private void OnDisable()
	{
		UICamera.useInputEnabler = false;
	}

	private void OnReconnectServer()
	{
		PlayerInfoScript.GetInstance().ReloadGame();
	}
}
