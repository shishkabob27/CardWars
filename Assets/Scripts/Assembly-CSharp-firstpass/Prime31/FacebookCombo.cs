using System.Collections.Generic;

namespace Prime31
{
	public static class FacebookCombo
	{
		public static void init()
		{
			FacebookAndroid.init();
		}

		public static string getAppLaunchUrl()
		{
			return FacebookAndroid.getAppLaunchUrl();
		}

		public static void login()
		{
			loginWithReadPermissions(new string[0]);
		}

		public static void loginWithReadPermissions(string[] permissions)
		{
			FacebookAndroid.loginWithReadPermissions(permissions);
		}

		public static void reauthorizeWithReadPermissions(string[] permissions)
		{
			FacebookAndroid.reauthorizeWithReadPermissions(permissions);
		}

		public static void reauthorizeWithPublishPermissions(string[] permissions, FacebookSessionDefaultAudience defaultAudience)
		{
			FacebookAndroid.reauthorizeWithPublishPermissions(permissions, defaultAudience);
		}

		public static bool isSessionValid()
		{
			return FacebookAndroid.isSessionValid();
		}

		public static string getAccessToken()
		{
			return FacebookAndroid.getAccessToken();
		}

		public static List<object> getSessionPermissions()
		{
			return FacebookAndroid.getSessionPermissions();
		}

		public static void logout()
		{
			FacebookAndroid.logout();
		}

		public static void showDialog(string dialogType, Dictionary<string, string> options)
		{
			FacebookAndroid.showDialog(dialogType, options);
		}

		public static void showFacebookShareDialog(Dictionary<string, object> parameters)
		{
			FacebookAndroid.showFacebookShareDialog(parameters);
		}

		public static void setAppVersion(string version)
		{
			FacebookAndroid.setAppVersion(version);
		}

		public static void logEvent(string eventName, Dictionary<string, object> parameters = null)
		{
			FacebookAndroid.logEvent(eventName, parameters);
		}

		public static void logEvent(string eventName, double valueToSum, Dictionary<string, object> parameters = null)
		{
			FacebookAndroid.logEvent(eventName, valueToSum, parameters);
		}
	}
}
