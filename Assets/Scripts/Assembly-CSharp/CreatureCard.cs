public class CreatureCard : CardForm
{
	public override int BaseATK { get; set; }

	public override int BaseDEF { get; set; }

	public CreatureCard()
	{
		base.Type = CardType.Creature;
	}
}
