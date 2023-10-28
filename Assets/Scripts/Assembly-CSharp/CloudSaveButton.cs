using System.Collections;
using UnityEngine;

public class CloudSaveButton : MonoBehaviour
{
	public bool save = true;

	public GameObject enabledButton;

	public GameObject disabledButton;

	private void OnEnable()
	{
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		bool flag = false;
		if (enabledButton != null)
		{
			enabledButton.SetActive(flag);
		}
		if (disabledButton != null)
		{
			disabledButton.SetActive(!flag);
		}
	}

	private void OnClick()
	{
		if (save)
		{
			bool flag = PlayerInfoScript.GetInstance().CloudSaveExists();
			SLOTGame.GetInstance().ShowErrorPopup(KFFLocalization.Get("!!SAVE_TO_CLOUD"), KFFLocalization.Get((!flag) ? "!!CONFIRM_CLOUD_SAVE" : "!!CONFIRM_OVERWRITE_CLOUD_SAVE"), 3, new string[3]
			{
				null,
				string.Empty,
				string.Empty
			}, ConfirmSaveCallback);
		}
		else
		{
			SLOTGame.GetInstance().ShowErrorPopup(KFFLocalization.Get("!!LOAD_FROM_CLOUD"), KFFLocalization.Get("!!CONFIRM_CLOUD_LOAD"), 3, new string[3]
			{
				null,
				string.Empty,
				string.Empty
			}, ConfirmLoadCallback);
		}
		UpdateButtons();
	}

	private void ConfirmSaveCallback(int buttonIndex)
	{
		if (buttonIndex == 1)
		{
			bool flag = PlayerInfoScript.GetInstance().SaveToCloud();
			SLOTGame.GetInstance().ShowErrorPopup(KFFLocalization.Get("!!SAVE_TO_CLOUD"), KFFLocalization.Get((!flag) ? "!!CLOUD_SAVE_FAIL" : "!!CLOUD_SAVE_SUCCESS"), 1, new string[1] { string.Empty });
		}
	}

	private void ConfirmLoadCallback(int buttonIndex)
	{
		if (buttonIndex == 1)
		{
			UICamera.useInputEnabler = true;
			if (!PlayerInfoScript.LoadFromCloud(LoadCallback))
			{
				UICamera.useInputEnabler = false;
				SLOTGame.GetInstance().ShowErrorPopup(KFFLocalization.Get("!!LOAD_FROM_CLOUD"), KFFLocalization.Get("!!CLOUD_LOAD_FAIL"), 1, new string[1] { string.Empty });
			}
		}
	}

	private void LoadCallback(bool success)
	{
		UICamera.useInputEnabler = false;
		if (success)
		{
			StartCoroutine(UpdatePlayerStats());
		}
		SLOTGame.GetInstance().ShowErrorPopup(KFFLocalization.Get("!!LOAD_FROM_CLOUD"), KFFLocalization.Get((!success) ? "!!CLOUD_LOAD_FAIL" : "!!CLOUD_LOAD_SUCCESS"), 1, new string[1] { string.Empty });
	}

	private IEnumerator UpdatePlayerStats()
	{
		CWUpdatePlayerStats playerStats = CWUpdatePlayerStats.GetInstance();
		if (playerStats != null && playerStats.holdUpdateFlag)
		{
			playerStats.holdUpdateFlag = false;
			yield return null;
			playerStats.holdUpdateFlag = true;
		}
	}
}
