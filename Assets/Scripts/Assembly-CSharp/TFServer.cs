#define ASSERTS_ON
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using MiniJSON;
using UnityEngine;

public class TFServer
{
	public delegate void JsonStringHandler(string jsonResponse, HttpStatusCode status);

	public delegate void JsonResponseHandler(Dictionary<string, object> dict, HttpStatusCode status);

	public const string ERROR_KEY = "error";

	public const string NETWORK_ERROR = "Network error";

	private const bool LOG_FAILED_REQUESTS = true;

	public static readonly string NETWORK_ERROR_JSON = "{\"success\": false, \"error\": \"Network error\"}";

	private static readonly string LOG_LOCATION = Application.persistentDataPath + Path.DirectorySeparatorChar + "error";

	private static int errorCount = 0;

	private CookieContainer cookies = new CookieContainer();

	private Dictionary<object, JsonStringHandler> reqs = new Dictionary<object, JsonStringHandler>();

	private bool shortCircuitRequests;

	public TFServer(CookieContainer cookies, int maxConnections)
	{
		this.cookies = cookies;
		TFWebClient.maxConnections = maxConnections;
	}

	public void ShortCircuitAllRequests()
	{
		shortCircuitRequests = true;
	}

	public void PostToJSON(string url, Dictionary<string, object> postDict, JsonResponseHandler callback)
	{
		string text = EncodePostData(postDict);
		TFWebClient tFWebClient = RegisterCallback(callback);
		if (shortCircuitRequests)
		{
			TFUtils.DebugLog("shortcircuiting a post to " + url);
			GetCallback(tFWebClient)(NETWORK_ERROR_JSON, HttpStatusCode.ServiceUnavailable);
			tFWebClient.Dispose();
		}
		else
		{
			TFUtils.DebugLog("posting data to " + url);
			TFUtils.DebugLog("post data: " + text);
			tFWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
			tFWebClient.UploadStringCompleted += OnUploadComplete;
			tFWebClient.UploadStringAsync(new Uri(url), text);
		}
	}

	public void PostToString(string url, Dictionary<string, object> postDict, JsonStringHandler callback)
	{
		string text = EncodePostData(postDict);
		TFWebClient tFWebClient = RegisterCallback(callback);
		if (shortCircuitRequests)
		{
			TFUtils.DebugLog("shortcircuiting a post to " + url);
			GetCallback(tFWebClient)("Network error", HttpStatusCode.ServiceUnavailable);
			tFWebClient.Dispose();
		}
		else
		{
			TFUtils.DebugLog("posting data to " + url);
			TFUtils.DebugLog("post data: " + text);
			tFWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
			tFWebClient.UploadStringCompleted += OnUploadComplete;
			tFWebClient.UploadStringAsync(new Uri(url), text);
		}
	}

	public void GetToJSON(string url, JsonResponseHandler callback)
	{
		TFWebClient tFWebClient = RegisterCallback(callback);
		if (shortCircuitRequests)
		{
			TFUtils.DebugLog("Shortcircuiting a request to " + url);
			GetCallback(tFWebClient)(NETWORK_ERROR_JSON, HttpStatusCode.ServiceUnavailable);
			tFWebClient.Dispose();
		}
		else
		{
			TFUtils.DebugLog("Making request to " + url);
			tFWebClient.DownloadStringCompleted += OnDownloadComplete;
			tFWebClient.DownloadStringAsync(new Uri(url));
		}
	}

	public Cookie GetCookie(Uri uri, string key)
	{
		return cookies.GetCookies(uri)[key];
	}

	private TFWebClient RegisterCallback(JsonStringHandler callback)
	{
		TFWebClient tFWebClient = new TFWebClient(cookies);
		reqs[tFWebClient] = callback;
		tFWebClient.NetworkError += OnNetworkError;
		return tFWebClient;
	}

	private TFWebClient RegisterCallback(JsonResponseHandler callback)
	{
		TFWebClient tFWebClient = new TFWebClient(cookies);
		reqs[tFWebClient] = JsCallback(callback);
		tFWebClient.NetworkError += OnNetworkError;
		return tFWebClient;
	}

	private string EncodePostData(Dictionary<string, object> d)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, object> item in d)
		{
			string s = item.Value.ToString();
			list.Add(item.Key + "=" + WWW.EscapeURL(s));
		}
		return string.Join("&", list.ToArray());
	}

	private JsonStringHandler JsCallback(JsonResponseHandler cb)
	{
		return delegate(string jsonResponse, HttpStatusCode code)
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(jsonResponse);
			cb(dict, code);
		};
	}

	private void OnNetworkError(object sender, WebException e)
	{
		if (e.Response != null)
		{
			LogResponse(e.Response as HttpWebResponse);
		}
		if (e.Response != null)
		{
			GetCallback(sender)(NETWORK_ERROR_JSON, ((HttpWebResponse)e.Response).StatusCode);
		}
		else
		{
			GetCallback(sender)(NETWORK_ERROR_JSON, HttpStatusCode.ServiceUnavailable);
		}
	}

	private void OnDownloadComplete(object sender, DownloadStringCompletedEventArgs e)
	{
		TFUtils.DebugLog("in onDownloadComplete...");
		JsonStringHandler jsonStringHandler = OnRequestComplete(sender, e);
		if (jsonStringHandler == null)
		{
			return;
		}
		TFUtils.DebugLog("web result: " + e.Result);
		string result = e.Result;
		if (e.Error == null)
		{
			jsonStringHandler(result, HttpStatusCode.OK);
			return;
		}
		WebException ex = e.Error as WebException;
		if (ex != null)
		{
			jsonStringHandler(result, ((HttpWebResponse)ex.Response).StatusCode);
		}
		else
		{
			jsonStringHandler(result, HttpStatusCode.Unused);
		}
	}

	private void OnUploadComplete(object sender, UploadStringCompletedEventArgs e)
	{
		TFUtils.DebugLog("in onUploadComplete...");
		JsonStringHandler jsonStringHandler = OnRequestComplete(sender, e);
		if (jsonStringHandler == null)
		{
			return;
		}
		TFUtils.DebugLog("web result: " + e.Result);
		if (e.Error == null)
		{
			jsonStringHandler(e.Result, HttpStatusCode.OK);
			return;
		}
		WebException ex = e.Error as WebException;
		if (ex != null)
		{
			jsonStringHandler(e.Result, ((HttpWebResponse)ex.Response).StatusCode);
		}
		else
		{
			jsonStringHandler(e.Result, HttpStatusCode.Unused);
		}
	}

	private JsonStringHandler OnRequestComplete(object sender, AsyncCompletedEventArgs e)
	{
		TFUtils.DebugLog("in onRequestCompete...");
		JsonStringHandler callback = GetCallback(sender);
		TFUtils.Assert(null != e, "No event args happened.");
		if (e.Error != null && callback != null)
		{
			WebException ex = e.Error as WebException;
			if (ex != null)
			{
				if (ex != null && ex.Response != null)
				{
					LogResponse((HttpWebResponse)ex.Response);
				}
				callback(NETWORK_ERROR_JSON, (ex.Response == null) ? HttpStatusCode.RequestTimeout : ((HttpWebResponse)ex.Response).StatusCode);
			}
			else
			{
				callback(NETWORK_ERROR_JSON, HttpStatusCode.Unused);
			}
		}
		else if (!e.Cancelled)
		{
			return callback;
		}
		return null;
	}

	private JsonStringHandler GetCallback(object sender)
	{
		if (reqs.ContainsKey(sender))
		{
			JsonStringHandler result = reqs[sender];
			reqs.Remove(sender);
			return result;
		}
		return null;
	}

	private void LogResponse(HttpWebResponse response)
	{
		Stream responseStream = response.GetResponseStream();
		using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
		{
			string text = streamReader.ReadToEnd();
			if (!string.IsNullOrEmpty(text))
			{
				TFUtils.DebugLog("Writing out error: " + text);
				File.WriteAllText(string.Format("{0}{1}.html", LOG_LOCATION, ++errorCount), text);
			}
		}
		responseStream.Dispose();
	}
}
