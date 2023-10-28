using System.Collections.Generic;

public class AndroidUiStack
{
	private static AndroidUiStack instance;

	private List<IAndroidBackActivator> activators;

	public static AndroidUiStack Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AndroidUiStack();
			}
			return instance;
		}
	}

	private AndroidUiStack()
	{
		activators = new List<IAndroidBackActivator>();
	}

	public void ActivateEscape()
	{
		List<IAndroidBackActivator> list;
		lock (activators)
		{
			list = new List<IAndroidBackActivator>(activators);
		}
		int num = list.Count - 1;
		while (num >= 0 && !list[num].TryActivate())
		{
			num--;
		}
	}

	public void Add(IAndroidBackActivator activator)
	{
		lock (activators)
		{
			activators.Add(activator);
		}
	}

	public void Remove(IAndroidBackActivator activator)
	{
		lock (activators)
		{
			activators.Remove(activator);
		}
	}
}
