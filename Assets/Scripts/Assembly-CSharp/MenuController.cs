using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : ReloadHandler
{
	private enum MenuStartupStates
	{
		Logo_Panel_Visible,
		Returning_To_Menu,
		Logging_In,
		Accept_Policies,
		Startup_Checklist,
		Startup_Complete
	}

	public enum MenuStates
	{
		None,
		Wait,
		Start,
		MainMenu,
		Options,
		Market,
		Gacha,
		Battle,
		Dungeon,
		Deck,
		Update,
		Messages,
		Reload,
		AcceptPolicies
	}

	private static MenuController g_menuController;

	public MenuStates MenuState;

	public GameObject AsyncLoaders;

	public GameObject MainLogo;

	public GameObject PlayerStats;

	public LogoPanelScript LogoPanel;

	public Transform BattleSelectCameraSnap;

	public Transform BattleSelectCameraTargetSnap;

	public Transform DungeonSelectCameraSnap;

	public Transform DungeonSelectCameraTargetSnap;

	public GameObject MainMenuCamera;

	public GameObject WaitHide;

	public GameObject WaitShow;

	public GameObject StartHide;

	public GameObject StartShow;

	public GameObject AcceptPoliciesShow;

	public GameObject AcceptPoliciesHide;

	public GameObject MainMenuHide;

	public GameObject MainMenuShow;

	public GameObject OptionsHide;

	public GameObject OptionsShow;

	public GameObject MarketHide;

	public GameObject MarketShow;

	public GameObject GachaHide;

	public GameObject GachaShow;

	public GameObject BattleHide;

	public GameObject BattleShow;

	public GameObject DungeonHide;

	public GameObject DungeonShow;

	public GameObject DeckHide;

	public GameObject DeckShow;

	public GameObject UpdateHide;

	public GameObject UpdateShow;

	public GameObject MessagesHide;

	public GameObject MessagesShow;

	public GameObject ReloadHide;

	public GameObject ReloadShow;

	public UIButtonTween CalendarShow;

	public UIButtonTween TooManyCardsShow;

	public UIButtonTween DungeonExtrasShow;

	public GameObject YouGotThis;

	public UIButtonTween ElFistoVictoryShow;

	public UIButtonTween ElFistoCompleteShow;

	public GameObject FCMapButton;

    public GameObject QuitDialog;

    private MenuStartupStates MenuStartupState;

	private bool mainMenuFlowActive;

	private bool gachaFlowActive;

	public bool hasAwardStuff;

	private void Awake()
	{
		if (g_menuController == null)
		{
			g_menuController = this;
		}
		hasAwardStuff = false;
	}

	public static MenuController GetInstance()
	{
		return g_menuController;
	}

	private void Start()
	{
		MenuStartupState = ((GlobalFlags.Instance.ReturnToMainMenu || GlobalFlags.Instance.ReturnToBuildDeck) ? MenuStartupStates.Returning_To_Menu : MenuStartupStates.Logo_Panel_Visible);
	}

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
			SwitchToQuit();
        }

        if (MenuStartupState == MenuStartupStates.Startup_Complete)
		{
			return;
		}
		if (MenuStartupState == MenuStartupStates.Returning_To_Menu)
		{
			if (IsAsyncLoadComplete())
			{
				BattleResult.Menu returnMenu = BattleResult.Menu.MapMain;
				if (GlobalFlags.Instance.BattleResult != null)
				{
					returnMenu = GlobalFlags.Instance.BattleResult.returnMenu;
				}
				else if (GlobalFlags.Instance.ReturnToBuildDeck)
				{
					returnMenu = BattleResult.Menu.BuildDeck;
				}
				StartCoroutine(CoroutineMenuReturn(returnMenu));
				MenuStartupState = MenuStartupStates.Startup_Complete;
			}
			return;
		}
		if (GlobalFlags.Instance.ReturnToMainMenu)
		{
			if (GlobalFlags.Instance.BattleResult == null)
			{
				SwitchToBattle();
			}
			else
			{
				SwitchToMainMenu();
			}
			MenuStartupState = MenuStartupStates.Startup_Complete;
			return;
		}
		if (GlobalFlags.Instance.ReturnToBuildDeck)
		{
			SwitchToMainMenu();
			MenuStartupState = MenuStartupStates.Startup_Complete;
			return;
		}
		if (MenuStartupState == MenuStartupStates.Logo_Panel_Visible)
		{
			if (LogoPanel != null && !LogoPanel.IsComplete)
			{
				return;
			}
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (!instance.isInitialized || !IsAsyncLoadComplete())
			{
				SwitchToWait();
			}
			MenuStartupState = MenuStartupStates.Logging_In;
		}
		if (MenuStartupState == MenuStartupStates.Logging_In)
		{
			SessionManager instance2 = SessionManager.GetInstance();
			if (!instance2.IsReady() || !IsAsyncLoadComplete())
			{
				return;
			}
			MenuStartupState = MenuStartupStates.Startup_Checklist;
		}
		if (MenuStartupState == MenuStartupStates.Startup_Checklist)
		{
			SessionManager instance3 = SessionManager.GetInstance();
			PlayerInfoScript instance4 = PlayerInfoScript.GetInstance();
			if (instance3.NeedsForcedUpdate)
			{
				SwitchToUpdate();
				MenuStartupState = MenuStartupStates.Startup_Complete;
			}
			else if (null != instance4 && (!instance4.HasAgreedToTOS || !instance4.HasAgreedToPP))
			{
				StartCoroutine(SwitchToAcceptPolicies());
				MenuStartupState = MenuStartupStates.Accept_Policies;
			}
			else if (instance3.HasNewMessagesReady)
			{
				StartCoroutine(SwitchToMessages(0.5f));
				MenuStartupState = MenuStartupStates.Startup_Complete;
			}
			else
			{
				SwitchToStart(0.5f);
				MenuStartupState = MenuStartupStates.Startup_Complete;
			}
		}
		if (MenuStartupState == MenuStartupStates.Accept_Policies)
		{
			PlayerInfoScript instance5 = PlayerInfoScript.GetInstance();
			if (null != instance5 && instance5.HasAgreedToTOS && instance5.HasAgreedToPP)
			{
				MenuStartupState = MenuStartupStates.Startup_Checklist;
			}
		}
	}

	private IEnumerator CoroutineMenuReturn(BattleResult.Menu returnMenu)
	{
		switch (returnMenu)
		{
		case BattleResult.Menu.BuildDeck:
			yield return StartCoroutine(CoroutineMenuReturnBuildDeck());
			break;
		case BattleResult.Menu.BattleModeSelect:
			yield return StartCoroutine(CoroutineMenuReturnBattleModeSelect());
			break;
		case BattleResult.Menu.DungeonSelect:
			yield return StartCoroutine(CoroutineMenuReturnDungeonSelect());
			break;
		default:
			yield return StartCoroutine(CoroutineMenuReturnMapMain());
			break;
		}
		if (LogoPanel != null)
		{
			LogoPanel.Complete();
		}
		if (MainLogo != null)
		{
			MainLogo.SetActive(false);
		}
		if (PlayerStats != null)
		{
			PlayerStats.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	private IEnumerator CoroutineMenuReturnBuildDeck()
	{
		yield return null;
		CWDeckManagerAdditiveLoad deckManager = CWDeckManagerAdditiveLoad.GetInstance();
		if (deckManager != null)
		{
			deckManager.NavigateToDeckManager();
		}
	}

	private IEnumerator CoroutineMenuReturnBattleModeSelect()
	{
		yield return null;
		if (BattleSelectCameraSnap != null && BattleSelectCameraTargetSnap != null)
		{
			SnapWorldCamera(BattleSelectCameraSnap.position, BattleSelectCameraTargetSnap.position);
		}
		SwitchToBattle();
		yield return null;
		GlobalFlags.Instance.ReturnToMainMenu = false;
		GlobalFlags.Instance.BattleResult = null;
	}

	private IEnumerator CoroutineMenuReturnDungeonSelect()
	{
		yield return null;
		if (DungeonSelectCameraSnap != null && DungeonSelectCameraTargetSnap != null)
		{
			SnapWorldCamera(DungeonSelectCameraSnap.position, DungeonSelectCameraTargetSnap.position);
		}
		SwitchDungeon();
		yield return new WaitForSeconds(0.5f);
		SwitchToDungeonExtras();
		yield return null;
		GlobalFlags.Instance.ReturnToMainMenu = false;
		GlobalFlags.Instance.BattleResult = null;
	}

	private IEnumerator CoroutineMenuReturnMapMain()
	{
		yield return null;
		if (BattleSelectCameraSnap != null && BattleSelectCameraTargetSnap != null)
		{
			SnapWorldCamera(BattleSelectCameraSnap.position, BattleSelectCameraTargetSnap.position);
		}
		SwitchToBattle();
		CWQuestMapAdditiveLoad questMapManager = ((!(AsyncLoaders != null)) ? null : AsyncLoaders.GetComponent<CWQuestMapAdditiveLoad>());
		if (questMapManager != null)
		{
			questMapManager.NavigateToMapScene();
		}
	}

	private void SnapWorldCamera(Vector3 position, Vector3 targetPosition)
	{
		PanelManager instance = PanelManager.GetInstance();
		instance.newCamera.transform.position = position;
		instance.newCameraTarget.transform.position = targetPosition;
		CWMenuCameraTarget component = instance.newCameraTarget.GetComponent<CWMenuCameraTarget>();
		component.followFlag = true;
	}

	private bool IsAsyncLoadComplete()
	{
		if (AsyncLoaders == null)
		{
			return true;
		}
		AsyncLoader[] components = AsyncLoaders.GetComponents<AsyncLoader>();
		AsyncLoader[] array = components;
		foreach (AsyncLoader asyncLoader in array)
		{
			if (!asyncLoader.IsReady)
			{
				return false;
			}
		}
		return true;
	}

	public void AcceptPolicies()
	{
		if (MenuStartupState == MenuStartupStates.Accept_Policies)
		{
			PlayerInfoScript.GetInstance().AcceptTOS();
			PlayerInfoScript.GetInstance().AcceptPP();
		}
	}

	private IEnumerator SwitchToAcceptPolicies(float waitSecs = 0.5f)
	{
		if (waitSecs > 0f)
		{
			yield return new WaitForSeconds(waitSecs);
		}
		TransitionState(AcceptPoliciesShow, MenuStates.AcceptPolicies);
	}

	public void SwitchToWait()
	{
		TransitionState(WaitShow, MenuStates.Wait);
	}

	public void SwitchToStart(float waitSecs = 0f)
	{
		if (MenuState != MenuStates.Start)
		{
			StartCoroutine(SwitchToStartCoroutine(waitSecs));
		}
	}

	private IEnumerator SwitchToStartCoroutine(float waitSecs)
	{
		if (waitSecs > 0f)
		{
			yield return new WaitForSeconds(waitSecs);
		}
		TFUtils.DebugLog("SwitchToStartCoroutine -- Getting product data", "iap");
		Singleton<PurchaseManager>.Instance.GetProductData(null);
		TransitionState(StartShow, MenuStates.Start);
	}

	public void SwitchToMainMenu()
	{
		TransitionState(MainMenuShow, MenuStates.MainMenu);
		StartCoroutine(MainMenuItems());
	}

	private IEnumerator MainMenuItems()
	{
		if (mainMenuFlowActive)
		{
			yield break;
		}
		mainMenuFlowActive = true;
		SessionManager sessionMan = SessionManager.GetInstance();
		if (null != sessionMan && sessionMan.LocalRemoteSaveGameConflict)
		{
			if (null != AuthDialogController.GetInstance())
			{
				AuthDialogController.GetInstance().DisplayAuthDialog();
			}
		}
		else
		{
			if (MenuState == MenuStates.MainMenu && ActivateCalendar())
			{
				yield break;
			}
			yield return StartCoroutine(ShowPlacement("main_menu", 1.5f));
		}
		mainMenuFlowActive = false;
	}

	public bool ActivateCalendar()
	{
		TFUtils.DebugLog("CalendarCheck", "calendar");
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (null != CalendarShow && null != instance && instance.HasUnclaimedCalendarGift())
		{
			TFUtils.DebugLog("Will show calendar", "calendar");
			StartCoroutine(ShowCalendar());
			return true;
		}
		return false;
	}

	private IEnumerator ShowCalendar()
	{
		UICamera.useInputEnabler = true;
		yield return new WaitForSeconds(1f);
		UICamera.useInputEnabler = false;
		CalendarShow.Play(true);
		yield return null;
	}

	public void SwitchToOptions()
	{
		TransitionState(OptionsShow, MenuStates.Options);
	}

	public void SwitchToMarket()
	{
		TransitionState(MarketShow, MenuStates.Market);
	}

	public void SwitchToGacha()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (TooManyCardsShow != null && instance.DeckManager.CardCount() >= instance.MaxInventory)
		{
			TooManyCardsShow.Play(true);
			return;
		}
		TransitionState(GachaShow, MenuStates.Gacha);
		StartCoroutine(GachaItems());
	}

	private IEnumerator GachaItems()
	{
		if (!gachaFlowActive)
		{
			gachaFlowActive = true;
			yield return StartCoroutine(ShowPlacement("game_gacha", 1f));
			gachaFlowActive = false;
		}
	}

	public void CloseTOS()
	{
		if (MenuState == MenuStates.AcceptPolicies)
		{
			StartCoroutine(SwitchToAcceptPolicies());
		}
	}

	public void ClosePP()
	{
		if (MenuState == MenuStates.AcceptPolicies)
		{
			StartCoroutine(SwitchToAcceptPolicies());
		}
	}

	private IEnumerator ShowPlacement(string placement, float waitSecs = 0f)
	{
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		if (!(null == pinfo) && pinfo.IsCalendarUnlocked())
		{
			UICamera.useInputEnabler = true;
			yield return KFFRequestorController.GetInstance().ShowContentCoroutine(placement, waitSecs);
			UICamera.useInputEnabler = false;
			KFFUpsightVGController upsightController = KFFUpsightVGController.GetInstance();
			while (upsightController != null && upsightController.IsPlacementInProgress)
			{
				yield return null;
			}
		}
	}

	public void SwitchToBattle()
	{
		TransitionState(BattleShow, MenuStates.Battle);
		StartCoroutine(AwardStuff());
	}

	private IEnumerator AwardStuff()
	{
		hasAwardStuff = true;
		yield return StartCoroutine(AwardFCDemoCards());
		yield return StartCoroutine(AwardElFistoCards());
	}

	private IEnumerator AwardFCDemoCards()
	{
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		if (null == pinfo)
		{
			yield break;
		}
		if (pinfo.HasCompletedFCDemo())
		{
			yield return new WaitForSeconds(1f);
			if (!pinfo.HasReceivedFCCards())
			{
				if (null == YouGotThis)
				{
					yield break;
				}
				YouGotThisController youGotThisController = YouGotThis.GetComponent<YouGotThisController>();
				if (null == youGotThisController)
				{
					yield break;
				}
				yield return StartCoroutine(youGotThisController.AwardLeader("Leader_Fionna"));
				yield return StartCoroutine(youGotThisController.AwardLeader("Leader_Cake"));
				PlayerInfoScript.GetInstance().SetHasReceivedFCCards();
				Singleton<AnalyticsManager>.Instance.LogFCHeroesAwarded();
				yield return new WaitForSeconds(2.5f);
			}
			if (!pinfo.HasSeenFCUpsellScreen() && null != FCMapButton)
			{
				FCMapButton.SendMessage("OnClick");
				pinfo.SetHasSeenFCUpsellScreen();
			}
		}
		yield return null;
	}

	public IEnumerator AwardSideQuestCards(SideQuestData sqd)
	{
		YouGotThisController youGotThisController = YouGotThis.GetComponent<YouGotThisController>();
		if (!(null == youGotThisController))
		{
			yield return StartCoroutine(youGotThisController.AwardCard(sqd.RewardID));
		}
	}

	public IEnumerator AwardCard(string cardID)
	{
		YouGotThisController youGotThisController = YouGotThis.GetComponent<YouGotThisController>();
		if (!(null == youGotThisController))
		{
			if (cardID.StartsWith("Leader"))
			{
				yield return StartCoroutine(youGotThisController.AwardLeader(cardID));
			}
			else
			{
				yield return StartCoroutine(youGotThisController.AwardCard(cardID));
			}
		}
	}

	private IEnumerator AwardElFistoCards()
	{
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		if (!(null == pinfo))
		{
			ElFistoController efc2 = base.gameObject.AddComponent<ElFistoController>();
			if (efc2.ShouldAward())
			{
				UICamera.useInputEnabler = true;
				yield return new WaitForSeconds(1f);
				yield return StartCoroutine(efc2.DisplayAward(YouGotThis));
				yield return new WaitForSeconds(2.5f);
				yield return StartCoroutine(efc2.DisplayElFisto(ElFistoVictoryShow, ElFistoCompleteShow));
				UICamera.useInputEnabler = false;
			}
			Object.Destroy(efc2);
			efc2 = null;
			yield return null;
			hasAwardStuff = false;
		}
	}

	public void SwitchDungeon()
	{
		TransitionState(DungeonShow, MenuStates.Dungeon);
	}

	public void SwitchToDungeonExtras()
	{
		if (null != DungeonExtrasShow && MenuState == MenuStates.Dungeon)
		{
			DungeonExtrasShow.Play(true);
		}
	}

	public void SwitchToDeck()
	{
		TransitionState(DeckShow, MenuStates.Deck);
	}

	public override void SwitchToUpdate()
	{
		HideCurrent();
		DoTweens(UpdateShow);
		MenuState = MenuStates.Update;
	}

	public override void SwitchToReload()
	{
		TransitionState(ReloadShow, MenuStates.Reload);
	}

	public void SwitchToDeckBuild()
	{
		TransitionState(MenuStates.Deck);
	}

	public void SwitchToQuit()
	{
        if (!QuitDialog.activeSelf)
        {
            QuitDialog.SetActive(true);
        }
        TransitionState(MenuStates.None);
	}

	private IEnumerator SwitchToMessages(float wait = 0f)
	{
		if (wait > 0f)
		{
			yield return new WaitForSeconds(wait);
		}
		TransitionState(MessagesShow, MenuStates.Messages);
	}

	private void TransitionState(MenuStates state)
	{
		TransitionState(null, state);
	}

	private void TransitionState(GameObject target, MenuStates state)
	{
		lock (this)
		{
			HideCurrent();
			DoTweens(target);
			MenuState = state;
		}
	}

	private void HideCurrent()
	{
		switch (MenuState)
		{
		case MenuStates.Wait:
			DoTweens(WaitHide);
			break;
		case MenuStates.Start:
			DoTweens(StartHide);
			break;
		case MenuStates.MainMenu:
			DoTweens(MainMenuHide);
			break;
		case MenuStates.Options:
			DoTweens(OptionsHide);
			break;
		case MenuStates.Market:
			DoTweens(MarketHide);
			break;
		case MenuStates.Gacha:
			DoTweens(GachaHide);
			break;
		case MenuStates.Battle:
			DoTweens(BattleHide);
			break;
		case MenuStates.Dungeon:
			DoTweens(DungeonHide);
			break;
		case MenuStates.Deck:
			DoTweens(DeckHide);
			break;
		case MenuStates.Update:
			DoTweens(UpdateHide);
			break;
		case MenuStates.Messages:
			DoTweens(MessagesHide);
			break;
		case MenuStates.AcceptPolicies:
			DoTweens(AcceptPoliciesHide);
			break;
		}
		List<GameObject> list = new List<GameObject>();
		if (MenuState != MenuStates.Wait)
		{
			list.Add(WaitHide);
		}
		if (MenuState != MenuStates.Start)
		{
			list.Add(StartHide);
		}
		if (MenuState != MenuStates.MainMenu)
		{
			list.Add(MainMenuHide);
		}
		if (MenuState != MenuStates.Options)
		{
			list.Add(OptionsHide);
		}
		if (MenuState != MenuStates.Market)
		{
			list.Add(MarketHide);
		}
		if (MenuState != MenuStates.Gacha)
		{
			list.Add(GachaHide);
		}
		if (MenuState != MenuStates.Battle)
		{
			list.Add(BattleHide);
		}
		if (MenuState != MenuStates.Dungeon)
		{
			list.Add(DungeonHide);
		}
		if (MenuState != MenuStates.Deck)
		{
			list.Add(DeckHide);
		}
		if (MenuState != MenuStates.Update)
		{
			list.Add(UpdateHide);
		}
		if (MenuState != MenuStates.Messages)
		{
			list.Add(MessagesHide);
		}
		foreach (GameObject item in list)
		{
			UIButtonTween[] components = item.GetComponents<UIButtonTween>();
			UIButtonTween[] array = components;
			foreach (UIButtonTween uIButtonTween in array)
			{
				NGUITools.SetActive(uIButtonTween.tweenTarget, false);
			}
		}
	}

	private void DoTweens(GameObject obj)
	{
		if (obj != null)
		{
			UIButtonTween[] componentsInChildren = obj.GetComponentsInChildren<UIButtonTween>(true);
			UIButtonTween[] array = componentsInChildren;
			foreach (UIButtonTween uIButtonTween in array)
			{
				uIButtonTween.Play(true);
			}
		}
	}

	private void SpawnEnvironmentPrefab(string scheduleCategory)
	{
		List<ScheduleData> itemsAvailableAndUnlocked = ScheduleDataManager.Instance.GetItemsAvailableAndUnlocked(scheduleCategory, TFUtils.ServerTime.Ticks);
		foreach (ScheduleData item in itemsAvailableAndUnlocked)
		{
			if (TryLoadEnvironmentPrefab(item.ID))
			{
				break;
			}
		}
	}

	private bool TryLoadEnvironmentPrefab(string prefab)
	{
		string path = "Environment/" + prefab;
		Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(path);
		if (@object != null)
		{
			Object.Instantiate(@object);
			return true;
		}
		return false;
	}
}
