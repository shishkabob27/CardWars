using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
	public class TwitterAndroid
	{
		private static AndroidJavaObject _plugin;

		static TwitterAndroid()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.TwitterPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}

		public static void init(string consumerKey, string consumerSecret, string callbackUrlScheme = "twitterplugin")
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("init", consumerKey, consumerSecret, callbackUrlScheme);
			}
		}

		public static bool isLoggedIn()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			return _plugin.Call<bool>("isLoggedIn", new object[0]);
		}

		public static string getAccessToken()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return string.Empty;
			}
			return _plugin.Call<string>("getAccessToken", new object[0]);
		}

		public static string getTokenSecret()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return string.Empty;
			}
			return _plugin.Call<string>("getTokenSecret", new object[0]);
		}

		public static void showLoginDialog(bool useExternalBrowserForAuthentication = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showLoginDialog", useExternalBrowserForAuthentication);
			}
		}

		public static void logout()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("logout");
			}
		}

		public static void postStatusUpdate(string status)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("status", status);
			Dictionary<string, string> parameters = dictionary;
			performRequest("post", "/1.1/statuses/update.json", parameters);
		}

		public static void postStatusUpdate(string update, byte[] image)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("postUpdateWithImage", update, image);
			}
		}

		public static void getHomeTimeline()
		{
			performRequest("get", "/1.1/statuses/home_timeline.json", null);
		}

		public static void getFollowers()
		{
			performRequest("get", "/1.1/followers/ids.json", null);
		}

		public static void performRequest(string methodType, string path, Dictionary<string, string> parameters)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				string text = ((parameters == null) ? string.Empty : parameters.toJson());
				_plugin.Call("performRequest", methodType, path, text);
			}
		}
	}
}
