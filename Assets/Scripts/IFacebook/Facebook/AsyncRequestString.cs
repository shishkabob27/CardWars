using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook
{
	public class AsyncRequestString : MonoBehaviour
	{
		protected string url;

		protected HttpMethod method;

		protected Dictionary<string, string> formData;

		protected WWWForm query;

		protected FacebookDelegate callback;

		internal static void Post(string url, Dictionary<string, string> formData = null, FacebookDelegate callback = null)
		{
			Request(url, HttpMethod.POST, formData, callback);
		}

		internal static void Get(string url, Dictionary<string, string> formData = null, FacebookDelegate callback = null)
		{
			Request(url, HttpMethod.GET, formData, callback);
		}

		internal static void Request(string url, HttpMethod method, WWWForm query = null, FacebookDelegate callback = null)
		{
			FBComponentFactory.AddComponent<AsyncRequestString>().SetUrl(url).SetMethod(method)
				.SetQuery(query)
				.SetCallback(callback);
		}

		internal static void Request(string url, HttpMethod method, Dictionary<string, string> formData = null, FacebookDelegate callback = null)
		{
			FBComponentFactory.AddComponent<AsyncRequestString>().SetUrl(url).SetMethod(method)
				.SetFormData(formData)
				.SetCallback(callback);
		}

		private IEnumerator Start()
		{
			WWW www;
			if (method == HttpMethod.GET)
			{
				string urlParams = ((!url.Contains("?")) ? "?" : "&");
				if (formData != null)
				{
					foreach (KeyValuePair<string, string> pair2 in formData)
					{
						urlParams += string.Format("{0}={1}&", Uri.EscapeDataString(pair2.Key), Uri.EscapeDataString(pair2.Value));
					}
				}
				www = new WWW(url + urlParams);
			}
			else
			{
				if (query == null)
				{
					query = new WWWForm();
				}
				if (method == HttpMethod.DELETE)
				{
					query.AddField("method", "delete");
				}
				if (formData != null)
				{
					foreach (KeyValuePair<string, string> pair in formData)
					{
						query.AddField(pair.Key, pair.Value);
					}
				}
				www = new WWW(url, query);
			}
			yield return www;
			if (callback != null)
			{
				callback(new FBResult(www));
			}
			www.Dispose();
			UnityEngine.Object.Destroy(this);
		}

		internal AsyncRequestString SetUrl(string url)
		{
			this.url = url;
			return this;
		}

		internal AsyncRequestString SetMethod(HttpMethod method)
		{
			this.method = method;
			return this;
		}

		internal AsyncRequestString SetFormData(Dictionary<string, string> formData)
		{
			this.formData = formData;
			return this;
		}

		internal AsyncRequestString SetQuery(WWWForm query)
		{
			this.query = query;
			return this;
		}

		internal AsyncRequestString SetCallback(FacebookDelegate callback)
		{
			this.callback = callback;
			return this;
		}
	}
}
