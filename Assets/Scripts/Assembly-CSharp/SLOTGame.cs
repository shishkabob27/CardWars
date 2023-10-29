using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiniJSON;
using UnityEngine;

public class SLOTGame : BusyIconController
{
	public enum AssetBundleType
	{
		DownloadFromServer,
		Local,
		Disabled
	}

	public delegate void CheckAssetDownloadsCallback(bool success, string err);

	public delegate void ShowAssetDownloadProgressCallback(float percent, float totalPercent);

	public delegate bool AssetBundleLoadedCallback(string url, AssetBundle bundle);

	public const string CHECK_ASSET_DOWNLOADS_URL = "http://cardwars.retroretreat.net/AdventureTime/CardWars/IAPReceiptVerificationServer/check_asset_downloads.php";

	public const string ASSET_DOWNLOAD_SUBDIRECTORY = "data_1.01";

	public AssetBundleType assetBundleType;

	public string errorPopupPrefab;

	private static SLOTGame the_instance;

	private CheckAssetDownloadsCallback checkassetdownloadscallback;

	private ShowAssetDownloadProgressCallback showprogresscallback;

	private AssetBundleLoadedCallback assetbundleloadedcallback;

	private List<KFFNetwork.WWWInfo> checkassetdownloadswwwinfolist = new List<KFFNetwork.WWWInfo>();

	private int checkassetdownloadsassetbundlecount;

	private int checkassetdownloadsassetbundleloadedcount;

	private int checkassetdownloadsassetbundledonecount;

	private float checkassetdownloadspercentdone;

	private int saveMaxReqCount;

	public static SLOTGame GetInstance()
	{
		if (the_instance == null)
		{
			the_instance = UnityEngine.Object.FindObjectOfType(typeof(SLOTGame)) as SLOTGame;
		}
		if (Application.isEditor && Application.isPlaying && !the_instance)
		{
			GameObject gameObject = Resources.Load("SLOTGame") as GameObject;
			if (gameObject != null)
			{
				gameObject = UnityEngine.Object.Instantiate(gameObject);
				if (gameObject != null)
				{
					the_instance = gameObject.GetComponent(typeof(SLOTGame)) as SLOTGame;
					if (!the_instance)
					{
						UnityEngine.Object.Destroy(gameObject);
						gameObject = null;
					}
				}
			}
			if (!gameObject)
			{
				gameObject = new GameObject();
				if ((bool)gameObject)
				{
					the_instance = gameObject.AddComponent(typeof(SLOTGame)) as SLOTGame;
				}
			}
			if ((bool)gameObject)
			{
				gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
				gameObject.name = "AutomaticallyCreatedGame";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		return the_instance;
	}

	private void SetupScreen()
	{
		string text = SystemInfo.deviceModel.ToLower();
		int num = text.IndexOf("amazon");
		if (num >= 0)
		{
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = false;
			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
	}

	private void Awake()
	{
		Application.targetFrameRate = 60;
		KFFNetwork.deserializeJSONCallback = DeserializeJSON;
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(SLOTGame));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			SLOTGame sLOTGame = (SLOTGame)array2[i];
			if (sLOTGame.gameObject.name == "AutomaticallyCreatedGame" && sLOTGame != this)
			{
				UnityEngine.Object.DestroyImmediate(sLOTGame);
			}
		}
		Singleton<AnalyticsManager>.Instance.ResetBattleCount();
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		ShowBusyIcon(false);
	}

	private object DeserializeJSON(string json)
	{
		return Json.Deserialize(json);
	}

	public static byte[] StringToBytes(string str)
	{
		if (str == null)
		{
			return null;
		}
		return Encoding.UTF8.GetBytes(str);
	}

	public static string BytesToString(byte[] bytes)
	{
		if (bytes == null)
		{
			return null;
		}
		return Encoding.UTF8.GetString(bytes);
	}

	public static void SavePersistentData(string filename, byte[] bytes)
	{
		try
		{
			string text = Path.Combine(Application.persistentDataPath, filename);
			FileInfo fileInfo = new FileInfo(text);
			fileInfo.Directory.Create();
			File.WriteAllBytes(text, bytes);
		}
		catch (Exception)
		{
		}
	}

	public static byte[] LoadPersistentData(string filename)
	{
		byte[] result = null;
		try
		{
			string path = Path.Combine(Application.persistentDataPath, filename);
			if (File.Exists(path))
			{
				byte[] array = File.ReadAllBytes(path);
				result = array;
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static bool DoesPersistentDataExist(string filename)
	{
		try
		{
			string path = Path.Combine(Application.persistentDataPath, filename);
			return File.Exists(path);
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void CheckAssetDownloads(CheckAssetDownloadsCallback callback, ShowAssetDownloadProgressCallback callback2, AssetBundleLoadedCallback callback3)
	{
		if (assetBundleType == AssetBundleType.Disabled)
		{
			if (callback != null)
			{
				callback(true, null);
			}
			return;
		}
		saveMaxReqCount = KFFNetwork.GetMaxConcurrentWWWRequestCount();
		KFFNetwork.SetMaxConcurrentWWWRequestCount(1);
		checkassetdownloadscallback = callback;
		showprogresscallback = callback2;
		assetbundleloadedcallback = callback3;
		string text = "http://cardwars.retroretreat.net/AdventureTime/CardWars/IAPReceiptVerificationServer/check_asset_downloads.php?lowend=" + (IsLowEndDevice() ? 1 : 0);
		text += "&subdirectory=data_1.01";
		text += "&platform=";
		text += "Android";
		text += "&json=1";
		KFFNetwork.GetInstance().SendWWWRequest(text, checkAssetDownloadsCallback, null);
	}

	private void checkAssetDownloadsCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param)
	{
		KFFNetwork.WWWRequestResult wWWRequestResult = resultObj as KFFNetwork.WWWRequestResult;
		if (wWWRequestResult == null || !wWWRequestResult.isValid())
		{
			if (checkassetdownloadscallback != null)
			{
				checkassetdownloadscallback(false, (err != null) ? err : ((wWWRequestResult == null) ? null : wWWRequestResult.GetValueAsString("ERROR_MSG")));
				checkassetdownloadscallback = null;
			}
			return;
		}
		checkassetdownloadspercentdone = -1f;
		checkassetdownloadsassetbundlecount = 0;
		checkassetdownloadsassetbundleloadedcount = 0;
		checkassetdownloadsassetbundledonecount = 0;
		bool flag = false;
		while (true)
		{
			string key = "assetbundle" + checkassetdownloadsassetbundlecount;
			if (!wWWRequestResult.ContainsKey(key))
			{
				break;
			}
			checkassetdownloadsassetbundlecount++;
		}
		int num = 0;
		while (true)
		{
			string key2 = "assetbundle" + num;
			if (!wWWRequestResult.ContainsKey(key2))
			{
				break;
			}
			string valueAsString = wWWRequestResult.GetValueAsString(key2);
			int valueAsInt = wWWRequestResult.GetValueAsInt("assetbundleversion" + num);
			int valueAsInt2 = wWWRequestResult.GetValueAsInt("assetbundlesize" + num);
			KFFNetwork.WWWInfo wWWInfo = LoadAssetBundle(valueAsString, valueAsInt2, valueAsInt);
			if (wWWInfo != null)
			{
				checkassetdownloadswwwinfolist.Add(wWWInfo);
				flag = true;
			}
			num++;
		}
		if (!flag && checkassetdownloadscallback != null)
		{
			checkassetdownloadscallback(true, null);
			checkassetdownloadscallback = null;
		}
	}

	public KFFNetwork.WWWInfo LoadAssetBundle(string url, int size, int version)
	{
		if (assetBundleType == AssetBundleType.DownloadFromServer)
		{
			return KFFNetwork.GetInstance().LoadFromCacheOrDownload(url, version, LoadFromCacheOrDownloadCallback, string.Empty + size);
		}
		return null;
	}

	private void LoadFromCacheOrDownloadCallback(KFFNetwork.WWWInfo wwwinfo, object result, string err, object param)
	{
		WWW wWW = result as WWW;
		if (wWW != null)
		{
			AssetBundle assetBundle = wWW.assetBundle;
			if (assetBundle != null)
			{
				checkassetdownloadsassetbundleloadedcount++;
				if (assetbundleloadedcallback != null)
				{
					if (!assetbundleloadedcallback(wwwinfo.url, assetBundle))
					{
						assetBundle.Unload(false);
					}
				}
				else
				{
					assetBundle.Unload(false);
				}
			}
			else if (assetbundleloadedcallback != null)
			{
				assetbundleloadedcallback(wwwinfo.url, null);
			}
		}
		else if (assetbundleloadedcallback != null)
		{
			assetbundleloadedcallback(wwwinfo.url, null);
		}
		checkassetdownloadsassetbundledonecount++;
	}

	private void Update()
	{
		UpdateCheckAssetDownloads();
		SetupScreen();
	}

	private void UpdateCheckAssetDownloads()
	{
		if (checkassetdownloadswwwinfolist.Count > 0)
		{
			float num = 0f;
			float num2 = 0f;
			foreach (KFFNetwork.WWWInfo item in checkassetdownloadswwwinfolist)
			{
				int num3 = int.Parse(item.callbackParam as string);
				num += (float)num3;
				if (item.www != null)
				{
					num2 = ((!item.www.isDone) ? (num2 + item.www.progress * (float)num3) : (num2 + (float)num3));
				}
			}
			if (num2 != checkassetdownloadspercentdone)
			{
				if (showprogresscallback != null)
				{
					showprogresscallback(num2, num);
				}
				checkassetdownloadspercentdone = num2;
			}
		}
		if (checkassetdownloadsassetbundlecount > 0 && checkassetdownloadsassetbundlecount == checkassetdownloadsassetbundledonecount)
		{
			if (checkassetdownloadscallback != null)
			{
				checkassetdownloadscallback(checkassetdownloadsassetbundleloadedcount == checkassetdownloadsassetbundlecount, null);
				checkassetdownloadscallback = null;
			}
			checkassetdownloadsassetbundlecount = 0;
			checkassetdownloadswwwinfolist.Clear();
			KFFNetwork.SetMaxConcurrentWWWRequestCount(saveMaxReqCount);
		}
	}

	private void CancelCheckAssetDownloads()
	{
		foreach (KFFNetwork.WWWInfo item in checkassetdownloadswwwinfolist)
		{
			KFFNetwork.GetInstance().CancelWWWRequest(item);
		}
		checkassetdownloadswwwinfolist.Clear();
	}

	public static bool IsLowEndDevice()
	{
		return KFFLODManager.IsLowEndDevice();
	}

	public static Vector3 GetScale(Transform t)
	{
		Vector3 result = new Vector3(1f, 1f, 1f);
		if (t != null)
		{
			result.x *= t.localScale.x;
			result.y *= t.localScale.y;
			result.z *= t.localScale.z;
			if (t.parent != null)
			{
				Vector3 scale = GetScale(t.parent);
				result.x *= scale.x;
				result.y *= scale.y;
				result.z *= scale.z;
			}
		}
		return result;
	}

	public static Bounds GetBoundsRecursive(Transform t)
	{
		bool flag = true;
		Bounds result = new Bounds(t.position, new Vector3(0f, 0f, 0f));
		Component[] componentsInChildren = t.GetComponentsInChildren(typeof(Collider));
		if (componentsInChildren != null)
		{
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				Collider collider = component as Collider;
				if (collider != null && collider.bounds.size.x > 0f && collider.bounds.size.y > 0f && collider.bounds.size.z > 0f)
				{
					if (flag)
					{
						result = collider.bounds;
						flag = false;
					}
					else
					{
						result.Encapsulate(collider.bounds);
					}
				}
			}
		}
		return result;
	}

	public static Bounds GetRendererBoundsRecursive(Transform t)
	{
		bool flag = true;
		Bounds result = new Bounds(t.position, new Vector3(0f, 0f, 0f));
		Component[] componentsInChildren = t.GetComponentsInChildren(typeof(Renderer));
		if (componentsInChildren != null)
		{
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				Renderer renderer = component as Renderer;
				if (renderer != null && renderer.bounds.size.x > 0f && renderer.bounds.size.y > 0f && renderer.bounds.size.z > 0f)
				{
					if (flag)
					{
						result = renderer.bounds;
						flag = false;
					}
					else
					{
						result.Encapsulate(renderer.bounds);
					}
				}
			}
		}
		return result;
	}

	public static Bounds GetMeshRendererBoundsRecursive(Transform t)
	{
		bool flag = true;
		Bounds result = new Bounds(t.position, new Vector3(0f, 0f, 0f));
		Component[] componentsInChildren = t.GetComponentsInChildren(typeof(Renderer));
		if (componentsInChildren != null)
		{
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				if (component is ParticleSystemRenderer)
				{
					continue;
				}
				Renderer renderer = component as Renderer;
				if (renderer != null && renderer.bounds.size.x > 0f && renderer.bounds.size.y > 0f && renderer.bounds.size.z > 0f)
				{
					if (flag)
					{
						result = renderer.bounds;
						flag = false;
					}
					else
					{
						result.Encapsulate(renderer.bounds);
					}
				}
			}
		}
		return result;
	}

	public static Bounds GetUIBoundsRecursive(Transform t)
	{
		bool flag = true;
		Bounds result = new Bounds(t.position, new Vector3(0f, 0f, 0f));
		Component[] componentsInChildren = t.GetComponentsInChildren(typeof(Transform));
		if (componentsInChildren != null)
		{
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				Transform transform = component as Transform;
				if (transform != null)
				{
					Bounds bounds = new Bounds(transform.position, GetScale(transform));
					if (flag)
					{
						result = bounds;
						flag = false;
					}
					else
					{
						result.Encapsulate(bounds);
					}
				}
			}
		}
		return result;
	}

	public static void SetLayerRecursive(GameObject obj, int l)
	{
		if (obj == null)
		{
			return;
		}
		obj.layer = l;
		foreach (Transform item in obj.transform)
		{
			SetLayerRecursive(item.gameObject, l);
		}
	}

	public static GameObject InstantiateGO(GameObject original)
	{
		GameObject gameObject = PoolManager.Fetch(original);
		if (gameObject != null)
		{
			OnInstantiate(gameObject);
		}
		return gameObject;
	}

	public static GameObject InstantiateGO(GameObject original, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = PoolManager.Fetch(original, position, rotation);
		if (gameObject != null)
		{
			OnInstantiate(gameObject);
		}
		return gameObject;
	}

	public static GameObject InstantiateGO(string PoolName, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = PoolManager.Fetch(PoolName, position, rotation);
		if (gameObject != null)
		{
			OnInstantiate(gameObject);
		}
		return gameObject;
	}

	public static UnityEngine.Object InstantiateFX(UnityEngine.Object original)
	{
		UnityEngine.Object @object = Instantiate(original);
		if (@object != null)
		{
			OnInstantiate(@object);
		}
		return @object;
	}

	public static UnityEngine.Object InstantiateFX(UnityEngine.Object original, Vector3 position, Quaternion rotation)
	{
		UnityEngine.Object @object = UnityEngine.Object.Instantiate(original, position, rotation);
		if (@object != null)
		{
			OnInstantiate(@object);
		}
		return @object;
	}

	private static void OnInstantiate(UnityEngine.Object clone)
	{
		if (clone != null)
		{
			GameObject gameObject = clone as GameObject;
			if (gameObject != null)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().UpdateAudioVolumes(gameObject);
			}
		}
	}

	public static Bounds GetRelativeBoundsRecursive(Transform t)
	{
		return GetRelativeBoundsRecursive(t, t);
	}

	public static Bounds GetRelativeBoundsRecursive(Transform root, Transform child)
	{
		Collider[] componentsInChildren = child.GetComponentsInChildren<Collider>();
		if (componentsInChildren.Length == 0)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			Collider collider = componentsInChildren[i];
			Vector3 size = collider.bounds.size;
			Vector3 center = collider.bounds.center;
			Vector3 v = center - size * 0.5f;
			v = worldToLocalMatrix.MultiplyPoint3x4(v);
			vector2 = Vector3.Max(v, vector2);
			vector = Vector3.Min(v, vector);
			v = center + size * 0.5f;
			v = worldToLocalMatrix.MultiplyPoint3x4(v);
			vector2 = Vector3.Max(v, vector2);
			vector = Vector3.Min(v, vector);
		}
		Bounds result = new Bounds(vector, Vector3.zero);
		result.Encapsulate(vector2);
		return result;
	}

	private void OnLevelWasLoaded(int level)
	{
		FixupTutorialFlow();
	}

	public void FixupTutorialFlow()
	{
		TutorialManager instance = TutorialManager.Instance;
		if (instance != null)
		{
			instance.FixupFlow();
		}
	}

	public static T GetComponentInChildren<T>(GameObject obj, bool findDisabledComponent) where T : Component
	{
		T[] componentsInChildren = obj.GetComponentsInChildren<T>(findDisabledComponent);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			return componentsInChildren[0];
		}
		return (T)null;
	}

	public SimpleErrorPopup ShowErrorPopup(string title, string message, int buttonCount, string[] buttonStrings = null, SimpleErrorPopup.ErrorPopupButtonClickedCallback callback = null)
	{
		SimpleErrorPopup simpleErrorPopup = null;
		GameObject gameObject = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(errorPopupPrefab, typeof(GameObject)) as GameObject;
		if (gameObject != null)
		{
			GameObject gameObject2 = InstantiateFX(gameObject) as GameObject;
			if (gameObject2 != null)
			{
				simpleErrorPopup = gameObject2.GetComponent<SimpleErrorPopup>();
				if (simpleErrorPopup != null)
				{
					simpleErrorPopup.Setup(title, message, buttonCount, buttonStrings, callback);
				}
				else
				{
					UnityEngine.Object.Destroy(gameObject2);
				}
			}
		}
		return simpleErrorPopup;
	}
}
