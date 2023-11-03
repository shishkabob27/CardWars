using System;
#if UNITY_EDITOR
using UnityEditor;

public class RebasedEditorGUIUtility
{
	public static void LookLikeControls (float labelWidth)
	{
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUIUtility.fieldWidth = 0f;
	}

}
#endif