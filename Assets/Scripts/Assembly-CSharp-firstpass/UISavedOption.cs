using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public string keyName;

	private UIPopupList mList;

	private UICheckbox mCheck;

	private string key
	{
		get
		{
			return (!string.IsNullOrEmpty(keyName)) ? keyName : ("NGUI State: " + base.name);
		}
	}

	private void Awake()
	{
		mList = GetComponent<UIPopupList>();
		mCheck = GetComponent<UICheckbox>();
		if (mList != null)
		{
			UIPopupList uIPopupList = mList;
			uIPopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(uIPopupList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
		if (mCheck != null)
		{
			UICheckbox uICheckbox = mCheck;
			uICheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uICheckbox.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
	}

	private void OnDestroy()
	{
		if (mCheck != null)
		{
			UICheckbox uICheckbox = mCheck;
			uICheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(uICheckbox.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
		if (mList != null)
		{
			UIPopupList uIPopupList = mList;
			uIPopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(uIPopupList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
	}

	private void OnEnable()
	{
		if (mList != null)
		{
			string @string = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(@string))
			{
				mList.selection = @string;
			}
			return;
		}
		if (mCheck != null)
		{
			mCheck.isChecked = PlayerPrefs.GetInt(key, 1) != 0;
			return;
		}
		string string2 = PlayerPrefs.GetString(key);
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UICheckbox uICheckbox = componentsInChildren[i];
			uICheckbox.isChecked = uICheckbox.name == string2;
		}
	}

	private void OnDisable()
	{
		if (!(mCheck == null) || !(mList == null))
		{
			return;
		}
		UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UICheckbox uICheckbox = componentsInChildren[i];
			if (uICheckbox.isChecked)
			{
				SaveSelection(uICheckbox.name);
				break;
			}
		}
	}

	private void SaveSelection(string selection)
	{
		PlayerPrefs.SetString(key, selection);
	}

	private void SaveState(bool state)
	{
		PlayerPrefs.SetInt(key, state ? 1 : 0);
	}
}
