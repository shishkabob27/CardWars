// Assembly-CSharp-firstpass, Version=1.4.1003.3007, Culture=neutral, PublicKeyToken=null
// KFFLODManager
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
        if (!isFirstCall)
        {
            return isLowEndDevice || (!ignoreOverride && isLowEndOverride);
        }
        isFirstCall = false;
        isLowEndOverride = PlayerPrefs.GetInt("IsLowEndOverride", 0) != 0;
        bool result = !ignoreOverride && isLowEndOverride;
        if (SystemInfo.deviceModel == "samsung SPH-L710")
        {
            return result;
        }
        if (SystemInfo.deviceModel == "samsung SPH-L720")
        {
            return result;
        }
        if (SystemInfo.deviceModel == "samsung SPH-P600")
        {
            return result;
        }
        if (SystemInfo.deviceModel == "samsung SPH-L900")
        {
            return result;
        }
        int systemMemorySize = SystemInfo.systemMemorySize;
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        int width = Screen.width;
        int height = Screen.height;
        if (systemMemorySize <= 1664 || width <= 480 || height <= 480)
        {
            isLowEndDevice = true;
            return isLowEndDevice;
        }
        bool flag = false;
        string text = SystemInfo.deviceModel.ToLower();
        int num = text.IndexOf("amazon");
        if (num >= 0)
        {
            flag = true;
        }
        if (flag)
        {
            return result;
        }
        return result;
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
