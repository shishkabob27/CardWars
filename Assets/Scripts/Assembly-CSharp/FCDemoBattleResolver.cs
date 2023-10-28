public class FCDemoBattleResolver : BattleResolver
{
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

	public void GetOverrideDropCard(ref bool dropCard, ref CardItem card)
	{
	}

	public bool SkipRegularLogic()
	{
		return true;
	}

	public void SetResult(PlayerType winner)
	{
		GlobalFlags.Instance.BattleResult = new BattleResult();
	}
}
