using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public List<GameObject> objectMasterList = new List<GameObject>();

	private Dictionary<string, Stack<GameObject>> objectStore = new Dictionary<string, Stack<GameObject>>();

	public List<PreloadEntry> preload = new List<PreloadEntry>();

	public int stackCount = 1;

	private static PoolManager _instance = null;

	private static readonly char[] cloneChars = new char[2] { '(', '[' };

	private static PoolManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public static GameObject GOInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return (GameObject)SLOTGame.InstantiateFX(prefab, position, rotation);
	}

	public static string GameObjectBaseName(GameObject go)
	{
		string text = go.name;
		return text.Substring(0, text.IndexOfAny(cloneChars));
	}

	public static void GODestroy(GameObject go)
	{
		if (!(go == null))
		{
			Object.Destroy(go);
		}
	}

	public static void GODestroy(GameObject go, float delay)
	{
		Object.Destroy(go, delay);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	public static void TimeStamp(string message, params object[] args)
	{
		message = string.Format(message, args);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	public static void WarnStamp(string message, params object[] args)
	{
		message = string.Format(message, args);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	public static void ErrorStamp(string message, params object[] args)
	{
		message = string.Format(message, args);
	}

	private static string GetPoolName(GameObject prefab)
	{
		return string.Format("{0}[{1}](Pool)", prefab.name, prefab.GetInstanceID());
	}

	public static GameObject Fetch(GameObject prefab)
	{
		string poolName = GetPoolName(prefab);
		Dictionary<string, Stack<GameObject>> dictionary = Instance.objectStore;
		if (!dictionary.ContainsKey(poolName))
		{
			PopulateStore(prefab, Instance.stackCount);
		}
		else if (dictionary[poolName].Count == 0)
		{
			PopulateStore(prefab, Instance.stackCount);
		}
		GameObject gameObject = dictionary[poolName].Pop();
		NGUITools.SetActive(gameObject, true);
		return gameObject;
	}

	public static GameObject Fetch(GameObject prefab, Vector3 position, Quaternion rotation, int maxPool)
	{
		string poolName = GetPoolName(prefab);
		Dictionary<string, Stack<GameObject>> dictionary = Instance.objectStore;
		if (!dictionary.ContainsKey(poolName))
		{
			if (maxPool <= 0)
			{
				maxPool = Instance.stackCount;
			}
			PopulateStore(prefab, maxPool);
		}
		else if (dictionary[poolName].Count == 0)
		{
			PopulateStore(prefab, maxPool);
		}
		GameObject gameObject = dictionary[poolName].Pop();
		Transform transform = gameObject.transform;
		transform.position = position;
		transform.rotation = rotation;
		transform.parent = null;
		NGUITools.SetActive(gameObject, true);
		return gameObject;
	}

	public static GameObject Fetch(string poolName, Vector3 position, Quaternion rotation)
	{
		Dictionary<string, Stack<GameObject>> dictionary = Instance.objectStore;
		if (!dictionary.ContainsKey(poolName))
		{
			return null;
		}
		GameObject gameObject = dictionary[poolName].Pop();
		Transform transform = gameObject.transform;
		transform.position = position;
		transform.rotation = rotation;
		transform.parent = null;
		NGUITools.SetActive(gameObject, true);
		return gameObject;
	}

	public static GameObject Fetch(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return Fetch(prefab, position, rotation, Instance.stackCount);
	}

	public static GameObject FetchChild(GameObject parent, GameObject prefab)
	{
		GameObject gameObject = Fetch(prefab);
		if (gameObject != null && parent != null)
		{
			Transform transform = gameObject.transform;
			transform.parent = parent.transform;
			NGUITools.MarkParentAsChanged(gameObject);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			gameObject.layer = parent.layer;
		}
		return gameObject;
	}

	public static GameObject GOFind(string name)
	{
		return GameObject.Find(name);
	}

	public static void RemoveAll(GameObject prefab)
	{
		if (prefab == null)
		{
			return;
		}
		string poolName = GetPoolName(prefab);
		Dictionary<string, Stack<GameObject>> dictionary = Instance.objectStore;
		if (!dictionary.ContainsKey(poolName))
		{
			return;
		}
		Stack<GameObject> stack = dictionary[poolName];
		foreach (GameObject item in stack)
		{
			Object.Destroy(item);
		}
		stack = null;
		dictionary.Remove(poolName);
	}

	public static void Release(GameObject obj)
	{
		if (!(obj == null))
		{
			string key = obj.name;
			Dictionary<string, Stack<GameObject>> dictionary = Instance.objectStore;
			if (!dictionary.ContainsKey(key))
			{
				Object.Destroy(obj);
				return;
			}
			NGUITools.SetActive(obj, false);
			obj.transform.parent = Instance.transform;
			NGUITools.MarkParentAsChanged(obj);
			dictionary[key].Push(obj);
		}
	}

	public static IEnumerator Release(GameObject obj, float duration)
	{
		yield return new WaitForSeconds(duration);
		Release(obj);
	}

	public static void ReleaseLater(GameObject obj, float duration)
	{
		if (Instance != null)
		{
			Instance.StartCoroutine(Release(obj, duration));
		}
	}

	public static void PopulateStore(GameObject prefab, int count)
	{
		string poolName = GetPoolName(prefab);
		PopulateStore(prefab, poolName, count);
	}

	public static void PopulateStore(GameObject prefab, string poolName, int count)
	{
		Transform parent = Instance.transform;
		Stack<GameObject> value = null;
		Dictionary<string, Stack<GameObject>> dictionary = Instance.objectStore;
		if (!dictionary.TryGetValue(poolName, out value))
		{
			value = new Stack<GameObject>(count);
		}
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = Instantiate(prefab) as GameObject;
			NGUITools.SetActive(gameObject, false);
			gameObject.name = poolName;
			gameObject.transform.parent = parent;
			Instance.objectMasterList.Add(gameObject);
			value.Push(gameObject);
		}
		dictionary[poolName] = value;
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private IEnumerator Start()
	{
		if (preload == null)
		{
			yield break;
		}
		yield return null;
		foreach (PreloadEntry pl in preload)
		{
			if (pl != null && !(pl.prefab == null))
			{
				PopulateStore(pl.prefab, pl.count);
				yield return null;
			}
		}
	}
}
