using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UGuiLocalizedText : MonoBehaviour
{
	private void Start()
	{
		Refresh();
	}

	public void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		Text component = GetComponent<Text>();
		component.text = UGuiTextReplacement.Instance.Get(component.text);
	}
}
