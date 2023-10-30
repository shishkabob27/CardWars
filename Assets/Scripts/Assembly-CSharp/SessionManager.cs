using System;
using System.IO;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
	public enum States
	{
		WAITING_FOR_USERID,
		LOGGING_IN,
		LOAD_DATA,
		CHECK_SAVE_CONFLICT,
		LOADING,
		VALIDATE_PATCH,
		VERSION_CHECK,
		WAITING_FOR_RESTART,
		PATCHING,
		MESSAGE_FETCH,
		SAVING,
		QUERYING,
		READY
	}

	public delegate void AssignFacebookIDToUserCallback(bool success);

	public delegate void OnReadyDelegate();

	public delegate void OnSaveDelegate(bool success);

	private const string DEVICEID_FILE = "deviceName";

	private const int CurrentVersion = 1;

	public GameObject BusyIcon;

	public string PlayerID;

	public string LoginID;

	public string NetState;

	public string DeviceID;

	private States state;

	private bool isPatched;

	public bool LocalRemoteSaveGameConflict;

	private bool checkSaveConflictFinished;

	private bool loadingDataFinished;

	private OnReadyDelegate myOnReadyCallback;

	private OnSaveDelegate saveToServerCallback;

	private string saveToServerData;

	private int? saveToServerResponse;

	private OnReadyDelegate attemptConnectionCallback;

	private int? attemptConnectionResponse;

	public static bool loginCompletedWithoutError;

	private bool checkedVersion;

	private static SessionManager instance;

	private Session session;

	public bool NeedsForcedUpdate { get; private set; }

	public bool HasNewMessagesReady
	{
		get
		{
			return session != null && session.TheGame != null && session.TheGame.MyMessages != null && session.TheGame.MyMessages.Count > 0;
		}
	}

	private States State
	{
		get
		{
			return state;
		}
		set
		{
			TFUtils.DebugLog("Changing State: " + state.ToString() + " ==> " + value.ToString() + ", Time: " + DateTime.Now.ToString("HH:mm:ss.ff"), "session");
			state = value;
			NetState = Enum.GetName(typeof(States), value);
		}
	}

	public OnReadyDelegate OnReadyCallback
	{
		get
		{
			return myOnReadyCallback;
		}
		set
		{
			myOnReadyCallback = value;
		}
	}

	public Session theSession
	{
		get
		{
			return session;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	public static SessionManager GetInstance()
	{
		return instance;
	}

	private void Start()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("SessionMgr");
		if (array.Length > 1)
		{
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				SessionManager component = gameObject.GetComponent<SessionManager>();
				if (!component || component != instance)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}
		DeviceID = LoadDeviceId();
		session = null;
		State = States.WAITING_FOR_USERID;
	}

	public string LoadDeviceId()
	{
		string path = Path.Combine(Application.persistentDataPath, "deviceName");
		if (File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		string text = Guid.NewGuid().ToString();
		File.WriteAllText(path, text);
		return text;
	}

	public void Login(bool doFacebookLogin, string fbAccessToken, string fbid)
	{
		string text = ((fbid != null) ? fbid : "default");
		if (!(text == LoginID) || session == null || !session.IsAuthenticated())
		{
			LoginID = ((fbid != null) ? fbid : "default");
			State = States.LOGGING_IN;
			session = new Session(1, DeviceID, fbid, doFacebookLogin, fbAccessToken);
			session.TheGame = new Game();
		}
	}

	public void Logout()
	{
		session = null;
		State = States.WAITING_FOR_USERID;
	}

	public bool IsLoggedIn()
	{
		bool result = false;
		if (session != null)
		{
			result = session.IsLoggedIn();
		}
		return result;
	}

	public bool IsAuthenticated()
	{
		return session != null && session.IsAuthenticated();
	}

	public bool IsReady()
	{
		return IsLoggedIn() && State == States.READY;
	}

	public void StartSyncStreamingAssets()
	{
		session.StartPatch();
	}

	public bool IsPatchingSyncDone()
	{
		return session.IsPatchDone();
	}

	public bool IsMessageSyncDone()
	{
		return session.IsMessagelistLoaded();
	}

	private bool IsSaveDone()
	{
		if (session == null || session.TheGame == null)
		{
			return true;
		}
		return session.TheGame.IsDoneServerAccess();
	}

	public string GetStreamingAssetsPath(string fname)
	{
		string result = Path.Combine(Application.streamingAssetsPath, fname);
		if (DebugFlagsScript.GetInstance().UseLocalJsonFiles)
		{
			return result;
		}
		string persistentAssetsPath = TFUtils.GetPersistentAssetsPath();
		if (!string.IsNullOrEmpty(persistentAssetsPath))
		{
			string text = Path.Combine(persistentAssetsPath, fname);
			if (File.Exists(text))
			{
				return text;
			}
		}
		return result;
	}

	public string GetPlayerDataPath(string fname)
	{
		return session.ThePlayer.CacheFile(fname);
	}

	public void SetGameStateJson(string gameData)
	{
		if (session != null && session.TheGame != null)
		{
			session.TheGame.SaveLocally(gameData);
		}
	}

	public void SaveToServer(string gameData, OnSaveDelegate callback = null)
	{
		if (session != null && session.TheGame != null)
		{
			if (callback == null)
			{
				session.TheGame.SaveToServer(session, gameData);
				return;
			}
			saveToServerData = gameData;
			saveToServerCallback = callback;
			AttemptSave();
		}
	}

	private void AttemptSave()
	{
		if (GetInstance().LocalRemoteSaveGameConflict)
		{
			TFUtils.DebugLog("LocalRemoteSaveGameConflict, not saving to server until resolved", "saveload");
		}
		else if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			STDErrorDialog sTDErrorDialog = STDErrorDialog.GetInstance();
			if (sTDErrorDialog != null)
			{
				sTDErrorDialog.ShowError("Error N01: Connection Interrupted", AttemptSave);
			}
			else
			{
				saveToServerCallback(false);
			}
		}
		else
		{
			session.WebFileServer.SaveGameData(saveToServerData, RecordSaveResponse, session);
		}
	}

	public void RecordSaveResponse(TFWebFileResponse response)
	{
		saveToServerResponse = (int)response.StatusCode;
	}

	public void HandleSaveResponse()
	{
		int? num = saveToServerResponse;
		int num2 = (num.HasValue ? num.Value : 0);
		saveToServerResponse = null;
		if (num2 != 200 && num2 != 201)
		{
			string error = string.Format("HTTP Status {0}: There was a problem accessing the server.", num2);
			STDErrorDialog sTDErrorDialog = STDErrorDialog.GetInstance();
			if (sTDErrorDialog != null)
			{
				sTDErrorDialog.ShowError(error, AttemptSave);
			}
			else
			{
				saveToServerCallback(false);
			}
		}
		else
		{
			saveToServerCallback(true);
		}
	}

	public void AttemptConnection(OnReadyDelegate callback)
	{
		if (session != null && session.TheGame != null && callback != null)
		{
			attemptConnectionCallback = callback;
			AttemptConnection();
		}
	}

	private void AttemptConnection()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			STDErrorDialog.GetInstance().ShowError("Error N01: Connection Interrupted", AttemptConnection);
		}
		else
		{
			session.WebFileServer.GetServerVersion(RecordConnectionResponse);
		}
	}

	public void RecordConnectionResponse(TFWebFileResponse response)
	{
		attemptConnectionResponse = (int)response.StatusCode;
	}

	public void HandleConnectionResponse()
	{
		int? num = attemptConnectionResponse;
		int num2 = (num.HasValue ? num.Value : 0);
		attemptConnectionResponse = null;
		if (num2 != 200)
		{
			string error = string.Format("HTTP Status {0}: There was a problem accessing the server.", num2);
			STDErrorDialog.GetInstance().ShowError(error, AttemptConnection);
		}
		else
		{
			attemptConnectionCallback();
		}
	}

	public void LoadFromServer()
	{
		if (session != null && session.TheGame != null)
		{
			session.LoadGameFromNetwork();
		}
	}

	public void CheckFromServer()
	{
		if (session != null && session.TheGame != null)
		{
			session.CheckGameFromNetwork();
		}
	}

	public void DeleteFromServer()
	{
		if (session != null && session.TheGame != null)
		{
			session.DeleteGameFromNetwork();
		}
	}

	public void ClearSaveStateLocal()
	{
		if (session != null && session.TheGame != null)
		{
			session.TheGame.ClearCachedSaveState(session);
		}
	}

	public void DeleteLocal()
	{
		if (session != null && session.TheGame != null)
		{
			session.TheGame.DestroyCache(session.ThePlayer);
		}
	}

	public string GetGameStateJson()
	{
		if (session != null && session.TheGame != null)
		{
			return session.TheGame.LoadLocally();
		}
		return null;
	}

	public void RequestMyUserInfo()
	{
		if (State == States.READY)
		{
			State = States.QUERYING;
			if (session != null && session.TheGame != null)
			{
				session.GetUserInfo();
			}
		}
	}

	public Version GetServerVersion()
	{
		if (session != null && session.TheGame != null)
		{
			return session.TheGame.MyServerVersion;
		}
		return null;
	}

	public void TestConnectivity()
	{
		if (session != null && session.TheGame != null)
		{
			session.TestConnectivity();
		}
	}

	public string GetMyUserInfoJson()
	{
		if (session != null && session.TheGame != null)
		{
			return session.TheGame.MyUserInfo;
		}
		return null;
	}

	public void RequestMyMessagesList()
	{
		if (State == States.READY)
		{
			State = States.QUERYING;
			if (session != null && session.TheGame != null)
			{
				session.GetMessagesList();
			}
		}
	}

	public void RequestMyMessage(string id)
	{
		if (State == States.READY)
		{
			State = States.QUERYING;
			if (session != null && session.TheGame != null)
			{
				session.GetMessage(id);
			}
		}
	}

	private void StartLoadingData()
	{
		loadingDataFinished = false;
		//Language.ReloadLanguage();
		StartCoroutine(LoadingManager.Instance.LoadAll(FinishedLoadingData));
	}

	private void StartCheckSaveConflict()
	{
		checkSaveConflictFinished = false;
		PlayerInfoScript.ValidateAndFixLocalSave();
		CheckFromServer();
	}

	public void FinishedLoadingData()
	{
		isPatched = true;
		loadingDataFinished = true;
	}

	public bool IsLoadDataDone()
	{
		return loadingDataFinished;
	}

	public void FinishedCheckSaveConflict()
	{
		checkSaveConflictFinished = true;
	}

	public bool IsCheckSaveConflictDone()
	{
		return checkSaveConflictFinished;
	}

	private void Update()
	{
		if (session != null)
		{
			session.Update();
		}
		if (State == States.LOGGING_IN && IsLoggedIn())
		{
			PlayerID = session.ThePlayer.playerId;
			State = States.VALIDATE_PATCH;
			session.ValidateLastPatch();
		}
		if (State == States.VALIDATE_PATCH && !session.ValidatingLastPatch)
		{
			State = States.VERSION_CHECK;
			StartSyncStreamingAssets();
		}
		if (State == States.VERSION_CHECK)
		{
			if (!checkedVersion)
			{
				checkedVersion = true;
				session.GetServerVersion();
			}
			if (GetServerVersion() != null)
			{
				if (GetServerVersion() > new Version(Application.version))
				{
					NeedsForcedUpdate = true;
					State = States.WAITING_FOR_RESTART;
				}
				else
				{
					State = States.PATCHING;
				}
				checkedVersion = false;
			}
		}
		if (State == States.PATCHING && IsPatchingSyncDone())
		{
			if (!isPatched)
			{
				State = States.LOAD_DATA;
				StartLoadingData();
			}
			else
			{
				State = States.CHECK_SAVE_CONFLICT;
				StartCheckSaveConflict();
			}
		}
		if (State == States.LOAD_DATA && IsLoadDataDone())
		{
			State = States.CHECK_SAVE_CONFLICT;
			StartCheckSaveConflict();
		}
		if (State == States.CHECK_SAVE_CONFLICT && IsCheckSaveConflictDone())
		{
			PlayerInfoScript.ValidateAndFixLocalSave();
			State = States.LOADING;
			if (!GetInstance().LocalRemoteSaveGameConflict)
			{
				LoadFromServer();
			}
		}
		if (State == States.LOADING && IsSaveDone())
		{
			PlayerInfoScript.Load();
			QuestManager.Instance.InitializeQuestStates();
			SideQuestManager.Instance.InitializeQuestStates();
			Singleton<AnalyticsManager>.Instance.LogTotalXP();
			Singleton<AnalyticsManager>.Instance.LogTotalCoins();
			State = States.MESSAGE_FETCH;
			session.GetMessagesList();
		}
		if (State == States.MESSAGE_FETCH && IsMessageSyncDone())
		{
			State = States.SAVING;
			PlayerInfoScript.GetInstance().Save();
		}
		if (State == States.SAVING && IsSaveDone())
		{
			State = States.READY;
			if (myOnReadyCallback != null)
			{
				myOnReadyCallback();
			}
		}
		if (State == States.QUERYING && IsSaveDone())
		{
			State = States.READY;
			if (myOnReadyCallback != null)
			{
				myOnReadyCallback();
			}
		}
		if (saveToServerResponse.HasValue)
		{
			HandleSaveResponse();
		}
		if (attemptConnectionResponse.HasValue)
		{
			HandleConnectionResponse();
		}
		if (session != null && session.TheGame != null)
		{
			if (session.TheGame.needsSaveSuccessfulDialog)
			{
				session.TheGame.needsSaveSuccessfulDialog = false;
				DebugPopupScript.CreateSavePopup(true);
			}
			if (session.TheGame.needsSaveFailedDialog)
			{
				session.TheGame.needsSaveFailedDialog = false;
				DebugPopupScript.CreateSavePopup(false);
			}
		}
	}

	public void OnApplicationPause(bool paused)
	{
		TFUtils.DebugLog("Application pausing" + paused);
		if (!paused && session != null)
		{
			session.GetServerTime();
		}
		if (!paused && IsReady() && IsLoggedIn())
		{
			StartSyncStreamingAssets();
			Singleton<AnalyticsManager>.Instance.LogTotalXP();
			Singleton<AnalyticsManager>.Instance.LogTotalCoins();
		}
	}

	private void onExternalMessage(string msg)
	{
		session.onExternalMessage(msg);
	}

	public void ClearMessages()
	{
		if (session != null && session.TheGame != null && session.TheGame.MyMessages != null)
		{
			session.TheGame.MyMessages.Clear();
		}
	}

	public void Clear()
	{
		instance = null;
	}
}
