using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using MiniJSON;
using UnityEngine;

public class Session : IDisposable
{
	public class FramerateWatcher
	{
		public float frequency = 0.5f;

		private float accum;

		private int frames;

		private float waitTime;

		private float prevWindowsFPS;

		public float Framerate
		{
			get
			{
				return prevWindowsFPS;
			}
		}

		public void Update()
		{
			accum += Time.timeScale / Time.deltaTime;
			frames++;
			waitTime += Time.deltaTime;
			if (waitTime > frequency)
			{
				waitTime = 0f;
				prevWindowsFPS = accum / (float)frames;
				accum = 0f;
				frames = 0;
			}
		}
	}

	public class Authorizing
	{
		private bool _finishedLogin;

		public void OnEnter(Session session, bool doFacebookAuth, string fbAccessToken)
		{
			TFUtils.DebugLog("Starting to User login");
			_finishedLogin = false;
			Player.LoadFromNetwork("userLogin", session, doFacebookAuth, fbAccessToken);
		}

		public void OnLeave(Session session)
		{
		}

		public void OnUpdate(Session session)
		{
			if (_finishedLogin)
			{
				return;
			}
			if (session.PlayerIsLoggedIn())
			{
				TFUtils.DebugLog("User logged In");
				_finishedLogin = true;
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)session.CheckAsyncRequest("userLogin");
			if (dictionary == null)
			{
				return;
			}
            foreach (var kvp in dictionary)
            {
                string key = kvp.Key;
                object value = kvp.Value;

                // Now you can use 'key' and 'value' as needed
                UnityEngine.Debug.Log("Key: "+key + "Value: "+value);
            }
			bool NetworkError = session.Server.IsNetworkError(dictionary);
			bool UsernameExists = PlayerPrefs.GetString("user") == "";
            //if (dictionary.ContainsKey("success"))
            //{
            //	flag2 = (bool)dictionary["success"];
            //}
            if (!NetworkError || UsernameExists)
			{
				if (Session.OnSessionUserLoginFail != null)
				{
					Session.OnSessionUserLoginFail();
				}
				session.Server.SetLoggedOut();
				session.ThePlayer = Player.LoadFromFilesystem();
			}
			else
			{
				if (Session.OnSessionUserLoginSucceed != null)
				{
					Session.OnSessionUserLoginSucceed();
				}
				session.ThePlayer = Player.LoadFromResponse(PlayerPrefs.GetString("user"), true);
			}
			session.TheGame.SetPlayer(session.ThePlayer);
			session.ThePlayer.SaveLocally();
			session.WebFileServer.SetPlayerInfo(session.ThePlayer);
		}

		public bool IsLoggedIn()
		{
			return _finishedLogin;
		}
	}

	public delegate void GameloopAction();

	public delegate void AsyncAction();

	private const string LOAD_GAME = "loadGame";

	private const string LOAD_GAME_CHECK = "loadGameCheck";

	private const string DELETE_GAME = "deleteGame";

	private const string GET_USERINFO = "getUserInfo";

	private const string GET_SERVERVERSION = "getServerVersion";

	private const string GET_MESSAGES_LIST = "getMessagesList";

	private const string GET_MESSAGE = "getMessage";

	private const string TEST_CONNECTIVITY = "testConnectivity";

	private const string USER_LOGIN = "userLogin";

	private SessionManager.AssignFacebookIDToUserCallback assignFacebookIDToUserCallback;

	private AndroidJavaObject androidActivity;

	private Player player;

	private SQServer server;

	private SQWebFileServer webFileServer;

	private SQAuth auth;

	private Game game;

	private Authorizing authorizing;

	private int currentVersion;

	private bool messageListLoaded;

	private List<string> queuedResponses = new List<string>();

	private bool needsReload;

	private Dictionary<string, TFServer.JsonResponseHandler> externalRequests = new Dictionary<string, TFServer.JsonResponseHandler>();

	private Dictionary<string, object> asyncRequests = new Dictionary<string, object>();

	private Dictionary<string, TFWebFileResponse> asyncFileRequests = new Dictionary<string, TFWebFileResponse>();

	private SQContentPatcher contentPatcher;

	private string LocalManifestVersion;

	private bool _finishedPatching;

	private Thread _validationThread;

	private object _validationLock = new object();

	public SQServer Server
	{
		get
		{
			return server;
		}
	}

	public SQWebFileServer WebFileServer
	{
		get
		{
			return webFileServer;
		}
	}

	public string Username
	{
		set
		{
			webFileServer.Username = value;
		}
	}

	public SQAuth Auth
	{
		get
		{
			return auth;
		}
	}

	public Game TheGame
	{
		get
		{
			return game;
		}
		set
		{
			game = value;
		}
	}

	public Player ThePlayer
	{
		get
		{
			return player;
		}
		set
		{
			player = value;
		}
	}

	public string UpdateUrl { get; private set; }

	public bool NeedsReload
	{
		get
		{
			return needsReload;
		}
	}

	public bool ValidatingLastPatch
	{
		get
		{
			return _validationThread != null;
		}
	}

	[method: MethodImpl(32)]
	public static event Action OnSessionUserLoginFail;

	[method: MethodImpl(32)]
	public static event Action OnSessionUserLoginSucceed;

	public Session(int currentVersion, string deviceId, string fbid, bool doFacebookLogin, string fbAccessToken)
	{
		TFUtils.Init(fbid);
		SQSettings.Init();
		TFUtils.DebugLog("Trying to create the session...", "session");
		authorizing = new Authorizing();
		CookieContainer cookies = new CookieContainer();
		server = new SQServer(cookies);
		webFileServer = new SQWebFileServer(cookies, deviceId);
		auth = new SQAuth(Application.platform);
		this.currentVersion = currentVersion;
		OnInit();
		authorizing.OnEnter(this, doFacebookLogin, fbAccessToken);
	}

	public void ProcessAsyncResponse(string key)
	{
		TFWebFileResponse tFWebFileResponse = CheckAsyncFileRequest(key);
		if (tFWebFileResponse == null)
		{
			return;
		}
		switch (key)
		{
		case "loadGameCheck":
			SessionManager.GetInstance().LocalRemoteSaveGameConflict = false;
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				Dictionary<string, object> asJSONDict2 = tFWebFileResponse.GetAsJSONDict();
				bool flag = asJSONDict2 != null && asJSONDict2.ContainsKey("PlayerName");
				SessionManager.GetInstance().LocalRemoteSaveGameConflict = flag && game.GameExists(player) && !webFileServer.HasLocalDeviceTag(tFWebFileResponse);
			}
			SessionManager.GetInstance().FinishedCheckSaveConflict();
			break;
		case "loadGame":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				Dictionary<string, object> asJSONDict = tFWebFileResponse.GetAsJSONDict();
				if (asJSONDict != null && asJSONDict.ContainsKey("PlayerName"))
				{
					if (!webFileServer.HasLocalDeviceTag(tFWebFileResponse))
					{
						TFUtils.DebugLog("Server returned success (gamedata). Loading from network response", "session");
						game.SaveLocally(tFWebFileResponse.Data);
						Singleton<AnalyticsManager>.Instance.LogDebug("sever_override");
					}
					else
					{
						TFUtils.DebugLog("Server returned success (gamedata). False positive data - ignoring...");
					}
					SessionManager.loginCompletedWithoutError = true;
				}
				else
				{
					TFUtils.DebugLog("Server returned invalid gamedata: " + tFWebFileResponse.Data, "session");
				}
				break;
			}
			TFUtils.DebugLog(string.Concat("Server returned status ", tFWebFileResponse.StatusCode, ". Loading from local data"), "session");
			SessionManager.loginCompletedWithoutError = tFWebFileResponse.StatusCode == HttpStatusCode.NotFound || tFWebFileResponse.StatusCode == HttpStatusCode.NotModified;
			if (game.GameExists(player))
			{
				TFUtils.DebugLog("Creating game from local file");
			}
			else if (tFWebFileResponse.StatusCode == HttpStatusCode.NotFound)
			{
				TFUtils.DebugLog("Initializing new game", "session");
				WebFileServer.DeleteETagFile();
			}
			else if (tFWebFileResponse.StatusCode == HttpStatusCode.NotModified)
			{
				TFUtils.DebugLog(string.Concat("What is going on? This is not an expected outcome: response status ", tFWebFileResponse.StatusCode, " Network down: ", tFWebFileResponse.NetworkDown), "session");
				WebFileServer.DeleteETagFile();
			}
			else
			{
				TFUtils.DebugLog(string.Concat("What is going on? This is not an expected outcome: response status ", tFWebFileResponse.StatusCode, " Network down: ", tFWebFileResponse.NetworkDown), "session");
			}
			break;
		case "deleteGame":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (delete game).", "session");
			}
			else
			{
				TFUtils.DebugLog(string.Concat("Server returned status ", tFWebFileResponse.StatusCode, ". Nothing we can do...."), "session");
			}
			break;
		case "getMessagesList":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (my messages). Loading from network response", "session");
				if (game != null)
				{
					game.MyMessagesList = ProcessMessageListData(tFWebFileResponse.Data);
				}
			}
			else
			{
				messageListLoaded = true;
			}
			break;
		case "getServerVersion":
		case "testConnectivity":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				if (game != null)
				{
					game.MyServerVersion = ProcessVersionData(tFWebFileResponse.Data);
				}
			}
			else
			{
				game.MyServerVersion = new Version(0, 0, 0);
			}
			break;
		case "getUserInfo":
		case "getMessage":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (" + key + "). Loading from network response", "session");
				if (game != null)
				{
					TFUtils.DebugLog("Return = " + tFWebFileResponse.Data, "session");
					switch (key)
					{
					case "getUserInfo":
						game.MyUserInfo = tFWebFileResponse.Data;
						break;
					case "getMessage":
						game.MyMessages.Add(ProcessMessageData(tFWebFileResponse.Data));
						break;
					}
				}
			}
			else
			{
				TFUtils.DebugLog(string.Concat("Server returned status ", tFWebFileResponse.StatusCode, ". Nothing we can do...."), "session");
				game.MyUserInfo = "{\"error\":\"no data\"}";
			}
			break;
		}
		game.AccessDone = true;
	}

	public void Update()
	{
		OnUpdate();
	}

	private void OnUpdate()
	{
		authorizing.OnUpdate(this);
		ProcessAsyncResponses();
	}

	public void Dispose()
	{
		OnDispose();
	}

	public void ClearNeedsReload()
	{
		needsReload = false;
	}

	public void CheckGameFromNetwork()
	{
		game.CheckFromNetwork("loadGameCheck", this);
	}

	public void LoadGameFromNetwork()
	{
		game.LoadFromNetwork("loadGame", this);
	}

	public void DeleteGameFromNetwork()
	{
		game.DeleteFromNetwork("deleteGame", this);
	}

	public void GetMessagesList()
	{
		game.GetMessagesList("getMessagesList", this);
	}

	public void GetMessage(string id)
	{
		game.GetMessage("getMessage", id, this);
	}

	public void GetUserInfo()
	{
		game.GetUserInfo("getUserInfo", this);
	}

	public void GetServerVersion()
	{
		game.GetServerVersion("getServerVersion", this);
	}

	public void TestConnectivity()
	{
		game.GetServerVersion("testConnectivity", this);
	}

	public void GetServerTime()
	{
		TFServer.JsonResponseHandler handler = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			if (status == HttpStatusCode.OK)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
				DateTime dateTime = (TFUtils.lastServerTimeUpdate = DateTime.Parse(dictionary["server_time"].ToString()));
				TimeSpan timeSpan = dateTime.Subtract(DateTime.Now);
				double num = Math.Abs((timeSpan - TFUtils.serverTimeDiff).TotalSeconds);
				if (num > 10.0)
				{
					TFUtils.serverTimeDiff = timeSpan;
					TFUtils.DebugLog("Server time difference = " + timeSpan.TotalSeconds, "session");
				}
			}
		};
		Server.GetTime(handler);
	}

	public bool IsLoggedIn()
	{
		return authorizing.IsLoggedIn();
	}

	public bool IsAuthenticated()
	{
		return auth.IsAuthenticated();
	}

	public bool IsMessagelistLoaded()
	{
		return messageListLoaded;
	}

	public int GetLocalVersion()
	{
		return currentVersion;
	}

	public bool PlayerIsLoggedIn()
	{
		return player != null;
	}

	public void onExternalMessage(string msg)
	{
		TFUtils.DebugLog("decoding message: " + msg, "session");
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(msg);
		string text = dictionary["requestId"] as string;
		if (externalRequests.ContainsKey(text))
		{
			TFServer.JsonResponseHandler jsonResponseHandler = externalRequests[text];
			externalRequests.Remove(text);
			if (dictionary["data"] is Dictionary<string, object>)
			{
				jsonResponseHandler(dictionary["data"] as Dictionary<string, object>, HttpStatusCode.OK);
			}
			else
			{
				TFUtils.ErrorLog("Callback result is not a Dictionary<string, object>");
			}
		}
		else
		{
			TFUtils.DebugLog("No handler found for id: " + text, "session");
		}
	}

	public void registerExternalCallback(string requestId, TFServer.JsonResponseHandler callback)
	{
		externalRequests[requestId] = callback;
	}

	public AndroidJavaObject getAndroidActivity()
	{
		if (androidActivity == null)
		{
			int num = AndroidJNI.AttachCurrentThread();
			TFUtils.DebugLog("attach result: " + num, "session");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			androidActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		return androidActivity;
	}

	private List<string> ProcessMessageListData(string data)
	{
		List<string> list = GiftMessage.ProcessMessageListData(data);
		GetNextMessage(list);
		return list;
	}

	private string ProcessMessageData(string data)
	{
		game.MyMessagesList.RemoveAt(0);
		GetNextMessage(game.MyMessagesList);
		return data;
	}

	private void GetNextMessage(List<string> list)
	{
		if (list == null || list.Count == 0)
		{
			messageListLoaded = true;
		}
		else
		{
			GetMessage(list[0]);
		}
	}

	private Version ProcessVersionData(string response)
	{
        char[] delimiterChar = {'.'};
		string[] version = response.Split(delimiterChar);

        return new Version(
			int.Parse(version[0]),
            int.Parse(version[1]),
            int.Parse(version[2])
			);
	}

	protected void ProcessAsyncResponses()
	{
		if (queuedResponses.Count <= 0)
		{
			return;
		}
		List<string> list = new List<string>(queuedResponses);
		foreach (string item in list)
		{
			ProcessAsyncResponse(item);
		}
	}

	protected void QueueResponse(string key)
	{
		queuedResponses.Add(key);
	}

	public void AddAsyncResponse(string key, object val)
	{
		UnityEngine.Debug.Log("Adding Key: '" + key + "'");
		lock (asyncRequests)
		{
			if (asyncRequests.ContainsKey(key))
			{
				TFUtils.DebugLog("Warning: got second async response for " + key + "; Existing value was: " + asyncRequests[key]);
			}
			asyncRequests[key] = val;
		}
	}

	public object CheckAsyncRequest(string key)
	{
		object result = null;
		lock (asyncRequests)
		{
			if (asyncRequests.ContainsKey(key))
			{
				result = asyncRequests[key];
				asyncRequests.Remove(key);
			}
		}
		return result;
	}

	public TFServer.JsonResponseHandler AsyncResponder(string key)
	{
		return delegate(Dictionary<string, object> response, HttpStatusCode status)
		{
			AddAsyncResponse(key, response);
		};
	}

	public void AddAsyncFileResponse(string key, TFWebFileResponse val)
	{
		lock (asyncFileRequests)
		{
			asyncFileRequests[key] = val;
			game.AccessDone = false;
			QueueResponse(key);
		}
	}

	public TFWebFileResponse CheckAsyncFileRequest(string key)
	{
		TFWebFileResponse result = null;
		lock (asyncFileRequests)
		{
			if (asyncFileRequests.ContainsKey(key))
			{
				result = asyncFileRequests[key];
				asyncFileRequests.Remove(key);
			}
		}
		return result;
	}

	public TFWebFileServer.FileCallbackHandler AsyncFileResponder(string key)
	{
		return delegate(TFWebFileResponse response)
		{
			AddAsyncFileResponse(key, response);
		};
	}

	private void OnInit()
	{
		_validationThread = null;
		_finishedPatching = false;
	}

	private void OnDispose()
	{
		lock (_validationLock)
		{
			if (_validationThread != null)
			{
				_validationThread.Abort();
				_validationThread.Join();
				_validationThread = null;
			}
		}
	}

	public string GetLocalManifestVersion()
	{
		return LocalManifestVersion;
	}

	public bool IsPatchDone()
	{
		return _finishedPatching;
	}

	public void ValidateLastPatch()
	{
		lock (_validationLock)
		{
			if (_validationThread != null)
			{
				return;
			}
			SQContentPatcher patcher = new SQContentPatcher();
			Session me = this;
			_validationThread = new Thread((ThreadStart)delegate
			{
				patcher.ValidateAndFixDownloadedManifests();
				lock (me._validationLock)
				{
					me._validationThread = null;
				}
			});
			_validationThread.Start();
		}
	}

	public bool UpdatePatching()
	{
		if (contentPatcher != null || ValidatingLastPatch)
		{
			return contentPatcher != null;
		}
		TFUtils.DebugLog("UpdatePatching - contentPatcher is null");
		contentPatcher = new SQContentPatcher();
		SQContentPatcher sQContentPatcher = contentPatcher;
		sQContentPatcher.AddListener(OnPatchingEvent);
		sQContentPatcher.ReadManifests();
		LocalManifestVersion = sQContentPatcher.LocalManifestVersion();
		return true;
	}

	private void OnPatchingEvent(string eventStr)
	{
		switch (eventStr)
		{
		case "patchingNecessary":
			_finishedPatching = true;
			contentPatcher.StartDownloadingPatchedContent();
			break;
		case "patchingDone":
			_finishedPatching = true;
			if (contentPatcher != null && contentPatcher.ContentChanged && SessionManager.GetInstance().IsLoadDataDone())
			{
				needsReload = true;
			}
			contentPatcher = null;
			break;
		case "patchingNotNecessary":
			contentPatcher = null;
			break;
		}
	}

	public void StartPatch()
	{
		TFUtils.DebugLog("Starting to Patch content");
		_finishedPatching = false;
		UpdatePatching();
	}
}
