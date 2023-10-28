public interface BattleResolver
{
	QuestData questData { get; }

	int questStars { get; }

	string questConditionId { get; }

	bool SkipRegularLogic();

	void GetOverrideDropCard(ref bool dropCard, ref CardItem card);

	void SetResult(PlayerType winner);
}
