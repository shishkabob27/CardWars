using UnityEngine;

public class OptionsScript : MonoBehaviour
{
	public UISlider sfxSlider;

	public UISlider voSlider;

	public UISlider musicSlider;

	public ButtonToggleScript toggleBattleWheel;

	public ButtonToggleScript toggleNotifications;

	public ButtonToggleScript toggleLowResolution;

	public GameObject LowResOverride;

	public GameObject AgeReset;

	private SLOTAudioManager audioMgr;

	private bool updating;

	private void Awake()
	{
		audioMgr = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		LowResOverride.SetActive(false);

	}

	private void Start()
	{
		if (null != AgeReset)
		{
			AgeReset.SetActive(true);
			AgeReset.GetComponentInChildren<UILabel>().text = "Logout";
		}
	}

	private void OnEnable()
	{
		UpdateSliders();
	}

	public void ChangeSFXVolume(float vol)
	{
		if (!updating && audioMgr != null)
		{
			audioMgr.SetSoundVolume(vol);
		}
	}

	public void ChangeMusicVolume(float vol)
	{
		if (!updating && audioMgr != null)
		{
			audioMgr.SetMusicVolume(vol);
		}
	}

	public void ChangeVOVolume(float vol)
	{
		if (!updating && audioMgr != null)
		{
			audioMgr.SetVOVolume(vol);
		}
	}

	public void UpdateSliders()
	{
		updating = true;
		if (audioMgr != null)
		{
			if (sfxSlider != null)
			{
				sfxSlider.sliderValue = audioMgr.GetSoundVolume();
			}
			if (voSlider != null)
			{
				voSlider.sliderValue = audioMgr.GetVOVolume();
			}
			if (musicSlider != null)
			{
				musicSlider.sliderValue = audioMgr.GetMusicVolume();
			}
			if (toggleBattleWheel != null)
			{
				toggleBattleWheel.SetToggle(!PlayerInfoScript.GetInstance().AutoBattleSetting);
			}
			if (toggleNotifications != null)
			{
				toggleNotifications.SetToggle(PlayerInfoScript.GetInstance().NotificationEnabled);
			}
		}
		if (toggleLowResolution != null)
		{
			toggleLowResolution.SetToggle(!KFFLODManager.GetLowEndOverride());
		}
		updating = false;
	}

	public void ToggleAutoBattle()
	{
		PlayerInfoScript.GetInstance().AutoBattleSetting = !PlayerInfoScript.GetInstance().AutoBattleSetting;
		PlayerInfoScript.GetInstance().Save();
	}

	public void ToggleNotifications()
	{
		PlayerInfoScript.GetInstance().NotificationEnabled = !PlayerInfoScript.GetInstance().NotificationEnabled;
		PlayerInfoScript.GetInstance().Save();
	}

	public void ToggleResolution()
	{
		KFFLODManager.SetLowEndOverride(!KFFLODManager.GetLowEndOverride());
		PlayerInfoScript.GetInstance().ReloadGame();
	}

	public void ResetAge()
	{
		PlayerPrefs.DeleteAll();
		SocialManager.Instance.playerDidLogOut();
        PlayerInfoScript.GetInstance().ReloadGame();
	}
}
