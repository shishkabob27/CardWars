using UnityEngine;

public class OpenUrlScript : MonoBehaviour
{
	public bool EULA;

	public bool CartoonNetwork;

	public bool Privacy;

	public bool Custom;

	private string LinkEULA = "http://www.cartoonnetwork.com/mobileapp/cnvid_tos.html";

	private string LinkCN = string.Empty;

	private string LinkPrivacy = "http://www.cartoonnetwork.com/mobileapp/cnvid_privacy.html";

	public string LinkCustom;

	public UIButtonTween FailedConnection;

	private void OnClick()
	{
		if (testInternetConnection())
		{
			if (EULA)
			{
				Application.OpenURL(LinkEULA);
			}
			else if (CartoonNetwork)
			{
				Application.OpenURL(LinkCN);
			}
			else if (Privacy)
			{
				Application.OpenURL(LinkPrivacy);
			}
			else if (Custom)
			{
				Application.OpenURL(LinkCustom);
			}
		}
		else if ((bool)FailedConnection)
		{
			FailedConnection.Play(true);
		}
	}

	private bool testInternetConnection()
	{
		bool flag = false;
		if (Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork && Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			showInternetAlert();
			return false;
		}
		return true;
	}

	private void showInternetAlert()
	{
	}
}
