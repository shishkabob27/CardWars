public class AIDecision
{
	public bool IsFloop { get; set; }

	public bool IsLeaderAbility { get; set; }

	public CardItem CardChoice { get; set; }

	public Lane LaneChoice { get; set; }

	public AIDecision()
	{
		IsFloop = false;
		IsLeaderAbility = false;
	}
}
