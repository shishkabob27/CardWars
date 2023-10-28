using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Prime31
{
	public class P31RestKit
	{
		protected string _baseUrl;

		public bool debugRequests = false;

		protected bool forceJsonResponse;

		private GameObject _surrogateGameObject;

		private MonoBehaviour _surrogateMonobehaviour;

		protected virtual GameObject surrogateGameObject
		{
			get
			{
				if (_surrogateGameObject == null)
				{
					_surrogateGameObject = GameObject.Find("P31CoroutineSurrogate");
					if (_surrogateGameObject == null)
					{
						_surrogateGameObject = new GameObject("P31CoroutineSurrogate");
						UnityEngine.Object.DontDestroyOnLoad(_surrogateGameObject);
					}
				}
				return _surrogateGameObject;
			}
			set
			{
				_surrogateGameObject = value;
			}
		}

		protected MonoBehaviour surrogateMonobehaviour
		{
			get
			{
				if (_surrogateMonobehaviour == null)
				{
					_surrogateMonobehaviour = surrogateGameObject.AddComponent<MonoBehaviour>();
				}
				return _surrogateMonobehaviour;
			}
			set
			{
				_surrogateMonobehaviour = value;
			}
		}

		protected virtual IEnumerator send(string path, HTTPVerb httpVerb, Dictionary<string, object> parameters, Action<string, object> onComplete)
		{
			if (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}
			WWW www = processRequest(path, httpVerb, parameters);
			yield return www;
			if (debugRequests)
			{
				Debug.Log("response error: " + www.error);
				Debug.Log("response text: " + www.text);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Response Headers:\n");
				foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
				{
					stringBuilder.AppendFormat("{0}: {1}\n", responseHeader.Key, responseHeader.Value);
				}
				Debug.Log(stringBuilder.ToString());
			}
			if (onComplete != null)
			{
				processResponse(www, onComplete);
			}
			www.Dispose();
		}

		protected virtual WWW processRequest(string path, HTTPVerb httpVerb, Dictionary<string, object> parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!path.StartsWith("http"))
			{
				stringBuilder.Append(_baseUrl).Append(path);
			}
			else
			{
				stringBuilder.Append(path);
			}
			bool flag = httpVerb != HTTPVerb.GET;
			WWWForm wWWForm = ((!flag) ? null : new WWWForm());
			if (parameters != null && parameters.Count > 0)
			{
				if (flag)
				{
					foreach (KeyValuePair<string, object> parameter in parameters)
					{
						if (parameter.Value is string)
						{
							wWWForm.AddField(parameter.Key, parameter.Value as string);
						}
						else if (parameter.Value is byte[])
						{
							wWWForm.AddBinaryData(parameter.Key, parameter.Value as byte[]);
						}
					}
				}
				else
				{
					bool flag2 = true;
					if (path.Contains("?"))
					{
						flag2 = false;
					}
					foreach (KeyValuePair<string, object> parameter2 in parameters)
					{
						if (parameter2.Value is string)
						{
							stringBuilder.AppendFormat("{0}{1}={2}", (!flag2) ? "&" : "?", WWW.EscapeURL(parameter2.Key), WWW.EscapeURL(parameter2.Value as string));
							flag2 = false;
						}
					}
				}
			}
			if (debugRequests)
			{
				Debug.Log("url: " + stringBuilder.ToString());
			}
			return (!flag) ? new WWW(stringBuilder.ToString()) : new WWW(stringBuilder.ToString(), wWWForm);
		}

		protected virtual void processResponse(WWW www, Action<string, object> onComplete)
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				onComplete(www.error, null);
			}
			else if (isResponseJson(www))
			{
				object obj = Json.decode(www.text);
				if (obj == null)
				{
					obj = www.text;
				}
				onComplete(null, obj);
			}
			else
			{
				onComplete(null, www.text);
			}
		}

		protected bool isResponseJson(WWW www)
		{
			bool flag = false;
			if (forceJsonResponse)
			{
				flag = true;
			}
			if (!flag)
			{
				foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
				{
					if (responseHeader.Key.ToLower() == "content-type" && (responseHeader.Value.ToLower().Contains("/json") || responseHeader.Value.ToLower().Contains("/javascript")))
					{
						flag = true;
					}
				}
			}
			if (flag && !www.text.StartsWith("[") && !www.text.StartsWith("{"))
			{
				return false;
			}
			return flag;
		}

		public void get(string path, Action<string, object> completionHandler)
		{
			get(path, null, completionHandler);
		}

		public void get(string path, Dictionary<string, object> parameters, Action<string, object> completionHandler)
		{
			surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.GET, parameters, completionHandler));
		}

		public void post(string path, Dictionary<string, object> parameters, Action<string, object> completionHandler)
		{
			surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.POST, parameters, completionHandler));
		}
	}
}
