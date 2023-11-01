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
			Application.OpenURL("https://github.com/shishkabob27/CardWars/releases");
		}
	}
}
