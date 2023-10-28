public class LeaderScript : SpellScript
{
	public LeaderItem Leader { get; set; }

	public LeaderScript()
	{
		base.GameInstance = GameState.Instance;
		base.Data = new CardItem(new SpellCard());
		base.Data.Form.Faction = Faction.Universal;
		base.Data.Form.Cost = 0;
	}

	public new static bool CanPlay(PlayerType player, int lane, CardForm card)
	{
		if (GameState.Instance.GetLeaderCooldown(player) <= 0)
		{
			return true;
		}
		return false;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane Candidate, CardItem item)
	{
		return GameState.Instance.ScoreBoard();
	}
}
