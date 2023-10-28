public class DungeonBattleResult : BattleResult
{
	public string DungeonID;

	public int QuestIndex;

	public DungeonBattleResult()
		: base(Menu.DungeonSelect)
	{
	}
}
