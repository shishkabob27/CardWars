using UnityEngine;

public static class FbDebug
{
	public static void Log(string msg)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(msg);
		}
	}

	public static void Info(string msg)
	{
		Debug.Log(msg);
	}

	public static void Warn(string msg)
	{
		Debug.LogWarning(msg);
	}

	public static void Error(string msg)
	{
		Debug.LogError(msg);
	}
}
