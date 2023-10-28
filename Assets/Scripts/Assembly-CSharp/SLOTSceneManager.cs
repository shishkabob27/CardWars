using UnityEngine;
using UnityEngine.SceneManagement;

public class SLOTSceneManager : SLOTGameSingleton<SLOTSceneManager>
{
	public delegate void LoadLevelAsyncCallback();

	public bool useLocalScenes;

	private AssetBundle assetBundle;

	private LoadLevelAsyncCallback loadLevelAsyncCallback;

	private AsyncOperation asyncOperation;

	public bool SetAssetBundle(AssetBundle bundle)
	{
		if (bundle != null && !useLocalScenes)
		{
			assetBundle = bundle;
			return true;
		}
		return false;
	}

	private static string GetLevelName(string name)
	{
		if (SLOTGame.IsLowEndDevice())
		{
			return "low_" + name;
		}
		return name;
	}

	public void LoadLevel(string name)
	{
		if (name != null && name.Length > 0)
		{
			CheckAsyncOperationDone(true);
            //Application.LoadLevel(GetLevelName(name));
            SceneManager.LoadScene(GetLevelName(name));
        }
	}

	public void LoadLevelAdditive(string name)
	{
		if (name != null && name.Length > 0)
		{
			CheckAsyncOperationDone(true);
            //Application.LoadLevelAdditive(GetLevelName(name));
            SceneManager.LoadScene(GetLevelName(name), LoadSceneMode.Additive);
        }
	}

	public AsyncOperation LoadLevelAsync(string name)
	{
		return LoadLevelAsync(name, null);
	}

	public AsyncOperation LoadLevelAsync(string name, LoadLevelAsyncCallback cb)
	{
		if (name == null || name.Length <= 0)
		{
			return null;
		}
		CheckAsyncOperationDone(true);
		AsyncOperation result = Application.LoadLevelAsync(GetLevelName(name));
		if (cb != null)
		{
			loadLevelAsyncCallback = cb;
			asyncOperation = result;
		}
		else
		{
			loadLevelAsyncCallback = null;
			asyncOperation = null;
		}
		return result;
	}

	public AsyncOperation LoadLevelAdditiveAsync(string name)
	{
		return LoadLevelAdditiveAsync(name, null);
	}

	public AsyncOperation LoadLevelAdditiveAsync(string name, LoadLevelAsyncCallback cb)
	{
		if (name == null || name.Length <= 0)
		{
			return null;
		}
		CheckAsyncOperationDone(true);
		AsyncOperation result = Application.LoadLevelAdditiveAsync(GetLevelName(name));
		if (cb != null)
		{
			loadLevelAsyncCallback = cb;
			asyncOperation = result;
		}
		else
		{
			loadLevelAsyncCallback = null;
			asyncOperation = null;
		}
		return result;
	}

	private void OnDestroy()
	{
		if (assetBundle != null)
		{
			assetBundle.Unload(true);
			assetBundle = null;
		}
	}

	private void Update()
	{
		CheckAsyncOperationDone(false);
	}

	private void CheckAsyncOperationDone(bool forceDone)
	{
		if (asyncOperation != null && loadLevelAsyncCallback != null && (asyncOperation.isDone || forceDone))
		{
			loadLevelAsyncCallback();
			asyncOperation = null;
			loadLevelAsyncCallback = null;
		}
	}
}
