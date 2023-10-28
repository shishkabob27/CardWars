using System;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook
{
	public abstract class AbstractFacebook : MonoBehaviour, IFacebook
	{
		internal const string AccessTokenKey = "access_token";

		protected bool isInitialized;

		protected bool isLoggedIn;

		protected string userId;

		protected string accessToken;

		protected DateTime accessTokenExpiresAt;

		protected bool limitEventUsage;

		private List<FacebookDelegate> authDelegates;

		private int nextAsyncId;

		private Dictionary<string, FacebookDelegate> facebookDelegates;

		public bool IsInitialized
		{
			get
			{
				return isInitialized;
			}
		}

		public bool IsLoggedIn
		{
			get
			{
				return isLoggedIn;
			}
		}

		public string UserId
		{
			get
			{
				return userId;
			}
			set
			{
				userId = value;
			}
		}

		public virtual string AccessToken
		{
			get
			{
				return accessToken;
			}
			set
			{
				accessToken = value;
			}
		}

		public virtual DateTime AccessTokenExpiresAt
		{
			get
			{
				return accessTokenExpiresAt;
			}
		}

		public abstract int DialogMode { get; set; }

		public abstract bool LimitEventUsage { get; set; }

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			isInitialized = false;
			isLoggedIn = false;
			userId = string.Empty;
			accessToken = string.Empty;
			accessTokenExpiresAt = DateTime.MinValue;
			authDelegates = new List<FacebookDelegate>();
			nextAsyncId = 0;
			facebookDelegates = new Dictionary<string, FacebookDelegate>();
			OnAwake();
		}

		protected abstract void OnAwake();

		public abstract void Init(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null);

		public abstract void Login(string scope = "", FacebookDelegate callback = null);

		public abstract void Logout();

		public virtual void GetAuthResponse(FacebookDelegate callback = null)
		{
			AddAuthDelegate(callback);
		}

		[Obsolete]
		public virtual void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
		{
			AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
		}

		public abstract void AppRequest(string message, OGActionType actionType, string objectId, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null);

		public abstract void FeedRequest(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null);

		public abstract void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null);

		public abstract void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null);

		public abstract void GameGroupJoin(string id, FacebookDelegate callback = null);

		public virtual void API(string query, HttpMethod method, Dictionary<string, string> formData = null, FacebookDelegate callback = null)
		{
			Dictionary<string, string> dictionary = ((formData == null) ? new Dictionary<string, string>() : CopyByValue(formData));
			if (!dictionary.ContainsKey("access_token") && !query.Contains("access_token="))
			{
				dictionary["access_token"] = AccessToken;
			}
			AsyncRequestString.Request(GetGraphUrl(query), method, dictionary, callback);
		}

		public virtual void API(string query, HttpMethod method, WWWForm formData, FacebookDelegate callback = null)
		{
			if (formData == null)
			{
				formData = new WWWForm();
			}
			formData.AddField("access_token", AccessToken);
			AsyncRequestString.Request(GetGraphUrl(query), method, formData, callback);
		}

		public abstract void PublishInstall(string appId, FacebookDelegate callback = null);

		public abstract void ActivateApp(string appId = null);

		public abstract void GetDeepLink(FacebookDelegate callback);

		public abstract void AppEventsLogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null);

		public abstract void AppEventsLogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null);

		protected void AddAuthDelegate(FacebookDelegate callback)
		{
			authDelegates.Add(callback);
		}

		protected void OnAuthResponse(FBResult result)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["is_logged_in"] = IsLoggedIn;
			dictionary["user_id"] = UserId;
			dictionary["access_token"] = AccessToken;
			dictionary["access_token_expires_at"] = AccessTokenExpiresAt;
			FBResult result2 = new FBResult(Json.Serialize(dictionary), result.Error);
			foreach (FacebookDelegate authDelegate in authDelegates)
			{
				if (authDelegate != null)
				{
					authDelegate(result2);
				}
			}
			authDelegates.Clear();
		}

		protected string AddFacebookDelegate(FacebookDelegate callback)
		{
			nextAsyncId++;
			facebookDelegates.Add(nextAsyncId.ToString(), callback);
			return nextAsyncId.ToString();
		}

		protected void OnFacebookResponse(string uniqueId, FBResult result)
		{
			FacebookDelegate value;
			if (facebookDelegates.TryGetValue(uniqueId, out value))
			{
				if (value != null)
				{
					value(result);
				}
				facebookDelegates.Remove(uniqueId);
			}
		}

		protected Dictionary<string, string> CopyByValue(Dictionary<string, string> data)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(data.Count);
			foreach (KeyValuePair<string, string> datum in data)
			{
				dictionary[datum.Key] = string.Copy(datum.Value);
			}
			return dictionary;
		}

		private string GetGraphUrl(string query)
		{
			if (!query.StartsWith("/"))
			{
				query = "/" + query;
			}
			return IntegratedPluginCanvasLocation.GraphUrl + query;
		}
	}
}
