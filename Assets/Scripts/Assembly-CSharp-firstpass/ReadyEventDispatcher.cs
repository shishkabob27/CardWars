using System;

public class ReadyEventDispatcher : EventDispatcher
{
	private bool ready;

	public bool IsReady
	{
		get
		{
			return ready;
		}
	}

	public override void AddListener(Action value)
	{
		if (ready && value != null)
		{
			value();
		}
		else
		{
			base.AddListener(value);
		}
	}

	public override void FireEvent()
	{
		ready = true;
		base.FireEvent();
		ClearListeners();
	}
}
