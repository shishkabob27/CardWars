using System.Collections;
using UnityEngine;

public class TutorialMonitor : MonoBehaviour
{
	public GameObject landscapeCardTutorialText;

	public GameObject playersFirstTurnLabel;

	public TutorialBattleRing attackTutorialRing;

	public TutorialBattleRing defendTutorialRing;

	public UITweener MainMenuTweener;

	public CWCardTutorial cardTutorial;

	private static TutorialMonitor instance;

	private CWTutorialsPopup Popup;

	private DebugFlagsScript debugFlag;

	private GameObject saveEventReceiver;

	private string saveCallWhenFinished;

	private UITweener craftingTweener;

	private bool saveUseInputEnabler;

	public bool PopupActive { get; set; }

	public static TutorialMonitor Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		if (cardTutorial != null)
		{
			cardTutorial.Show(false);
		}
	}

	private void Start()
	{
		Popup = GetComponent<CWTutorialsPopup>();
		debugFlag = DebugFlagsScript.GetInstance();
	}

	public bool TriggerTutorial(TutorialTrigger trigger)
	{
		TutorialManager tutorialManager = TutorialManager.Instance;
		TutorialInfo info = tutorialManager.Find(trigger);
		return TriggerTutorial(info);
	}

	public bool TriggerTutorial(TutorialInfo info)
	{
		if (ShouldTriggerTutorial(info, false) && info != null)
		{
			if (!info.dummy)
			{
				StartTutorial(info);
			}
			else
			{
				OnStartTutorial(info);
				StartCoroutine(CompleteDummyTutorial(info.TutorialID));
			}
			return true;
		}
		return false;
	}

	private IEnumerator CompleteDummyTutorial(string tutorialID)
	{
		yield return null;
		TutorialManager Manager = TutorialManager.Instance;
		Manager.markTutorialCompleted(tutorialID);
		TutorialInfo targetInfo = TutorialManager.Instance.Find(tutorialID);
		if (targetInfo != null)
		{
			TutorialInfo targetInfo2 = ((!string.IsNullOrEmpty(targetInfo.Button1.ButtonAction)) ? TutorialManager.Instance.Find(targetInfo.Button1.ButtonAction) : null);
			if (targetInfo2 != null)
			{
				TriggerTutorial(targetInfo2);
			}
		}
	}

	public bool ShouldTriggerTutorial(TutorialTrigger trigger)
	{
		return ShouldTriggerTutorial(trigger, true);
	}

	public bool ShouldTriggerTutorial(TutorialTrigger trigger, bool ignoreDummy)
	{
		TutorialManager tutorialManager = TutorialManager.Instance;
		TutorialInfo info = tutorialManager.Find(trigger);
		return ShouldTriggerTutorial(info, ignoreDummy);
	}

	public bool ShouldTriggerTutorial(TutorialInfo info, bool ignoreDummy)
	{
		if (info != null)
		{
			if (debugFlag.stopTutorial && !info.Reusable)
			{
				return false;
			}
			PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
			TutorialManager tutorialManager = TutorialManager.Instance;
			if ((info.DependencyTrigger == TutorialTrigger.None || tutorialManager.isTutorialCompleted(info.DependencyTrigger)) && (!PopupActive || info.CanOverride) && (!tutorialManager.isTutorialCompleted(info.TutorialID) || info.Reusable) && (!ignoreDummy || !info.dummy))
			{
				QuestData currentQuest = playerInfoScript.GetCurrentQuest();
				int? currentQuest2 = info.CurrentQuest;
				QuestData questData = (currentQuest2.HasValue ? QuestManager.Instance.GetQuest(info.CurrentQuest.Value) : null);
				int? lastClearedQuest = info.LastClearedQuest;
				QuestData questData2 = (lastClearedQuest.HasValue ? QuestManager.Instance.GetQuest(info.LastClearedQuest.Value) : null);
				if ((questData == null || currentQuest.iQuestID == questData.iQuestID) && (questData2 == null || playerInfoScript.GetQuestProgress(questData2) > 0))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void StartTutorial(string id)
	{
		TutorialInfo tutorialInfo = TutorialManager.Instance.Find(id);
		if (tutorialInfo != null)
		{
			StartTutorial(tutorialInfo);
		}
	}

	public void StartTutorial(TutorialInfo info)
	{
		Popup.tutorial_ID = info.TutorialID;
		Popup.OnClick(false);
	}

	public void StopTutorialAudio()
	{
		GetComponent<AudioSource>().Stop();
	}

	public void PlayAudioForTutorial(AudioClip audioClip, float delay)
	{
		StopTutorialAudio();
		StartCoroutine(PlayDelayed(audioClip, delay));
	}

	private IEnumerator PlayDelayed(AudioClip audioClip, float delay)
	{
		if (Time.timeScale == 0f)
		{
			float startTime = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup - startTime < delay)
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(delay);
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), audioClip);
	}

	public void OnStartTutorial(TutorialInfo info)
	{
		TutorialTrigger trigger = info.Trigger;
		if (info.PauseGame)
		{
			Time.timeScale = 0f;
		}
		switch (trigger)
		{
		case TutorialTrigger.EnterMainMenu:
		{
			GameObject gameObject = GameObject.Find("F_BattleButton");
			if (gameObject != null)
			{
				UITweener uITweener2 = gameObject.GetComponent(typeof(UITweener)) as UITweener;
				if (uITweener2 != null)
				{
					StartCoroutine(StartTween(uITweener2));
				}
			}
			break;
		}
		case TutorialTrigger.LandscapeSelection:
			if (landscapeCardTutorialText != null)
			{
				landscapeCardTutorialText.SetActive(true);
				UILabel uILabel = landscapeCardTutorialText.gameObject.GetComponentInChildren(typeof(UILabel)) as UILabel;
				if (uILabel != null)
				{
					uILabel.enabled = true;
				}
			}
			break;
		case TutorialTrigger.FirstLandscape:
		case TutorialTrigger.SecondLandscape:
		case TutorialTrigger.ThirdLandscape:
		case TutorialTrigger.FourthLandscape:
		{
			TutorialManager tutorialManager = TutorialManager.Instance;
			if (!tutorialManager.isTutorialCompleted(TutorialTrigger.FirstHand))
			{
				CWKetchupBottleScript.forceStartingPlayer = DebugFlagsScript.ForceStartingPlayer.Me;
			}
			break;
		}
		case TutorialTrigger.PlayersFirstTurn:
		case TutorialTrigger.SecondHand:
		case TutorialTrigger.ThirdHand:
		{
			if (playersFirstTurnLabel != null)
			{
				playersFirstTurnLabel.SetActive(true);
			}
			CWPlayerCardManager cWPlayerCardManager = Object.FindObjectOfType(typeof(CWPlayerCardManager)) as CWPlayerCardManager;
			if (cWPlayerCardManager != null)
			{
				cWPlayerCardManager.StartFirstTurnTutorial();
			}
			UICamera.useInputEnabler = true;
			break;
		}
		case TutorialTrigger.PressQuestStartButton:
		{
			PlayQuestButton playQuestButton = Object.FindObjectOfType(typeof(PlayQuestButton)) as PlayQuestButton;
			if (playQuestButton != null && playQuestButton.blinkTween != null)
			{
				StartCoroutine(StartTween(playQuestButton.blinkTween));
			}
			break;
		}
		case TutorialTrigger.AttackTutorial:
			if (attackTutorialRing != null)
			{
				attackTutorialRing.SetPhase(0);
			}
			break;
		case TutorialTrigger.AttackTutorial1:
			if (attackTutorialRing != null)
			{
				attackTutorialRing.SetPhase(1);
			}
			break;
		case TutorialTrigger.AttackTutorial2:
			if (attackTutorialRing != null)
			{
				attackTutorialRing.SetPhase(2);
			}
			break;
		case TutorialTrigger.AttackTutorial3:
			if (attackTutorialRing != null)
			{
				attackTutorialRing.SetPhase(3);
			}
			break;
		case TutorialTrigger.AttackTutorial4:
			if (attackTutorialRing != null)
			{
				attackTutorialRing.SetPhase(4);
			}
			break;
		case TutorialTrigger.AttackTutorial5:
			if (attackTutorialRing != null)
			{
				attackTutorialRing.SetPhase(5);
			}
			break;
		case TutorialTrigger.DefendTutorial:
			if (defendTutorialRing != null)
			{
				defendTutorialRing.SetPhase(0);
			}
			break;
		case TutorialTrigger.DefendTutorial1:
			if (defendTutorialRing != null)
			{
				defendTutorialRing.SetPhase(1);
			}
			break;
		case TutorialTrigger.DefendTutorial2:
			if (defendTutorialRing != null)
			{
				defendTutorialRing.SetPhase(2);
			}
			break;
		case TutorialTrigger.DefendTutorial3:
			if (defendTutorialRing != null)
			{
				defendTutorialRing.SetPhase(3);
			}
			break;
		case TutorialTrigger.DefendTutorial4:
			if (defendTutorialRing != null)
			{
				defendTutorialRing.SetPhase(4);
			}
			break;
		case TutorialTrigger.DefendTutorial5:
			if (defendTutorialRing != null)
			{
				defendTutorialRing.SetPhase(5);
			}
			break;
		case TutorialTrigger.CloseCardCrafting:
		{
			GameObject gameObject = GameObject.Find("B_1_BackToMenu/0. Back");
			if (gameObject != null)
			{
				gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			gameObject = GameObject.Find("C_0_Fuse");
			if (gameObject != null)
			{
				UITweener uITweener = gameObject.GetComponent(typeof(UITweener)) as UITweener;
				if (uITweener != null)
				{
					saveUseInputEnabler = UICamera.useInputEnabler;
					UICamera.useInputEnabler = true;
					craftingTweener = uITweener;
					saveEventReceiver = uITweener.eventReceiver;
					saveCallWhenFinished = uITweener.callWhenFinished;
					uITweener.eventReceiver = base.gameObject;
					uITweener.callWhenFinished = "OnCraftingTweenerFinished";
				}
			}
			break;
		}
		case TutorialTrigger.GatchaBackFromMapScreen:
		{
			GameObject gameObject = GameObject.Find("F_0_QuestsMenu_BottomInfo/BackButton");
			if (gameObject != null)
			{
				gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case TutorialTrigger.EnterGatcha:
		{
			CWGachaController cWGachaController = CWGachaController.GetInstance();
			cWGachaController.OpenPremiumChestForFree = true;
			break;
		}
		case TutorialTrigger.ExitGatcha:
			TutorialManager.Instance.markTutorialCompleted(info.TutorialID);
			if (ShouldTriggerTutorial(TutorialTrigger.GatchaBackToMainMenu, false))
			{
				UICamera.useInputEnabler = true;
			}
			break;
		case TutorialTrigger.DeckManagerTapBuildings:
		{
			PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
			if (!deckManager.HasCard(CardType.Building))
			{
				string id = "Building_ComfyCave";
				CardForm card4 = CardDataManager.Instance.GetCard(id);
				CardItem card5 = new CardItem(card4);
				deckManager.AddCard(card5);
			}
			break;
		}
		case TutorialTrigger.DeckManagerTutorialDone:
		{
			GameObject gameObject = GameObject.Find("C_1_DeckMenu_BottomInfo/BackButton");
			if (gameObject != null)
			{
				gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case TutorialTrigger.CardTutorial:
			if (cardTutorial != null)
			{
				cardTutorial.Show(true);
			}
			break;
		case TutorialTrigger.CardCraftingUnlocked:
		{
			CWFuseShowRecipe cWFuseShowRecipe = Object.FindObjectOfType(typeof(CWFuseShowRecipe)) as CWFuseShowRecipe;
			if (!(cWFuseShowRecipe != null))
			{
				break;
			}
			string text = "Creature_CareCrow";
			PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
			deckManager.PrecacheCounts();
			CardForm card = CardDataManager.Instance.GetCard(text);
			RecipeData recipe = FusionManager.Instance.GetRecipe(card);
			foreach (RecipeIngredientData ingredient in recipe.ingredients)
			{
				int num = deckManager.CardCount(ingredient.Form);
				if (num < ingredient.Count)
				{
					CardForm card2 = CardDataManager.Instance.GetCard(ingredient.Form.ID);
					for (int i = num; i < ingredient.Count; i++)
					{
						CardItem card3 = new CardItem(card2);
						deckManager.AddCard(card3);
						deckManager.PrecacheCounts();
					}
				}
			}
			PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
			if (playerInfoScript.Coins < recipe.cost)
			{
				playerInfoScript.Coins = recipe.cost;
			}
			CWFuseRecipeClicked.ShowRecipe(cWFuseShowRecipe, text, true);
			break;
		}
		case TutorialTrigger.CraftingBackFromMapScreen:
		{
			GameObject gameObject = GameObject.Find("F_0_QuestsMenu_BottomInfo/BackButton");
			if (gameObject != null)
			{
				gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		}
		if (!string.IsNullOrEmpty(info.Flow) && info.IsLastInFlow)
		{
			TutorialManager.Instance.markTutorialCompleted(info.Flow);
			PlayerInfoScript.GetInstance().Save();
		}
		float timeScale = Time.timeScale;
		Object[] array = Object.FindObjectsOfType(typeof(PauseMenu));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			PauseMenu pauseMenu = @object as PauseMenu;
			if (pauseMenu != null)
			{
				pauseMenu.gameObject.SendMessage("TutorialPopupShown", null, SendMessageOptions.DontRequireReceiver);
			}
		}
		Time.timeScale = timeScale;
	}

	public void OnTutorialStarted(TutorialInfo info, GameObject popup)
	{
		TutorialTrigger trigger = info.Trigger;
		TutorialTrigger tutorialTrigger = trigger;
		if (tutorialTrigger == TutorialTrigger.CardTutorial && cardTutorial != null)
		{
			cardTutorial.SetPopup(popup);
		}
	}

	private IEnumerator StartTween(UITweener tw)
	{
		yield return null;
		tw.enabled = true;
		tw.Play(true);
		tw.Reset();
	}

	public void HidePlayersFirstTurnLabel()
	{
		if (playersFirstTurnLabel != null)
		{
			playersFirstTurnLabel.SetActive(false);
		}
		CWPlayerCardManager cWPlayerCardManager = Object.FindObjectOfType(typeof(CWPlayerCardManager)) as CWPlayerCardManager;
		if (cWPlayerCardManager != null)
		{
			cWPlayerCardManager.EndFirstTurnTutorial();
		}
	}

	public void QueueTutorial(TutorialTrigger trigger)
	{
		StartCoroutine(TriggerTutorialCoroutine(trigger));
	}

	private IEnumerator TriggerTutorialCoroutine(TutorialTrigger trigger)
	{
		while (Instance.PopupActive)
		{
			yield return null;
		}
		Instance.TriggerTutorial(trigger);
	}

	public void ToDeckManager()
	{
		saveUseInputEnabler = UICamera.useInputEnabler;
		UICamera.useInputEnabler = true;
		StartCoroutine(ToDeckManagerCoroutine());
	}

	private IEnumerator ToDeckManagerCoroutine()
	{
		yield return null;
		GameObject obj = GameObject.Find("F_0_QuestsMenu_BottomInfo/BackButton");
		if (obj != null)
		{
			obj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		if (MainMenuTweener != null)
		{
			saveEventReceiver = MainMenuTweener.eventReceiver;
			saveCallWhenFinished = MainMenuTweener.callWhenFinished;
			MainMenuTweener.eventReceiver = base.gameObject;
			MainMenuTweener.callWhenFinished = "OnMainMenuTweenerFinished3";
		}
	}

	private void OnMainMenuTweenerFinished3()
	{
		if (MainMenuTweener != null)
		{
			if (saveEventReceiver != null && !string.IsNullOrEmpty(saveCallWhenFinished))
			{
				saveEventReceiver.SendMessage(saveCallWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			MainMenuTweener.eventReceiver = saveEventReceiver;
			MainMenuTweener.callWhenFinished = saveCallWhenFinished;
		}
		GameObject gameObject = GameObject.Find("9_MainMenuButtons/C_DeckButton");
		if (gameObject != null)
		{
			gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		UICamera.useInputEnabler = saveUseInputEnabler;
	}

	public void ToMarket()
	{
		saveUseInputEnabler = UICamera.useInputEnabler;
		UICamera.useInputEnabler = true;
		StartCoroutine(ToMarketCoroutine());
	}

	private IEnumerator ToMarketCoroutine()
	{
		yield return null;
		GameObject obj = GameObject.Find("F_0_QuestsMenu_BottomInfo/BackButton");
		if (obj != null)
		{
			obj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		if (MainMenuTweener != null)
		{
			saveEventReceiver = MainMenuTweener.eventReceiver;
			saveCallWhenFinished = MainMenuTweener.callWhenFinished;
			MainMenuTweener.eventReceiver = base.gameObject;
			MainMenuTweener.callWhenFinished = "OnMainMenuTweenerFinished4";
		}
	}

	private void OnMainMenuTweenerFinished4()
	{
		if (MainMenuTweener != null)
		{
			if (saveEventReceiver != null && !string.IsNullOrEmpty(saveCallWhenFinished))
			{
				saveEventReceiver.SendMessage(saveCallWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			MainMenuTweener.eventReceiver = saveEventReceiver;
			MainMenuTweener.callWhenFinished = saveCallWhenFinished;
		}
		GameObject gameObject = GameObject.Find("9_MainMenuButtons/D_MarketButton");
		if (gameObject != null)
		{
			gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		UICamera.useInputEnabler = saveUseInputEnabler;
	}

	private void OnCraftingTweenerFinished()
	{
		if (craftingTweener != null)
		{
			if (saveEventReceiver != null && !string.IsNullOrEmpty(saveCallWhenFinished))
			{
				saveEventReceiver.SendMessage(saveCallWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			craftingTweener.eventReceiver = saveEventReceiver;
			craftingTweener.callWhenFinished = saveCallWhenFinished;
		}
		UICamera.useInputEnabler = saveUseInputEnabler;
	}
}
