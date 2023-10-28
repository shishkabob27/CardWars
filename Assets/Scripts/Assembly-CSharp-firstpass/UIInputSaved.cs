using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Saved)")]
public class UIInputSaved : UIInput
{
	public string playerPrefsField;

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = value;
			SaveToPlayerPrefs(value);
		}
	}

	private void Awake()
	{
		onSubmit = SaveToPlayerPrefs;
		if (!string.IsNullOrEmpty(playerPrefsField) && PlayerPrefs.HasKey(playerPrefsField))
		{
			text = PlayerPrefs.GetString(playerPrefsField);
		}
	}

	private void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(playerPrefsField))
		{
			PlayerPrefs.SetString(playerPrefsField, val);
		}
	}

	private void OnApplicationQuit()
	{
		SaveToPlayerPrefs(text);
	}
}
