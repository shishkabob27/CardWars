using UnityEngine;

public class CWMPMapController : MonoBehaviour
{
	public class MPData
	{
		public string[] mLandscapes;

		public string[] mCards;

		public string OpponentLeader;

		public int mLeaderLevel;

		public string mMatchID;

		public string PlayerPVPName;

		public string OpponentPVPName;

		public int TrophyWin;

		public int TrophyLoss;
	}

	public UIButtonTween ShowPVPQuestInfo;

	public UIButtonTween HidePVPQuestInfo;

	private static CWMPMapController g_mapManager;

	private bool initialized;

	private GlobalFlags gflags;

	private PlayerInfoScript pInfo;

	public MPData mLastMPData = new MPData();

	private void Awake()
	{
		g_mapManager = this;
	}

	private void Start()
	{
		gflags = GlobalFlags.Instance;
	}

	public static CWMPMapController GetInstance()
	{
		return g_mapManager;
	}

	private void RefreshCamera()
	{
		float cameraOrthoSizeFromPlayerPref = CWMapController.GetCameraOrthoSizeFromPlayerPref();
		float orthographicSize = ((cameraOrthoSizeFromPlayerPref != 0f) ? cameraOrthoSizeFromPlayerPref : 22f);
		if (gflags != null && gflags.NewlyCleared)
		{
			orthographicSize = 22f;
		}
		if (gflags != null && gflags.InMPMode)
		{
			orthographicSize = 14f;
		}
		if (MapControllerBase.GetInstance() != null && MapControllerBase.GetInstance().QuestMapInfo != null)
		{
			GameObject mPCameraObj = MapControllerBase.GetInstance().QuestMapInfo.MPCameraObj;
			if (mPCameraObj != null && (bool)mPCameraObj.GetComponent<Camera>())
			{
				mPCameraObj.GetComponent<Camera>().orthographicSize = orthographicSize;
			}
		}
	}

	public void MPMapRefresh()
	{
		RefreshCamera();
		if (null != ShowPVPQuestInfo && gflags != null && gflags.InMPMode)
		{
			ShowPVPQuestInfo.Play(true);
		}
	}

	private void Update()
	{
		if (!initialized && gflags.InMPMode)
		{
			SessionManager instance = SessionManager.GetInstance();
			pInfo = PlayerInfoScript.GetInstance();
			if (instance.IsReady())
			{
				MPMapRefresh();
				initialized = true;
			}
		}
	}
}
