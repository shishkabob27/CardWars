using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	public static bool isDebugBuild
	{
		get
		{
			return false;
		}
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		UnityEngine.Debug.DrawLine(start, end, color);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DrawLine(Vector3 start, Vector3 end)
	{
		UnityEngine.Debug.DrawLine(start, end);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		UnityEngine.Debug.DrawRay(start, dir, color);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		UnityEngine.Debug.DrawRay(start, dir);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		UnityEngine.Debug.DrawRay(start, dir, color);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	[Conditional("KFF_DEBUG_LEVEL_ERROR")]
	[Conditional("KFF_DEBUG_LEVEL_WARN")]
	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	[Conditional("KFF_DEBUG_LEVEL_WARN")]
	[Conditional("KFF_DEBUG_LEVEL_ERROR")]
	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void Print(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void Log(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	[Conditional("KFF_DEBUG_LEVEL_ERROR")]
	[Conditional("KFF_DEBUG_LEVEL_WARN")]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	[Conditional("KFF_DEBUG_LEVEL_WARN")]
	[Conditional("KFF_DEBUG_LEVEL_ERROR")]
	public static void LogError(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	[Conditional("KFF_DEBUG_LEVEL_WARN")]
	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	[Conditional("KFF_DEBUG_LEVEL_WARN")]
	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void LogException(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogException(exception, context);
	}

	[Conditional("KFF_DEBUG_LEVEL_LOG")]
	public static void ClearDeveloperConsole()
	{
		UnityEngine.Debug.ClearDeveloperConsole();
	}
}
