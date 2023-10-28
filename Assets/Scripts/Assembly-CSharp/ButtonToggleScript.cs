using System.Collections.Generic;
using UnityEngine;

public class ButtonToggleScript : MonoBehaviour
{
	public List<GameObject> On = new List<GameObject>();

	public List<GameObject> Off = new List<GameObject>();

	public bool activated;

	private void OnClick()
	{
		foreach (GameObject item in On)
		{
			item.SetActive(!activated);
		}
		foreach (GameObject item2 in Off)
		{
			item2.SetActive(activated);
		}
		activated = !activated;
	}

	public void SetToggle(bool toggled)
	{
		foreach (GameObject item in On)
		{
			item.SetActive(toggled);
		}
		foreach (GameObject item2 in Off)
		{
			item2.SetActive(!toggled);
		}
		activated = toggled;
	}
}
