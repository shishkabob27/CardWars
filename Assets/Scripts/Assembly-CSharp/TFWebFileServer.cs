using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using Ionic.Zlib;
using JsonFx.Json;

public class TFWebFileServer
{
	private class CallbackInfo
	{
		public FileCallbackHandler Callback { get; set; }

		public object UserData { get; set; }
	}

	public delegate void FileCallbackHandler(TFWebFileResponse response);

	protected CookieContainer cookies;

	public TFWebFileServer()
	{
	}

	public TFWebFileServer(CookieContainer cookies)
	{
		this.cookies = cookies;
	}

	public void GetFile(string uri, WebHeaderCollection headers, FileCallbackHandler callback, object userData = null)
	{
		TFUtils.DebugLog("Getting file " + uri);
		using (TFWebClient tFWebClient = new TFWebClient(cookies))
		{
			tFWebClient.NetworkError += OnNetworkError;
			tFWebClient.Headers = headers;
			foreach (string header in tFWebClient.Headers)
			{
				TFUtils.DebugLog("Outbound:" + header + "-->" + tFWebClient.Headers[header]);
			}
			try
			{
				tFWebClient.DownloadDataCompleted += OnGetComplete;
				CallbackInfo callbackInfo = new CallbackInfo();
				callbackInfo.Callback = callback;
				callbackInfo.UserData = userData;
				tFWebClient.DownloadDataAsync(new Uri(uri), callbackInfo);
			}
			catch (Exception message)
			{
				TFUtils.DebugLog(message);
			}
		}
	}

	protected void OnGetComplete(object sender, DownloadDataCompletedEventArgs e)
	{
		OnCallComplete(sender, e, true);
	}

	private void PopulateResponse(TFWebFileResponse response, HttpWebResponse httpRes)
	{
		response.StatusCode = httpRes.StatusCode;
		response.headers = httpRes.Headers;
		response.URI = httpRes.ResponseUri.ToString();
		try
		{
			byte[] array = new byte[4096];
			Stream responseStream = httpRes.GetResponseStream();
			int num = 0;
			MemoryStream memoryStream = new MemoryStream();
			while ((num = responseStream.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, num);
			}
			Encoding uTF = Encoding.UTF8;
			response.Data = uTF.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		}
		catch (Exception message)
		{
			TFUtils.DebugLog(message);
		}
	}

	public void SaveFile(string uri, string contents, WebHeaderCollection headers, FileCallbackHandler callback, object userdata = null)
	{
		using (TFWebClient tFWebClient = new TFWebClient(cookies))
		{
			tFWebClient.NetworkError += OnNetworkError;
			tFWebClient.Headers = headers;
			foreach (string header in tFWebClient.Headers)
			{
				TFUtils.DebugLog("Outbound:" + header + "-->" + tFWebClient.Headers[header]);
			}
			try
			{
				Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(contents);
				string text2 = string.Empty;
				if (dictionary.ContainsKey("PlayerName"))
				{
					text2 = text2 + "username=" + (string)dictionary["PlayerName"] + "&";
				}
				if (dictionary.ContainsKey("GP"))
				{
					text2 = text2 + "score=" + (int)dictionary["GP"] + "&";
				}
				if (dictionary.ContainsKey("Guild"))
				{
					text2 = text2 + "guild=" + (string)dictionary["Guild"] + "&";
				}
				text2 += "data=";
				byte[] bytes = Encoding.UTF8.GetBytes(text2);
				byte[] inArray = TFUtils.Zip(contents);
				byte[] bytes2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(inArray));
				byte[] array = new byte[bytes.Length + bytes2.Length];
				bytes.CopyTo(array, 0);
				bytes2.CopyTo(array, bytes.Length);
				tFWebClient.UploadDataCompleted += OnSaveComplete;
				CallbackInfo callbackInfo = new CallbackInfo();
				callbackInfo.Callback = callback;
				callbackInfo.UserData = userdata;
				tFWebClient.UploadDataAsync(new Uri(uri), "PUT", array, callbackInfo);
			}
			catch (Exception ex)
			{
				TFUtils.ErrorLog("TFWebFileServer.SaveFile:  corrupt data " + ex.ToString());
			}
		}
	}

	protected void OnSaveComplete(object sender, UploadDataCompletedEventArgs e)
	{
		OnCallComplete(sender, e, false);
	}

	protected string DecodeZippedData(byte[] input)
	{
		byte[] array = null;
		string text = null;
		if ((input[0] == 117 && input[1] == 115 && input[2] == 101 && input[3] == 114) || (input[0] == 72 && input[1] == 52 && input[2] == 115))
		{
			string @string = Encoding.UTF8.GetString(input);
			int num = @string.IndexOf("&data=");
			string s = @string.Remove(0, num + 6);
			array = Convert.FromBase64String(s);
		}
		else
		{
			array = input;
		}
		try
		{
			return TFUtils.Unzip(array);
		}
		catch (ZlibException)
		{
			TFUtils.DebugLog("Error uncompressing data. Returning result directly.");
			return Encoding.UTF8.GetString(array);
		}
	}

	protected void OnCallComplete(object sender, AsyncCompletedEventArgs e, bool uncompress)
	{
		try
		{
			TFWebClient tFWebClient = (TFWebClient)sender;
			TFWebFileResponse tFWebFileResponse = new TFWebFileResponse();
			tFWebFileResponse.URI = tFWebClient.BaseAddress;
			if (e.Error == null)
			{
				tFWebFileResponse.StatusCode = HttpStatusCode.OK;
				if (e is DownloadDataCompletedEventArgs)
				{
					DownloadDataCompletedEventArgs downloadDataCompletedEventArgs = (DownloadDataCompletedEventArgs)e;
					tFWebFileResponse.Data = DecodeZippedData(downloadDataCompletedEventArgs.Result);
				}
				else if (e is UploadDataCompletedEventArgs)
				{
					UploadDataCompletedEventArgs uploadDataCompletedEventArgs = (UploadDataCompletedEventArgs)e;
					tFWebFileResponse.Data = Encoding.UTF8.GetString(uploadDataCompletedEventArgs.Result);
				}
				tFWebFileResponse.headers = tFWebClient.ResponseHeaders;
				foreach (string header in tFWebFileResponse.headers)
				{
					TFUtils.DebugLog(header + "-->" + tFWebFileResponse.headers[header]);
				}
				try
				{
					string value = tFWebFileResponse.headers["Date"];
					DateTime dateTime = (TFUtils.lastServerTimeUpdate = Convert.ToDateTime(value));
					TimeSpan timeSpan = dateTime.Subtract(DateTime.Now);
					double num = Math.Abs((timeSpan - TFUtils.serverTimeDiff).TotalSeconds);
					if (num > 10.0)
					{
						TFUtils.serverTimeDiff = timeSpan;
						TFUtils.DebugLog("Server time difference = " + timeSpan.TotalSeconds);
					}
				}
				catch (Exception)
				{
				}
			}
			else if (e.Error.GetType().Name == "WebException")
			{
				WebException ex2 = (WebException)e.Error;
				TFUtils.DebugLog(ex2);
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
				if (httpWebResponse != null)
				{
					PopulateResponse(tFWebFileResponse, httpWebResponse);
				}
				else
				{
					tFWebFileResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
					tFWebFileResponse.NetworkDown = true;
				}
			}
			else
			{
				TFUtils.DebugLog("Server returned error");
				TFUtils.DebugLog(e.Error);
				tFWebFileResponse.NetworkDown = true;
				tFWebFileResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
			}
			CallbackInfo callbackInfo = (CallbackInfo)e.UserState;
			tFWebFileResponse.UserData = callbackInfo.UserData;
			callbackInfo.Callback(tFWebFileResponse);
		}
		catch (Exception message)
		{
			TFUtils.DebugLog(message);
		}
	}

	public void DeleteFile(string uri, WebHeaderCollection headers, FileCallbackHandler callback, object userData = null)
	{
		TFUtils.DebugLog("Deleting file " + uri, "saveload");
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(uri));
		httpWebRequest.Method = "DELETE";
		httpWebRequest.CookieContainer = cookies;
		httpWebRequest.Headers = headers;
		foreach (string header in httpWebRequest.Headers)
		{
			TFUtils.DebugLog("Outbound:" + header + "-->" + httpWebRequest.Headers[header], "saveload");
		}
		try
		{
			httpWebRequest.GetResponse();
		}
		catch (Exception message)
		{
			TFUtils.ErrorLog(message, "saveload");
		}
	}

	private void OnNetworkError(object sender, WebException e)
	{
		TFUtils.ErrorLog("Got webException !!!!" + e, "saveload");
	}
}
