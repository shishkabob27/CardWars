using UnityEngine;
using System;

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

	public StringPairTable stringPairTable;
	public StringObjectTable stringObjectTable;
	public ObjectPairTable objectPairTable;
	public StringExampleStructTable2 stringExampleStructTableTwoLine;
	public StringExampleStructTable stringExampleStructTableOneLine;
	public stringIntArrayTable arrayIsNotSupport;
}
