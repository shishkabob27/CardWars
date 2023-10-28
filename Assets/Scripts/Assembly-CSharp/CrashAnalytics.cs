using System;
using UnityEngine;

public class CrashAnalytics
{
	private static AndroidJavaClass jniInst;

	private static bool inited;

	public static void LogBreadcrumb(string breadCrumb)
	{
		AndroidJavaClass jniInstance = GetJniInstance();
		if (jniInstance != null)
		{
			jniInstance.CallStatic("logBreadcrumb", breadCrumb);
		}
	}

	public static void LogException(Exception e)
	{
		AndroidJavaClass jniInstance = GetJniInstance();
		if (jniInstance != null && e != null)
		{
			jniInstance.CallStatic("logException", e.ToString());
		}
	}

	private static AndroidJavaClass GetJniInstance()
	{
		return null;
		if (!inited)
		{
			inited = true;
			jniInst = new AndroidJavaClass("com.kungfufactory.androidplugin.KFFCrashlytics");
			if (jniInst != null && !jniInst.CallStatic<bool>("isActive", new object[0]))
			{
				jniInst = null;
			}
		}
		return jniInst;
	}
}
