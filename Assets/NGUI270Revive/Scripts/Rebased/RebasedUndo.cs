using System;
#if UNITY_EDITOR
using UnityEditor;

public class RebasedUndo
{	
	public static void RegisterUndo (UnityEngine.Object objectToUndo, string name)
	{
		Undo.RecordObject (objectToUndo, name);
	}
}

#endif