using UnityEngine;

public class AuthDialogController : MonoBehaviour
{
	public UIButtonTween AuthDialogTween;

	private static AuthDialogController instance;

	public static AuthDialogController GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		instance = this;
	}

	public void DisplayAuthDialog()
	{
		if (AuthDialogTween != null)
		{
			AuthDialogTween.Play(true);
		}
	}

	private void Update()
	{
	}

	public void PauseGame()
	{
	}

	public void KeepLocal()
	{
		SessionManager sessionManager = SessionManager.GetInstance();
		if (null != sessionManager && sessionManager.theSession != null)
		{
			sessionManager.LocalRemoteSaveGameConflict = false;
			sessionManager.theSession.WebFileServer.DeleteETagFile();
			PlayerInfoScript.GetInstance().Save();
		}
	}

	public void ReloadFromServer()
	{
		SessionManager sessionManager = SessionManager.GetInstance();
		if (null != sessionManager && sessionManager.theSession != null)
		{
			sessionManager.LocalRemoteSaveGameConflict = false;
			sessionManager.ClearSaveStateLocal();
			PlayerInfoScript.GetInstance().ReloadGame();
		}
	}
}
