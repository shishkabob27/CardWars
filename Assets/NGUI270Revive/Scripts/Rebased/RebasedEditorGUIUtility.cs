using System;
using UnityEditor;

public class RebasedEditorGUIUtility
{
	public static void LookLikeControls (float labelWidth)
	{
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = 0f;
	}

}


