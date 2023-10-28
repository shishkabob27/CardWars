using System;
using UnityEngine;

namespace Prime31
{
	public static class ActionExtensions
	{
		private static void invoke(Delegate listener, object[] args)
		{
			if (!listener.Method.IsStatic && (listener.Target == null || listener.Target.Equals(null)))
			{
				Debug.LogError("an event listener is still subscribed to an event with the method " + listener.Method.Name + " even though it is null. Be sure to balance your event subscriptions.");
			}
			else
			{
				listener.Method.Invoke(listener.Target, args);
			}
		}

		public static void fire(this Action handler)
		{
			if (handler != null)
			{
				object[] args = new object[0];
				Delegate[] invocationList = handler.GetInvocationList();
				foreach (Delegate listener in invocationList)
				{
					invoke(listener, args);
				}
			}
		}

		public static void fire<T>(this Action<T> handler, T param)
		{
			if (handler != null)
			{
				object[] args = new object[1] { param };
				Delegate[] invocationList = handler.GetInvocationList();
				foreach (Delegate listener in invocationList)
				{
					invoke(listener, args);
				}
			}
		}

		public static void fire<T, U>(this Action<T, U> handler, T param1, U param2)
		{
			if (handler != null)
			{
				object[] args = new object[2] { param1, param2 };
				Delegate[] invocationList = handler.GetInvocationList();
				foreach (Delegate listener in invocationList)
				{
					invoke(listener, args);
				}
			}
		}

		public static void fire<T, U, V>(this Action<T, U, V> handler, T param1, U param2, V param3)
		{
			if (handler != null)
			{
				object[] args = new object[3] { param1, param2, param3 };
				Delegate[] invocationList = handler.GetInvocationList();
				foreach (Delegate listener in invocationList)
				{
					invoke(listener, args);
				}
			}
		}
	}
}
