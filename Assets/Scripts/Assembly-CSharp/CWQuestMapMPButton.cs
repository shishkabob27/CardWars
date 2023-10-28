using System.Collections;
using Multiplayer;
using UnityEngine;

public class CWQuestMapMPButton : AsyncData<MultiplayerData>
{
	public string daggerAnimation;

	public string idleAnimation;

	public Animation battleMapAnimation;

	public GameObject enterMapEvents;

	public Camera mainMenuCamera;

	public MultiAnimationScript AnimationScripts;

	public UIButtonTween ShowEnterMPNameUI;

	public UIButtonTween HideEnterMPNameUI;

	public UIButtonTween HideBottonInfo;

	public UIButtonTween NotEnoughMoney;

	public UIButtonTween UnderMaintenance;

	public UIButtonTween LoadingActivityShow;

	public UIButtonTween LoadingActivityHide;

	public UIButtonTween ConnectionFailedShow;

	public UIButtonTween ConnectionFailedHide;

	private bool daggerAnimationPlaying;

	private void OnClick()
	{
		if ((!(mainMenuCamera != null) || (mainMenuCamera.gameObject.activeInHierarchy && mainMenuCamera.enabled)) && (!(AnimationScripts != null) || !AnimationScripts.IsPlayingStartAnimRevert()) && Asyncdata.processed && SessionManager.GetInstance().IsReady())
		{
			if ((bool)LoadingActivityShow)
			{
				LoadingActivityShow.Play(true);
			}
			ReauthenticationHelper component = GetComponent<ReauthenticationHelper>();
			if (!(component != null) || !component.Reauthenticate(delegate(ReauthenticationHelper.Result result)
			{
				switch (result)
				{
				case ReauthenticationHelper.Result.SUCCESS:
					StartMultiplayerActivation();
					break;
				case ReauthenticationHelper.Result.SUCCESS_FORCED_RESTART:
					if ((bool)LoadingActivityHide)
					{
						LoadingActivityHide.Play(true);
					}
					break;
				default:
					if ((bool)LoadingActivityHide)
					{
						LoadingActivityHide.Play(true);
					}
					if ((bool)ConnectionFailedShow)
					{
						ConnectionFailedShow.Play(true);
					}
					break;
				}
			}))
			{
				StartMultiplayerActivation();
			}
		}
	}

	private void StartMultiplayerActivation()
	{
		global::Multiplayer.Multiplayer.GetMultiplayerStatus(SessionManager.GetInstance().theSession, MultiplayerDataCallback);
	}

	private void MultiplayerDataCallback(MultiplayerData data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if ((bool)LoadingActivityHide)
			{
				LoadingActivityHide.Play(true);
			}
			if (Asyncdata.MP_Data == null)
			{
				if (Asyncdata.flag == ResponseFlag.None)
				{
					if ((bool)ShowEnterMPNameUI)
					{
						ShowEnterMPNameUI.Play(true);
					}
					if ((bool)HideBottonInfo)
					{
						HideBottonInfo.Play(true);
					}
				}
				else if ((bool)UnderMaintenance)
				{
					UnderMaintenance.Play(true);
				}
			}
			else
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.MultiplayerUnlocked);
				if ((bool)instance)
				{
					instance.TotalTrophies = Asyncdata.MP_Data.trophies;
					instance.MPPlayerName = Asyncdata.MP_Data.name;
				}
				EnterMap();
			}
		}
		if (daggerAnimationPlaying && battleMapAnimation != null && idleAnimation != null && !battleMapAnimation.isPlaying)
		{
			daggerAnimationPlaying = false;
			UICamera.useInputEnabler = false;
			EnterMap();
			battleMapAnimation.Play(idleAnimation);
		}
	}

	private void EnterMap()
	{
		if (enterMapEvents != null)
		{
			GlobalFlags instance = GlobalFlags.Instance;
			instance.InMPMode = true;
			instance.BattleResult = null;
			CWMapController.Activate(true);
			enterMapEvents.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			StartCoroutine(CompleteMultiplayerTutorial());
		}
	}

	public static IEnumerator CompleteMultiplayerTutorial()
	{
		while (Time.timeScale == 0f)
		{
			yield return null;
		}
		TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.MultiplayerTutorialCompleted);
	}
}
