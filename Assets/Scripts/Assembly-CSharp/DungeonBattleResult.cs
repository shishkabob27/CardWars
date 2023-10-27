public class DungeonBattleResult : BattleResult
{
	public DungeonBattleResult() : base(default(BattleResult.Menu), default(bool), default(bool))
	{
	}

	public string DungeonID;
	public int QuestIndex;
}
