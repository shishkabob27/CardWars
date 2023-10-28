using UnityEngine;

public class AssetLoaderUI : MonoBehaviour
{
	private enum State
	{
		Init,
		CheckAssetDownloads,
		CheckingAssetDownloads,
		LoadScene,
		LoadingScene
	}

	public string startupSceneName = "AdventureTime";

	public UITexture barTexture;

	public UITexture barBG;

	public GameObject retryButton;

	public UILabel messageLabel;

	public BusyIconController busyIconController;

	private float origWidth;

	private State state;

	private string failedToLoadURL;

	private bool popupDone = true;

	private void Start()
	{
		if (barTexture != null)
		{
			origWidth = barTexture.transform.localScale.x;
		}
		if (busyIconController == null)
		{
			busyIconController = SLOTGame.GetInstance();
		}
		ShowRetryButton(false);
		ShowProgressBar(false);
		HideMessage();
	}

	public void SetProgress(float progress)
	{
		if (barTexture != null)
		{
			if (progress < 0f)
			{
				progress = 0f;
			}
			else if (progress > 1f)
			{
				progress = 1f;
			}
			Vector3 localScale = barTexture.transform.localScale;
			localScale.x = origWidth * progress;
			barTexture.transform.localScale = localScale;
		}
	}

	private void Update()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		switch (state)
		{
		case State.Init:
			state = State.CheckAssetDownloads;
			break;
		case State.CheckAssetDownloads:
			CheckAssetDownloads();
			break;
		case State.CheckingAssetDownloads:
			break;
		case State.LoadScene:
			busyIconController.ShowBusyIcon(true);
			SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevelAsync(startupSceneName, LoadLevelDoneCallback);
			state = State.LoadingScene;
			break;
		}
	}

	private void LoadLevelDoneCallback()
	{
		busyIconController.ShowBusyIcon(false);
	}

	private void CheckAssetDownloads()
	{
		state = State.CheckingAssetDownloads;
		ShowProgressBar(true);
		ShowAssetDownloadProgressCallback(0f, 1f);
		busyIconController.ShowBusyIcon(true);
		SLOTGame.GetInstance().CheckAssetDownloads(CheckAssetDownloadsCallback, ShowAssetDownloadProgressCallback, AssetBundleLoadedCallback);
	}

	public void CheckAssetDownloadsCallback(bool success, string err)
	{
		HideMessage();
		if (success)
		{
			ShowProgressBar(false);
			busyIconController.ShowBusyIcon(false);
			state = State.LoadScene;
			return;
		}
		busyIconController.ShowBusyIcon(false);
		if (failedToLoadURL != null)
		{
		}
		string errorMessage = ((err == null || err.Length <= 0) ? KFFLocalization.Get("!!ERROR_DOWNLOADING_ASSETS") : err);
		ShowRetryMessage(errorMessage);
	}

	public void ShowAssetDownloadProgressCallback(float percent, float totalpercent)
	{
		ShowMessage(string.Format(KFFLocalization.Get("!!FORMAT_DOWNLOADING_ASSETS"), (percent / totalpercent * 100f).ToString("f1")));
		SetProgress((!(totalpercent <= 0f)) ? (percent / totalpercent) : 0f);
	}

	private void ShowRetryMessage(string errorMessage)
	{
		ShowMessage(errorMessage);
		ShowRetryButton(true);
		ShowProgressBar(false);
		popupDone = false;
	}

	public bool AssetBundleLoadedCallback(string url, AssetBundle bundle)
	{
		if (bundle != null)
		{
			int num = url.LastIndexOf('/');
			switch ((num < 0) ? url : url.Substring(num + 1))
			{
			case "Resources.assetbundle":
				if (SLOTGameSingleton<SLOTResourceManager>.GetInstance().SetAssetBundle(bundle))
				{
					return true;
				}
				break;
			case "Scenes.assetbundle":
				if (SLOTGameSingleton<SLOTSceneManager>.GetInstance().SetAssetBundle(bundle))
				{
					return true;
				}
				break;
			}
		}
		else if (failedToLoadURL == null)
		{
			failedToLoadURL = url;
		}
		return false;
	}

	private void ShowMessage(string message)
	{
		if (messageLabel != null)
		{
			NGUITools.SetActive(messageLabel.gameObject, true);
			messageLabel.text = message;
		}
	}

	private void HideMessage()
	{
		if (messageLabel != null)
		{
			NGUITools.SetActive(messageLabel.gameObject, false);
		}
	}

	private void ShowRetryButton(bool b)
	{
		if (retryButton != null)
		{
			NGUITools.SetActive(retryButton.gameObject, b);
		}
	}

	private void ShowProgressBar(bool b)
	{
		if (barTexture != null)
		{
			NGUITools.SetActive(barTexture.gameObject, b);
		}
		if (barBG != null)
		{
			NGUITools.SetActive(barBG.gameObject, b);
		}
	}

	private void RetryClicked()
	{
		ShowRetryButton(false);
		HideMessage();
		if (!popupDone)
		{
			popupDone = true;
			CheckAssetDownloads();
		}
	}
}
