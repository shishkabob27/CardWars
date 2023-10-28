using System;
using Multiplayer;
using UnityEngine;

public class CWBattleEndRewardWinner : AsyncData<string>
{
	public UILabel TrophyEarned;

	public UILabel StreakPanelWinStreak;

	public UILabel StreakPanelWinBonus;

	public UILabel StreakBonus;

	public UILabel TotalTrophyEarned;

	public GameObject[] StarConditions;

	public GameObject StreakPanel;

	private void OnClick()
	{
		RewardsWinner();
	}

	private void StringCallback(string data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void AwardTreasureCatLoot()
	{
		if (!GameState.Instance.ActiveQuest.IsQuestType("tcat"))
		{
			return;
		}
		Deck deck = AIDeckManager.Instance.GetDeck("tcat_Deck");
		int num = PlayerInfoScript.GetInstance().IncQuestProgress(GameState.Instance.ActiveQuest);
		if (num == 1)
		{
			GlobalFlags.Instance.NewlyCleared = true;
		}
		if (deck != null)
		{
			CardItem weightedCard = deck.GetWeightedCard();
			if (weightedCard != null)
			{
				QuestEarningManager.GetInstance().earnedCards.Add(weightedCard);
				QuestEarningManager.GetInstance().earnedCardsName.Add(weightedCard.Form.ID);
				QuestEarningManager.GetInstance().hasCardFlag.Add(PlayerInfoScript.GetInstance().DeckManager.HasCard(weightedCard.Form.ID));
				Singleton<AnalyticsManager>.Instance.LogTreasureChestCatCardAwarded(GameState.Instance.ActiveQuest.iQuestID, weightedCard.Form.ID);
			}
			else
			{
				TFUtils.ErrorLog("No treasure cat card was dropped");
			}
		}
		else
		{
			TFUtils.ErrorLog("Couldn't find treasure cat deck tcat_Deck");
		}
	}

	private void RewardsWinner()
	{
		GlobalFlags instance = GlobalFlags.Instance;
		PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
		AwardTreasureCatLoot();
		QuestEarningManager.GetInstance().InitCardHistory(instance2);
		BattleResolver battleResolver = GameState.Instance.BattleResolver;
		bool flag = battleResolver != null && battleResolver.SkipRegularLogic();
		if (battleResolver != null)
		{
			battleResolver.SetResult(PlayerType.User);
		}
		if (!flag)
		{
			if (instance.InMPMode && Asyncdata.processed)
			{
				global::Multiplayer.Multiplayer.MatchFinish(SessionManager.GetInstance().theSession, CWMPMapController.GetInstance().mLastMPData.mMatchID, false, StringCallback);
				if ((bool)TrophyEarned)
				{
					TrophyEarned.text = instance2.MPWinTrophies.ToString();
				}
				if ((bool)StreakBonus)
				{
					StreakBonus.text = instance2.StreakBonus.ToString();
				}
				if ((bool)TotalTrophyEarned)
				{
					TotalTrophyEarned.text = (instance2.MPWinTrophies + instance2.StreakBonus).ToString();
				}
				if ((bool)StreakPanelWinStreak)
				{
					StreakPanelWinStreak.text = KFFLocalization.Get("!!WIN_STREAK") + " X" + instance2.WinStreak + "!";
				}
				if ((bool)StreakPanelWinBonus)
				{
					StreakPanelWinBonus.text = KFFLocalization.Get("!!WIN_STREAK_SHOUTOUT");
				}
				for (int i = 0; i < StarConditions.Length; i++)
				{
					if (i < instance2.WinStreak)
					{
						StarConditions[i].SetActive(true);
					}
					else
					{
						StarConditions[i].SetActive(false);
					}
				}
				if ((bool)StreakPanel && instance2.WinStreak > 0)
				{
					StreakPanel.SetActive(true);
				}
			}
			else
			{
				QuestData activeQuest = GameState.Instance.ActiveQuest;
				QuestData lastClearedQuest = instance2.GetLastClearedQuest();
				instance.Cleared = true;
				Singleton<AnalyticsManager>.Instance.LogQuestWin();
				instance2.mQuestMatchStats[activeQuest.QuestType].Wins++;
				int questProgress = instance2.GetQuestProgress(activeQuest);
				if (questProgress < 3)
				{
					if (questProgress == 0)
					{
						instance.NewlyCleared = true;
						if ((bool)KFFRequestorController.GetInstance())
						{
							KFFRequestorController.GetInstance().RequestContent("game_levelend");
						}
						instance2.SetLastClearedQuest(activeQuest);
					}
					QuestConditionManager instance3 = QuestConditionManager.Instance;
					string conditionID = activeQuest.Condition[questProgress];
					if (instance3.StatsMeetCondition(conditionID))
					{
						questProgress = instance2.IncQuestProgress(activeQuest);
						if (questProgress == 3)
						{
							instance2.Gems++;
						}
					}
				}
				instance.lastStamina = instance2.Stamina;
				Process1PWinCommon(activeQuest, instance.NewlyCleared);
			}
		}
		instance2.Save();
	}

	private void Update()
	{
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (Asyncdata.MP_Data != null)
			{
				instance.TotalTrophies = Convert.ToInt32(Asyncdata.MP_Data);
				instance.Save();
			}
		}
	}

	private void Process1PWinCommon(QuestData qd, bool newlyCleared)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
		if (leader != null)
		{
			leader.XP += qd.XPRewarded;
		}
		Deck selectedDeck = instance.GetSelectedDeck();
		selectedDeck.Leader = leader;
		QuestEarningManager instance2 = QuestEarningManager.GetInstance();
		foreach (CardItem earnedCard in instance2.earnedCards)
		{
			instance.DeckManager.AddCard(new CardItem(earnedCard.Form, earnedCard.DropLevel));
		}
		instance.Coins += qd.CoinsRewarded;
		instance.Coins += instance2.earnedCoin;
		instance.Gems += instance2.earnedGem;
		if (qd.LeaderAwarded != string.Empty && newlyCleared)
		{
			LeaderManager.Instance.AddNewLeaderIfUnique(qd.LeaderAwarded);
		}
		if (qd.StaminaAwarded != 0 && newlyCleared)
		{
			instance.Stamina_Max++;
			instance.Stamina = Mathf.Max(instance.Stamina, instance.Stamina_Max);
		}
		SideQuestManager.Instance.OnBattleEndWinner(qd);
	}
}
