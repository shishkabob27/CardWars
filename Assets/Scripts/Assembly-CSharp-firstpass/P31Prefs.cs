using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class P31Prefs
{
	private static bool _iCloudDocumentStoreAvailable;

	public static bool iCloudDocumentStoreAvailable
	{
		get
		{
			return _iCloudDocumentStoreAvailable;
		}
	}

	public static bool synchronize()
	{
		PlayerPrefs.Save();
		return true;
	}

	public static bool hasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public static List<object> allKeys()
	{
		return new List<object>();
	}

	public static void removeObjectForKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	public static void removeAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static void setInt(string key, int val)
	{
		PlayerPrefs.SetInt(key, val);
	}

	public static int getInt(string key)
	{
		return PlayerPrefs.GetInt(key);
	}

	public static void setFloat(string key, float val)
	{
		PlayerPrefs.SetFloat(key, val);
	}

	public static float getFloat(string key)
	{
		return PlayerPrefs.GetFloat(key);
	}

	public static void setString(string key, string val)
	{
		PlayerPrefs.SetString(key, val);
	}

	public static string getString(string key)
	{
		return PlayerPrefs.GetString(key);
	}

	public static void setBool(string key, bool val)
	{
		PlayerPrefs.SetInt(key, val ? 1 : 0);
	}

	public static bool getBool(string key)
	{
		return PlayerPrefs.GetInt(key, 0) == 1;
	}

	public static void setDictionary(string key, Hashtable val)
	{
		string value = Json.encode(val);
		PlayerPrefs.SetString(key, value);
	}

	public static IDictionary getDictionary(string key)
	{
		string @string = PlayerPrefs.GetString(key);
		return @string.dictionaryFromJson();
	}
}
