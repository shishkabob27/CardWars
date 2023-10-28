using UnityEngine;

public class SLOTOmniture : SLOTGameSingleton<SLOTOmniture>
{
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.adobe.adms.TrackingHelper");
			AndroidJavaObject androidJavaObject = null;
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
				androidJavaClass.CallStatic("configureAppMeasurement", androidJavaObject);
				androidJavaClass.CallStatic("startActivity", androidJavaObject);
			}
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.adobe.adms.TrackingHelper");
		using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			if (paused)
			{
				androidJavaClass.CallStatic("stopActivity");
				return;
			}
			androidJavaClass.CallStatic("startActivity", @static);
		}
	}
}
