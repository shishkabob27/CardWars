public class ElFistoBattleResolver : BattleResolver
{
	private ElFistoController efc;

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

	public ElFistoBattleResolver(ElFistoController elFistoController)
	{
		efc = elFistoController;
	}

	public void GetOverrideDropCard(ref bool dropCard, ref CardItem card)
	{
	}

	public bool SkipRegularLogic()
	{
		return false;
	}

	public void SetResult(PlayerType winner)
	{
		GlobalFlags.Instance.BattleResult = new BattleResult(BattleResult.Menu.MapMain);
	}
}
