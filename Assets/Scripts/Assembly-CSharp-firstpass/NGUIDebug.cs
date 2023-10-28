using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	private static List<string> mLines = new List<string>();

	private static NGUIDebug mInstance = null;

	public static void Log(string text)
	{
		if (Application.isPlaying)
		{
			if (mLines.Count > 20)
			{
				mLines.RemoveAt(0);
			}
			mLines.Add(text);
			if (mInstance == null)
			{
				GameObject gameObject = new GameObject("_NGUI Debug");
				mInstance = gameObject.AddComponent<NGUIDebug>();
				Object.DontDestroyOnLoad(gameObject);
			}
		}
	}

	public static void DrawBounds(Bounds b)
	{
		Vector3 center = b.center;
		Vector3 vector = b.center - b.extents;
		Vector3 vector2 = b.center + b.extents;
	}

	private void OnGUI()
	{
		int i = 0;
		for (int count = mLines.Count; i < count; i++)
		{
			GUILayout.Label(mLines[i]);
		}
	}
}
