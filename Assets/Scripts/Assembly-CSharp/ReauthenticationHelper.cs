using System.Collections;

public class ReauthenticationHelper : ReloadDetection
{
	public enum Result
	{
		SUCCESS,
		SUCCESS_FORCED_RESTART,
		FAIL
	}

	public delegate void Callback(Result success);

	public bool Busy { get; private set; }

	private void Awake()
	{
		Busy = false;
	}

	public bool Reauthenticate(Callback callback)
	{
		if (Busy)
		{
			return false;
		}
		if (PlayerInfoScript.GetInstance().Reauthenticate())
		{
			Busy = true;
			StartCoroutine(WaitAuthenticationComplete(callback));
			return true;
		}
		return false;
	}

	private IEnumerator WaitAuthenticationComplete(Callback callback)
	{
		SessionManager sessionMgr = SessionManager.GetInstance();
		while (!sessionMgr.IsReady())
		{
			yield return null;
		}
		Busy = false;
		if (!SocialManager.Instance.IsPlayerAuthenticated())
		{
			callback(Result.FAIL);
		}
		else if (sessionMgr.theSession.NeedsReload || sessionMgr.NeedsForcedUpdate)
		{
			callback(Result.SUCCESS_FORCED_RESTART);
		}
		else
		{
			callback(Result.SUCCESS);
		}
	}
}
