using System;
using System.Runtime.CompilerServices;

public class EventDispatcher<T, U>
{
	[method: MethodImpl(32)]
	private event Action<T, U> eventListener;

	public Delegate[] GetInvocationList()
	{
		return this.eventListener.GetInvocationList();
	}

	public void AddListener(Action<T, U> value)
	{
		if (value == null)
		{
			return;
		}
		if (this.eventListener == null)
		{
			this.eventListener = (Action<T, U>)Delegate.Combine(this.eventListener, value);
			return;
		}
		Delegate[] invocationList = this.eventListener.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (invocationList[i] == value)
			{
				return;
			}
		}
		this.eventListener = (Action<T, U>)Delegate.Combine(this.eventListener, value);
	}

	public void RemoveListener(Action<T, U> value)
	{
		if (this.eventListener != null)
		{
			this.eventListener = (Action<T, U>)Delegate.Remove(this.eventListener, value);
		}
	}

	public void ClearListeners()
	{
		this.eventListener = null;
	}

	public void FireEvent(T arg1, U arg2)
	{
		if (this.eventListener != null)
		{
			this.eventListener(arg1, arg2);
		}
	}
}
public class EventDispatcher<T>
{
	[method: MethodImpl(32)]
	private event Action<T> eventListener;

	public void SetListener(Action<T> value)
	{
		this.eventListener = value;
	}

	public Action<T> GetListener()
	{
		return this.eventListener;
	}

	public void AddListener(Action<T> value)
	{
		if (value == null)
		{
			return;
		}
		if (this.eventListener == null)
		{
			this.eventListener = (Action<T>)Delegate.Combine(this.eventListener, value);
			return;
		}
		Delegate[] invocationList = this.eventListener.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (invocationList[i] == value)
			{
				return;
			}
		}
		this.eventListener = (Action<T>)Delegate.Combine(this.eventListener, value);
	}

	public void RemoveListener(Action<T> value)
	{
		if (this.eventListener != null)
		{
			this.eventListener = (Action<T>)Delegate.Remove(this.eventListener, value);
		}
	}

	public void ClearListeners()
	{
		this.eventListener = null;
	}

	public void FireEvent(T message)
	{
		if (this.eventListener != null)
		{
			this.eventListener(message);
		}
	}
}
public class EventDispatcher
{
	public bool HasListeners
	{
		get
		{
			return this.eventListener != null;
		}
	}

	[method: MethodImpl(32)]
	private event Action eventListener;

	public virtual void AddListener(Action value)
	{
		if (value == null)
		{
			return;
		}
		if (this.eventListener == null)
		{
			this.eventListener = (Action)Delegate.Combine(this.eventListener, value);
			return;
		}
		Delegate[] invocationList = this.eventListener.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (invocationList[i] == value)
			{
				return;
			}
		}
		this.eventListener = (Action)Delegate.Combine(this.eventListener, value);
	}

	public virtual void RemoveListener(Action value)
	{
		if (this.eventListener != null)
		{
			this.eventListener = (Action)Delegate.Remove(this.eventListener, value);
		}
	}

	public void ClearListeners()
	{
		this.eventListener = null;
	}

	public virtual void FireEvent()
	{
		if (this.eventListener != null)
		{
			this.eventListener();
		}
	}
}
