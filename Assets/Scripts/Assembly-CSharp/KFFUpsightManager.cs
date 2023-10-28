using UnityEngine;

public class KFFUpsightManager : MonoBehaviour
{
	public string androidAppToken;

	public string androidAppSecret;

	public string gcmProjectNumber;

	public string androidAppTokenDebug;

	public string androidAppSecretDebug;

	public string gcmProjectNumberDebug;

	public string iosAppToken;

	public string iosAppSecret;

	public string iosAppTokenDebug;

	public string iosAppSecretDebug;

	private KFFNetwork.WWWInfo wwwVerifyPurchase;

	private string PurchaseID;

	public static bool UpsightIAPLoggingEnabled = true;

	private void Start()
	{
		string text = androidAppToken;
		string text2 = androidAppSecret;
		string text3 = gcmProjectNumber;
		string text4 = iosAppToken;
		string text5 = iosAppSecret;
		Upsight.init(text, text2, text3);
		TFUtils.DebugLog("Initialized Upsight. androidAppToken: " + text + ", androidAppSecret: " + text2 + ", gcmProjectNumber: " + text3, "upsight");
		Upsight.requestAppOpen();
		Upsight.preloadContentRequest("game_launch");
		TFUtils.DebugLog("Preloading content: game_launch", "upsight");
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			Upsight.requestAppOpen();
		}
	}
}
