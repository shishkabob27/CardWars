// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// SpellCard
public class SpellCard : CardForm
{
	public override int BaseATK
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public override int BaseDEF
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public string ParticleName { get; set; }

	public SpellCard()
	{
		base.Type = CardType.Spell;
	}
}
