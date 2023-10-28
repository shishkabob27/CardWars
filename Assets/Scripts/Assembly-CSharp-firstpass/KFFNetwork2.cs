using System;
using System.Collections.Generic;
using System.Text;
using MiniJSON;
using UnityEngine;

public class KFFNetwork2 : MonoBehaviour
{
	private static int MAX_CONCURRENT_WWW_REQUEST_COUNT = 1;

	private List<WWWInfo> wwwList = new List<WWWInfo>();

	private int activeRequestCount;

	private int currSleepTimeout;

	private static KFFNetwork2 the_instance;

	public static KFFNetwork2 GetInstance()
	{
		if (!the_instance)
		{
			the_instance = UnityEngine.Object.FindObjectOfType(typeof(KFFNetwork2)) as KFFNetwork2;
		}
		if (Application.isPlaying && !the_instance)
		{
			GameObject gameObject = new GameObject();
			if ((bool)gameObject)
			{
				the_instance = gameObject.AddComponent(typeof(KFFNetwork2)) as KFFNetwork2;
			}
			if ((bool)gameObject)
			{
				gameObject.transform.position = new Vector3(999999f, 999999f, 999999f);
				gameObject.name = "AutomaticallyCreatedKFFNetwork2";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		return the_instance;
	}

	public WWWInfo SendWWWRequest(string scriptNameAndParams, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam)
	{
		return SendWWWRequestWithForm(null, scriptNameAndParams, WWWRequestCallback, callbackParam);
	}

	public WWWInfo SendWWWRequestWithForm(WWWForm form, string scriptNameAndParams, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam)
	{
		if (scriptNameAndParams != null && scriptNameAndParams.ToLower().StartsWith("https://"))
		{
			return null;
		}
		WWWInfo wWWInfo = null;
		if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
		{
			WWW wWW = ((form == null) ? new WWW(scriptNameAndParams) : new WWW(scriptNameAndParams, form));
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
			wWWInfo.scriptNameAndParams = scriptNameAndParams;
			wWWInfo.form = form;
			wwwList.Add(wWWInfo);
		}
		else if (WWWRequestCallback != null)
		{
			WWWRequestCallback(null, null, null, callbackParam);
		}
		return wWWInfo;
	}

	public WWWInfo SendHTTPSRequest(string url, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam, Dictionary<string, string> postData)
	{
		WWWInfo wWWInfo = null;
		if (activeRequestCount < MAX_CONCURRENT_WWW_REQUEST_COUNT)
		{
			if (KFFAndroidPlugin.HttpRequest(url, AndroidHttpRequestCallback, postData))
			{
				wWWInfo = new WWWInfo();
				wWWInfo.queued = false;
				wWWInfo.active = true;
				activeRequestCount++;
			}
		}
		else
		{
			wWWInfo = new WWWInfo();
			wWWInfo.queued = true;
			wWWInfo.active = false;
		}
		if (wWWInfo != null)
		{
			wWWInfo.callback = WWWRequestCallback;
			wWWInfo.callbackParam = callbackParam;
			wWWInfo.scriptNameAndParams = url;
			wWWInfo.isHttps = true;
			wWWInfo.isHttpsDone = false;
			if (postData != null)
			{
				wWWInfo.postData = postData;
			}
			wwwList.Add(wWWInfo);
		}
		else if (WWWRequestCallback != null)
		{
			WWWRequestCallback(null, null, null, callbackParam);
		}
		return wWWInfo;
	}

	public WWWInfo LoadFromCacheOrDownload(string url, int version, WWWInfo.RequestCallback WWWRequestCallback, object callbackParam)
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
			wWWInfo.scriptNameAndParams = url;
			wWWInfo.form = null;
			wwwList.Add(wWWInfo);
		}
		else if (WWWRequestCallback != null)
		{
			WWWRequestCallback(null, null, null, callbackParam);
		}
		return wWWInfo;
	}

	public void CancelWWWRequest(WWWInfo info)
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

	private void Update()
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
					string scriptNameAndParams = wWWInfo.scriptNameAndParams;
					if (wWWInfo.isHttps)
					{
						if (KFFAndroidPlugin.HttpRequest(scriptNameAndParams, AndroidHttpRequestCallback))
						{
							wWWInfo.queued = false;
							wWWInfo.active = true;
							activeRequestCount++;
						}
						else
						{
							wwwList.RemoveAt(num2);
						}
					}
					else
					{
						WWW wWW = ((wWWInfo.version >= 0) ? WWW.LoadFromCacheOrDownload(scriptNameAndParams, wWWInfo.version) : ((wWWInfo.form == null) ? new WWW(scriptNameAndParams) : new WWW(scriptNameAndParams, wWWInfo.form)));
						if (wWW != null)
						{
							wWWInfo.www = wWW;
							wWWInfo.queued = false;
							wWWInfo.active = true;
							activeRequestCount++;
						}
						else
						{
							wwwList.RemoveAt(num2);
						}
					}
				}
			}
			else if ((!wWWInfo.isHttps && wWWInfo.www == null) || (wWWInfo.isHttps && wWWInfo.isHttpsDone) || (wWWInfo.www != null && (wWWInfo.www.isDone || wWWInfo.www.error != null)))
			{
				object obj = null;
				string text = null;
				bool flag = true;
				string text2 = null;
				if (wWWInfo.isHttps)
				{
					text2 = wWWInfo.httpsResult;
				}
				else if (wWWInfo.www != null && wWWInfo.www.error == null)
				{
					text2 = wWWInfo.www.text;
				}
				if (text2 != null)
				{
					try
					{
						object obj2 = Json.Deserialize(text2);
						Dictionary<string, object> dictionary = obj2 as Dictionary<string, object>;
						KFFServerRequestResult kFFServerRequestResult = null;
						if (dictionary != null)
						{
							kFFServerRequestResult = new KFFServerRequestResult();
							foreach (string key in dictionary.Keys)
							{
								if (dictionary[key] != null)
								{
									kFFServerRequestResult.SetValue(key, dictionary[key]);
								}
								else
								{
									kFFServerRequestResult.SetValue(key, null);
								}
							}
						}
						text = (wWWInfo.isHttps ? null : ((wWWInfo.www == null) ? null : wWWInfo.www.error));
						if (kFFServerRequestResult != null)
						{
							obj = kFFServerRequestResult;
							kFFServerRequestResult.CreateDictionary();
						}
					}
					catch (Exception ex)
					{
						text = string.Concat("Error parsing JSON: ", wWWInfo.scriptNameAndParams, "\n\nexception: ", ex, (wWWInfo.www != null) ? ("\n\nwww.text:\n" + wWWInfo.www.text) : string.Empty);
					}
				}
				else
				{
					text = ((wWWInfo.www == null) ? null : wWWInfo.www.error);
				}
				if (wWWInfo.callback != null && flag)
				{
					wWWInfo.callback(wWWInfo, obj, text, wWWInfo.callbackParam);
				}
				wwwList.RemoveAt(num2);
				activeRequestCount--;
			}
		}
	}

	private static byte[] StringToUTF8(string str)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetBytes(str);
	}

	private void AndroidHttpRequestCallback(string result)
	{
		if (wwwList.Count > 0 && wwwList[0] != null)
		{
			wwwList[0].isHttpsDone = true;
			wwwList[0].httpsResult = result;
		}
	}
}
