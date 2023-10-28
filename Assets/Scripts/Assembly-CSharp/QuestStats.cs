using System;

public class QuestStats
{
	public int[] UsedTypes;

	public int NumTurns;

	public int[] UsedFactions;

	public int NumCreaturesDefeated;

	public int NumActionPointsPerCard;

	public int NumActionPointsTotal;

	public int NumFloopsUsed;

	public int HPLost;

	public int NumCreaturesLost;

	public int NumLandscapesUsed;

	public int DeckCost;

	public QuestStats()
	{
		UsedTypes = new int[Enum.GetNames(typeof(CardType)).Length];
		UsedFactions = new int[Enum.GetNames(typeof(Faction)).Length];
	}

	public void IncrementTypeUsed(CardType typ)
	{
		UsedTypes[(int)typ]++;
	}

	public void IncrementFactionUsed(Faction fac)
	{
		UsedFactions[(int)fac]++;
	}

	public void AddActionPoints(int points)
	{
		if (points > NumActionPointsPerCard)
		{
			NumActionPointsPerCard = points;
		}
		NumActionPointsTotal += points;
	}
}
