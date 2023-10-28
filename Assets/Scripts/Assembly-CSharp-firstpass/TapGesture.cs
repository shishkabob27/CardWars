using System;

[Serializable]
public class TapGesture : DiscreteGesture
{
	private int taps;

	internal bool Down;

	internal bool WasDown;

	internal float LastDownTime;

	internal float LastTapTime;

	public int Taps
	{
		get
		{
			return taps;
		}
		internal set
		{
			taps = value;
		}
	}
}
