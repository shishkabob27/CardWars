// Assembly-UnityScript-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// KFFNetwork
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
[Serializable]
public class KFFNetwork : MonoBehaviour
{
    [Serializable]
    public enum ErrorID
    {
        None,
        Generic,
        NotLoggedIn,
        InvalidClientVersion
    }

    [Serializable]
    [XmlRoot("RequestResult")]
    public class WWWRequestResult : KFFDictionary
    {
        public override bool isValid()
        {
            return IsValid();
        }

        public virtual bool IsValid()
        {
            int result;
            if (entries.Count < 2)
            {
                result = 0;
            }
            else
            {
                KFFDictionaryEntry kFFDictionaryEntry = entries[0];
                if (kFFDictionaryEntry.key != "ERROR_ID")
                {
                    result = 0;
                }
                else if (int.Parse(kFFDictionaryEntry.value) != 0)
                {
                    result = 0;
                }
                else
                {
                    kFFDictionaryEntry = entries[1];
                    result = ((!(kFFDictionaryEntry.key != "ERROR_MSG")) ? 1 : 0);
                }
            }
            return (byte)result != 0;
        }
    }

    [Serializable]
    public class WWWInfo
    {
        public delegate void RequestCallback(WWWInfo info, object obj, string err, object param);

        public WWW www;

        public RequestCallback callback;

		public object callbackParam;

        public string url;

        public WWWForm form;

        public bool rawRequest;

        public byte[] postData;

        public Dictionary<string, string> headers;

        public bool queued;

        public bool active;

        public int version;

        public WWWInfo()
        {
            version = -1;
            form = null;
            rawRequest = false;
            postData = null;
            headers = null;
        }
    }

    //[NonSerialized]
    //public static __KFFNetwork_deserializeJSONCallback$callable0$19_53__ deserializeJSONCallback;

	private List<WWWInfo> wwwList;

    private int activeRequestCount;

    private int currSleepTimeout;

    [NonSerialized]
    private static KFFNetwork the_instance;

    [NonSerialized]
    public static int MAX_CONCURRENT_WWW_REQUEST_COUNT = 3;

    public KFFNetwork()
    {
        wwwList = new List<WWWInfo>();
    }

    public static KFFNetwork GetInstance()
    {
        if (!the_instance)
        {
            the_instance = (KFFNetwork)UnityEngine.Object.FindObjectOfType(typeof(KFFNetwork));
        }
        if (Application.isPlaying && !the_instance)
        {
            GameObject gameObject = new GameObject();
            if ((bool)gameObject)
            {
                the_instance = (KFFNetwork)gameObject.AddComponent(typeof(KFFNetwork));
            }
            if ((bool)gameObject)
            {
                gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
                gameObject.name = "AutomaticallyCreatedNetwork";
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
        }
        return the_instance;
    }

    public virtual WWWInfo SendWWWRequest(string url, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam)
    {
        return SendWWWRequestWithForm(null, url, WWWRequestCallback, callbackParam, false);
    }

    public virtual WWWInfo SendWWWRequestWithForm(WWWForm form, string url, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam)
    {
        return SendWWWRequestWithForm(form, url, WWWRequestCallback, callbackParam, false);
    }

    public virtual WWWInfo SendWWWRequestWithForm(WWWForm form, string url, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam, bool rawrequest)
    {
        return SendWWWRequestWithForm(form, url, WWWRequestCallback, callbackParam, rawrequest, null, null);
    }

    public virtual WWWInfo SendWWWRequestWithForm(WWWForm form, string url, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam, bool rawrequest, byte[] postData, Dictionary<string, string> headers)
    {
        WWWInfo wWWInfo = null;
        if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
        {
            WWW wWW = null;
            wWW = (postData != null && headers != null) ? new WWW(url, postData, headers) : (form == null ? new WWW(url) : new WWW(url, form));
            if (wWW != null)
            {
                wWWInfo = new WWWInfo();
                wWWInfo.www = wWW;
                wWWInfo.queued = false;
                wWWInfo.active = true;
                activeRequestCount++;
            }
        }
        else
        {
            wWWInfo = new WWWInfo();
            wWWInfo.www = null;
            wWWInfo.queued = true;
            wWWInfo.active = false;
        }
        if (wWWInfo != null)
        {
            wWWInfo.callback = WWWRequestCallback;
            wWWInfo.callbackParam = callbackParam;
            wWWInfo.url = url;
            wWWInfo.form = form;
            wWWInfo.rawRequest = rawrequest;
            wWWInfo.postData = postData;
            wWWInfo.headers = headers;
            wwwList.Add(wWWInfo);
        }
        else if (WWWRequestCallback != null)
        {
            WWWRequestCallback(null, null, null, callbackParam);
        }
        return wWWInfo;
    }

    public virtual WWWInfo LoadFromCacheOrDownload(string url, int version, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam)
    {
        WWWInfo wWWInfo = null;
        if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
        {
            WWW wWW = WWW.LoadFromCacheOrDownload(url, version);
            if (wWW != null)
            {
                wWWInfo = new WWWInfo();
                wWWInfo.www = wWW;
                wWWInfo.queued = false;
                wWWInfo.active = true;
                wWWInfo.version = version;
                activeRequestCount++;
            }
        }
        else
        {
            wWWInfo = new WWWInfo();
            wWWInfo.www = null;
            wWWInfo.queued = true;
            wWWInfo.active = false;
            wWWInfo.version = version;
        }
        if (wWWInfo != null)
        {
            wWWInfo.callback = WWWRequestCallback;
            wWWInfo.callbackParam = callbackParam;
            wWWInfo.url = url;
            wWWInfo.form = null;
            wWWInfo.rawRequest = true;
            wwwList.Add(wWWInfo);
        }
        else if (WWWRequestCallback != null)
        {
            WWWRequestCallback(null, null, null, callbackParam);
        }
        return wWWInfo;
    }

    public virtual void CancelWWWRequest(WWWInfo info)
    {
        if (info != null)
        {
            if ((info.active || info.queued) && info.callback != null)
            {
                info.callback(info, null, null, info.callbackParam);
            }
            if (info.active && activeRequestCount > 0)
            {
                activeRequestCount--;
            }
            wwwList.Remove(info);
        }
    }

    public virtual void Update()
    {
        int num = ((wwwList.Count <= 0) ? (-2) : (-1));
        if (currSleepTimeout != num)
        {
            currSleepTimeout = num;
            Screen.sleepTimeout = num;
        }
        for (int num2 = wwwList.Count - 1; num2 >= 0; num2--)
        {
            WWWInfo wWWInfo = wwwList[num2];
            if (wWWInfo.queued)
            {
                if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
                {
                    string url = wWWInfo.url;
                    WWW wWW = null;
                    wWW = ((wWWInfo.version >= 0) ? WWW.LoadFromCacheOrDownload(url, wWWInfo.version) : ((wWWInfo.postData != null && wWWInfo.headers != null) ? new WWW(url, wWWInfo.postData, wWWInfo.headers) : (wWWInfo.form == null ? new WWW(url) : new WWW(url, wWWInfo.form))));
                    if (wWW != null)
                    {
                        wWWInfo.www = wWW;
                        wWWInfo.queued = false;
                        wWWInfo.active = true;
                        activeRequestCount++;
                    }
                    else
                    {
                        if (wWWInfo.callback != null && wWWInfo.callback != null)
                        {
                            wWWInfo.callback(wWWInfo, null, null, wWWInfo.callbackParam);
                        }
                        wwwList.RemoveAt(num2);
                    }
                }
            }
            else if (wWWInfo.www == null || wWWInfo.www.isDone || !string.IsNullOrEmpty(wWWInfo.www.error))
            {
                object arg = null;
                string text = null;
                bool flag = true;
                if (wWWInfo.www != null && wWWInfo.www.error == null)
                {
                    if (wWWInfo.rawRequest)
                    {
                        arg = wWWInfo.www;
                        text = ((wWWInfo.www == null) ? null : wWWInfo.www.error);
                    }
                    else
                    {
                        try
                        {
                            object obj = Json.Deserialize(wWWInfo.www.text);
                            Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
                            WWWRequestResult wWWRequestResult = null;
                            if (dictionary != null)
                            {
                                wWWRequestResult = new WWWRequestResult();

                                foreach (string text2 in dictionary.Keys)
                                {
                                    if (dictionary[text2] != null)
                                    {
                                        Type valueType = dictionary[text2].GetType();

                                        if (valueType == typeof(float))
                                        {
                                            float value = (float)dictionary[text2];
                                            wWWRequestResult.SetValue(text2, value);
                                        }
                                        else if (valueType == typeof(long))
                                        {
                                            int value2 = (int)(long)dictionary[text2];
                                            wWWRequestResult.SetValue(text2, value2);
                                        }
                                        else if (valueType == typeof(string))
                                        {
                                            wWWRequestResult.SetValue(text2, dictionary[text2] as string);
                                        }
                                    }
                                    else
                                    {
                                        wWWRequestResult.SetValue(text2, null);
                                    }
                                }
                            }
                            text = (wWWInfo.www == null ? null : wWWInfo.www.error);
                            if (wWWRequestResult != null)
                            {
                                arg = wWWRequestResult;
                                wWWRequestResult.CreateDictionary();
                            }
                        }
                        catch (Exception rhs)
                        {
                            Debug.Log("error: " + rhs + " www.text: " + wWWInfo.www.text);
                            text = "Error parsing JSON: " + wWWInfo.url + "\n\nerror: " + rhs + "\n\nwww.text:\n" + wWWInfo.www.text;
                        }
                    }
                }
                else
                {
                    text = ((wWWInfo.www == null) ? null : wWWInfo.www.error);
                }
                if (wWWInfo.callback != null && flag)
                {
                    wWWInfo.callback(wWWInfo, arg, text, wWWInfo.callbackParam);
                }
                wwwList.RemoveAt(num2);
                activeRequestCount--;
            }
        }
    }

    public static void SetMaxConcurrentWWWRequestCount(int count)
    {
        MAX_CONCURRENT_WWW_REQUEST_COUNT = count;
    }

    public static int GetMaxConcurrentWWWRequestCount()
    {
        return MAX_CONCURRENT_WWW_REQUEST_COUNT;
    }

    public virtual void Main()
    {
    }
}
