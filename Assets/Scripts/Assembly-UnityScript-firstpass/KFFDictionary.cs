// Assembly-UnityScript-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// KFFDictionary
/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Boo.Lang.Runtime;
using UnityScript.Lang;

[Serializable]
[XmlRoot("KFFDictionary")]
public class KFFDictionary
{
    [XmlArrayItem("KeyValuePair")]
    [XmlArray("KeyValuePairs")]
    public List<KFFDictionaryEntry> entries;

    private Dictionary<string, string> dict;

    public KFFDictionary()
    {
        dict = new Dictionary<string, string>();
        entries = new List<KFFDictionaryEntry>();
    }

    public virtual bool isValid()
    {
        int result;
        if (entries.Count < 2)
        {
            result = 0;
        }
        else
        {
            KFFDictionaryEntry kFFDictionaryEntry = entries[0];
            if (kFFDictionaryEntry.key != "ERROR_ID")
            {
                result = 0;
            }
            else if (int.Parse(kFFDictionaryEntry.value) != 0)
            {
                result = 0;
            }
            else
            {
                kFFDictionaryEntry = entries[1];
                result = ((!(kFFDictionaryEntry.key != "ERROR_MSG")) ? 1 : 0);
            }
        }
        return (byte)result != 0;
    }

    public virtual void CreateDictionary()
    {
        dict.Clear();
        IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(entries);
        while (enumerator.MoveNext())
        {
            object obj = enumerator.Current;
            if (!(obj is KFFDictionaryEntry))
            {
                obj = RuntimeServices.Coerce(obj, typeof(KFFDictionaryEntry));
            }
            KFFDictionaryEntry kFFDictionaryEntry = (KFFDictionaryEntry)obj;
            dict[kFFDictionaryEntry.key] = kFFDictionaryEntry.value;
            UnityRuntimeServices.Update(enumerator, kFFDictionaryEntry);
        }
    }

    public virtual string GetValue(string key)
    {
        object result;
        if (!ContainsKey(key))
        {
            result = null;
        }
        else
        {
            string value = string.Empty;
            dict.TryGetValue(key, out value);
            result = value;
        }
        return (string)result;
    }

    public virtual int GetValueAsInt(string key)
    {
        string value = GetValue(key);
        int result;
        if (value == null || value == string.Empty)
        {
            result = 0;
        }
        else
        {
            int num;
            try
            {
                num = int.Parse(value);
            }
            catch (Exception)
            {
                goto IL_0041;
            }
            result = num;
        }
        goto IL_0045;
    IL_0045:
        return result;
    IL_0041:
        result = 0;
        goto IL_0045;
    }

    public virtual float GetValueAsFloat(string key)
    {
        string value = GetValue(key);
        float result;
        if (value == null || value == string.Empty)
        {
            result = 0f;
        }
        else
        {
            float num;
            try
            {
                num = float.Parse(GetValue(key));
            }
            catch (Exception)
            {
                goto IL_0048;
            }
            result = num;
        }
        goto IL_004d;
    IL_004d:
        return result;
    IL_0048:
        result = 0f;
        goto IL_004d;
    }

    public virtual string GetValueAsString(string key)
    {
        string value = GetValue(key);
        return (!(value == null)) ? value : string.Empty;
    }

    public virtual bool ContainsKey(string key)
    {
        return dict.ContainsKey(key);
    }

    public virtual void SetValue(string key, string value)
    {
        KFFDictionaryEntry kFFDictionaryEntry = null;
        if (ContainsKey(key))
        {
            IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(entries);
            while (enumerator.MoveNext())
            {
                object obj = enumerator.Current;
                if (!(obj is KFFDictionaryEntry))
                {
                    obj = RuntimeServices.Coerce(obj, typeof(KFFDictionaryEntry));
                }
                kFFDictionaryEntry = (KFFDictionaryEntry)obj;
                if (kFFDictionaryEntry.key == key)
                {
                    kFFDictionaryEntry.value = value;
                    UnityRuntimeServices.Update(enumerator, kFFDictionaryEntry);
                    break;
                }
            }
        }
        else
        {
            kFFDictionaryEntry = new KFFDictionaryEntry();
            kFFDictionaryEntry.key = key;
            kFFDictionaryEntry.value = value;
            entries.Add(kFFDictionaryEntry);
        }
        dict[key] = value;
    }

    public virtual void SetValue(string key, float value)
    {
        SetValue(key, value.ToString("F16"));
    }

    public virtual void SetValue(string key, int value)
    {
        SetValue(key, string.Empty + value);
    }
}
*/