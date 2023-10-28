using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> _keys = new List<TKey>();

	[SerializeField]
	private List<TValue> _values = new List<TValue>();

	private Dictionary<TKey, TValue> _dic;

	[SerializeField]
	private TKey _newKey;

	public Dictionary<TKey, TValue> Dic
	{
		get
		{
			if (_dic == null)
			{
				_dic = new Dictionary<TKey, TValue>();
			}
			return _dic;
		}
		set
		{
			_dic = value;
		}
	}

	public void OnBeforeSerialize()
	{
		Save();
	}

	public void OnAfterDeserialize()
	{
		Load();
	}

	public void Save()
	{
		if (_dic == null)
		{
			return;
		}
		_keys = new List<TKey>();
		_values = new List<TValue>();
		foreach (KeyValuePair<TKey, TValue> item in _dic)
		{
			_keys.Add(item.Key);
			_values.Add(item.Value);
		}
	}

	public void Load()
	{
		_dic = new Dictionary<TKey, TValue>();
		for (int i = 0; i < _keys.Count; i++)
		{
			if (_keys[i] != null)
			{
				if (_dic.ContainsKey(_keys[i]))
				{
				}
				TValue value = default(TValue);
				if (_values.Count > i)
				{
					value = _values[i];
				}
				_dic[_keys[i]] = value;
			}
		}
	}
}
