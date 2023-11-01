using System;
using System.Collections;
using UnityEngine;

public class SLOTStartup : MonoBehaviour
{
	public string startupScene;

	private bool started;

	private string StoragePath;

	private void Update()
	{
		if (!started)
		{
			started = true;
			Startup();
		}
	}

	private void DownloadOBB()
	{
		StoragePath = GooglePlayDownloader.GetExpansionFilePath();
		if (StoragePath != null)
		{
			if (GooglePlayDownloader.GetMainOBBPath(StoragePath) == null)
			{
				GooglePlayDownloader.FetchOBB();
			}
			StartCoroutine(loadLevel());
		}
	}

	protected IEnumerator loadLevel()
	{
		string mainPath;
		do
		{
			yield return new WaitForSeconds(0.5f);
			mainPath = GooglePlayDownloader.GetMainOBBPath(StoragePath);
		}
		while (mainPath == null);
		Startup();
	}

	private void Startup()
	{
		if (startupScene != null && startupScene.Length > 0)
		{
			bool flag = false;
			try
			{
				SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel(startupScene);
				flag = true;
			}
			catch (Exception)
			{
				flag = false;
			}
			if (flag)
			{
			}
		}
	}
}
