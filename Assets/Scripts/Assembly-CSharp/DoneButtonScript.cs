using UnityEngine;

public class DoneButtonScript : MonoBehaviour
{
	private void Start()
	{
		NGUITools.SetActive(base.gameObject, false);
	}

	private void OnClick()
	{
		NGUITools.SetActive(base.gameObject, false);
	}
}
