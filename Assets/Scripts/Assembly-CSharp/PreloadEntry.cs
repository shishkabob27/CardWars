using System;
using UnityEngine;

[Serializable]
public class PreloadEntry
{
	public string name;

	public GameObject prefab;

	public int count;

	public PreloadEntry(GameObject pf)
	{
		prefab = pf;
		name = pf.name;
	}
}
