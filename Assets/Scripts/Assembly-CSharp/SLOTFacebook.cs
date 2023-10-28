using System;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class SLOTFacebook : MonoBehaviour
{
	public class PortraitCacheItem
	{
		public string facebookUserID;

		public Texture2D portrait;
	}

	public delegate void LoginCallback(bool success);

	public delegate void FacebookCallback(string error, object obj);

	public string app_id;

	public string app_secret;

	private string userID;

	private string email;

	private LoginCallback loginCallback;

	private static string myFacebookUserID;

	private static string myFacebookEmail;

	private static string myFacebookName;

	private static FacebookCallback facebookcallback;

	private static SLOTFacebook the_instance;

	public static SLOTFacebook GetInstance()
	{
		if (the_instance == null)
		{
			the_instance = UnityEngine.Object.FindObjectOfType(typeof(SLOTFacebook)) as SLOTFacebook;
		}
		if (Application.isEditor && Application.isPlaying && !the_instance)
		{
			GameObject gameObject = new GameObject();
			if (gameObject != null)
			{
				the_instance = gameObject.AddComponent(typeof(SLOTFacebook)) as SLOTFacebook;
				gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
				gameObject.name = "AutomaticallyCreatedFacebook";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		return the_instance;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Awake()
	{
		if (the_instance == null)
		{
			the_instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		FacebookCombo.init();
	}

	private void OnInitComplete()
	{
		FBSettings.Xfbml = false;
		FBSettings.Status = false;
		FB.AppEvents.LimitEventUsage = true;
		FB.AppEvents.LogEvent("fb_mobile_activate_app");
	}

	private void OnHideUnity(bool isGameShown)
	{
	}

	public void Login(LoginCallback callback)
	{
	}

	public void Login(string[] permissions, LoginCallback callback)
	{
	}

	public void Logout()
	{
	}

	public bool IsLoggedIn()
	{
		return false;
	}

	public string GetMyUserID()
	{
		return myFacebookUserID;
	}

	public string GetMyEmail()
	{
		return myFacebookEmail;
	}

	public string GetMyName()
	{
		return myFacebookName;
	}

	public void GetMyInfo(FacebookCallback callback)
	{
		facebookcallback = callback;
	}

	public void facebookLogin()
	{
		GetMyInfo(facebookinfoonmecallback);
	}

	public void facebookinfoonmecallback(string err, object obj)
	{
		if (loginCallback != null)
		{
			loginCallback(err == null);
			loginCallback = null;
		}
	}

	public void facebookLoginFailed(P31Error err)
	{
		if (loginCallback != null)
		{
			loginCallback(false);
			loginCallback = null;
		}
	}

	public void facebookDidLogoutEvent()
	{
	}

	public void facebookReauthorizationSucceededEvent()
	{
	}

	public void facebookReauthorizationFailedEvent(P31Error err)
	{
	}

	public void facebookSessionInvalidatedEvent()
	{
	}

	public void GetFriends(Action<string, object> callback)
	{
		GetFriends("id,name", callback);
	}

	public void GetFriends(string fields, Action<string, object> callback)
	{
		if (fields != null && fields.Length > 0)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["fields"] = fields;
		}
	}

	public void RequestDataForMultipleUsers(string idList, string fields, Action<string, object> callback)
	{
		string text = "?ids=" + idList;
		if (fields != null && fields.Length > 0)
		{
			text = text + "&fields=" + fields;
		}
	}
}
