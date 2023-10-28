using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QuestLaunchHelper))]
public class DungeonScreenController : MonoBehaviour
{
	public GameObject TemplateWorldItem;

	public GameObject TemplateStageItem;

	public UITweener StageActivationTween;

	public UILabel InfoLabel;

	public GameObject StartButton;

	public UIButtonTween NotEnoughStaminaTween;

	public UIButtonTween TooManyCardsTween;

	public UIGrid WidgetWorlds;

	public UIGrid WidgetStages;

	private List<DungeonData> Dungeons;

	private DungeonWorldItem SelectedWorld;

	private DungeonStageItem SelectedStage;

	private bool StartButtonEnabled;

	public GameObject heartEarnedPanel;

	public AudioClip heartEarnedSound;

	public UISprite leaderSprite;

	public UILabel leaderLabel;

	public UILabel leaderAbility;

	public GameObject leaderCardPanel;

	public AudioClip leaderAquiredSound;

	public UISprite recipeSprite;

	public UILabel recipeLabel;

	public UILabel recipeTypeLabel;

	public GameObject recipeAquiredPanel;

	public AudioClip recipeAquiredSound;

	public UILabel levelLimitLabel;

	public UILabel invalidHeroLabel;

	private bool _keyPressed;

	private bool IsValidLevel
	{
		get
		{
			Deck currentDeck = GetCurrentDeck();
			bool result = false;
			if (currentDeck != null)
			{
				LeaderItem leader = currentDeck.Leader;
				if (leader != null)
				{
					result = leader.Rank >= ParametersManager.Instance.Min_Dungeon_Level;
				}
			}
			return result;
		}
	}

	private bool IsValidHero
	{
		get
		{
			Deck currentDeck = GetCurrentDeck();
			return currentDeck != null && !currentDeck.Leader.Form.FCWorld;
		}
	}

	private void OnEnable()
	{
		RebuildUI();
		if (IsValidHero && IsValidLevel)
		{
			PopulateDungeons();
		}
		StartCoroutine(QuestUnlockSequence());
	}

	private void OnDisable()
	{
		StartButtonEnabled = false;
		InfoLabel.text = string.Empty;
	}

	private void Update()
	{
		StartButton.SetActive(StartButtonEnabled);
	}

	private void RebuildUI()
	{
		GlobalFlags instance = GlobalFlags.Instance;
		if (instance.ReturnToMainMenu && instance.BattleResult != null && instance.BattleResult.returnMenu == BattleResult.Menu.DungeonSelect)
		{
			instance.ReturnToMainMenu = false;
		}
		ClearList(WidgetWorlds);
		WidgetWorlds.repositionNow = true;
		ClearList(WidgetStages);
		WidgetStages.repositionNow = true;
		UpdateLabels();
	}

	private void UpdateLabels()
	{
		if (levelLimitLabel != null)
		{
			levelLimitLabel.gameObject.SetActive(!IsValidLevel);
		}
		if (invalidHeroLabel != null)
		{
			invalidHeroLabel.gameObject.SetActive(!IsValidHero && IsValidLevel);
		}
	}

	private void PopulateDungeons()
	{
		DateTime serverTime = TFUtils.ServerTime;
		Dungeons = ((!DebugFlagsScript.GetInstance().disableDungeonTimeLock) ? DungeonDataManager.Instance.GetAvailableDungeons(serverTime.Ticks) : DungeonDataManager.Instance.GetAllDungeons());
		foreach (DungeonData dungeon in Dungeons)
		{
			GameObject gameObject = NGUITools.AddChild(WidgetWorlds.gameObject, TemplateWorldItem);
			DungeonWorldItem componentInChildren = gameObject.GetComponentInChildren<DungeonWorldItem>();
			componentInChildren.SetData(dungeon);
			componentInChildren.OnSelectEvent += OnDungeonSelect;
		}
		WidgetWorlds.repositionNow = true;
	}

	private void OnDungeonSelect(DungeonWorldItem widgetWorld)
	{
		SelectedStage = null;
		StartButtonEnabled = false;
		if (SelectedWorld != null)
		{
			SelectedWorld.SetHighlighted(false);
			ClearList(WidgetStages);
			WidgetStages.repositionNow = true;
		}
		SelectedWorld = widgetWorld;
		if (widgetWorld != null)
		{
			widgetWorld.SetHighlighted(true);
			InfoLabel.text = KFFLocalization.Get(widgetWorld.Dungeon.Info);
			if (!widgetWorld.Locked)
			{
				BuildStageList(widgetWorld.Dungeon);
			}
		}
		UpdateLabels();
	}

	private void BuildStageList(DungeonData dungeon)
	{
		foreach (DungeonData.Quest quest in dungeon.Quests)
		{
			GameObject gameObject = NGUITools.AddChild(WidgetStages.gameObject, TemplateStageItem);
			DungeonStageItem componentInChildren = gameObject.GetComponentInChildren<DungeonStageItem>();
			componentInChildren.SetData(quest);
			componentInChildren.OnSelectEvent += OnStageSelect;
		}
		WidgetStages.repositionNow = true;
	}

	private void OnStageSelect(DungeonStageItem widgetStage)
	{
		if (!(widgetStage == null) && !widgetStage.StageUnavailable)
		{
			if (SelectedStage != null)
			{
				SelectedStage.SetHighlighted(false);
			}
			SelectedStage = widgetStage;
			SelectedStage.SetHighlighted(true);
			StartButtonEnabled = true;
			HeartAnimation heartAnimation = StartButton.GetComponent(typeof(HeartAnimation)) as HeartAnimation;
			heartAnimation.SetHeartCount(widgetStage.Stage.HeartCost);
			UpdateLabels();
		}
	}

	public void OnStagePlay()
	{
		if (ValidateQuestStart(SelectedStage))
		{
			HeartAnimation heartAnimation = StartButton.GetComponent(typeof(HeartAnimation)) as HeartAnimation;
			heartAnimation.StartAnimation();
		}
	}

	public void OnStageStart()
	{
		UITweener.OnFinished onLoadNextLevel = null;
		onLoadNextLevel = delegate
		{
			if (StageActivationTween != null)
			{
				UITweener stageActivationTween2 = StageActivationTween;
				stageActivationTween2.onFinished = (UITweener.OnFinished)Delegate.Remove(stageActivationTween2.onFinished, onLoadNextLevel);
			}
			LaunchGame(SelectedStage);
		};
		if (StageActivationTween != null)
		{
			UITweener stageActivationTween = StageActivationTween;
			stageActivationTween.onFinished = (UITweener.OnFinished)Delegate.Combine(stageActivationTween.onFinished, onLoadNextLevel);
		}
		else
		{
			onLoadNextLevel(null);
		}
	}

	private void ClearList(UIGrid list)
	{
		Transform transform = list.transform;
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = transform.GetChild(num).gameObject;
			gameObject.SetActive(false);
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void LaunchGame(DungeonStageItem widgetStage)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData questData = widgetStage.QuestData;
		instance.StartMatch(questData, widgetStage.Stage.HeartCost);
		instance.Save();
		GlobalFlags instance2 = GlobalFlags.Instance;
		instance2.InMPMode = false;
		GetComponent<QuestLaunchHelper>().LaunchQuest(questData.QuestID, instance.GetSelectedDeckCopy(), null, new DungeonBattleResolver(SelectedWorld.Dungeon.ID, widgetStage.Stage.Index));
	}

	public void OnYesUseGems()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.Gems--;
		instance.Stamina = instance.Stamina_Max;
		OnStagePlay();
	}

	public bool ValidateQuestStart(DungeonStageItem widgetStage)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck selectedDeckCopy = instance.GetSelectedDeckCopy();
		if (instance.Stamina < widgetStage.Stage.HeartCost)
		{
			if (null != NotEnoughStaminaTween)
			{
				NotEnoughStaminaTween.Play(true);
			}
			return false;
		}
		if (selectedDeckCopy.CardCount() < ParametersManager.Instance.Min_Cards_In_Deck)
		{
			return false;
		}
		if (selectedDeckCopy.CardCount() > selectedDeckCopy.Leader.RankValues.DeckMaxSize)
		{
			return false;
		}
		if (instance.DeckManager.CardCount() > instance.MaxInventory)
		{
			if (null != TooManyCardsTween)
			{
				TooManyCardsTween.Play(true);
			}
			return false;
		}
		return true;
	}

	private IEnumerator QuestUnlockSequence()
	{
		GlobalFlags gflags = GlobalFlags.Instance;
		DungeonBattleResult result = gflags.BattleResult as DungeonBattleResult;
		if (result == null)
		{
			yield break;
		}
		string questId = DungeonDataManager.Instance.GetDungeonQuestID(result.DungeonID, result.QuestIndex);
		if (string.IsNullOrEmpty(questId))
		{
			yield break;
		}
		QuestData qd = QuestManager.Instance.GetDungeonQuest(questId);
		if (qd != null)
		{
			UICamera.useInputEnabler = true;
			if (qd.StaminaAwarded != 0 && result.newlyCleared)
			{
				yield return StartCoroutine(HeartEarningSequence());
			}
			if (qd.LeaderAwarded != string.Empty && result.newlyCleared)
			{
				yield return StartCoroutine(LeaderCardSequence(qd));
			}
			CardForm cd = FusionManager.Instance.GetCardFormByQuestUnlock(qd.QuestID);
			if (cd != null && result.newlyCleared)
			{
				yield return StartCoroutine(RecipeUnlockSequence(cd));
			}
			gflags.BattleResult = null;
			gflags.ReturnToMainMenu = false;
			gflags.Cleared = false;
			CWUpdatePlayerStats playerStats = CWUpdatePlayerStats.GetInstance();
			if (playerStats != null)
			{
				playerStats.holdUpdateFlag = false;
			}
			UICamera.useInputEnabler = false;
		}
	}

	private IEnumerator HeartEarningSequence()
	{
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(heartEarnedSound);
		heartEarnedPanel.SetActive(true);
		_keyPressed = false;
		yield return StartCoroutine(WaitForKeyPress());
		yield return new WaitForSeconds(3f);
	}

	private IEnumerator LeaderCardSequence(QuestData qd)
	{
		LeaderManager leaderManager = LeaderManager.Instance;
		if (leaderManager.leaderForms.ContainsKey(qd.LeaderAwarded))
		{
			LeaderForm leader = leaderManager.leaderForms[qd.LeaderAwarded];
			leaderSprite.spriteName = leader.SpriteName;
			leaderLabel.text = leader.Name;
			leaderAbility.text = leader.Desc.Replace("<val1>", leader.BaseVal1.ToString());
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(leaderAquiredSound);
			leaderCardPanel.SetActive(true);
			_keyPressed = false;
			yield return StartCoroutine(WaitForKeyPress());
		}
	}

	private IEnumerator RecipeUnlockSequence(CardForm cd)
	{
		recipeSprite.spriteName = cd.SpriteName;
		recipeLabel.text = cd.Name;
		recipeTypeLabel.text = KFFLocalization.Get("!!" + cd.Type.ToString().ToUpper());
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(leaderAquiredSound);
		recipeAquiredPanel.SetActive(true);
		yield return StartCoroutine(WaitForKeyPress());
	}

	private IEnumerator WaitForKeyPress()
	{
		while (!_keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				yield return null;
				break;
			}
			yield return 0;
		}
	}

	private Deck GetCurrentDeck()
	{
		Deck result = null;
		if (CWDeckController.GetInstance() != null)
		{
			int currentDeck = CWDeckController.GetInstance().currentDeck;
			result = PlayerInfoScript.GetInstance().DeckManager.Decks[currentDeck];
		}
		else if (GameState.Instance != null)
		{
			result = GameState.Instance.GetDeck(PlayerType.User);
		}
		return result;
	}
}
