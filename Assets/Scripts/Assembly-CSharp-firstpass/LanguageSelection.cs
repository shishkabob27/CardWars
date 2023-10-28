using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Language Selection")]
[RequireComponent(typeof(UIPopupList))]
public class LanguageSelection : MonoBehaviour
{
	private UIPopupList mList;

	private void Start()
	{
		mList = GetComponent<UIPopupList>();
		UpdateList();
		mList.eventReceiver = base.gameObject;
		mList.functionName = "OnLanguageSelection";
	}

	private void UpdateList()
	{
		if (!(Localization.instance != null) || Localization.instance.languages == null || Localization.instance.languages.Length <= 0)
		{
			return;
		}
		mList.items.Clear();
		int i = 0;
		for (int num = Localization.instance.languages.Length; i < num; i++)
		{
			TextAsset textAsset = Localization.instance.languages[i];
			if (textAsset != null)
			{
				mList.items.Add(textAsset.name);
			}
		}
		mList.selection = Localization.instance.currentLanguage;
	}

	private void OnLanguageSelection(string language)
	{
		if (Localization.instance != null)
		{
			Localization.instance.currentLanguage = language;
		}
	}
}
