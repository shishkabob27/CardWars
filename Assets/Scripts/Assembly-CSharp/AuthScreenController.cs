using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthScreenController : MonoBehaviour
{
	public ConfirmPopupController GeneralPopup;

	public UIButtonTween GeneralPopupShowTween;

	public UIButtonTween AgeGateShowTween;

	public UIButtonTween LoginOptionsTween;

	public string NextSceneName = "AssetLoader";

	public BusyIconController busyIconController;

	private bool isComplete;

	public static bool AuthStarted;

	private static bool staticInited;

	private void SocialLogin()
	{
		UnityEngine.Debug.Log("Doing SocialLogin");
		PlayerPrefs.DeleteKey("RetrySocialLogin");
		if (SocialManager.Instance.IsPlayerAuthenticated())
		{
			OnPlayerAuthenticated();
			return;
		}
		SocialManager.Instance.AuthenticatePlayer(true);
	}

	private void OnPlayerAuthenticated()
	{
        string @string = PlayerPrefs.GetString("SocialLogin", null);
		string text = SocialManager.Instance.PlayerIdentifierHash();
		if (!string.IsNullOrEmpty(@string) && @string != text)
		{
			PlayerInfoScript.ResetPlayerName();
		}
		PlayerPrefs.SetString("SocialLogin", text);
		SetRetrySocialLoginNextTime();
		if ((bool)DebugFlagsScript.GetInstance() && DebugFlagsScript.GetInstance().resetAchievements)
		{
			TFUtils.DebugLog("resetAchievements is true, attempting to reset...");
			SocialManager.Instance.ResetAllAchievements();
		}
		ClearAuthEvents();

		StartGameLoginFlow();
	}

	private void OnPlayerFailedToAuthenticate(string error)
	{
		UnityEngine.Debug.LogError(error);
		ConfirmPopupController.ClickCallback clickCallback = delegate(bool yes)
		{
			if (yes)
			{
				LoginOptionsTween.Play(true);
			}
			else
			{
                PlayerPrefs.DeleteKey("SocialLogin");
                PlayerPrefs.DeleteKey("RetrySocialLogin");
                PlayerPrefs.DeleteKey("user");
                PlayerPrefs.DeleteKey("pass");
                ClearAuthEvents();
				StartGameLoginFlow();
			}
		};

		StartCoroutine(CoroutineShowPopup(error +"\nRetry?", clickCallback));
	}

    private void OnPlayerCreateAccount()
    {
        ConfirmPopupController.ClickCallback clickCallback = delegate (bool yes)
        {
            if (yes)
            {
                WWW www = new WWW(SQSettings.SERVER_URL+ "account/create?user=" + PlayerPrefs.GetString("user") + "&pass=" + PlayerPrefs.GetString("pass"));
                UnityEngine.Debug.Log("Attempting to create player account: " + www.url);

                while (!www.isDone)
                {
                }

                object response = null;


                try
                {
                    response = MiniJSON.Json.Deserialize(www.text);
                }
                catch (Exception e)
                {
                    OnPlayerFailedToAuthenticate(www.error);
                }

                UnityEngine.Debug.Log(response);

                if (response == null)
                {
                    UnityEngine.Debug.Log(www.error);
                    OnPlayerFailedToAuthenticate(www.error);
                }
                else
                {
                    Dictionary<string, object> responseData = (Dictionary<string, object>)response;

                    if (responseData.ContainsKey("success") && (bool)responseData["success"])
                    {
                        UnityEngine.Debug.Log("Account Created");
						SocialLogin();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Authentication failed");
                        OnPlayerFailedToAuthenticate((string)responseData["message"]);
                    }
                }

            }
            else
            {
                LoginOptionsTween.Play(true);
            }
        };

        StartCoroutine(CoroutineShowPopup("The account, '"+ PlayerPrefs.GetString("user")+"' does not exist.\nWould you like to create it?", clickCallback));
    }

    private void AddAuthEvents()
	{
		SocialManager.Instance.playerAuthenticated += OnPlayerAuthenticated;
		SocialManager.Instance.playerFailedToAuthenticate += OnPlayerFailedToAuthenticate;
	}

	private void ClearAuthEvents()
	{
		SocialManager.Instance.playerAuthenticated -= OnPlayerAuthenticated;
		SocialManager.Instance.playerFailedToAuthenticate -= OnPlayerFailedToAuthenticate;
	}

	private void OnEnable()
	{
		HowOldAreYou.AgeGateDone += OnAgeGateDone;
		LoginOptions.LoginDone += OnLoginDone;
	}

	private void OnDisable()
	{
		HowOldAreYou.AgeGateDone -= OnAgeGateDone;
		LoginOptions.LoginDone -= OnLoginDone;
	}

	private void OnAgeGateDone(int playerAge)
	{
		PlayerPrefs.SetInt("PlayerAge", playerAge);

		if (PlayerInfoScript.GetInstance().IsUnderage)
		{
			PlayerPrefs.DeleteKey("SocialLogin");
            PlayerPrefs.DeleteKey("RetrySocialLogin");
            PlayerPrefs.DeleteKey("user");
            PlayerPrefs.DeleteKey("pass");
            Invoke("StartGameLoginFlow", 0.5f);
        }
		else
		{
            if (LoginOptionsTween != null)
            {
                LoginOptionsTween.Play(true);
            }
        }
    }

	private void OnLoginDone(string Username, string Password)
	{

        if (Username == string.Empty || Password == string.Empty)
        {
            ConfirmPopupController.ClickCallback clickCallback = delegate (bool yes)
            {
                if (yes)
                {
                    LoginOptionsTween.Play(true);
                }
                else
                {
                    PlayerPrefs.DeleteKey("SocialLogin");
                    PlayerPrefs.DeleteKey("RetrySocialLogin");
                    PlayerPrefs.DeleteKey("user");
                    PlayerPrefs.DeleteKey("pass");
                    ClearAuthEvents();
                    StartGameLoginFlow();
                }
            };

            StartCoroutine(CoroutineShowPopup("No username or password provided.\nRetry?", clickCallback));

        }
        else
        {
            PlayerPrefs.SetString("user", Username);
            PlayerPrefs.SetString("pass", Password);

            WWW www = new WWW(SQSettings.SERVER_URL + "account/exists?user=" + PlayerPrefs.GetString("user"));
            UnityEngine.Debug.Log("Attempting to authenticate player: " + www.url);

            while (!www.isDone)
            {
            }

            object response = null;

            UnityEngine.Debug.Log(response);

			if (www.text == "true")
			{
				Invoke("SocialLogin", 0.5f);
			}
			else if (www.text == "false")
			{
				OnPlayerCreateAccount();
			}
			else
			{
				OnPlayerFailedToAuthenticate("Server did not respond.");
			}
            
        }
    }

	private IEnumerator CoroutineShowPopup(string message, ConfirmPopupController.ClickCallback callback)
	{
		if (GeneralPopup == null || GeneralPopupShowTween == null)
		{
			if (callback != null)
			{
				yield return null;
				callback(false);
			}
			yield break;
		}
		bool hasResponse = false;
		bool confirmYes = false;
		ConfirmPopupController.ClickCallback interimCallback2 = null;
		interimCallback2 = delegate(bool yes)
		{
			ConfirmPopupController generalPopup2 = GeneralPopup;
			generalPopup2.OnSelect = (ConfirmPopupController.ClickCallback)Delegate.Remove(generalPopup2.OnSelect, interimCallback2);
			hasResponse = true;
			confirmYes = yes;
		};
		ConfirmPopupController generalPopup = GeneralPopup;
		generalPopup.OnSelect = (ConfirmPopupController.ClickCallback)Delegate.Combine(generalPopup.OnSelect, interimCallback2);
		GeneralPopup.Label = message;
		GeneralPopupShowTween.Play(true);
		while (!hasResponse)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		if (callback != null)
		{
			callback(confirmYes);
		}
	}

	private void StartGameLoginFlow()
	{
		PlayerInfoScript.GetInstance().Login();
		LoadNextLevel();
	}

	public static void SetRetrySocialLoginNextTime()
	{
		if (!PlayerInfoScript.GetInstance().IsUnderage)
		{
			PlayerPrefs.SetString("RetrySocialLogin", "true");
		}
	}

	private static void OnSocialLogout()
	{
        PlayerPrefs.DeleteKey("user");
        PlayerPrefs.DeleteKey("pass");
        PlayerPrefs.DeleteKey("RetrySocialLogin");
        PlayerPrefs.DeleteKey("SocialLogin");
    }

	private void Awake()
	{
		if (!staticInited)
		{
			SocialManager.Instance.playerLoggedOut += OnSocialLogout;
            AddAuthEvents();
            staticInited = true;
		}
	}

	private void Start()
	{
		AuthStarted = true;
		if (busyIconController == null)
		{
			busyIconController = SLOTGame.GetInstance();
		}
		if (!PlayerPrefs.HasKey("PlayerAge") && SocialManager.Instance.IsAgeGateRequired())
		{
			if (AgeGateShowTween != null)
			{
				AgeGateShowTween.Play(true);
			}
		}
		else if (!PlayerPrefs.HasKey("PlayerAge"))
		{
			OnAgeGateDone(100);
		}
		else if (PlayerPrefs.HasKey("RetrySocialLogin"))
		{
			SocialLogin();
		}
		else
		{
            PlayerPrefs.DeleteKey("user");
            PlayerPrefs.DeleteKey("pass");
            PlayerPrefs.DeleteKey("SocialLogin");
            PlayerPrefs.DeleteKey("SocialLogin");
            StartGameLoginFlow();
        }
	}

	private void LoadNextLevel()
	{
		busyIconController.ShowBusyIcon(true);
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevelAsync(NextSceneName, LoadLevelDoneCallback);
	}

	private void Update()
	{
		if (!isComplete && PlayerInfoScript.GetInstance().IsReady())
		{
			isComplete = true;
		}
	}

	private void LoadLevelDoneCallback()
	{
		busyIconController.ShowBusyIcon(false);
	}
}
