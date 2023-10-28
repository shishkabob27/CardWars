using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook
{
	public interface IFacebook
	{
		string UserId { get; }

		string AccessToken { get; }

		DateTime AccessTokenExpiresAt { get; }

		bool IsInitialized { get; }

		bool IsLoggedIn { get; }

		int DialogMode { get; set; }

		bool LimitEventUsage { get; set; }

		void Init(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null);

		void Login(string scope = "", FacebookDelegate callback = null);

		void Logout();

		void GetAuthResponse(FacebookDelegate callback = null);

		void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null);

		void AppRequest(string message, OGActionType actionType, string objectId, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null);

		void FeedRequest(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null);

		void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null);

		void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null);

		void GameGroupJoin(string id, FacebookDelegate callback = null);

		void API(string query, HttpMethod method, Dictionary<string, string> formData = null, FacebookDelegate callback = null);

		void API(string query, HttpMethod method, WWWForm formData, FacebookDelegate callback = null);

		void AppEventsLogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null);

		void AppEventsLogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null);

		[Obsolete("use ActivateApp")]
		void PublishInstall(string appId, FacebookDelegate callback = null);

		void ActivateApp(string appId = null);

		void GetDeepLink(FacebookDelegate callback);
	}
}
