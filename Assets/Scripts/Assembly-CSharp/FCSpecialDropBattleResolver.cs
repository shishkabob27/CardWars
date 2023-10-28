public class FCSpecialDropBattleResolver : BattleResolver
{
	private string specialDropCardId;

	public QuestData questData
	{
		get
		{
			return GameState.Instance.ActiveQuest;
		}
	}

	public int questStars
	{
		get
		{
			return PlayerInfoScript.GetInstance().GetQuestProgress(questData);
		}
	}

	public string questConditionId
	{
		get
		{
			return questData.Condition[questStars];
		}
	}

	public FCSpecialDropBattleResolver(string inSpecialDropCardId)
	{
		specialDropCardId = inSpecialDropCardId;
	}

	public void GetOverrideDropCard(ref bool dropCard, ref CardItem card)
	{
		if (!string.IsNullOrEmpty(specialDropCardId) && QuestEarningManager.GetInstance().earnedCards.Count <= 0)
		{
			CardForm card2 = CardDataManager.Instance.GetCard(specialDropCardId);
			if (card2 != null)
			{
				dropCard = true;
				card = new CardItem(card2);
			}
		}
	}

	public bool SkipRegularLogic()
	{
		return false;
	}

	public void SetResult(PlayerType winner)
	{
	}
}
