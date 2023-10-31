using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

public class Player
{
	private const string UNAUTHED_FOLDER = "p_ua";

	private const string UNAUTHED_PLAYER_ID = "ua";

	private const string LAST_PLAYED = "lastplayer";

	public const string LOCAL_TEXTURE_CACHE = "TextureCache";

	private const string USER_FILE = "user.json";

	public string playerId;

	public bool isNew;

	private static readonly string CACHE_ROOT = Application.persistentDataPath + Path.DirectorySeparatorChar;

	private static readonly string LAST_PLAYED_FILE = CACHE_ROOT + "lastplayer";

	public static readonly string LOCAL_TEXTURE_CACHE_DIRECTORY = CACHE_ROOT + "TextureCache";

	private string cacheDir;

	public Player(string playerId, bool isNew)
	{
		TFUtils.DebugLog("new player: " + playerId + " isNew: " + isNew);
		this.playerId = playerId;
		this.isNew = isNew;
		cacheDir = CACHE_ROOT + PlayerFolder();
		if (!isNew || !(playerId != "ua"))
		{
			return;
		}
		Player player = LoadFromFilesystem();
		if (player.isNew)
		{
			return;
		}
		Directory.CreateDirectory(cacheDir);
		string data = TFUtils.ReadFile(player.CacheFile("game.json"));
		TFUtils.WriteFile(CacheFile("game.json"), data);
		if (!isNew)
		{
			string path = player.CacheFile("lastEtag");
			if (File.Exists(path))
			{
				string contents = File.ReadAllText(path);
				File.WriteAllText(CacheFile("lastEtag"), contents);
			}
		}
	}

	public static Player LoadFromFilesystem()
	{
		if (File.Exists(LAST_PLAYED_FILE))
		{
			string text = TFUtils.ReadFile(LAST_PLAYED_FILE).Trim();
			string text2 = CACHE_ROOT + text;
			string text3 = text2 + Path.DirectorySeparatorChar + "user.json";
			if (Directory.Exists(text2) && File.Exists(text3))
			{
				return LoadFromFile(text3);
			}
		}
		return CreateUnauthedPlayer();
	}

	public static void LoadFromNetwork(string key, Session session, bool doFacebookAuth, string fbAccessToken)
	{
		session.Auth.AuthUser(session, session.AsyncResponder(key), doFacebookAuth, fbAccessToken);
	}

	public static Player LoadFromDataDict(Dictionary<string, object> data)
	{
		if (data.ContainsKey("data"))
		{
			data = (Dictionary<string, object>)data["data"];
		}
		return new Player((string)data["user_id"], (bool)data["is_new"]);
	}

    public static Player LoadFromResponse(string user_id, bool is_new)
    {
        return new Player(user_id, is_new);
    }

    public string CacheFile(string fileName)
	{
		return cacheDir + Path.DirectorySeparatorChar + fileName;
	}

	public string CacheDir()
	{
		return cacheDir;
	}

	public string LastPlayedFile()
	{
		return LAST_PLAYED_FILE;
	}

	public void SaveLocally()
	{
		if (!Directory.Exists(cacheDir))
		{
			Directory.CreateDirectory(cacheDir);
		}
		string filename = CacheFile("user.json");
		string data = Json.Serialize(ToDict());
		TFUtils.WriteFile(filename, data);
		TFUtils.WriteFile(LAST_PLAYED_FILE, PlayerFolder());
	}

	private Dictionary<string, object> ToDict()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["user_id"] = playerId;
		dictionary["is_new"] = false;
		return dictionary;
	}

	private static Player LoadFromFile(string filePath)
	{
		string json = TFUtils.ReadFile(filePath);
		Dictionary<string, object> data = (Dictionary<string, object>)Json.Deserialize(json);
		return LoadFromDataDict(data);
	}

	private static Player CreateUnauthedPlayer()
	{
		string text = CACHE_ROOT + "p_ua";
		string text2 = text + Path.DirectorySeparatorChar + "user.json";
		if (Directory.Exists(text) && File.Exists(text2))
		{
			return LoadFromFile(text2);
		}
		return new Player("ua", true);
	}

	private string PlayerFolder()
	{
		return "p_" + playerId;
	}
}
