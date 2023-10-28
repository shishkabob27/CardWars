// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DweebCard
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
