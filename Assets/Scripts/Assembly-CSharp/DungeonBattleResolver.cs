using UnityEngine;

public class DungeonBattleResolver : BattleResolver
{
	private string dungeonId;

	private int questIndex;

	public QuestData questData { get; private set; }

	public int questStars
	{
		get
		{
			return -1;
		}
	}

	public string questConditionId
	{
		get
		{
			return null;
		}
	}

	public DungeonBattleResolver(string inDungeonId, int inQuestIndex)
	{
		dungeonId = inDungeonId;
		questIndex = inQuestIndex;
		string dungeonQuestID = DungeonDataManager.Instance.GetDungeonQuestID(dungeonId, questIndex);
		questData = QuestManager.Instance.GetDungeonQuest(dungeonQuestID);
	}

	public void GetOverrideDropCard(ref bool dropCard, ref CardItem card)
	{
	}

	public bool SkipRegularLogic()
	{
		return true;
	}

	public void SetResult(PlayerType winner)
	{
		DungeonBattleResult dungeonBattleResult = new DungeonBattleResult();
		dungeonBattleResult.DungeonID = dungeonId;
		dungeonBattleResult.QuestIndex = questIndex;
		DungeonBattleResult dungeonBattleResult2 = dungeonBattleResult;
		if (winner != PlayerType.User)
		{
			dungeonBattleResult2.SetCleared(false);
			if (winner == null)
			{
				Singleton<AnalyticsManager>.Instance.LogDungeonQuestQuit(dungeonId, questIndex);
			}
			else
			{
				Singleton<AnalyticsManager>.Instance.LogDungeonQuestLoss(dungeonId, questIndex);
			}
		}
		else
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (instance.GetDungeonProgress(dungeonId) < questIndex)
			{
				dungeonBattleResult2.SetCleared(true, true);
				instance.SetDungeonProgress(dungeonBattleResult2);
			}
			else
			{
				dungeonBattleResult2.SetCleared(true);
			}
			RewardUser(PlayerInfoScript.GetInstance(), dungeonBattleResult2.newlyCleared);
			Singleton<AnalyticsManager>.Instance.LogDungeonQuestWin(dungeonId, questIndex);
		}
		GlobalFlags.Instance.BattleResult = dungeonBattleResult2;
	}

	private void RewardUser(PlayerInfoScript pInfo, bool newlyCleared)
	{
		QuestData questData = this.questData;
		LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
		if (leader != null)
		{
			leader.XP += questData.XPRewarded;
		}
		Deck selectedDeck = pInfo.GetSelectedDeck();
		selectedDeck.Leader = leader;
		QuestEarningManager instance = QuestEarningManager.GetInstance();
		foreach (CardItem earnedCard in instance.earnedCards)
		{
			pInfo.DeckManager.AddCard(new CardItem(earnedCard.Form, earnedCard.DropLevel));
		}
		pInfo.Coins += questData.CoinsRewarded;
		pInfo.Coins += instance.earnedCoin;
		pInfo.Gems += instance.earnedGem;
		if (questData.LeaderAwarded.Trim().Length > 0 && newlyCleared)
		{
			LeaderManager.Instance.AddNewLeaderIfUnique(questData.LeaderAwarded);
		}
		if (questData.StaminaAwarded != 0 && newlyCleared)
		{
			pInfo.Stamina_Max++;
			pInfo.Stamina = Mathf.Max(pInfo.Stamina, pInfo.Stamina_Max);
		}
	}
}
