using UnityEngine;

public class KFFPlayhavenPlacement : MonoBehaviour
{
	public string placementTag;

	public string CustomAndroidURL;

	public void OnClick()
	{
		string text = SystemInfo.deviceModel.ToLower();
		if (text.IndexOf("amazon") >= 0 && CustomAndroidURL != null)
		{
			Application.OpenURL(CustomAndroidURL);
		}
		else if ((bool)KFFRequestorController.GetInstance())
		{
			KFFRequestorController.GetInstance().RequestContent(placementTag);
		}
	}
}
