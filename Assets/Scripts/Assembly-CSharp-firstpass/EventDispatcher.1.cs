// Assembly-CSharp-firstpass, Version=1.4.1003.3007, Culture=neutral, PublicKeyToken=null
// EventDispatcher<T>
using System;
using System.Runtime.CompilerServices;

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
