public class DweebCard : CardForm
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

	public DweebCard()
	{
		base.Type = CardType.Dweeb;
	}
}
