// Assembly-CSharp-firstpass, Version=1.4.1003.3007, Culture=neutral, PublicKeyToken=null
// LocalizationSettings
using System;
using UnityEngine;

[Serializable]
public class LocalizationSettings : ScriptableObject
{
    public string[] sheetTitles;

    public bool useSystemLanguagePerDefault = true;

    public string defaultLangCode = "EN";

    public static LanguageCode GetLanguageEnum(string langCode)
    {
        langCode = langCode.ToUpper();
        foreach (int value in Enum.GetValues(typeof(LanguageCode)))
        {
            if (string.Concat((LanguageCode)value, string.Empty) == langCode)
            {
                return (LanguageCode)value;
            }
        }
        return LanguageCode.EN;
    }
}
