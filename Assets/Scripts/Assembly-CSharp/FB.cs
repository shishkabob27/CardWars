using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Facebook;
using UnityEngine;

public sealed class FB : ScriptableObject
{
	public sealed class AppEvents
	{
		public static bool LimitEventUsage
		{
			get
			{
				return facebook != null && facebook.LimitEventUsage;
			}
			set
			{
				facebook.LimitEventUsage = value;
			}
		}

		public static void LogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			FacebookImpl.AppEventsLogEvent(logEvent, valueToSum, parameters);
		}

		public static void LogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
		{
			FacebookImpl.AppEventsLogPurchase(logPurchase, currency, parameters);
		}
	}

	public sealed class Canvas
	{
		public static void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
		{
			FacebookImpl.Pay(product, action, quantity, quantityMin, quantityMax, requestId, pricepointId, testCurrency, callback);
		}

		public static void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate = 0, params FBScreen.Layout[] layoutParams)
		{
			FBScreen.SetResolution(width, height, fullscreen, preferredRefreshRate, layoutParams);
		}

		public static void SetAspectRatio(int width, int height, params FBScreen.Layout[] layoutParams)
		{
			FBScreen.SetAspectRatio(width, height, layoutParams);
		}
	}

	public sealed class Android
	{
		public static string KeyHash
		{
			get
			{
				AndroidFacebook androidFacebook = facebook as AndroidFacebook;
				return (!(androidFacebook != null)) ? string.Empty : androidFacebook.KeyHash;
			}
		}
	}

	public abstract class RemoteFacebookLoader : MonoBehaviour
	{
		public delegate void LoadedDllCallback(IFacebook fb);

		private const string facebookNamespace = "Facebook.";

		private const int maxRetryLoadCount = 3;

		private static int retryLoadCount;

		protected abstract string className { get; }

		public static IEnumerator LoadFacebookClass(string className, LoadedDllCallback callback)
		{
			string url = string.Format(IntegratedPluginCanvasLocation.DllUrl, className);
			WWW www = new WWW(url);
			FbDebug.Log("loading dll: " + url);
			yield return www;
			if (www.error != null)
			{
				FbDebug.Error(www.error);
				if (retryLoadCount < 3)
				{
					retryLoadCount++;
				}
				www.Dispose();
				yield break;
			}
            
            byte[] assemblyBytes = www.bytes;
            Assembly assembly = Assembly.Load(assemblyBytes);
            //Assembly assembly = Security.LoadAndVerifyAssembly(www.bytes);
            if (assembly == null)
			{
				FbDebug.Error("Could not securely load assembly from " + url);
				www.Dispose();
				yield break;
			}
			Type facebookClass = assembly.GetType("Facebook." + className);
			if (facebookClass == null)
			{
				FbDebug.Error(className + " not found in assembly!");
				www.Dispose();
				yield break;
			}
			IFacebook fb = typeof(FBComponentFactory).GetMethod("GetComponent").MakeGenericMethod(facebookClass).Invoke(null, new object[1] { IfNotExist.AddNew }) as IFacebook;
			if (fb == null)
			{
				FbDebug.Error(className + " couldn't be created.");
				www.Dispose();
			}
			else
			{
				callback(fb);
				www.Dispose();
			}
		}

		private IEnumerator Start()
		{
			IEnumerator loader = LoadFacebookClass(className, OnDllLoaded);
			while (loader.MoveNext())
			{
				yield return loader.Current;
			}
			UnityEngine.Object.Destroy(this);
		}

		private void OnDllLoaded(IFacebook fb)
		{
			facebook = fb;
			FB.OnDllLoaded();
		}
	}

	public abstract class CompiledFacebookLoader : MonoBehaviour
	{
		protected abstract IFacebook fb { get; }

		private void Start()
		{
			facebook = fb;
			OnDllLoaded();
			UnityEngine.Object.Destroy(this);
		}
	}

	public static InitDelegate OnInitComplete;

	public static HideUnityDelegate OnHideUnity;

	private static IFacebook facebook;

	private static string authResponse;

	private static bool isInitCalled;

	private static string appId;

	private static bool cookie;

	private static bool logging;

	private static bool status;

	private static bool xfbml;

	private static bool frictionlessRequests;

	private static IFacebook FacebookImpl
	{
		get
		{
			if (facebook == null)
			{
				throw new NullReferenceException("Facebook object is not yet loaded.  Did you call FB.Init()?");
			}
			return facebook;
		}
	}

	public static string AppId
	{
		get
		{
			return appId;
		}
	}

	public static string UserId
	{
		get
		{
			return (facebook == null) ? string.Empty : facebook.UserId;
		}
	}

	public static string AccessToken
	{
		get
		{
			return (facebook == null) ? string.Empty : facebook.AccessToken;
		}
	}

	public static DateTime AccessTokenExpiresAt
	{
		get
		{
			return (facebook == null) ? DateTime.MinValue : facebook.AccessTokenExpiresAt;
		}
	}

	public static bool IsLoggedIn
	{
		get
		{
			return facebook != null && facebook.IsLoggedIn;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return facebook != null && facebook.IsInitialized;
		}
	}

	public static void Init(InitDelegate onInitComplete, HideUnityDelegate onHideUnity = null, string authResponse = null)
	{
		Init(onInitComplete, FBSettings.AppId, FBSettings.Cookie, FBSettings.Logging, FBSettings.Status, FBSettings.Xfbml, FBSettings.FrictionlessRequests, onHideUnity, authResponse);
	}

	public static void Init(InitDelegate onInitComplete, string appId, bool cookie = true, bool logging = true, bool status = true, bool xfbml = false, bool frictionlessRequests = true, HideUnityDelegate onHideUnity = null, string authResponse = null)
	{
		FB.appId = appId;
		FB.cookie = cookie;
		FB.logging = logging;
		FB.status = status;
		FB.xfbml = xfbml;
		FB.frictionlessRequests = frictionlessRequests;
		FB.authResponse = authResponse;
		OnInitComplete = onInitComplete;
		OnHideUnity = onHideUnity;
		if (!isInitCalled)
		{
			FBBuildVersionAttribute versionAttributeOfType = FBBuildVersionAttribute.GetVersionAttributeOfType(typeof(IFacebook));
			if (versionAttributeOfType == null)
			{
				FbDebug.Warn("Cannot find Facebook SDK Version");
			}
			else
			{
				FbDebug.Info(string.Format("Using SDK {0}, Build {1}", versionAttributeOfType.SdkVersion, versionAttributeOfType.BuildVersion));
			}
			FBComponentFactory.GetComponent<AndroidFacebookLoader>();
			isInitCalled = true;
		}
		else
		{
			FbDebug.Warn("FB.Init() has already been called.  You only need to call this once and only once.");
			if (FacebookImpl != null)
			{
				OnDllLoaded();
			}
		}
	}

	private static void OnDllLoaded()
	{
		FBBuildVersionAttribute versionAttributeOfType = FBBuildVersionAttribute.GetVersionAttributeOfType(FacebookImpl.GetType());
		if (versionAttributeOfType != null)
		{
			FbDebug.Log(string.Format("Finished loading Facebook dll. Version {0} Build {1}", versionAttributeOfType.SdkVersion, versionAttributeOfType.BuildVersion));
		}
		FacebookImpl.Init(OnInitComplete, appId, cookie, logging, status, xfbml, FBSettings.ChannelUrl, authResponse, frictionlessRequests, OnHideUnity);
	}

	public static void Login(string scope = "", FacebookDelegate callback = null)
	{
		FacebookImpl.Login(scope, callback);
	}

	public static void Logout()
	{
		FacebookImpl.Logout();
	}

	public static void AppRequest(string message, OGActionType actionType, string objectId, string[] to, string data = "", string title = "", FacebookDelegate callback = null)
	{
		FacebookImpl.AppRequest(message, actionType, objectId, to, null, null, null, data, title, callback);
	}

	public static void AppRequest(string message, OGActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
	{
		FacebookImpl.AppRequest(message, actionType, objectId, null, filters, excludeIds, maxRecipients, data, title, callback);
	}

	public static void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
	{
		FacebookImpl.AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
	}

	public static void Feed(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
	{
		FacebookImpl.FeedRequest(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference, properties, callback);
	}

	public static void API(string query, HttpMethod method, FacebookDelegate callback = null, Dictionary<string, string> formData = null)
	{
		FacebookImpl.API(query, method, formData, callback);
	}

	public static void API(string query, HttpMethod method, FacebookDelegate callback, WWWForm formData)
	{
		FacebookImpl.API(query, method, formData, callback);
	}

	[Obsolete("use FB.ActivateApp()")]
	public static void PublishInstall(FacebookDelegate callback = null)
	{
		FacebookImpl.PublishInstall(AppId, callback);
	}

	public static void ActivateApp()
	{
		FacebookImpl.ActivateApp(AppId);
	}

	public static void GetDeepLink(FacebookDelegate callback)
	{
		FacebookImpl.GetDeepLink(callback);
	}

	public static void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
	{
		FacebookImpl.GameGroupCreate(name, description, privacy, callback);
	}

	public static void GameGroupJoin(string id, FacebookDelegate callback = null)
	{
		FacebookImpl.GameGroupJoin(id, callback);
	}
}
