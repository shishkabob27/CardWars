using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook
{
	internal class EditorFacebook : AbstractFacebook, IFacebook
	{
		private AbstractFacebook fb;

		private FacebookDelegate loginCallback;

		public override int DialogMode
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override bool LimitEventUsage
		{
			get
			{
				return limitEventUsage;
			}
			set
			{
				limitEventUsage = value;
			}
		}

		protected override void OnAwake()
		{
			StartCoroutine(FB.RemoteFacebookLoader.LoadFacebookClass("CanvasFacebook", OnDllLoaded));
		}

		public override void Init(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null)
		{
			StartCoroutine(OnInit(onInitComplete, appId, cookie, logging, status, xfbml, channelUrl, authResponse, frictionlessRequests, hideUnityDelegate));
		}

		private IEnumerator OnInit(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null)
		{
			while (fb == null)
			{
				yield return null;
			}
			fb.Init(onInitComplete, appId, cookie, logging, status, xfbml, channelUrl, authResponse, frictionlessRequests, hideUnityDelegate);
			isInitialized = true;
			if (onInitComplete != null)
			{
				onInitComplete();
			}
		}

		private void OnDllLoaded(IFacebook fb)
		{
			this.fb = (AbstractFacebook)fb;
		}

		public override void Login(string scope = "", FacebookDelegate callback = null)
		{
			AddAuthDelegate(callback);
			FBComponentFactory.GetComponent<EditorFacebookAccessToken>();
		}

		public override void Logout()
		{
			isLoggedIn = false;
			userId = string.Empty;
			accessToken = string.Empty;
			fb.UserId = string.Empty;
			fb.AccessToken = string.Empty;
		}

		public override void AppRequest(string message, OGActionType actionType, string objectId, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
		{
			fb.AppRequest(message, actionType, objectId, to, filters, excludeIds, maxRecipients, data, title, callback);
		}

		public override void FeedRequest(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
		{
			fb.FeedRequest(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference, properties, callback);
		}

		public override void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
		{
			FbDebug.Info("Pay method only works with Facebook Canvas.  Does nothing in the Unity Editor, iOS or Android");
		}

		public override void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
		{
			throw new PlatformNotSupportedException("There is no Facebook GameGroupCreate Dialog on Editor");
		}

		public override void GameGroupJoin(string id, FacebookDelegate callback = null)
		{
			throw new PlatformNotSupportedException("There is no Facebook GameGroupJoin Dialog on Editor");
		}

		public override void GetAuthResponse(FacebookDelegate callback = null)
		{
			fb.GetAuthResponse(callback);
		}

		public override void PublishInstall(string appId, FacebookDelegate callback = null)
		{
		}

		public override void ActivateApp(string appId = null)
		{
			FbDebug.Info("This only needs to be called for iOS or Android.");
		}

		public override void GetDeepLink(FacebookDelegate callback)
		{
			FbDebug.Info("No Deep Linking in the Editor");
			if (callback != null)
			{
				callback(new FBResult("<platform dependent>"));
			}
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			FbDebug.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
		{
			FbDebug.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		public void MockLoginCallback(FBResult result)
		{
			UnityEngine.Object.Destroy(FBComponentFactory.GetComponent<EditorFacebookAccessToken>());
			if (result.Error != null)
			{
				BadAccessToken(result.Error);
				return;
			}
			try
			{
				List<object> list = (List<object>)Json.Deserialize(result.Text);
				List<string> list2 = new List<string>();
				foreach (object item in list)
				{
					if (item is Dictionary<string, object>)
					{
						Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
						if (dictionary.ContainsKey("body"))
						{
							list2.Add((string)dictionary["body"]);
						}
					}
				}
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)Json.Deserialize(list2[0]);
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)Json.Deserialize(list2[1]);
				if (FB.AppId != (string)dictionary3["id"])
				{
					BadAccessToken("Access token is not for current app id: " + FB.AppId);
					return;
				}
				userId = (string)dictionary2["id"];
				fb.UserId = userId;
				fb.AccessToken = accessToken;
				isLoggedIn = true;
				OnAuthResponse(new FBResult(string.Empty));
			}
			catch (Exception ex)
			{
				BadAccessToken("Could not get data from access token: " + ex.Message);
			}
		}

		public void MockCancelledLoginCallback()
		{
			OnAuthResponse(new FBResult(string.Empty));
		}

		private void BadAccessToken(string error)
		{
			FbDebug.Error(error);
			userId = string.Empty;
			fb.UserId = string.Empty;
			accessToken = string.Empty;
			fb.AccessToken = string.Empty;
			FBComponentFactory.GetComponent<EditorFacebookAccessToken>();
		}
	}
}
