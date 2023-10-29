using UnityEngine;

public class UserInfo : MonoBehaviour
{
	public UILabel UserID;

	public UILabel ManifestVersion;

	public UILabel Server;

	public UILabel ClientVersion;

	public UILabel BundleID;

	public UILabel PlayerID;

	private void Update()
	{
		if (UserID != null)
		{
			UserID.text = UserID.text.Replace("<Val>", PlayerInfoScript.GetInstance().GetPlayerCode());
		}
		if (ManifestVersion != null)
		{
			ManifestVersion.text = ManifestVersion.text.Replace("<Val>", SessionManager.GetInstance().theSession.GetLocalManifestVersion());
		}
		if (Server != null)
		{
			string serverURL = SQSettings.ServerURL;
			int num = serverURL.IndexOf("//") + 2;
			int num2 = serverURL.IndexOf(".", num);
			string newValue = serverURL.Substring(num, num2 - num);
			Server.text = Server.text.Replace("<Val>", newValue);
		}
		if (ClientVersion != null)
		{
            //ClientVersion.text = ClientVersion.text.Replace("<Val>", KFFCSUtils.GetManifestKeyString("CFBundleVersion"));
            ClientVersion.text = ClientVersion.text.Replace("<Val>", Application.version);
        }
		if (Debug.isDebugBuild)
		{
			if (BundleID != null)
			{
				BundleID.gameObject.SetActive(true);
			}
			if (PlayerID != null)
			{
				PlayerID.gameObject.SetActive(true);
				PlayerID.text = "PlayerID:" + PlayerInfoScript.GetInstance().PlayerName;
			}
		}
	}
}
