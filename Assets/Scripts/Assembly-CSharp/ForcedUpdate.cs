using UnityEngine;

public class ForcedUpdate : MonoBehaviour
{
	public void GoToStore()
	{
		string updateUrl = SessionManager.GetInstance().theSession.UpdateUrl;
		if (updateUrl != null)
		{
			Application.OpenURL(updateUrl);
		}
		else
		{
			Application.OpenURL("http://lmgtfy.com/?q=adventure+time+card+wars");
		}
	}
}
