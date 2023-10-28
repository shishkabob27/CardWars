using System;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGameObject : MonoBehaviour
{
	private static Dictionary<string, WeakReference> singletons = new Dictionary<string, WeakReference>();

	private static T GetInstance<T>() where T : MonoBehaviour
	{
		try
		{
			return (T)singletons[typeof(T).ToString()].Target;
		}
		catch (KeyNotFoundException)
		{
			return (T)null;
		}
	}

	public virtual void Awake()
	{
		string key = GetType().ToString();
		try
		{
			WeakReference weakReference = singletons[key];
			if (weakReference.Target == null)
			{
				weakReference.Target = this;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		catch (KeyNotFoundException)
		{
			singletons.Add(key, new WeakReference(this));
		}
	}

	protected virtual void OnDestroy()
	{
		string key = GetType().ToString();
		try
		{
			WeakReference weakReference = singletons[key];
			if (weakReference.Target == this)
			{
				weakReference.Target = null;
			}
		}
		catch (KeyNotFoundException)
		{
		}
	}
}
