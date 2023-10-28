using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Prime31
{
	public class Facebook : P31RestKit
	{
		public string accessToken;

		public string appAccessToken;

		public bool useSessionBabysitter = true;

		private static Facebook _instance;

		public static Facebook instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Facebook();
				}
				return _instance;
			}
		}

		public Facebook()
		{
			_baseUrl = "https://graph.facebook.com/";
			forceJsonResponse = true;
		}

		protected override IEnumerator send(string path, HTTPVerb httpVerb, Dictionary<string, object> parameters, Action<string, object> onComplete)
		{
			if (parameters == null)
			{
				parameters = new Dictionary<string, object>();
			}
			if (!parameters.ContainsKey("access_token"))
			{
				parameters.Add("access_token", accessToken);
			}
			if (httpVerb == HTTPVerb.PUT || httpVerb == HTTPVerb.DELETE)
			{
				parameters.Add("method", httpVerb.ToString());
			}
			return base.send(path, httpVerb, parameters, onComplete);
		}

		protected bool shouldSendRequest()
		{
			if (!useSessionBabysitter)
			{
				return true;
			}
			try
			{
				Type type = typeof(Facebook).Assembly.GetType("FacebookAndroid");
				if (type != null)
				{
					MethodInfo method = type.GetMethod("isSessionValid", BindingFlags.Static | BindingFlags.Public);
					object obj = method.Invoke(null, null);
					return (bool)obj;
				}
			}
			catch (Exception)
			{
			}
			return true;
		}

		public void prepareForMetroUse(GameObject go, MonoBehaviour mb)
		{
			UnityEngine.Object.DontDestroyOnLoad(go);
			surrogateGameObject = go;
			base.surrogateMonobehaviour = mb;
		}

		public void graphRequest(string path, Action<string, object> completionHandler)
		{
			graphRequest(path, HTTPVerb.GET, completionHandler);
		}

		public void graphRequest(string path, HTTPVerb verb, Action<string, object> completionHandler)
		{
			graphRequest(path, verb, null, completionHandler);
		}

		public void graphRequest(string path, HTTPVerb verb, Dictionary<string, object> parameters, Action<string, object> completionHandler)
		{
			if (shouldSendRequest())
			{
				base.surrogateMonobehaviour.StartCoroutine(send(path, verb, parameters, completionHandler));
				return;
			}
			try
			{
				Type type = typeof(Facebook).Assembly.GetType("FacebookAndroid");
				if (type == null)
				{
					return;
				}
				MethodInfo method = type.GetMethod("babysitRequest", BindingFlags.Static | BindingFlags.NonPublic);
				if (method != null)
				{
					Action action = delegate
					{
						base.surrogateMonobehaviour.StartCoroutine(send(path, verb, parameters, completionHandler));
					};
					method.Invoke(null, new object[2]
					{
						verb == HTTPVerb.POST,
						action
					});
				}
			}
			catch (Exception)
			{
			}
		}

		public void graphRequestBatch(IEnumerable<FacebookBatchRequest> requests, Action<string, object> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (FacebookBatchRequest request in requests)
			{
				list.Add(request.requestDictionary());
			}
			dictionary.Add("batch", Json.encode(list));
			base.surrogateMonobehaviour.StartCoroutine(send(string.Empty, HTTPVerb.POST, dictionary, completionHandler));
		}

		public void fetchProfileImageForUserId(string userId, Action<Texture2D> completionHandler)
		{
			string url = "http://graph.facebook.com/" + userId + "/picture?type=large";
			base.surrogateMonobehaviour.StartCoroutine(fetchImageAtUrl(url, completionHandler));
		}

		public IEnumerator fetchImageAtUrl(string url, Action<Texture2D> completionHandler)
		{
			WWW www = new WWW(url);
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				if (completionHandler != null)
				{
					completionHandler(null);
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(www.texture);
			}
		}

		public void postMessage(string message, Action<string, object> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("message", message);
			Dictionary<string, object> parameters = dictionary;
			graphRequest("me/feed", HTTPVerb.POST, parameters, completionHandler);
		}

		public void postMessageWithLink(string message, string link, string linkName, Action<string, object> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("message", message);
			dictionary.Add("link", link);
			dictionary.Add("name", linkName);
			Dictionary<string, object> parameters = dictionary;
			graphRequest("me/feed", HTTPVerb.POST, parameters, completionHandler);
		}

		public void postMessageWithLinkAndLinkToImage(string message, string link, string linkName, string linkToImage, string caption, Action<string, object> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("message", message);
			dictionary.Add("link", link);
			dictionary.Add("name", linkName);
			dictionary.Add("picture", linkToImage);
			dictionary.Add("caption", caption);
			Dictionary<string, object> parameters = dictionary;
			graphRequest("me/feed", HTTPVerb.POST, parameters, completionHandler);
		}

		public void postImage(byte[] image, string message, Action<string, object> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("picture", image);
			dictionary.Add("message", message);
			Dictionary<string, object> parameters = dictionary;
			graphRequest("me/photos", HTTPVerb.POST, parameters, completionHandler);
		}

		public void postImageToAlbum(byte[] image, string caption, string albumId, Action<string, object> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("picture", image);
			dictionary.Add("message", caption);
			Dictionary<string, object> parameters = dictionary;
			graphRequest(albumId, HTTPVerb.POST, parameters, completionHandler);
		}

		public void getMe(Action<string, FacebookMeResult> completionHandler)
		{
			graphRequest("me", delegate(string error, object obj)
			{
				if (completionHandler != null)
				{
					if (error != null)
					{
						completionHandler(error, null);
					}
					else
					{
						completionHandler(null, Json.decodeObject<FacebookMeResult>(obj));
					}
				}
			});
		}

		public void getFriends(Action<string, FacebookFriendsResult> completionHandler)
		{
			graphRequest("me/friends", delegate(string error, object obj)
			{
				if (completionHandler != null)
				{
					if (error != null)
					{
						completionHandler(error, null);
					}
					else
					{
						completionHandler(null, Json.decodeObject<FacebookFriendsResult>(obj));
					}
				}
			});
		}

		public void extendAccessToken(string appId, string appSecret, Action<DateTime?> completionHandler)
		{
			if (instance.accessToken == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("client_id", appId);
			dictionary.Add("client_secret", appSecret);
			dictionary.Add("grant_type", "fb_exchange_token");
			dictionary.Add("fb_exchange_token", instance.accessToken);
			Dictionary<string, object> parameters = dictionary;
			get("oauth/access_token", parameters, delegate(string error, object obj)
			{
				if (obj is string)
				{
					string text = obj as string;
					if (text.StartsWith("access_token="))
					{
						Dictionary<string, string> dictionary2 = text.parseQueryString();
						instance.accessToken = dictionary2["access_token"];
						double value = double.Parse(dictionary2["expires"]);
						completionHandler(DateTime.Now.AddSeconds(value));
					}
					else
					{
						completionHandler(null);
					}
				}
				else
				{
					completionHandler(null);
				}
			});
		}

		public void checkSessionValidityOnServer(Action<bool> completionHandler)
		{
			get("me", delegate(string error, object obj)
			{
				if (error == null && obj != null && obj is IDictionary)
				{
					completionHandler(true);
				}
				else
				{
					completionHandler(false);
				}
			});
		}

		public void getSessionPermissionsOnServer(Action<string, List<string>> completionHandler)
		{
			get("me/permissions", delegate(string error, object obj)
			{
				if (error == null && obj != null && obj is IDictionary)
				{
					IDictionary dictionary = obj as IDictionary;
					IList list = dictionary["data"] as IList;
					IDictionary dictionary2 = list[0] as IDictionary;
					List<string> list2 = new List<string>();
					foreach (DictionaryEntry item in dictionary2)
					{
						if (item.Value.ToString() == "1")
						{
							list2.Add(item.Key.ToString());
						}
					}
					completionHandler(null, list2);
				}
				else
				{
					completionHandler(error, null);
				}
			});
		}

		public void getAppAccessToken(string appId, string appSecret, Action<string> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("client_id", appId);
			dictionary.Add("client_secret", appSecret);
			dictionary.Add("grant_type", "client_credentials");
			Dictionary<string, object> parameters = dictionary;
			get("oauth/access_token", parameters, delegate(string error, object obj)
			{
				if (obj is string)
				{
					string text = obj as string;
					if (text.StartsWith("access_token="))
					{
						appAccessToken = text.Replace("access_token=", string.Empty);
						completionHandler(appAccessToken);
					}
					else
					{
						completionHandler(null);
					}
				}
				else
				{
					completionHandler(null);
				}
			});
		}

		public void postScore(int score, Action<bool> completionHandler)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("score", score.ToString());
			Dictionary<string, object> parameters = dictionary;
			post("me/scores", parameters, delegate(string error, object obj)
			{
				if (error == null && obj is string)
				{
					completionHandler(((string)obj).ToLower() == "true");
				}
				else
				{
					completionHandler(false);
				}
			});
		}

		public void getScores(string userId, Action<string, object> onComplete)
		{
			string path = userId + "/scores";
			graphRequest(path, onComplete);
		}
	}
}
