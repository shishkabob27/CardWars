using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableDictionary<TKey, TValue>
{
	[SerializeField]
	private List<TKey> _keys;
	[SerializeField]
	private List<TValue> _values;
	[SerializeField]
	private TKey _newKey;
}
