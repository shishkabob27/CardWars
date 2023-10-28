using UnityEngine;

public static class FbDebug
{
	public static void Log(string msg)
	{
		if (Debug.isDebugBuild)
		{
			if (Application.isWebPlayer)
			{
				Application.ExternalCall("console.log", msg);
			}
			Debug.Log(msg);
		}
	}

	public static void Info(string msg)
	{
		if (Application.isWebPlayer)
		{
			Application.ExternalCall("console.info", msg);
		}
		Debug.Log(msg);
	}

	public static void Warn(string msg)
	{
		if (Application.isWebPlayer)
		{
			Application.ExternalCall("console.warn", msg);
		}
		Debug.LogWarning(msg);
	}

	public static void Error(string msg)
	{
		if (Application.isWebPlayer)
		{
			Application.ExternalCall("console.error", msg);
		}
		Debug.LogError(msg);
	}
}
