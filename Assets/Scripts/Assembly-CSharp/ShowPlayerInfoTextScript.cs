using UnityEngine;

public class ShowPlayerInfoTextScript : MonoBehaviour
{
	public string PlayerInfoField;

	private void Start()
	{
		UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.text = PlayerInfoScript.GetInstance().GetValue(PlayerInfoField);
		}
	}
}
