using System;
using System.Collections.Generic;
using System.Net;
using MiniJSON;
using UnityEngine;

public class SQAuth
{
	public delegate void OnUserDataFn(Session session, int userID);

	public delegate void KFFWWWRequestCallback(object wwwinfo, object obj, string str, object param);

	public delegate object KFFSendWWWRequestWithFormCallback(WWWForm form, string scriptNameAndParams, KFFWWWRequestCallback cb, object callbackParam);

	public delegate string LoadPlayerNameCallback();

	private const string AUTH_REQUEST = "authRequest";

	private const string DO_GC_AUTH = "do_gc_auth";

	private const string SETTINGS = "settings";

	public static bool g_reassignID;

	private bool ALLOW_DEBUG = true;

	public bool loggedIn;

	private string currentNonce;

	private RuntimePlatform platform;

	public static KFFSendWWWRequestWithFormCallback KFFSendWWWRequestWithFormFunction;

	public static LoadPlayerNameCallback LoadPlayerNameFunction;

	public SQAuth(RuntimePlatform platform)
	{
		this.platform = platform;
	}

	public void AuthUser(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		g_reassignID = false;
		CheckNonce(session, callback, doFacebookAuth, fbAccessToken);
	}

	public bool IsAuthenticated()
	{
		return currentNonce != null;
	}

	private void CheckNonce(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		if (currentNonce == null)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status != HttpStatusCode.OK)
				{
					callback((Dictionary<string, object>)Json.Deserialize(TFServer.NETWORK_ERROR_JSON), status);
					return;
				}
				try
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					currentNonce = (string)dictionary["nonce"];
					PlatformAuth(session, callback, doFacebookAuth, fbAccessToken);
				}
				catch (KeyNotFoundException)
				{
					callback((Dictionary<string, object>)Json.Deserialize(TFServer.NETWORK_ERROR_JSON), status);
				}
			};
			session.Server.PreAuth(callback2);
		}
		else
		{
			PlatformAuth(session, callback, doFacebookAuth, fbAccessToken);
		}
	}

	private void PlatformAuth(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		if (doFacebookAuth && KFFSendWWWRequestWithFormFunction != null)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, status);
				}
				if (session.Server.IsNetworkError(data))
				{
					callback(data, status);
				}
				else if ((bool)data["success"])
				{
					PlatformAuth2(session, callback, doFacebookAuth, fbAccessToken);
				}
				else
				{
					string text = ((LoadPlayerNameFunction == null) ? null : LoadPlayerNameFunction());
					if (text != null && text.Length > 0)
					{
						g_reassignID = true;
						PlatformAuth2(session, callback, false, null);
					}
					else
					{
						PlatformAuth2(session, callback, doFacebookAuth, fbAccessToken);
					}
				}
			};
			string url = SQSettings.SERVER_URL + "account/check_id/" + TFUtils.FacebookID;
			session.Server.GetToJSON(url, callback2);
		}
		else
		{
			PlatformAuth2(session, callback, doFacebookAuth, fbAccessToken);
		}
	}

	private void PlatformAuth2(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		if (!string.IsNullOrEmpty(fbAccessToken) && fbAccessToken != TFUtils.FacebookID)
		{
			AuthFromGameCenter(session, fbAccessToken, TFUtils.FacebookID, callback);
		}
		else
		{
			AuthFromGameCenter(session, TFUtils.FacebookID, TFUtils.FacebookID, callback);
		}
		session.Username = TFUtils.FacebookID;
	}

	private void AuthFromFbAccessToken(Session session, string accessToken, string expDate, TFServer.JsonResponseHandler callback)
	{
		TFUtils.DebugLog("attempting auth to TF");
		session.Server.FbLogin(accessToken, expDate, currentNonce, callback);
	}

	private void AuthFromGameCenter(Session session, string playerId, string alias, TFServer.JsonResponseHandler callback)
	{
		TFUtils.DebugLog("attempting gc auth to TF");
		session.Server.GcLogin(playerId, alias, currentNonce, callback);
	}

	private void DoLoginIOS(Session session, bool doFacebookAuth, string fbAccessToken, bool doGcAuth, TFServer.JsonResponseHandler callback)
	{
		throw new InvalidOperationException("Unsupported platform for iOS login");
	}

	private void DoLoginAndroid(Session session, TFServer.JsonResponseHandler callback)
	{
		AuthFromGameCenter(session, TFUtils.FacebookID, TFUtils.FacebookID, callback);
		session.Username = TFUtils.FacebookID;
	}
}
