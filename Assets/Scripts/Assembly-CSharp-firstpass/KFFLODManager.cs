using UnityEngine;

public class KFFLODManager
{
	private const string KEY_LOW_END_OVERRIDE = "IsLowEndOverride";

	private static bool isFirstCall = true;

	private static bool isLowEndDevice;

	private static bool isLowEndOverride;

	public static string hiResFolderName = "_hirez";

	public static string lowResFolderName = "_lowrez";

	public static bool GetLowEndOverride()
	{
		return isLowEndOverride;
	}

	public static void SetLowEndOverride(bool isLowEnd)
	{
		isLowEndOverride = isLowEnd;
		PlayerPrefs.SetInt("IsLowEndOverride", isLowEnd ? 1 : 0);
	}

	public static bool IsLowEndDevice(bool ignoreOverride = false)
	{
		return false;
	}

	public static string GetHiLowResFolderName()
	{
		if (IsLowEndDevice())
		{
			return lowResFolderName;
		}
		return hiResFolderName;
	}
}
