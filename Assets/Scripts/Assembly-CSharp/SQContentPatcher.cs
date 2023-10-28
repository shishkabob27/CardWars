using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zlib;
using MiniJSON;
using UnityEngine;

public class SQContentPatcher : EventDispatcher<string>
{
	private class Manifest
	{
		public Dictionary<string, ManifestEntry> Contents;

		public long Version { get; set; }

		public Manifest()
		{
			Contents = new Dictionary<string, ManifestEntry>();
		}
	}

	private class ManifestEntry
	{
		public string Name { get; set; }

		public string Digest { get; set; }

		public override string ToString()
		{
			return Name + "(" + Digest + ")";
		}
	}

	public delegate void PatchContentListener(string eventStr);

	public delegate void DownloadFileHandler(object sender, DownloadStringCompletedEventArgs e);

	private const string ETAG_FILE_NAME = "lastManifestEtag";

	private const string DIGEST_FILE_NAME = "ManifestDigest";

	public const string PATCHING_DONE_EVENT = "patchingDone";

	public const string PATCHING_NECESSARY_EVENT = "patchingNecessary";

	public const string PATCHING_NOT_NECESSARY_EVENT = "patchingNotNecessary";

	private Manifest _localManifest;

	private Manifest _remoteManifest;

	private string _manifestContents;

	private string _remoteManifestEtag;

	private Dictionary<string, ManifestEntry> _inProgressContent = new Dictionary<string, ManifestEntry>();

	private Dictionary<string, ManifestEntry> _remainingContent = new Dictionary<string, ManifestEntry>();

	private string dataPath;

	private string persistentAssetsPath;

	public bool ContentChanged;

	private bool patchingIsFinished;

	public SQContentPatcher()
	{
		dataPath = Path.Combine(Application.persistentDataPath, "TmpContents");
		persistentAssetsPath = TFUtils.GetPersistentAssetsPath();
	}

	public string LocalManifestVersion()
	{
		return _localManifest.Version.ToString();
	}

	public void ValidateAndFixDownloadedManifests()
	{
		if (!ValidateDownloadedManifests())
		{
			TFUtils.DebugLog("Bad downloaded manifest files detected. Removing bad files...");
			string path = persistentAssetsPath + Path.DirectorySeparatorChar + "manifest.json";
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			string path2 = persistentAssetsPath + Path.DirectorySeparatorChar + "lastManifestEtag";
			if (File.Exists(path2))
			{
				File.Delete(path2);
			}
			string path3 = persistentAssetsPath + Path.DirectorySeparatorChar + "ManifestDigest";
			if (File.Exists(path3))
			{
				File.Delete(path3);
			}
		}
	}

	public void ReadManifests()
	{
		string text = persistentAssetsPath + Path.DirectorySeparatorChar + "manifest.json";
		string filename = TFUtils.GetStreamingAssetsPath() + Path.DirectorySeparatorChar + "manifest.json";
		Manifest manifest = null;
		Manifest manifest2 = null;
		TFUtils.DebugLog("Reading bundled manifest");
		try
		{
			string jsonLocalContent = TFUtils.GetJsonLocalContent(filename);
			Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(jsonLocalContent);
			manifest = ReadManifest(dict);
		}
		catch (IOException)
		{
			manifest = ReadManifest(null);
		}
		TFUtils.DebugLog("Reading downloaded manifest at " + text);
		if (File.Exists(text))
		{
			TFUtils.DebugLog("Reading downloaded manifest");
			string json = File.ReadAllText(text);
			Dictionary<string, object> dict2 = (Dictionary<string, object>)Json.Deserialize(json);
			manifest2 = ReadManifest(dict2);
		}
		else
		{
			TFUtils.DebugLog("No downloaded manifest");
		}
		bool flag = true;
		if (manifest2 != null)
		{
			TFUtils.DebugLog(string.Format("Bundled content is running {0}, Downloaded content is running {1}", manifest.Version, manifest2.Version));
			flag = manifest.Version >= manifest2.Version;
			if (flag)
			{
				TFUtils.DebugLog("Bundled version is the same or greater than the downloaded version. Ignoring outdated downloaded content.");
			}
		}
		string path;
		if (flag)
		{
			if (Directory.Exists(TFUtils.GetPersistentAssetsPath()))
			{
				TFUtils.DebugLog("Erasing outdated downloaded content.");
				Directory.Delete(TFUtils.GetPersistentAssetsPath(), true);
			}
			_localManifest = manifest;
			path = TFUtils.GetStreamingAssetsPath() + Path.DirectorySeparatorChar + "lastManifestEtag";
		}
		else
		{
			_localManifest = manifest2;
			path = TFUtils.GetPersistentAssetsPath() + Path.DirectorySeparatorChar + "lastManifestEtag";
		}
		string currentETag = null;
		if (File.Exists(path))
		{
			currentETag = File.ReadAllText(path);
		}
		FetchRemoteManifest(currentETag);
	}

	private bool ValidateDownloadedManifests()
	{
		string text = persistentAssetsPath + Path.DirectorySeparatorChar;
		string text2 = text + "manifest.json";
		string path = text + "lastManifestEtag";
		string path2 = text + "ManifestDigest";
		bool flag = File.Exists(text2);
		bool flag2 = File.Exists(path);
		bool flag3 = File.Exists(path2);
		if (!flag || !flag2 || !flag3)
		{
			return !flag && !flag2 && !flag3;
		}
		string jsonLocalContent = TFUtils.GetJsonLocalContent(text2);
		if (!string.Equals(File.ReadAllText(path2), TFUtils.ComputeDigest(jsonLocalContent + File.ReadAllText(path)), StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		Manifest manifest = ReadManifest((Dictionary<string, object>)Json.Deserialize(jsonLocalContent));
		foreach (ManifestEntry value in manifest.Contents.Values)
		{
			string text3 = text + value.Name;
			if (!File.Exists(text3) || string.Equals(value.Digest, TFUtils.ComputeDigest(TFUtils.GetJsonLocalContent(text3)), StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			return false;
		}
		return true;
	}

	private Manifest ReadManifest(Dictionary<string, object> dict)
	{
		Manifest manifest = null;
		try
		{
			manifest = new Manifest();
			List<object> list = (List<object>)dict["contents"];
			manifest.Version = (long)dict["version"];
			foreach (object item in list)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
				ManifestEntry manifestEntry = new ManifestEntry();
				manifestEntry.Name = dictionary["n"].ToString();
				manifestEntry.Digest = dictionary["d"].ToString();
				manifest.Contents[manifestEntry.Name] = manifestEntry;
			}
		}
		catch (Exception)
		{
			manifest = new Manifest();
			manifest.Version = -1L;
		}
		return manifest;
	}

	private void FetchRemoteManifest(string currentETag)
	{
		GetFile(SQSettings.MANIFEST_URL, OnDownloadManifestComplete, currentETag);
	}

	private void OnDownloadManifestComplete(object sender, DownloadDataCompletedEventArgs e)
	{
		try
		{
			if (e.Error == null)
			{
				string text = decodeZippedData(e.Result);
				Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(text);
				_manifestContents = text;
				TFWebClient tFWebClient = (TFWebClient)sender;
				_remoteManifestEtag = tFWebClient.ResponseHeaders[HttpResponseHeader.ETag];
				_remoteManifest = ReadManifest(dict);
				ProcessManifests();
			}
			else
			{
				OnError(e.Error);
			}
		}
		catch (Exception e2)
		{
			OnError(e2);
		}
	}

	private void ProcessManifests()
	{
		ManifestEntry manifestEntry = null;
		TFUtils.DebugLog(string.Format("Processing manifests remote version {0} local version {1} ", _remoteManifest.Version, _localManifest.Version));
		if (_remoteManifest.Version > _localManifest.Version)
		{
			if (Directory.Exists(persistentAssetsPath))
			{
				string[] files = Directory.GetFiles(persistentAssetsPath, "*.*", SearchOption.AllDirectories);
				foreach (string text in files)
				{
					FileInfo fileInfo = new FileInfo(text);
					FileInfo fileInfo2 = new FileInfo(text.Replace(persistentAssetsPath, dataPath));
					if (!fileInfo2.Directory.Exists)
					{
						CreateDirectory(fileInfo2.FullName);
					}
					File.Copy(fileInfo.FullName, fileInfo2.FullName, true);
					TFUtils.DebugLog(string.Format("Copying {0} to {1}", fileInfo.FullName, fileInfo2.FullName));
				}
			}
			lock (this)
			{
				foreach (ManifestEntry value in _remoteManifest.Contents.Values)
				{
					if (_localManifest.Contents.ContainsKey(value.Name))
					{
						manifestEntry = _localManifest.Contents[value.Name];
						if (!manifestEntry.Digest.Equals(value.Digest))
						{
							TFUtils.DebugLog("Remote manifest contains new digest for " + value);
							string key = SQSettings.CDN_URL + value.Digest;
							_remainingContent[key] = value;
						}
					}
					else
					{
						TFUtils.DebugLog("Remote manifest contains entries that the local manifest does not have " + value.ToString());
						string key2 = SQSettings.CDN_URL + value.Digest;
						_remainingContent[key2] = value;
					}
				}
				if (_remainingContent.Keys.Count == 0)
				{
					FireEvent("patchingNotNecessary");
					PatchingDone();
				}
				else
				{
					FireEvent("patchingNecessary");
				}
				return;
			}
		}
		TFUtils.DebugLog("Remote manifest and local manifest both running content version " + _localManifest.Version);
		FireEvent("patchingNotNecessary");
		PatchingDone();
	}

	public void StartDownloadingPatchedContent()
	{
		lock (this)
		{
			IEnumerable<KeyValuePair<string, ManifestEntry>> enumerable = _remainingContent.Take(SQSettings.PATCHING_FILE_LIMIT - _inProgressContent.Count);
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, ManifestEntry> item in enumerable)
			{
				GetFile(item.Key, OnDownloadComplete);
				_inProgressContent.Add(item.Key, item.Value);
				list.Add(item.Key);
			}
			foreach (string item2 in list)
			{
				_remainingContent.Remove(item2);
			}
		}
	}

	private void GetFile(string url, DownloadDataCompletedEventHandler onDownloadComplete, string currentETag = null)
	{
		using (TFWebClient tFWebClient = new TFWebClient(null))
		{
			TFUtils.DebugLog("fetching content from " + url);
			if (currentETag != null)
			{
				tFWebClient.Headers.Add(HttpRequestHeader.IfNoneMatch, currentETag);
			}
			tFWebClient.DownloadDataCompleted += onDownloadComplete;
			tFWebClient.NetworkError += OnNetworkError;
			tFWebClient.DownloadDataAsync(new Uri(url), url);
		}
	}

	private static bool IsExpectedStatus(Exception ex)
	{
		WebException ex2 = ex as WebException;
		if (ex2 != null)
		{
			HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
			if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.NotModified)
			{
				return true;
			}
		}
		return false;
	}

	private void OnNetworkError(object sender, WebException e)
	{
		if (!IsExpectedStatus(e))
		{
			TFUtils.DebugLog("Error downloading file during content patching. Continuing without version patching");
		}
	}

	private void OnError(Exception e)
	{
		if (!IsExpectedStatus(e))
		{
			TFUtils.DebugLog(e);
		}
		else
		{
			FireEvent("patchingNotNecessary");
		}
		PatchingDone();
	}

	private static void CreateDirectory(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		Directory.CreateDirectory(directoryName);
	}

	private void SaveContentFile(string path, string contents)
	{
		string text = dataPath + Path.DirectorySeparatorChar + path;
		CreateDirectory(text);
		File.WriteAllText(text, contents);
		TFUtils.DebugLog("Saved patch file" + text);
	}

	protected static string decodeZippedData(byte[] input)
	{
		byte[] array = null;
		string text = null;
		if (input == null || input.Length < 3)
		{
			TFUtils.DebugLog("Empty or small data to decode");
			return "{}";
		}
		array = ((input[0] != 72 || input[1] != 52 || input[2] != 115) ? input : Convert.FromBase64String(Encoding.UTF8.GetString(input)));
		try
		{
			return TFUtils.Unzip(array);
		}
		catch (ZlibException ex)
		{
			TFUtils.DebugLog(ex.ToString());
			return Encoding.UTF8.GetString(array);
		}
	}

	private void OnDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
	{
		try
		{
			if (e.Error == null)
			{
				lock (this)
				{
					TFWebClient tFWebClient = (TFWebClient)sender;
					string text = e.UserState.ToString();
					TFUtils.DebugLog(text + " has finished downloading");
					if (_inProgressContent.ContainsKey(text))
					{
						ManifestEntry manifestEntry = _inProgressContent[text];
						TFUtils.DebugLog("Saving " + text + " to " + dataPath + manifestEntry.Name);
						string contents = decodeZippedData(e.Result);
						SaveContentFile(manifestEntry.Name, contents);
						_inProgressContent.Remove(text);
						StartDownloadingPatchedContent();
					}
					else
					{
						TFUtils.DebugLog(string.Concat("Entry ", tFWebClient.QueryString, " does not exist in the in progress content"));
					}
					if (_remainingContent.Keys.Count == 0 && _inProgressContent.Count == 0)
					{
						PatchingSuccess();
					}
					else
					{
						TFUtils.DebugLog(string.Format("{0} pieces of patched content left with {1} in progress", _remainingContent.Count, _inProgressContent.Count));
					}
					return;
				}
			}
			OnError(e.Error);
		}
		catch (Exception ex)
		{
			TFUtils.ErrorLog(ex);
			OnError(ex);
		}
	}

	private void PatchingSuccess()
	{
		SaveContentFile("manifest.json", _manifestContents);
		SaveContentFile("lastManifestEtag", _remoteManifestEtag);
		SaveContentFile("ManifestDigest", TFUtils.ComputeDigest(_manifestContents + _remoteManifestEtag));
		if (Directory.Exists(persistentAssetsPath))
		{
			Directory.Delete(persistentAssetsPath, true);
		}
		Directory.Move(dataPath, persistentAssetsPath);
		ContentChanged = true;
		PatchingDone();
	}

	private void PatchingDone()
	{
		lock (this)
		{
			if (!patchingIsFinished)
			{
				FireEvent("patchingDone");
				patchingIsFinished = true;
			}
		}
	}

	public void PerformSanityCheck()
	{
		foreach (ManifestEntry value in _remoteManifest.Contents.Values)
		{
			string text = null;
			string text2 = null;
			if (value.Name.Contains("Language"))
			{
				string text3 = Application.dataPath + Path.PathSeparator + value.Name;
				if (File.Exists(text3))
				{
					text = text3;
					TFUtils.DebugLog("Latest asset for " + value.Name + " is at " + text);
					text2 = File.ReadAllText(text);
				}
				else
				{
					TFUtils.DebugLog("Latest asset for " + value.Name + " is at in bundled resources");
					TextAsset textAsset = (TextAsset)SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(value.Name.Substring(0, value.Name.Length - 4), typeof(TextAsset));
					text2 = textAsset.text;
				}
			}
			else
			{
				text = TFUtils.GetStreamingAssetsFile(value.Name);
				TFUtils.DebugLog("Latest asset for " + value.Name + " is at " + text);
				text2 = File.ReadAllText(text);
			}
			string text4 = CalculateMD5Hash(text2);
			if (!value.Digest.Equals(text4))
			{
				TFUtils.ErrorLog(string.Concat("After done with patching, assets do not match digest ", value, " current digest is ", text4));
			}
		}
		TFUtils.DebugLog("Asset contents have been verified.");
	}

	public string CalculateMD5Hash(string input)
	{
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(input);
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
