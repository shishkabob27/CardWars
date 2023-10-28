using System.Collections.Generic;
using UnityEngine;

public class SLOTLoadPage : MonoBehaviour
{
	public const string BLUEPRINTS_FOLDER = "Blueprints";

	public string JSON_file;

	public string ID_column_name;

	public string ID_From_PlayerInfo;

	private void Start()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		string value = instance.GetValue(ID_From_PlayerInfo);
		Dictionary<string, object>[] array = SQUtils.ReadJSONData(JSON_file);
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			if (!dictionary.ContainsKey(ID_column_name))
			{
				continue;
			}
			string text = (string)dictionary[ID_column_name];
			if (value != null)
			{
				if (text == value)
				{
					FillInComponents(base.gameObject, dictionary);
					break;
				}
			}
			else
			{
				Transform transform = base.transform.Find(text);
				if (transform != null)
				{
					FillInComponents(transform.gameObject, dictionary);
				}
			}
		}
	}

	public static void FillInComponents(GameObject go, Dictionary<string, object> dict)
	{
		UILabel[] componentsInChildren = go.GetComponentsInChildren<UILabel>();
		UISprite[] componentsInChildren2 = go.GetComponentsInChildren<UISprite>();
		foreach (string key in dict.Keys)
		{
			bool flag = false;
			UILabel[] array = componentsInChildren;
			foreach (UILabel uILabel in array)
			{
				if (uILabel.name == key)
				{
					uILabel.text = (string)dict[key];
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			UISprite[] array2 = componentsInChildren2;
			foreach (UISprite uISprite in array2)
			{
				if (uISprite.name == key)
				{
					uISprite.spriteName = (string)dict[key];
					flag = true;
					break;
				}
			}
		}
	}
}
