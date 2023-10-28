using System;

public class CreatureScript : CardScript
{
	public int Damage { get; set; }

	public int DamageLastTurn { get; set; }

	public int ATKMod { get; set; }

	public int DEFMod { get; set; }

	public bool MarkedForDeath { get; set; }

	public int DamageReduction { get; set; }

	public float DamageFactor { get; set; }

	public float HealingFactor { get; set; }

	public float ATKFactor { get; set; }

	public float DEFFactor { get; set; }

	public int ATK
	{
		get
		{
			int num = (int)((float)(base.Data.ATK + ATKMod) * ATKFactor);
			if (num < 0)
			{
				ATKMod = -base.Data.ATK;
				num = 0;
			}
			return num;
		}
	}

	public int DEF
	{
		get
		{
			return (int)((float)(base.Data.DEF + DEFMod) * DEFFactor);
		}
	}

	public int Health
	{
		get
		{
			int num = DEF - Damage;
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
	}

	public bool InDanger
	{
		get
		{
			if (Enemy != null && Enemy.ATK > Health && ATK < Enemy.Health)
			{
				return true;
			}
			return false;
		}
	}

	public CreatureScript Enemy
	{
		get
		{
			return base.CurrentLane.OpponentLane.GetCreature();
		}
	}

	public bool CanWin
	{
		get
		{
			if (Enemy == null && ATK >= base.GameInstance.GetHealth(!base.Owner))
			{
				return true;
			}
			return false;
		}
	}

	public CreatureScript()
	{
		DamageFactor = 1f;
		HealingFactor = 1f;
		ATKFactor = 1f;
		DEFFactor = 1f;
		DamageLastTurn = 0;
	}

	public new static int EvaluateLanePlacement(PlayerType player, Lane lane, CardItem item)
	{
		return GameState.Instance.ScoreBoard(player, item, lane);
	}

	public float GetHealthPct()
	{
		return (float)Health / (float)DEF;
	}

	public int GetDamageFactor()
	{
		return Damage / base.Data.Level * AIManager.WEIGHT_SCALE / AIManager.DEF_FACTOR;
	}

	public virtual void Heal(int amount)
	{
		amount = (int)Math.Round((float)amount * HealingFactor);
		Damage -= amount;
		if (Damage < 0)
		{
			Damage = 0;
		}
	}

	public virtual void TakeDamage(CardScript Source, int amount)
	{
		int num = amount - DamageReduction;
		num = (int)((float)num * DamageFactor);
		if (num < 0)
		{
			num = 0;
		}
		Damage += num;
		DamageLastTurn = num;
		if (Health <= 0 && Source is CreatureScript && Source.CurrentLane.HasBuilding())
		{
			BuildingScript building = Source.CurrentLane.GetBuilding();
			building.OnCreatureWon();
		}
	}
}
