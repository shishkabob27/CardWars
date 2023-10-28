using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize")]
[RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
	public string key;

	private string mLanguage;

	private bool mStarted;

	private void OnLocalize(Localization loc)
	{
		if (mLanguage != loc.currentLanguage)
		{
			Localize();
		}
	}

	private void OnEnable()
	{
		if (mStarted && Localization.instance != null)
		{
			Localize();
		}
	}

	private void Start()
	{
		mStarted = true;
		if (Localization.instance != null)
		{
			Localize();
		}
	}

	public void Localize()
	{
		Localization instance = Localization.instance;
		UIWidget component = GetComponent<UIWidget>();
		UILabel uILabel = component as UILabel;
		UISprite uISprite = component as UISprite;
		if (string.IsNullOrEmpty(mLanguage) && string.IsNullOrEmpty(key) && uILabel != null)
		{
			key = uILabel.text;
		}
		string text = ((!string.IsNullOrEmpty(key)) ? instance.Get(key) : string.Empty);
		if (uILabel != null)
		{
			UIInput uIInput = NGUITools.FindInParents<UIInput>(uILabel.gameObject);
			if (uIInput != null && uIInput.label == uILabel)
			{
				uIInput.defaultText = text;
			}
			else
			{
				uILabel.text = text;
			}
		}
		else if (uISprite != null)
		{
			uISprite.spriteName = text;
			uISprite.MakePixelPerfect();
		}
		mLanguage = instance.currentLanguage;
	}
}
