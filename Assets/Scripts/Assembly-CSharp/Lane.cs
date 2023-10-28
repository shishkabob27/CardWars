public class Lane
{
	public const int MAX_ITEMS = 2;

	public const int LANE_SIDES = 2;

	public CardScript[] Scripts = new CardScript[2];

	public Lane[] AdjacentLanes = new Lane[2];

	public Lane OpponentLane { get; set; }

	public LandscapeType Type { get; set; }

	public bool Flipped { get; set; }

	public int Index { get; set; }

	public bool AbilitiesBlocked { get; set; }

	public bool Disabled { get; set; }

	public int FloopMod { get; set; }

	public int RarityGate { get; set; }

	public bool IsOuterLane
	{
		get
		{
			if (AdjacentLanes[0] == null || AdjacentLanes[1] == null)
			{
				return true;
			}
			return false;
		}
	}

	public Lane()
	{
		Type = LandscapeType.None;
		RarityGate = int.MaxValue;
	}

	public bool HasCreature()
	{
		return Scripts[0] != null;
	}

	public bool HasBuilding()
	{
		return Scripts[1] != null;
	}

	public bool HasCard(CardType type)
	{
		return Scripts[(int)type] != null;
	}

	public bool IsEmpty()
	{
		return !HasCreature();
	}

	public CreatureScript GetCreature()
	{
		return (CreatureScript)Scripts[0];
	}

	public BuildingScript GetBuilding()
	{
		return (BuildingScript)Scripts[1];
	}

	public CardScript GetScript(CardType type)
	{
		return Scripts[(int)type];
	}

	public bool IsCreatureFresh()
	{
		if (HasCreature())
		{
			return GetCreature().Fresh;
		}
		return false;
	}

	public Lane GetInnerLane()
	{
		if (AdjacentLanes[0] == null)
		{
			return AdjacentLanes[1];
		}
		if (AdjacentLanes[1] == null)
		{
			return AdjacentLanes[0];
		}
		return this;
	}

	public bool IsAdjacentTo(Lane CurrentLane)
	{
		Lane[] adjacentLanes = AdjacentLanes;
		foreach (Lane lane in adjacentLanes)
		{
			if (lane == CurrentLane)
			{
				return true;
			}
		}
		return false;
	}
}
