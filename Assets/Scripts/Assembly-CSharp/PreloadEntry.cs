using System;
using UnityEngine;

[Serializable]
public class PreloadEntry
{
	public PreloadEntry(GameObject pf)
	{
	}

	public string name;
	public GameObject prefab;
	public int count;
}
