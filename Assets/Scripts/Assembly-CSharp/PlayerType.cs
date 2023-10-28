public class PlayerType
{
	private static PlayerType user;

	private static PlayerType opponent;

	private int iValue;

	public static PlayerType User
	{
		get
		{
			if (user == null)
			{
				user = new PlayerType(0);
			}
			return user;
		}
	}

	public static PlayerType Opponent
	{
		get
		{
			if (opponent == null)
			{
				opponent = new PlayerType(1);
			}
			return opponent;
		}
	}

	public int IntValue
	{
		get
		{
			return iValue;
		}
	}

	public PlayerType(int value)
	{
		iValue = value;
	}

	public static implicit operator int(PlayerType pt)
	{
		return pt.IntValue;
	}

	public static implicit operator PlayerType(int ivalue)
	{
		return (ivalue != 0) ? Opponent : User;
	}

	public static PlayerType operator !(PlayerType pt)
	{
		return ((int)pt != 0) ? User : Opponent;
	}
}
