using UnityEngine;

public class Kochava : MonoBehaviour
{
	public enum KochSessionTracking
	{
		full = 0,
		basic = 1,
		minimal = 2,
		none = 3,
	}

	public string kochavaAppId;
	public string kochavaAppIdIOS;
	public string kochavaAppIdAndroid;
	public string kochavaAppIdKindle;
	public string kochavaAppIdBlackberry;
	public string kochavaAppIdWindowsPhone;
	public bool debugMode;
	public bool incognitoMode;
	public bool requestAttribution;
	public string appVersion;
	public string appIdentifier;
	public string partnerName;
	public bool appLimitAdTracking;
	public string userAgent;
	public bool adidSupressed;
	public string appCurrency;
	public KochSessionTracking sessionTracking;
}
