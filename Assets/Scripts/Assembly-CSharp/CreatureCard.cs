// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CreatureCard
public class CreatureCard : CardForm
{
	public override int BaseATK { get; set; }

	public override int BaseDEF { get; set; }

	public CreatureCard()
	{
		base.Type = CardType.Creature;
	}
}
