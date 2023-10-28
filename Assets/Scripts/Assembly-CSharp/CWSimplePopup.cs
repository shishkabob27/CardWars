using System;
using System.Collections.Generic;
using UnityEngine;

public class CWSimplePopup : MonoBehaviour
{
	public delegate void OnButtonPressedCallback(GameObject buttonObj);

	public const string JSON_file = "db_Popups.json";

	public const string ID_column_name = "Popup ID";

	public static string BUTTON1_NAME = "Button1";

	public static string BUTTON2_NAME = "Button2";

	public static string BUTTON3_NAME = "Button3";

	public string popup_ID;

	public OnButtonPressedCallback onButtonPressedCallback;

	private Transform childT;

	private GameObject popup;

	private GameObject col;

	public GameObject CreatePopup()
	{
		if (string.IsNullOrEmpty(popup_ID))
		{
			return null;
		}
		Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_Popups.json");
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			string text = (string)dictionary["Popup ID"];
			if (text == popup_ID)
			{
				return CreatePopup(dictionary);
			}
		}
		return null;
	}

	private GameObject CreatePopup(Dictionary<string, object> dict)
	{
		popup = (GameObject)SLOTGame.InstantiateFX(SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("SimplePopup"));
		col = (GameObject)SLOTGame.InstantiateFX(SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("BlankPanel"));
		childT = popup.transform.Find("Title");
		if (childT != null)
		{
			UILabel componentInChildren = childT.gameObject.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = (string)dict["Title"];
			}
		}
		childT = popup.transform.Find("Sprite");
		if (childT != null)
		{
			childT.localPosition = new Vector3(TFUtils.LoadFloat(dict, "Sprite Pos X"), TFUtils.LoadFloat(dict, "Sprite Pos Y"), childT.localPosition.z);
			childT.localScale = new Vector3(TFUtils.LoadFloat(dict, "Sprite Size X"), TFUtils.LoadFloat(dict, "Sprite Size Y"), childT.localScale.z);
			UISprite componentInChildren2 = childT.gameObject.GetComponentInChildren<UISprite>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.spriteName = (string)dict["Sprite"];
			}
		}
		childT = popup.transform.Find("Text");
		if (childT != null)
		{
			childT.localPosition = new Vector3(TFUtils.LoadFloat(dict, "Text Pos X"), TFUtils.LoadFloat(dict, "Text Pos Y"), childT.localPosition.z);
			UILabel componentInChildren3 = childT.gameObject.GetComponentInChildren<UILabel>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.text = TFUtils.LoadString(dict, "Text", string.Empty).Replace("<br>", "\n");
				componentInChildren3.text = ReplaceSpecialKeyword(componentInChildren3.text);
				componentInChildren3.lineWidth = TFUtils.LoadInt(dict, "Text Width");
			}
		}
		CWSimplePopupScript cWSimplePopupScript = childT.gameObject.AddComponent<CWSimplePopupScript>();
		cWSimplePopupScript.parentScript = this;
		setButton(popup, dict, BUTTON1_NAME);
		setButton(popup, dict, BUTTON2_NAME);
		setButton(popup, dict, BUTTON3_NAME);
		return popup;
	}

	public void setPopupActive(bool isOn)
	{
	}

	private void setButton(GameObject popup, Dictionary<string, object> dict, string buttonName)
	{
		childT = popup.transform.Find(buttonName);
		if (!(childT != null))
		{
			return;
		}
		string text = (string)dict[buttonName + " Action"];
		if (!string.IsNullOrEmpty(text))
		{
			bool flag = false;
			Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_Popups.json");
			Dictionary<string, object>[] array2 = array;
			foreach (Dictionary<string, object> dictionary in array2)
			{
				if ((string)dictionary["Popup ID"] == text)
				{
					CWSimplePopup cWSimplePopup = childT.gameObject.AddComponent<CWSimplePopup>();
					cWSimplePopup.popup_ID = (string)dictionary["Popup ID"];
					CWClosePanelScript cWClosePanelScript = childT.gameObject.AddComponent<CWClosePanelScript>();
					cWClosePanelScript.ObjectToClose = popup;
					cWClosePanelScript.col = col;
					flag = true;
				}
			}
			if (!flag)
			{
				try
				{
					Type type = Type.GetType(text);
					Component component = childT.gameObject.AddComponent(type);
					if (text == "CWClosePanelScript")
					{
						((CWClosePanelScript)component).ObjectToClose = popup;
						((CWClosePanelScript)component).col = col;
					}
					else
					{
						CWClosePanelScript cWClosePanelScript2 = childT.gameObject.AddComponent<CWClosePanelScript>();
						cWClosePanelScript2.ObjectToClose = popup;
						cWClosePanelScript2.col = col;
					}
				}
				catch
				{
				}
			}
			childT.localPosition = new Vector3(TFUtils.LoadFloat(dict, buttonName + " Pos X"), TFUtils.LoadFloat(dict, buttonName + " Pos Y"), childT.localPosition.z);
			Transform transform = childT.Find("Background");
			if (transform != null)
			{
				transform.localScale = new Vector3(TFUtils.LoadFloat(dict, buttonName + " Size X"), TFUtils.LoadFloat(dict, buttonName + " Size Y"), childT.localScale.z);
			}
			UISprite componentInChildren = childT.gameObject.GetComponentInChildren<UISprite>();
			if (componentInChildren != null)
			{
				try
				{
					if ((string)dict[buttonName + " Sprite"] != string.Empty)
					{
						componentInChildren.spriteName = (string)dict[buttonName + " Sprite"];
					}
				}
				catch (KeyNotFoundException)
				{
				}
			}
			UILabel componentInChildren2 = childT.gameObject.GetComponentInChildren<UILabel>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.text = (string)dict[buttonName + " Text"];
			}
			UIButtonMessage uIButtonMessage = childT.gameObject.AddComponent<UIButtonMessage>();
			if (uIButtonMessage != null)
			{
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "SLOTSimplePopupButtonPressed";
			}
		}
		else
		{
			NGUITools.SetActive(childT.gameObject, false);
		}
	}

	public string ReplaceSpecialKeyword(string str)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		string text = string.Empty;
		if (str.IndexOf(";") != -1)
		{
			Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_Cost.json");
			string[] array2 = str.Split(';');
			string text2 = array2[0];
			text = array2[1];
			while (text.IndexOf(">") != -1)
			{
				int num = text.IndexOf("<") + 1;
				int num2 = text.IndexOf(">");
				string text3 = text.Substring(num, num2 - num);
				string newValue = string.Empty;
				Dictionary<string, object>[] array3 = array;
				foreach (Dictionary<string, object> dictionary in array3)
				{
					if (text2 == (string)dictionary["CostID"])
					{
						newValue = ((text3 == "Recovery") ? (float.Parse((string)dictionary[text3]) * 100f).ToString() : ((!(text3 == "DropCount")) ? ((string)dictionary[text3]) : int.Parse((string)dictionary[text3]).ToString()));
						if (text.IndexOf("HardCurrency") != -1)
						{
							text = text.Replace("<HardCurrency>", instance.Gems.ToString());
						}
						break;
					}
				}
				text = text.Replace("<" + text3 + ">", newValue);
			}
		}
		if (text != string.Empty)
		{
			return text;
		}
		return str;
	}

	private void SLOTSimplePopupButtonPressed(GameObject obj)
	{
		if (onButtonPressedCallback != null)
		{
			onButtonPressedCallback(obj);
		}
	}
}
