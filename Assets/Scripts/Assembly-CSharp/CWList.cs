using System.Collections.Generic;
using UnityEngine;

public class CWList<T> : List<T>
{
	public T RandomItem()
	{
		if (Count <= 0)
		{
			return default(T);
		}
		int index = Random.Range(0, Count);
		return this[index];
	}
}
