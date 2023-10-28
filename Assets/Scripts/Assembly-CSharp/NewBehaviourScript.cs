using System;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	public StringVector3Table table;

	private void OnGUI()
	{
		if (GUILayout.Button("Add", GUILayout.Width(150f)))
		{
			table.Dic.Add(Guid.NewGuid().ToString(), Vector3.zero);
		}
		if (GUILayout.Button("Clear", GUILayout.Width(150f)))
		{
			table.Dic.Clear();
		}
	}
}
