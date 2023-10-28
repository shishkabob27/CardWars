using UnityEngine;

public class ReloadDetection : MonoBehaviour
{
	public ReloadHandler menu;

	private SessionManager sessionMgr;

	private void Start()
	{
		sessionMgr = SessionManager.GetInstance();
	}

	private void Update()
	{
		Session theSession = sessionMgr.theSession;
		if (sessionMgr.IsReady() && theSession != null && theSession.NeedsReload && menu != null)
		{
			menu.SwitchToReload();
		}
		if (sessionMgr.NeedsForcedUpdate && menu != null)
		{
			menu.SwitchToUpdate();
		}
	}
}
