using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class SLOTAtlasManager : SLOTGameSingleton<SLOTAtlasManager>
{
	public class WebClientParams
	{
		public string origTextureFilename;

		public string url;

		public string eTag;

		public string textureName;

		public bool cacheInMemory;

		public bool persistent;

		public string lastEtagPath;

		public string persistentDataPath;

		public int param;

		public int retryCount;
	}

	public class WebClient2 : WebClient
	{
		public string lastModifiedDatePath;

		public WebClientParams parameters = new WebClientParams();

		private string eTag;

		public WebClient2(string etag)
		{
			eTag = etag;
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest httpWebRequest = base.GetWebRequest(uri) as HttpWebRequest;
			if (eTag != null)
			{
				httpWebRequest.Headers.Add(HttpRequestHeader.IfNoneMatch, eTag);
			}
			return httpWebRequest;
		}
	}

	private class LoadTextureWebResponseInfo
	{
		public int type;

		public string textureName;

		public byte[] bytes;

		public string eTag;

		public WebClientParams param;
	}

	public delegate void LoadTextureCallback(Texture loadedTexture, string textureName);

	public delegate void NotModifiedCallback();

	public const string LOCAL_TEXTURE_CACHE_DIRECTORY = "TextureCache";

	public const string ETAG_EXTENSION = ".SlotRevolutionEtag";

	public const int MAX_RETRY_COUNT = 3;

	public UIAtlas[] atlases;

	public UIAtlas[] lowresAtlases;

	public UIFont[] fonts;

	public UIFont[] lowresFonts;

	private Dictionary<string, UIAtlas> atlasDict;

	private Dictionary<string, Texture> textureDict;

	private Dictionary<string, UIFont> fontDict;

	private HashSet<string> loadingTextures = new HashSet<string>();

	private int texturesToCache;

	private List<LoadTextureWebResponseInfo> responseList = new List<LoadTextureWebResponseInfo>();

	public static string ASSET_BUNDLE_SERVER_TEXTURES_URL
	{
		get
		{
			return string.Empty;
		}
	}

	public static string ASSET_BUNDLE_SERVER_TEXTURES_URL_HTTP
	{
		get
		{
			return "http://" + ASSET_BUNDLE_SERVER_TEXTURES_URL;
		}
	}

	public static string ASSET_BUNDLE_SERVER_TEXTURES_URL_HTTPS
	{
		get
		{
			return "https://" + ASSET_BUNDLE_SERVER_TEXTURES_URL;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			CreateAtlasDictionary();
			CreateTextureDictionary();
			CreateFontDictionary();
		}
	}

	private bool IsLowEndDevice()
	{
		return false;
	}

	private void CreateAtlasDictionary()
	{
		if (IsLowEndDevice())
		{
			CreateAtlasDictionary(lowresAtlases);
			atlases = lowresAtlases;
		}
		else
		{
			CreateAtlasDictionary(atlases);
			lowresAtlases = atlases;
		}
	}

	private void CreateAtlasDictionary(UIAtlas[] atlases)
	{
		if (atlases != null)
		{
			atlasDict = new Dictionary<string, UIAtlas>();
			foreach (UIAtlas atlas in atlases)
			{
				AddAtlas(atlas);
			}
		}
	}

	private void CreateTextureDictionary()
	{
		textureDict = new Dictionary<string, Texture>();
	}

	private void CreateFontDictionary()
	{
		if (IsLowEndDevice())
		{
			CreateFontDictionary(lowresFonts);
			fonts = lowresFonts;
		}
		else
		{
			CreateFontDictionary(fonts);
			lowresFonts = fonts;
		}
	}

	private void CreateFontDictionary(UIFont[] fonts)
	{
		if (fonts != null)
		{
			fontDict = new Dictionary<string, UIFont>();
			foreach (UIFont font in fonts)
			{
				AddFont(font);
			}
		}
	}

	public UIAtlas GetAtlas(string atlasName)
	{
		if (atlasDict == null)
		{
			CreateAtlasDictionary();
		}
		if (atlasDict != null && atlasDict.ContainsKey(atlasName))
		{
			return atlasDict[atlasName];
		}
		return null;
	}

	public UIAtlas.Sprite GetSprite(string atlasName, string spriteName)
	{
		UIAtlas atlas = GetAtlas(atlasName);
		if (atlas != null)
		{
			return atlas.GetSprite(spriteName);
		}
		return null;
	}

	public Texture GetTexture(string textureName)
	{
		if (textureDict == null)
		{
			CreateTextureDictionary();
		}
		if (textureDict != null && textureDict.ContainsKey(textureName))
		{
			return textureDict[textureName];
		}
		return null;
	}

	public UIFont GetFont(string fontName)
	{
		if (fontDict == null)
		{
			CreateFontDictionary();
		}
		if (fontDict != null && fontDict.ContainsKey(fontName))
		{
			return fontDict[fontName];
		}
		return null;
	}

	public void AddAtlas(UIAtlas atlas)
	{
		if (atlas != null && !atlasDict.ContainsKey(atlas.name))
		{
			atlasDict[atlas.name] = atlas;
		}
	}

	public bool AddTexture(Texture tex, string textureNameOverride = null)
	{
		if (tex != null)
		{
			string key = ((textureNameOverride == null) ? tex.name : textureNameOverride);
			if (!textureDict.ContainsKey(key))
			{
				textureDict[key] = tex;
				return true;
			}
		}
		return false;
	}

	public void RemoveTexture(Texture tex)
	{
		if (tex != null && textureDict.ContainsKey(tex.name))
		{
			textureDict.Remove(tex.name);
		}
	}

	public void CacheTexture(string textureFilename, string textureNameOverride = null)
	{
		if (textureDict == null)
		{
			CreateTextureDictionary();
		}
		string text = ((textureNameOverride == null) ? textureFilename : textureNameOverride);
		if ((text != null && textureDict.ContainsKey(text)) || (text != null && loadingTextures.Contains(text)))
		{
			return;
		}
		loadingTextures.Add(text);
		texturesToCache++;
		string text2 = null;
		string eTag = null;
		if (text != null)
		{
			text2 = Path.Combine("TextureCache", textureFilename + ".SlotRevolutionEtag");
			if (SLOTGame.DoesPersistentDataExist(text2))
			{
				byte[] bytes = SLOTGame.LoadPersistentData(text2);
				eTag = SLOTGame.BytesToString(bytes);
			}
		}
		LoadTextureFromCacheOrServer(textureFilename, eTag, text, true, true, text2);
	}

	public void UncacheTexture(string textureFilename)
	{
		if (textureFilename != null && textureDict.ContainsKey(textureFilename))
		{
			Texture texture = textureDict[textureFilename];
			if (texture != null)
			{
				UnityEngine.Object.DestroyImmediate(texture, true);
			}
			textureDict.Remove(textureFilename);
		}
	}

	public bool AreAllTexturesCached()
	{
		return texturesToCache == 0;
	}

	public void LoadTextureFromAssetBundleServer(LoadTextureCallback callback, string textureFilename, string textureNameOverride = null)
	{
		StartCoroutine(LoadTextureCoroutine(textureFilename, Path.Combine(ASSET_BUNDLE_SERVER_TEXTURES_URL_HTTPS, textureFilename), callback, false, false, (textureNameOverride == null) ? textureFilename : textureNameOverride));
	}

	public void LoadTexture(string origTextureFilename, string url, LoadTextureCallback callback, bool cacheInMemory = false, bool persistent = false, string textureName = null)
	{
		StartCoroutine(LoadTextureCoroutine(origTextureFilename, url, callback, cacheInMemory, persistent, textureName));
	}

	private IEnumerator LoadTextureCoroutine(string origTextureFilename, string url, LoadTextureCallback callback, bool cacheInMemory, bool persistent, string textureName, bool useLastEtag = true, string etag = null)
	{
		bool loaded = false;
		string lastEtagPath = null;
		if (persistent && textureName != null)
		{
			lastEtagPath = Path.Combine("TextureCache", origTextureFilename + ".SlotRevolutionEtag");
			if (useLastEtag && SLOTGame.DoesPersistentDataExist(lastEtagPath))
			{
				byte[] bytes3 = SLOTGame.LoadPersistentData(lastEtagPath);
				etag = SLOTGame.BytesToString(bytes3);
			}
		}
		WWWForm form2 = null;
		Dictionary<string, string> headers = null;
		if (useLastEtag && etag != null)
		{
			form2 = new WWWForm();
			form2.AddField("name", "value");
			headers = form2.headers;
			try
			{
				headers["If-None-Match"] = etag;
			}
			catch (KeyNotFoundException)
			{
				headers.Add("If-None-Match", etag);
			}
		}
		using (WWW www = new WWW(url, null, headers))
		{
			yield return www;
			if (www.error == null && www.texture != null)
			{
				if (textureName == null)
				{
					textureName = www.texture.name;
				}
				else
				{
					www.texture.name = textureName;
				}
				if (cacheInMemory)
				{
					AddTexture(www.texture, textureName);
				}
				if (persistent)
				{
					if (etag != null)
					{
						byte[] bytes2 = SLOTGame.StringToBytes(etag);
						SLOTGame.SavePersistentData(lastEtagPath, bytes2);
					}
					else if (www.responseHeaders != null && www.responseHeaders.ContainsKey("ETAG"))
					{
						byte[] bytes = SLOTGame.StringToBytes(www.responseHeaders["ETAG"]);
						SLOTGame.SavePersistentData(lastEtagPath, bytes);
					}
					SLOTGame.SavePersistentData(Path.Combine("TextureCache", origTextureFilename), www.bytes);
				}
				if (callback != null)
				{
					callback(www.texture, textureName);
				}
				loaded = true;
			}
			else if (www.error == null)
			{
			}
		}
		if (!loaded && callback != null)
		{
			callback(null, textureName);
		}
	}

	private void LoadTextureFromCacheOrServer(string origTextureFilename, string eTag = null, string textureName = null, bool cacheInMemory = false, bool persistent = false, string lastEtagPath = null, int retryCount = 0)
	{
		string text = Path.Combine(ASSET_BUNDLE_SERVER_TEXTURES_URL_HTTP, origTextureFilename);
		WebClientParams webClientParams = new WebClientParams();
		webClientParams.origTextureFilename = origTextureFilename;
		webClientParams.url = text;
		webClientParams.eTag = eTag;
		webClientParams.textureName = textureName;
		webClientParams.cacheInMemory = cacheInMemory;
		webClientParams.persistent = persistent;
		webClientParams.lastEtagPath = lastEtagPath;
		webClientParams.persistentDataPath = Application.persistentDataPath;
		webClientParams.retryCount = retryCount;
		using (WebClient2 webClient = new WebClient2(eTag))
		{
			webClient.parameters = webClientParams;
			webClient.DownloadDataCompleted += OnLoadTextureCompleted;
			webClient.DownloadDataAsync(new Uri(text), text);
		}
	}

	private void OnLoadTextureCompleted(object sender, DownloadDataCompletedEventArgs e)
	{
		WebClient2 webClient = (WebClient2)sender;
		try
		{
			WebClientParams parameters = webClient.parameters;
			if (e.Error == null)
			{
				byte[] result = e.Result;
				string textureName = parameters.textureName;
				if (result != null)
				{
					LoadTextureWebResponseInfo loadTextureWebResponseInfo = new LoadTextureWebResponseInfo();
					loadTextureWebResponseInfo.type = 0;
					loadTextureWebResponseInfo.textureName = textureName;
					loadTextureWebResponseInfo.bytes = result;
					loadTextureWebResponseInfo.param = parameters;
					string eTag = webClient.ResponseHeaders[HttpResponseHeader.ETag];
					loadTextureWebResponseInfo.eTag = eTag;
					lock (this)
					{
						responseList.Add(loadTextureWebResponseInfo);
						return;
					}
				}
				OnLoadTextureDone(parameters);
				return;
			}
			bool flag = false;
			if (IsNotModifiedException(e.Error))
			{
				flag = true;
			}
			else if (IsNotFoundException(e.Error))
			{
				flag = true;
			}
			else if (parameters.retryCount < 3)
			{
				LoadTextureFromCacheOrServer(parameters.origTextureFilename, parameters.eTag, parameters.textureName, parameters.cacheInMemory, parameters.persistent, parameters.lastEtagPath, parameters.retryCount + 1);
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			LoadTextureWebResponseInfo loadTextureWebResponseInfo2 = new LoadTextureWebResponseInfo();
			loadTextureWebResponseInfo2.type = 1;
			loadTextureWebResponseInfo2.param = parameters;
			lock (this)
			{
				responseList.Add(loadTextureWebResponseInfo2);
			}
		}
		catch (Exception)
		{
			OnLoadTextureDone(webClient.parameters);
		}
	}

	private void Update()
	{
		lock (this)
		{
			foreach (LoadTextureWebResponseInfo response in responseList)
			{
				if (response.type == 0)
				{
					LoadTextureFromWebResponse(response);
				}
				else
				{
					LoadLocalTexture(response);
				}
			}
			responseList.Clear();
		}
	}

	private void LoadTextureFromWebResponse(LoadTextureWebResponseInfo info)
	{
		Texture2D texture2D = new Texture2D(4, 4, TextureFormat.ARGB32, false);
		texture2D.LoadImage(info.bytes);
		string textureName = info.textureName;
		if (textureName == null)
		{
			textureName = texture2D.name;
		}
		else
		{
			texture2D.name = textureName;
		}
		if (info.param.cacheInMemory && !AddTexture(texture2D, textureName))
		{
			UnityEngine.Object.DestroyImmediate(texture2D, true);
		}
		SLOTGame.SavePersistentData(Path.Combine("TextureCache", info.param.origTextureFilename), info.bytes);
		byte[] bytes = SLOTGame.StringToBytes(info.eTag);
		SLOTGame.SavePersistentData(info.param.lastEtagPath, bytes);
		OnLoadTextureDone(info.param);
	}

	private void LoadLocalTexture(LoadTextureWebResponseInfo info)
	{
		byte[] array = SLOTGame.LoadPersistentData(Path.Combine("TextureCache", info.param.origTextureFilename));
		if (array != null)
		{
			Texture2D texture2D = new Texture2D(4, 4, TextureFormat.ARGB32, false);
			texture2D.LoadImage(array);
			string textureName = info.param.textureName;
			if (textureName == null)
			{
				textureName = texture2D.name;
			}
			else
			{
				texture2D.name = textureName;
			}
			if (info.param.cacheInMemory && !AddTexture(texture2D, textureName))
			{
				UnityEngine.Object.DestroyImmediate(texture2D, true);
			}
		}
		OnLoadTextureDone(info.param);
	}

	private void OnLoadTextureDone(WebClientParams clientParams)
	{
		texturesToCache--;
		loadingTextures.Remove(clientParams.textureName);
	}

	private bool IsNotModifiedException(Exception ex)
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

	private bool IsNotFoundException(Exception ex)
	{
		WebException ex2 = ex as WebException;
		if (ex2 != null)
		{
			HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
			if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.NotFound)
			{
				return true;
			}
		}
		return false;
	}

	public void AddFont(UIFont font)
	{
		if (font != null && !fontDict.ContainsKey(font.name))
		{
			fontDict[font.name] = font;
		}
	}

	public GameObject GetAtlasFunction(string atlasName)
	{
		UIAtlas atlas = GetAtlas(atlasName);
		if (atlas != null)
		{
			return atlas.gameObject;
		}
		return null;
	}

	public UIFont GetFontFunction(string fontName)
	{
		UIFont font = GetFont(fontName);
		if (font != null)
		{
			return font;
		}
		return null;
	}

	public void LoadResourcesFromAssetBundle(AssetBundle bundle)
	{
		LoadTexturesFromAssetBundle(bundle);
		LoadAtlasesFromAssetBundle(bundle);
	}

	public void LoadTexturesFromAssetBundle(AssetBundle bundle)
	{
		if (!(bundle != null))
		{
			return;
		}
		bool flag = DoesAssetBundleContainAtlases(bundle);
		UnityEngine.Object[] array = bundle.LoadAllAssets(typeof(Texture));
		if (array == null)
		{
			return;
		}
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			Texture texture = @object as Texture;
			if (texture != null && !flag)
			{
				AddTexture(texture);
			}
		}
	}

	public void LoadAtlasesFromAssetBundle(AssetBundle bundle)
	{
		if (!(bundle != null))
		{
			return;
		}
		UnityEngine.Object[] array = bundle.LoadAllAssets(typeof(UIAtlas));
		if (array == null)
		{
			return;
		}
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			UIAtlas uIAtlas = @object as UIAtlas;
			if (uIAtlas != null)
			{
				AddAtlas(uIAtlas);
			}
		}
	}

	public bool DoesAssetBundleContainAtlases(AssetBundle bundle)
	{
		UnityEngine.Object[] array = bundle.LoadAllAssets(typeof(UIAtlas));
		return array != null && array.Length > 0;
	}
}
