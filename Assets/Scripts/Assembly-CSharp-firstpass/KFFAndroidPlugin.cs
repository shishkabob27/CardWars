// Assembly-CSharp-firstpass, Version=1.4.1003.3007, Culture=neutral, PublicKeyToken=null
// KFFAndroidPlugin
using System.Collections.Generic;
using UnityEngine;

public class KFFAndroidPlugin : MonoBehaviour
{
    public delegate void HttpRequestCallback(string result);

    private static HttpRequestCallback httpRequestCallback = null;

    private static AndroidJavaClass httpPlugin;

    private static AndroidJavaClass utilsPlugin;

    private static AndroidJavaClass unityPlayerClass;

    private static object utilLock = new object();

    private void Awake()
    {
        Object.DontDestroyOnLoad(base.gameObject);
    }

    public static bool HttpRequest(string url, HttpRequestCallback callback = null, Dictionary<string, string> postData = null)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return false;
        }
        httpRequestCallback = callback;
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kungfufactory.androidplugin.KFFHttpRequest");
        AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
        AndroidJavaObject androidJavaObject2 = null;
        using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            androidJavaObject2 = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaObject.CallStatic("SetContext", androidJavaObject2);
            androidJavaObject.CallStatic("ClearPostData");
            if (postData != null)
            {
                foreach (string key in postData.Keys)
                {
                    string[] args = new string[2]
                    {
                        key,
                        postData[key]
                    };
                    androidJavaObject.CallStatic("AddPostData", args);
                }
            }
            androidJavaObject.CallStatic("HttpRequest", url);
        }
        return true;
    }

    private void HttpResponse(string result)
    {
        if (httpRequestCallback != null)
        {
            httpRequestCallback(result);
        }
    }

    public static string GetManifestVersionName()
    {
        string result = string.Empty;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<string>("GetManifestVersionName", new object[0]);
        }
        return result;
    }

    public static int GetManifestVersionCode()
    {
        int result = 0;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<int>("GetManifestVersionCode", new object[0]);
        }
        return result;
    }

    public static string GetManifestKey(string Key)
    {
        string result = string.Empty;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<string>("GetManifestKey", new object[1] { Key });
        }
        return result;
    }

    public static string GetManifestKeyString(string Key)
    {
        string result = string.Empty;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<string>("GetManifestKeyString", new object[1] { Key });
        }
        return result;
    }

    public static string[] GetManifestKeyStrings(string[] Keys)
    {
        if (UtilPluginReady())
        {
            AndroidJavaObject androidJavaObject = utilsPlugin.CallStatic<AndroidJavaObject>("GetManifestKeyStrings", new object[1] { Keys });
            return AndroidJNIHelper.ConvertFromJNIArray<string[]>(androidJavaObject.GetRawObject());
        }
        return new string[0];
    }

    public static bool GetManifestKeyBool(string Key)
    {
        bool result = false;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<bool>("GetManifestKeyBool", new object[1] { Key });
        }
        return result;
    }

    public static int GetManifestKeyInt(string Key)
    {
        int result = 0;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<int>("GetManifestKeyInt", new object[1] { Key });
        }
        return result;
    }

    public static float GetManifestKeyFloat(string Key)
    {
        float result = 0f;
        if (UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<float>("GetManifestKeyFloat", new object[1] { Key });
        }
        return result;
    }

    public static void SavePrivateFileData(string filename, string data)
    {
        if (filename != null && data != null && UtilPluginReady())
        {
            utilsPlugin.CallStatic("SavePrivateFileData", filename, data);
        }
    }

    public static string ReadPrivateFileData(string filename)
    {
        string result = null;
        if (filename != null && UtilPluginReady())
        {
            result = utilsPlugin.CallStatic<string>("ReadPrivateFileData", new object[1] { filename });
        }
        return result;
    }

    public static void DebugLog(string log)
    {
        if (log != null && UtilPluginReady())
        {
            utilsPlugin.CallStatic("DebugLog", log);
        }
    }

    public static void ConfirmQuit()
    {
        if (UtilPluginReady())
        {
            utilsPlugin.CallStatic("MinimizeOnQuit");
        }
    }

    public static bool UtilPluginReady()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return false;
        }
        lock (utilLock)
        {
            if (utilsPlugin == null)
            {
                utilsPlugin = new AndroidJavaClass("com.kungfufactory.androidplugin.KFFUtils");
                unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject @static = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                utilsPlugin.CallStatic("SetContext", @static);
            }
        }
        return true;
    }
}
