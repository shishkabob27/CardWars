using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class iTweenEvent : MonoBehaviour
{
	public enum TweenType
	{
		AudioFrom,
		AudioTo,
		AudioUpdate,
		CameraFadeFrom,
		CameraFadeTo,
		ColorFrom,
		ColorTo,
		ColorUpdate,
		FadeFrom,
		FadeTo,
		FadeUpdate,
		LookFrom,
		LookTo,
		LookUpdate,
		MoveAdd,
		MoveBy,
		MoveFrom,
		MoveTo,
		MoveUpdate,
		PunchPosition,
		PunchRotation,
		PunchScale,
		RotateAdd,
		RotateBy,
		RotateFrom,
		RotateTo,
		RotateUpdate,
		ScaleAdd,
		ScaleBy,
		ScaleFrom,
		ScaleTo,
		ScaleUpdate,
		ShakePosition,
		ShakeRotation,
		ShakeScale,
		Stab
	}

	public const string VERSION = "0.6.1";

	public string tweenName = string.Empty;

	public bool playAutomatically = true;

	public float delay;

	public TweenType type = TweenType.MoveTo;

	public bool showIconInInspector = true;

	[SerializeField]
	private string[] keys;

	[SerializeField]
	private int[] indexes;

	[SerializeField]
	private string[] metadatas;

	[SerializeField]
	private int[] ints;

	[SerializeField]
	private float[] floats;

	[SerializeField]
	private bool[] bools;

	[SerializeField]
	private string[] strings;

	[SerializeField]
	private Vector3[] vector3s;

	[SerializeField]
	private Color[] colors;

	[SerializeField]
	private Space[] spaces;

	[SerializeField]
	private iTween.EaseType[] easeTypes;

	[SerializeField]
	private iTween.LoopType[] loopTypes;

	[SerializeField]
	private GameObject[] gameObjects;

	[SerializeField]
	private Transform[] transforms;

	[SerializeField]
	private AudioClip[] audioClips;

	[SerializeField]
	private AudioSource[] audioSources;

	[SerializeField]
	private ArrayIndexes[] vector3Arrays;

	[SerializeField]
	private ArrayIndexes[] transformArrays;

	[SerializeField]
	private iTweenPath[] paths;

	private Dictionary<string, object> values;

	private bool stopped;

	private iTween instantiatedTween;

	private string internalName;

	public Dictionary<string, object> Values
	{
		get
		{
			if (values == null)
			{
				DeserializeValues();
			}
			return values;
		}
		set
		{
			values = value;
			SerializeValues();
		}
	}

	public static iTweenEvent GetEvent(GameObject obj, string name)
	{
		iTweenEvent[] components = obj.GetComponents<iTweenEvent>();
		if (components.Length > 0)
		{
			iTweenEvent iTweenEvent2 = components.FirstOrDefault((iTweenEvent tween) => tween.tweenName == name);
			if (iTweenEvent2 != null)
			{
				return iTweenEvent2;
			}
		}
		return null;
	}

	public void Start()
	{
		if (playAutomatically)
		{
			Play();
		}
	}

	public void Play()
	{
		if (!string.IsNullOrEmpty(internalName))
		{
			Stop();
		}
		stopped = false;
		StartCoroutine(StartEvent());
	}

	public void Stop()
	{
		iTween.StopByName(base.gameObject, internalName);
		internalName = string.Empty;
		stopped = true;
	}

	public void OnDrawGizmos()
	{
		if (showIconInInspector)
		{
			Gizmos.DrawIcon(base.transform.position, "iTweenIcon.tif");
		}
	}

	private IEnumerator StartEvent()
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (stopped)
		{
			yield return null;
		}
		Hashtable optionsHash = new Hashtable();
		foreach (KeyValuePair<string, object> pair in Values)
		{
			if ("path" == pair.Key && pair.Value.GetType() == typeof(string))
			{
				optionsHash.Add(pair.Key, iTweenPath.GetPath((string)pair.Value));
			}
			else
			{
				optionsHash.Add(pair.Key, pair.Value);
			}
		}
		internalName = ((!string.IsNullOrEmpty(tweenName)) ? tweenName : string.Empty);
		internalName = string.Format("{0}-{1}", internalName, Guid.NewGuid().ToString());
		optionsHash.Add("name", internalName);
		switch (type)
		{
		case TweenType.AudioFrom:
			iTween.AudioFrom(base.gameObject, optionsHash);
			break;
		case TweenType.AudioTo:
			iTween.AudioTo(base.gameObject, optionsHash);
			break;
		case TweenType.AudioUpdate:
			iTween.AudioUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.CameraFadeFrom:
			iTween.CameraFadeFrom(optionsHash);
			break;
		case TweenType.CameraFadeTo:
			iTween.CameraFadeTo(optionsHash);
			break;
		case TweenType.ColorFrom:
			iTween.ColorFrom(base.gameObject, optionsHash);
			break;
		case TweenType.ColorTo:
			iTween.ColorTo(base.gameObject, optionsHash);
			break;
		case TweenType.ColorUpdate:
			iTween.ColorUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.FadeFrom:
			iTween.FadeFrom(base.gameObject, optionsHash);
			break;
		case TweenType.FadeTo:
			iTween.FadeTo(base.gameObject, optionsHash);
			break;
		case TweenType.FadeUpdate:
			iTween.FadeUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.LookFrom:
			iTween.LookFrom(base.gameObject, optionsHash);
			break;
		case TweenType.LookTo:
			iTween.LookTo(base.gameObject, optionsHash);
			break;
		case TweenType.LookUpdate:
			iTween.LookUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.MoveAdd:
			iTween.MoveAdd(base.gameObject, optionsHash);
			break;
		case TweenType.MoveBy:
			iTween.MoveBy(base.gameObject, optionsHash);
			break;
		case TweenType.MoveFrom:
			iTween.MoveFrom(base.gameObject, optionsHash);
			break;
		case TweenType.MoveTo:
			iTween.MoveTo(base.gameObject, optionsHash);
			break;
		case TweenType.MoveUpdate:
			iTween.MoveUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.PunchPosition:
			iTween.PunchPosition(base.gameObject, optionsHash);
			break;
		case TweenType.PunchRotation:
			iTween.PunchRotation(base.gameObject, optionsHash);
			break;
		case TweenType.PunchScale:
			iTween.PunchScale(base.gameObject, optionsHash);
			break;
		case TweenType.RotateAdd:
			iTween.RotateAdd(base.gameObject, optionsHash);
			break;
		case TweenType.RotateBy:
			iTween.RotateBy(base.gameObject, optionsHash);
			break;
		case TweenType.RotateFrom:
			iTween.RotateFrom(base.gameObject, optionsHash);
			break;
		case TweenType.RotateTo:
			iTween.RotateTo(base.gameObject, optionsHash);
			break;
		case TweenType.RotateUpdate:
			iTween.RotateUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.ScaleAdd:
			iTween.ScaleAdd(base.gameObject, optionsHash);
			break;
		case TweenType.ScaleBy:
			iTween.ScaleBy(base.gameObject, optionsHash);
			break;
		case TweenType.ScaleFrom:
			iTween.ScaleFrom(base.gameObject, optionsHash);
			break;
		case TweenType.ScaleTo:
			iTween.ScaleTo(base.gameObject, optionsHash);
			break;
		case TweenType.ScaleUpdate:
			iTween.ScaleUpdate(base.gameObject, optionsHash);
			break;
		case TweenType.ShakePosition:
			iTween.ShakePosition(base.gameObject, optionsHash);
			break;
		case TweenType.ShakeRotation:
			iTween.ShakeRotation(base.gameObject, optionsHash);
			break;
		case TweenType.ShakeScale:
			iTween.ShakeScale(base.gameObject, optionsHash);
			break;
		case TweenType.Stab:
			iTween.Stab(base.gameObject, optionsHash);
			break;
		default:
			throw new ArgumentException("Invalid tween type: " + type);
		}
	}

	private void SerializeValues()
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		List<string> list3 = new List<string>();
		List<int> list4 = new List<int>();
		List<float> list5 = new List<float>();
		List<bool> list6 = new List<bool>();
		List<string> list7 = new List<string>();
		List<Vector3> list8 = new List<Vector3>();
		List<Color> list9 = new List<Color>();
		List<Space> list10 = new List<Space>();
		List<iTween.EaseType> list11 = new List<iTween.EaseType>();
		List<iTween.LoopType> list12 = new List<iTween.LoopType>();
		List<GameObject> list13 = new List<GameObject>();
		List<Transform> list14 = new List<Transform>();
		List<AudioClip> list15 = new List<AudioClip>();
		List<AudioSource> list16 = new List<AudioSource>();
		List<ArrayIndexes> list17 = new List<ArrayIndexes>();
		List<ArrayIndexes> list18 = new List<ArrayIndexes>();
		foreach (KeyValuePair<string, object> value in values)
		{
			Dictionary<string, Type> dictionary = EventParamMappings.mappings[this.type];
			Type type = dictionary[value.Key];
			if (typeof(int) == type)
			{
				AddToList(list, list2, list4, list3, value);
			}
			if (typeof(float) == type)
			{
				AddToList(list, list2, list5, list3, value);
			}
			else if (typeof(bool) == type)
			{
				AddToList(list, list2, list6, list3, value);
			}
			else if (typeof(string) == type)
			{
				AddToList(list, list2, list7, list3, value);
			}
			else if (typeof(Vector3) == type)
			{
				AddToList(list, list2, list8, list3, value);
			}
			else if (typeof(Color) == type)
			{
				AddToList(list, list2, list9, list3, value);
			}
			else if (typeof(Space) == type)
			{
				AddToList(list, list2, list10, list3, value);
			}
			else if (typeof(iTween.EaseType) == type)
			{
				AddToList(list, list2, list11, list3, value);
			}
			else if (typeof(iTween.LoopType) == type)
			{
				AddToList(list, list2, list12, list3, value);
			}
			else if (typeof(GameObject) == type)
			{
				AddToList(list, list2, list13, list3, value);
			}
			else if (typeof(Transform) == type)
			{
				AddToList(list, list2, list14, list3, value);
			}
			else if (typeof(AudioClip) == type)
			{
				AddToList(list, list2, list15, list3, value);
			}
			else if (typeof(AudioSource) == type)
			{
				AddToList(list, list2, list16, list3, value);
			}
			else if (typeof(Vector3OrTransform) == type)
			{
				if (value.Value == null || typeof(Transform) == value.Value.GetType())
				{
					AddToList(list, list2, list14, list3, value.Key, value.Value, "t");
				}
				else
				{
					AddToList(list, list2, list8, list3, value.Key, value.Value, "v");
				}
			}
			else
			{
				if (typeof(Vector3OrTransformArray) != type)
				{
					continue;
				}
				if (typeof(Vector3[]) == value.Value.GetType())
				{
					Vector3[] array = (Vector3[])value.Value;
					ArrayIndexes arrayIndexes = new ArrayIndexes();
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						list8.Add(array[i]);
						array2[i] = list8.Count - 1;
					}
					arrayIndexes.indexes = array2;
					AddToList(list, list2, list17, list3, value.Key, arrayIndexes, "v");
				}
				else if (typeof(Transform[]) == value.Value.GetType())
				{
					Transform[] array3 = (Transform[])value.Value;
					ArrayIndexes arrayIndexes2 = new ArrayIndexes();
					int[] array4 = new int[array3.Length];
					for (int j = 0; j < array3.Length; j++)
					{
						list14.Add(array3[j]);
						array4[j] = list14.Count - 1;
					}
					arrayIndexes2.indexes = array4;
					AddToList(list, list2, list18, list3, value.Key, arrayIndexes2, "t");
				}
				else if (typeof(string) == value.Value.GetType())
				{
					AddToList(list, list2, list7, list3, value.Key, value.Value, "p");
				}
			}
		}
		keys = list.ToArray();
		indexes = list2.ToArray();
		metadatas = list3.ToArray();
		ints = list4.ToArray();
		floats = list5.ToArray();
		bools = list6.ToArray();
		strings = list7.ToArray();
		vector3s = list8.ToArray();
		colors = list9.ToArray();
		spaces = list10.ToArray();
		easeTypes = list11.ToArray();
		loopTypes = list12.ToArray();
		gameObjects = list13.ToArray();
		transforms = list14.ToArray();
		audioClips = list15.ToArray();
		audioSources = list16.ToArray();
		vector3Arrays = list17.ToArray();
		transformArrays = list18.ToArray();
	}

	private void AddToList<T>(List<string> keyList, List<int> indexList, IList<T> valueList, List<string> metadataList, KeyValuePair<string, object> pair)
	{
		AddToList(keyList, indexList, valueList, metadataList, pair.Key, pair.Value);
	}

	private void AddToList<T>(List<string> keyList, List<int> indexList, IList<T> valueList, List<string> metadataList, KeyValuePair<string, object> pair, string metadata)
	{
		AddToList(keyList, indexList, valueList, metadataList, pair.Key, pair.Value, metadata);
	}

	private void AddToList<T>(List<string> keyList, List<int> indexList, IList<T> valueList, List<string> metadataList, string key, object value)
	{
		AddToList(keyList, indexList, valueList, metadataList, key, value, null);
	}

	private void AddToList<T>(List<string> keyList, List<int> indexList, IList<T> valueList, List<string> metadataList, string key, object value, string metadata)
	{
		keyList.Add(key);
		valueList.Add((T)value);
		indexList.Add(valueList.Count - 1);
		metadataList.Add(metadata);
	}

	private void DeserializeValues()
	{
		values = new Dictionary<string, object>();
		if (keys == null)
		{
			return;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			Dictionary<string, Type> dictionary = EventParamMappings.mappings[this.type];
			Type type = dictionary[keys[i]];
			if (typeof(int) == type)
			{
				values.Add(keys[i], ints[indexes[i]]);
			}
			else if (typeof(float) == type)
			{
				values.Add(keys[i], floats[indexes[i]]);
			}
			else if (typeof(bool) == type)
			{
				values.Add(keys[i], bools[indexes[i]]);
			}
			else if (typeof(string) == type)
			{
				values.Add(keys[i], strings[indexes[i]]);
			}
			else if (typeof(Vector3) == type)
			{
				values.Add(keys[i], vector3s[indexes[i]]);
			}
			else if (typeof(Color) == type)
			{
				values.Add(keys[i], colors[indexes[i]]);
			}
			else if (typeof(Space) == type)
			{
				values.Add(keys[i], spaces[indexes[i]]);
			}
			else if (typeof(iTween.EaseType) == type)
			{
				values.Add(keys[i], easeTypes[indexes[i]]);
			}
			else if (typeof(iTween.LoopType) == type)
			{
				values.Add(keys[i], loopTypes[indexes[i]]);
			}
			else if (typeof(GameObject) == type)
			{
				values.Add(keys[i], gameObjects[indexes[i]]);
			}
			else if (typeof(Transform) == type)
			{
				values.Add(keys[i], transforms[indexes[i]]);
			}
			else if (typeof(AudioClip) == type)
			{
				values.Add(keys[i], audioClips[indexes[i]]);
			}
			else if (typeof(AudioSource) == type)
			{
				values.Add(keys[i], audioSources[indexes[i]]);
			}
			else if (typeof(Vector3OrTransform) == type)
			{
				if ("v" == metadatas[i])
				{
					values.Add(keys[i], vector3s[indexes[i]]);
				}
				else if ("t" == metadatas[i])
				{
					values.Add(keys[i], transforms[indexes[i]]);
				}
			}
			else
			{
				if (typeof(Vector3OrTransformArray) != type)
				{
					continue;
				}
				if ("v" == metadatas[i])
				{
					ArrayIndexes arrayIndexes = vector3Arrays[indexes[i]];
					Vector3[] array = new Vector3[arrayIndexes.indexes.Length];
					for (int j = 0; j < arrayIndexes.indexes.Length; j++)
					{
						array[j] = vector3s[arrayIndexes.indexes[j]];
					}
					values.Add(keys[i], array);
				}
				else if ("t" == metadatas[i])
				{
					ArrayIndexes arrayIndexes2 = transformArrays[indexes[i]];
					Transform[] array2 = new Transform[arrayIndexes2.indexes.Length];
					for (int k = 0; k < arrayIndexes2.indexes.Length; k++)
					{
						array2[k] = transforms[arrayIndexes2.indexes[k]];
					}
					values.Add(keys[i], array2);
				}
				else if ("p" == metadatas[i])
				{
					values.Add(keys[i], strings[indexes[i]]);
				}
			}
		}
	}
}
