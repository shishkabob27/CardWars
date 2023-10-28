using UnityEngine;

public class ShowLastPlayerName : MonoBehaviour
{
	private void Start()
	{
		string text = PlayerInfoScript.LoadPlayerName();
		if (string.IsNullOrEmpty(text))
		{
			text = KFFLocalization.Get("!!TYPE_HERE");
		}
		UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.text = text;
		}
	}
}
