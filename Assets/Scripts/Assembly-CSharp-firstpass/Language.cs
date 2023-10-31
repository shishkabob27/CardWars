using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class Language
{
	public static string settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";

	public static LocalizationSettings settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));

	public static string Backup_settingsAssetPath = "Assets/Localization/Resources/Languages/BackupLocalizationSettings.asset";

	public static LocalizationSettings backup_settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(Backup_settingsAssetPath), typeof(LocalizationSettings));

	private static List<string> availableLanguages;

	private static LanguageCode currentLanguage = LanguageCode.N;

	private static Dictionary<string, Hashtable> currentEntrySheets;

	private static string _persistentDataPath = string.Empty;

	private static AndroidJavaObject _instLocaleAndroid;

	public static AndroidJavaObject InstLocaleAndroid
	{
		get
		{
			if (_instLocaleAndroid == null)
			{
				_instLocaleAndroid = new AndroidJavaClass("java.util.Locale").CallStatic<AndroidJavaObject>("getDefault", new object[0]);
			}
			return _instLocaleAndroid;
		}
	}

	public static string getDeviceLanguage()
	{
        System.Globalization.CultureInfo currentCulture = System.Globalization.CultureInfo.CurrentCulture;
        return currentCulture.TwoLetterISOLanguageName.ToLower();
	}

	public static string getDeviceLocale()
	{
        System.Globalization.CultureInfo currentCulture = System.Globalization.CultureInfo.CurrentCulture;
        return currentCulture.Name.ToLower();
	}

	public static void Init(string persistentPath)
	{
		_persistentDataPath = persistentPath;
		LoadAvailableLanguages();
		if (settings == null)
		{
			return;
		}
		bool useSystemLanguagePerDefault = settings.useSystemLanguagePerDefault;
		LanguageCode languageCode = LocalizationSettings.GetLanguageEnum(settings.defaultLangCode);
		if (useSystemLanguagePerDefault)
		{
			LanguageCode languageCode2 = LanguageNameToCode(Application.systemLanguage);
			if (languageCode2 == LanguageCode.N)
			{
				string twoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
				if (twoLetterISOLanguageName != "iv")
				{
					languageCode2 = LocalizationSettings.GetLanguageEnum(twoLetterISOLanguageName);
				}
			}
			if (availableLanguages.Contains(string.Concat(languageCode2, string.Empty)))
			{
				languageCode = languageCode2;
			}
		}
		string @string = PlayerPrefs.GetString("M2H_lastLanguage", string.Empty);
		if (languageCode == LanguageCode.JA || languageCode == LanguageCode.NL)
		{
			SwitchLanguage(LanguageCode.EN);
		}
		else if (@string != string.Empty && availableLanguages.Contains(@string))
		{
			SwitchLanguage(@string);
		}
		else
		{
			SwitchLanguage(languageCode);
		}
	}

	private static void LoadAvailableLanguages()
	{
		availableLanguages = new List<string>();
		if (settings == null || settings.sheetTitles == null || settings.sheetTitles.Length <= 0)
		{
			settings = backup_settings;
			if (backup_settings == null || backup_settings.sheetTitles == null || backup_settings.sheetTitles.Length <= 0)
			{
				return;
			}
		}
		foreach (int value in Enum.GetValues(typeof(LanguageCode)))
		{
			if (HasLanguageFile(string.Concat((LanguageCode)value, string.Empty), settings.sheetTitles[0]))
			{
				availableLanguages.Add(string.Concat((LanguageCode)value, string.Empty));
			}
		}
		Resources.UnloadUnusedAssets();
	}

	public static string[] GetLanguages()
	{
		return availableLanguages.ToArray();
	}

	public static bool SwitchLanguage(string langCode)
	{
		return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
	}

	public static bool SwitchLanguage(LanguageCode code)
	{
		if (availableLanguages.Contains(string.Concat(code, string.Empty)))
		{
			DoSwitch(code);
			return true;
		}
		if (currentLanguage == LanguageCode.N && availableLanguages.Count > 0)
		{
			DoSwitch(LocalizationSettings.GetLanguageEnum(availableLanguages[0]));
		}
		return false;
	}

	public static void ReloadLanguage()
	{
		DoSwitch(currentLanguage);
	}

	private static void DoSwitch(LanguageCode newLang)
	{
		PlayerPrefs.GetString("M2H_lastLanguage", string.Concat(newLang, string.Empty));
		currentLanguage = newLang;
		currentEntrySheets = new Dictionary<string, Hashtable>();
		XMLParser xMLParser = new XMLParser();
		string[] sheetTitles = settings.sheetTitles;
		foreach (string text in sheetTitles)
		{
			currentEntrySheets[text] = new Hashtable();
			Hashtable hashtable = (Hashtable)xMLParser.Parse(GetLanguageFileContents(text));
			ArrayList arrayList = (ArrayList)(((ArrayList)hashtable["entries"])[0] as Hashtable)["entry"];
			foreach (Hashtable item in arrayList)
			{
				string key = (string)item["@name"];
				string s = string.Concat(item["_text"], string.Empty).Trim();
				s = s.UnescapeXML();
				currentEntrySheets[text][key] = s;
			}
		}
		LocalizedAsset[] array = (LocalizedAsset[])UnityEngine.Object.FindObjectsOfType(typeof(LocalizedAsset));
		LocalizedAsset[] array2 = array;
		foreach (LocalizedAsset localizedAsset in array2)
		{
			localizedAsset.LocalizeAsset();
		}
		SendMonoMessage("ChangedLanguage", currentLanguage);
	}

	public static UnityEngine.Object GetAsset(string name)
	{
		return Resources.Load(string.Concat("Languages/Assets/", CurrentLanguage(), "/", name));
	}

	private static bool HasLanguageFile(string lang, string sheetTitle)
	{
		return (TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null;
	}

	private static string GetLanguageFileContents(string sheetTitle)
	{
		string text = string.Concat("Languages/", currentLanguage, "_", sheetTitle);
		
		string path = _persistentDataPath + "/" + text + ".xml";
        UnityEngine.Debug.Log(path);
        if (File.Exists(path))
		{
            UnityEngine.Debug.Log("lang file exist so return that");
            return File.ReadAllText(path);

		}
		TextAsset textAsset = (TextAsset)Resources.Load(text, typeof(TextAsset));
        UnityEngine.Debug.Log(textAsset.name);
        return textAsset.text;
	}

	public static LanguageCode CurrentLanguage()
	{
		return currentLanguage;
	}

	public static string Get(string key)
	{
		if (!key.StartsWith("!!") || settings == null)
		{
			return key;
		}
		return Get(key, settings.sheetTitles[0]);
	}

	public static string Get(string key, string sheetTitle)
	{
		if (!key.StartsWith("!!") || settings == null)
		{
			return key;
		}
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			return string.Empty;
		}
		if (currentEntrySheets[sheetTitle].ContainsKey(key))
		{
			return (string)currentEntrySheets[sheetTitle][key];
		}
		return null;
	}

	public static bool ContainsKey(string key)
	{
		if (key.StartsWith("!!"))
		{
			return ContainsKey(key, settings.sheetTitles[0]);
		}
		return false;
	}

	public static bool ContainsKey(string key, string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			return false;
		}
		return currentEntrySheets[sheetTitle].ContainsKey(key);
	}

	private static void SendMonoMessage(string methodString, params object[] parameters)
	{
		if (parameters == null || parameters.Length > 1)
		{
		}
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject && gameObject.transform.parent == null)
			{
				if (parameters != null && parameters.Length == 1)
				{
					gameObject.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					gameObject.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static LanguageCode LanguageNameToCode(SystemLanguage name)
	{
		switch (name)
		{
		case SystemLanguage.Afrikaans:
			return LanguageCode.AF;
		case SystemLanguage.Arabic:
			return LanguageCode.AR;
		case SystemLanguage.Basque:
			return LanguageCode.BA;
		case SystemLanguage.Belarusian:
			return LanguageCode.BE;
		case SystemLanguage.Bulgarian:
			return LanguageCode.BG;
		case SystemLanguage.Catalan:
			return LanguageCode.CA;
		case SystemLanguage.Chinese:
			return LanguageCode.ZH;
		case SystemLanguage.Czech:
			return LanguageCode.CS;
		case SystemLanguage.Danish:
			return LanguageCode.DA;
		case SystemLanguage.Dutch:
			return LanguageCode.NL;
		case SystemLanguage.English:
			return LanguageCode.EN;
		case SystemLanguage.Estonian:
			return LanguageCode.ET;
		case SystemLanguage.Faroese:
			return LanguageCode.FA;
		case SystemLanguage.Finnish:
			return LanguageCode.FI;
		case SystemLanguage.French:
			return LanguageCode.FR;
		case SystemLanguage.German:
			return LanguageCode.DE;
		case SystemLanguage.Greek:
			return LanguageCode.EL;
		case SystemLanguage.Hebrew:
			return LanguageCode.HE;
		case SystemLanguage.Hungarian:
			return LanguageCode.HU;
		case SystemLanguage.Icelandic:
			return LanguageCode.IS;
		case SystemLanguage.Indonesian:
			return LanguageCode.ID;
		case SystemLanguage.Italian:
			return LanguageCode.IT;
		case SystemLanguage.Japanese:
			return LanguageCode.JA;
		case SystemLanguage.Korean:
			return LanguageCode.KO;
		case SystemLanguage.Latvian:
			return LanguageCode.LA;
		case SystemLanguage.Lithuanian:
			return LanguageCode.LT;
		case SystemLanguage.Norwegian:
			return LanguageCode.NO;
		case SystemLanguage.Polish:
			return LanguageCode.PL;
		case SystemLanguage.Portuguese:
			return LanguageCode.PT;
		case SystemLanguage.Romanian:
			return LanguageCode.RO;
		case SystemLanguage.Russian:
			return LanguageCode.RU;
		case SystemLanguage.SerboCroatian:
			return LanguageCode.SH;
		case SystemLanguage.Slovak:
			return LanguageCode.SK;
		case SystemLanguage.Slovenian:
			return LanguageCode.SL;
		case SystemLanguage.Spanish:
			return LanguageCode.ES;
		case SystemLanguage.Swedish:
			return LanguageCode.SW;
		case SystemLanguage.Thai:
			return LanguageCode.TH;
		case SystemLanguage.Turkish:
			return LanguageCode.TR;
		case SystemLanguage.Ukrainian:
			return LanguageCode.UK;
		case SystemLanguage.Vietnamese:
			return LanguageCode.VI;
		default:
			switch (name)
			{
			case SystemLanguage.Hungarian:
				return LanguageCode.HU;
			case SystemLanguage.Unknown:
				return LanguageCode.N;
			default:
				return LanguageCode.N;
			}
		}
	}
}
