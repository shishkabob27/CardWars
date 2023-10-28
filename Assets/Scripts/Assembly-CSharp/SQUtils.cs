using System;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using UnityEngine;

public class SQUtils
{
	public delegate GameObject GetAtlasFunction(string atlasName);

	public const string BLUEPRINTS_FOLDER = "Blueprints";

	private static GetAtlasFunction getAtlasFunction = null;

	private static Dictionary<string, Dictionary<string, object>[]> jsonCache = new Dictionary<string, Dictionary<string, object>[]>();

	public static void FlushCache()
	{
		jsonCache.Clear();
	}

	public static Dictionary<string, object>[] ReadJSONData(string fname, string foldername, bool cacheFile = true)
	{
		string value = string.Empty;
		if (jsonCache.ContainsKey(foldername + fname))
		{
			Dictionary<string, object>[] value2;
			jsonCache.TryGetValue(foldername + fname, out value2);
			return value2;
		}
		string filename = Path.Combine(foldername, fname);
		try
		{
			value = TFUtils.GetJsonFileContent(filename);
		}
		catch (Exception ex)
		{
			TFUtils.DebugLog(ex.ToString());
		}
		Dictionary<string, object>[] array = JsonReader.Deserialize<Dictionary<string, object>[]>(value);
		if (array != null)
		{
			jsonCache.Add(foldername + fname, array);
		}
		return array;
	}

	public static Dictionary<string, object>[] ReadJSONData(string fname, bool cacheFile = true)
	{
		return ReadJSONData(fname, "Blueprints", cacheFile);
	}

	public static void UnloadJSONData(string fname)
	{
		UnloadJSONData(fname, "Blueprints");
	}

	public static void UnloadJSONData(string fname, string foldername)
	{
		string key = foldername + fname;
		if (jsonCache.ContainsKey(key))
		{
			jsonCache.Remove(key);
		}
	}

	public static Dictionary<string, object> ReadUserData(string fname)
	{
		SessionManager instance = SessionManager.GetInstance();
		if (instance == null || !instance.IsLoggedIn())
		{
			return null;
		}
		try
		{
			string playerDataPath = instance.GetPlayerDataPath(fname);
			TFUtils.DebugLog("SQUtils:ReadUserData filePath - " + playerDataPath);
			string empty = string.Empty;
			empty = File.ReadAllText(playerDataPath);
			return JsonReader.Deserialize<Dictionary<string, object>>(empty);
		}
		catch (FileNotFoundException)
		{
			return null;
		}
	}

	public static void WriteUserData(string fname, Dictionary<string, object> data)
	{
		SessionManager instance = SessionManager.GetInstance();
		if (!(instance == null) && instance.IsLoggedIn())
		{
			string playerDataPath = instance.GetPlayerDataPath(fname);
			string contents = JsonWriter.Serialize(data);
			File.WriteAllText(playerDataPath, contents);
		}
	}

	public static bool StringEqual(string a, string b)
	{
		return a.Equals(b, StringComparison.InvariantCultureIgnoreCase);
	}

	public static bool StartsWith(string a, string b)
	{
		return a.StartsWith(b, StringComparison.InvariantCultureIgnoreCase);
	}

	public static bool EndsWith(string a, string b)
	{
		return a.EndsWith(b, StringComparison.InvariantCultureIgnoreCase);
	}

	public static void SetLayer(GameObject obj, int newLayer)
	{
		if (obj == null)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			SetLayer(item.gameObject, newLayer);
		}
	}

	public static void SetLabel(GameObject obj, string value)
	{
		if (!(obj == null))
		{
			UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>(true);
			UILabel[] array = componentsInChildren;
			foreach (UILabel uILabel in array)
			{
				uILabel.text = value;
			}
		}
	}

	public static void SetLabel(Transform parent, string name, string value)
	{
		Transform transform = parent.Find(name);
		if (transform != null)
		{
			SetLabel(transform.gameObject, value);
		}
	}

	public static void SetTexture(GameObject obj, string textureName)
	{
		if (!(obj == null))
		{
			UITexture[] componentsInChildren = obj.GetComponentsInChildren<UITexture>(true);
			UITexture[] array = componentsInChildren;
			foreach (UITexture uITexture in array)
			{
				uITexture.mainTexture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(textureName) as Texture;
			}
		}
	}

	public static void SetIcon(UISprite sprite, string atlasName, string iconName, Color? color = null)
	{
		if (!(sprite == null))
		{
			GameObject gameObject = null;
			gameObject = ((getAtlasFunction == null) ? (SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(atlasName) as GameObject) : getAtlasFunction(atlasName));
			if (gameObject != null)
			{
				sprite.atlas = gameObject.GetComponent<UIAtlas>();
			}
			sprite.spriteName = iconName;
			if (color.HasValue)
			{
				sprite.color = color.Value;
			}
		}
	}

	public static void SetIcon(GameObject obj, string atlasName, string iconName, Color? color = null)
	{
		if (!(obj == null))
		{
			UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
			UISprite[] array = componentsInChildren;
			foreach (UISprite sprite in array)
			{
				SetIcon(sprite, atlasName, iconName, color);
			}
		}
	}

	public static void SetIcon(Transform parent, string name, string atlasName, string iconName, Color? color = null)
	{
		Transform transform = parent.Find(name);
		if (transform != null)
		{
			SetIcon(transform.gameObject, atlasName, iconName, color);
		}
	}

	public static void SetGray(GameObject gameObject, float val, string skipSuffix = null, string skipPrefix = null)
	{
		Color color = new Color(val, val, val, 1f);
		UIWidget[] componentsInChildren = gameObject.GetComponentsInChildren<UIWidget>(true);
		UIWidget[] array = componentsInChildren;
		foreach (UIWidget uIWidget in array)
		{
			if ((skipSuffix == null || !uIWidget.name.EndsWith(skipSuffix)) && (skipPrefix == null || !uIWidget.name.StartsWith(skipPrefix)))
			{
				uIWidget.color = color;
			}
		}
	}

	public static void SetActive(GameObject gameObject, string childName, bool state)
	{
		Transform transform = gameObject.transform.Find(childName);
		if (transform != null)
		{
			NGUITools.SetActive(transform.gameObject, state);
		}
	}

	public static string GetGearFrameSprite(string element)
	{
		switch (element)
		{
		case "Light":
			return "SQ_Gear_FrameLight";
		case "Dark":
			return "SQ_Gear_FrameDark";
		case "Earth":
			return "SQ_Gear_FrameEarth";
		default:
			return string.Empty;
		}
	}

	public static string GetElementIconSprite(string element)
	{
		switch (element)
		{
		case "Light":
			return "Icon_Light";
		case "Dark":
			return "Icon_Dark";
		case "Earth":
			return "Icon_Earth";
		default:
			return string.Empty;
		}
	}

	public static void SetGetAtlasFunction(GetAtlasFunction f)
	{
		getAtlasFunction = f;
	}

	public static float Lerp(float from, float to, float t)
	{
		float num = Mathf.Lerp(from, to, t);
		if (Mathf.Abs(num) < 0.001f)
		{
			num = Mathf.Sign(num) * 0.001f;
		}
		return num;
	}
}
