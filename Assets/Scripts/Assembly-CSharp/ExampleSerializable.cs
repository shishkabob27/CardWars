using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExampleSerializable : MonoBehaviour
{
	[Serializable]
	public class StringExampleStructTable2 : ScriptableDictionary<string, ExampleStruct>
	{
	}

	[Serializable]
	public class StringExampleStructTable : ScriptableDictionary<string, ExampleStruct>
	{
	}

	[Serializable]
	public class stringIntArrayTable : ScriptableDictionary<string, int[]>
	{
	}

	public Dictionary<string, string> table;

	public StringPairTable stringPairTable = new StringPairTable
	{
		Dic = new Dictionary<string, string>
		{
			{ "a", "1" },
			{ "b", "2" },
			{ "c", "3" }
		}
	};

	public StringObjectTable stringObjectTable;

	public ObjectPairTable objectPairTable;

	public StringExampleStructTable2 stringExampleStructTableTwoLine;

	public StringExampleStructTable stringExampleStructTableOneLine;

	[Tooltip("unity is not support \"jagged array\" serialize")]
	public stringIntArrayTable arrayIsNotSupport;

	private void OnGUI()
	{
		if (GUILayout.Button("Debug.Log : stringPairTable"))
		{
			Dictionary<string, string> dic = stringPairTable.Dic;
			dic.Keys.ToList().ForEach(delegate
			{
			});
			dic.Values.ToList().ForEach(delegate
			{
			});
		}
		if (GUILayout.Button("Clear : stringPairTable"))
		{
			stringPairTable.Dic.Clear();
		}
		if (GUILayout.Button("Add : stringPairTable"))
		{
			stringPairTable.Dic[Time.time.ToString("0")] = Guid.NewGuid().ToString();
		}
	}
}
