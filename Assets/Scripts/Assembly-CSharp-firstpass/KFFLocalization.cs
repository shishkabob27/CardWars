using System.IO;
using UnityEngine;

public class KFFLocalization
{
	private static bool initialized;

	private static string ANDROID_STRING_KEY_SUFFIX_GOOGLE = "__GOOGLE";

	private static string ANDROID_STRING_KEY_SUFFIX_AMAZON = "__AMAZON";

	public static LanguageCode lang;

	public static string Get(string key)
	{
		if (!initialized)
		{
			Language.Init(Path.Combine(Application.persistentDataPath, "Contents"));
			initialized = true;
		}
		if (key == null)
		{
			return string.Empty;
		}
		if (!key.StartsWith("!!"))
		{
			return key;
		}
		string multiSheet = GetMultiSheet(key);
		if (multiSheet != null)
		{
			return multiSheet;
		}
		return "MISSING LANG: " + key;
	}

	private static string GetMultiSheet(string key)
	{
		string text = null;
		string[] sheetTitles = Language.settings.sheetTitles;
		foreach (string sheetTitle in sheetTitles)
		{
			text = Language.Get(key, sheetTitle);
			if (text != null)
			{
				return text;
			}
		}
		string text2 = SystemInfo.deviceModel.ToLower();
		bool flag = text2.IndexOf("amazon") >= 0;
		string key2 = key + ((!flag) ? ANDROID_STRING_KEY_SUFFIX_GOOGLE : ANDROID_STRING_KEY_SUFFIX_AMAZON);
		string[] sheetTitles2 = Language.settings.sheetTitles;
		foreach (string sheetTitle2 in sheetTitles2)
		{
			text = Language.Get(key2, sheetTitle2);
			if (text != null)
			{
				return text;
			}
		}
		return null;
	}

	public static string Get(string key, string[] tagsToReplace, string[] values)
	{
		string text = Get(key);
		if (tagsToReplace != null && values != null && tagsToReplace.Length == values.Length)
		{
			for (int i = 0; i < tagsToReplace.Length; i++)
			{
				text = text.Replace(tagsToReplace[i], values[i]);
			}
		}
		return text;
	}

	public static string Get(string key, string tagToReplace, string value)
	{
		return Get(key, new string[1] { tagToReplace }, new string[1] { value });
	}

	public static LanguageCode ReturnLang()
	{
		if (!initialized)
		{
			Language.Init(Path.Combine(Application.persistentDataPath, "Contents"));
			initialized = true;
		}
		return Language.CurrentLanguage();
	}
}
