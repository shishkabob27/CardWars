using UnityEngine;

public class SLOTUIButton : MonoBehaviour
{
	private SLOTUIButtonTweenManager buttonTweenManager;

	private void Start()
	{
		buttonTweenManager = NGUITools.FindInParents<SLOTUIButtonTweenManager>(base.gameObject);
	}

	private void OnClick()
	{
		if (buttonTweenManager != null)
		{
			buttonTweenManager.OnButtonClick(this);
		}
	}
}
