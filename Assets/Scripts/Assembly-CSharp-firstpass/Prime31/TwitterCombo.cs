using System.Collections.Generic;
using System.IO;

namespace Prime31
{
	public static class TwitterCombo
	{
		private static string _username;

		static TwitterCombo()
		{
			TwitterManager.loginSucceededEvent += delegate(string username)
			{
				_username = username;
			};
		}

		public static void init(string consumerKey, string consumerSecret)
		{
			TwitterAndroid.init(consumerKey, consumerSecret);
		}

		public static bool isLoggedIn()
		{
			return TwitterAndroid.isLoggedIn();
		}

		public static string loggedInUsername()
		{
			return _username;
		}

		public static void showLoginDialog()
		{
			TwitterAndroid.showLoginDialog();
		}

		public static void logout()
		{
			TwitterAndroid.logout();
		}

		public static void postStatusUpdate(string status)
		{
			TwitterAndroid.postStatusUpdate(status);
		}

		public static void postStatusUpdate(string status, string pathToImage)
		{
			TwitterAndroid.postStatusUpdate(status, File.ReadAllBytes(pathToImage));
		}

		public static void performRequest(string methodType, string path, Dictionary<string, string> parameters)
		{
			TwitterAndroid.performRequest(methodType, path, parameters);
		}
	}
}
