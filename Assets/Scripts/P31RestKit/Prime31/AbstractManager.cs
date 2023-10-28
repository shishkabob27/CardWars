using System;
using UnityEngine;

namespace Prime31
{
	public abstract class AbstractManager : MonoBehaviour
	{
		private static LifecycleHelper _prime31LifecycleHelperRef;

		private static ThreadingCallbackHelper _threadingCallbackHelper;

		private static GameObject _prime31GameObject;

		public static LifecycleHelper coroutineSurrogate
		{
			get
			{
				if (_prime31LifecycleHelperRef == null)
				{
					GameObject prime31ManagerGameObject = getPrime31ManagerGameObject();
					_prime31LifecycleHelperRef = prime31ManagerGameObject.AddComponent<LifecycleHelper>();
				}
				return _prime31LifecycleHelperRef;
			}
		}

		public static LifecycleHelper lifecycleHelper
		{
			get
			{
				return coroutineSurrogate;
			}
		}

		public static ThreadingCallbackHelper getThreadingCallbackHelper()
		{
			return _threadingCallbackHelper;
		}

		public static void createThreadingCallbackHelper()
		{
			if (!(_threadingCallbackHelper != null))
			{
				_threadingCallbackHelper = UnityEngine.Object.FindObjectOfType(typeof(ThreadingCallbackHelper)) as ThreadingCallbackHelper;
				if (!(_threadingCallbackHelper != null))
				{
					GameObject prime31ManagerGameObject = getPrime31ManagerGameObject();
					_threadingCallbackHelper = prime31ManagerGameObject.AddComponent<ThreadingCallbackHelper>();
				}
			}
		}

		public static GameObject getPrime31ManagerGameObject()
		{
			if (_prime31GameObject != null)
			{
				return _prime31GameObject;
			}
			_prime31GameObject = GameObject.Find("prime[31]");
			if (_prime31GameObject == null)
			{
				_prime31GameObject = new GameObject("prime[31]");
				UnityEngine.Object.DontDestroyOnLoad(_prime31GameObject);
			}
			return _prime31GameObject;
		}

		public static void initialize(Type type)
		{
			try
			{
				MonoBehaviour monoBehaviour = UnityEngine.Object.FindObjectOfType(type) as MonoBehaviour;
				if (!(monoBehaviour != null))
				{
					GameObject prime31ManagerGameObject = getPrime31ManagerGameObject();
					GameObject gameObject = new GameObject(type.Name);
					gameObject.AddComponent(type);
					gameObject.transform.parent = prime31ManagerGameObject.transform;
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
			catch (UnityException)
			{
				Debug.LogWarning(string.Concat("It looks like you have the ", type, " on a GameObject in your scene. Our new prefab-less manager system does not require the ", type, " to be on a GameObject.\nIt will be added to your scene at runtime automatically for you. Please remove the script from your scene."));
			}
		}

		private void Awake()
		{
			base.gameObject.name = GetType().Name;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
	}
}
