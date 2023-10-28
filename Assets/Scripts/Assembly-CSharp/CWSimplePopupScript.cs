using UnityEngine;

public class CWSimplePopupScript : MonoBehaviour
{
	public CWSimplePopup parentScript;

	private void OnEnable()
	{
		if ((bool)parentScript)
		{
			parentScript.setPopupActive(true);
		}
	}

	private void OnDisable()
	{
		if ((bool)parentScript)
		{
			parentScript.setPopupActive(false);
		}
	}
}
