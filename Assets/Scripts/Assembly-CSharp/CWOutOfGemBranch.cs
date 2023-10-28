using UnityEngine;

public class CWOutOfGemBranch : MonoBehaviour
{
	public GameObject okTween;

	public GameObject errorTween;

	public AudioClip okSound;

	public AudioClip errorSound;

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		UIButtonSound component = GetComponent<UIButtonSound>();
		if (instance.Gems <= 0)
		{
			component.audioClip = errorSound;
			if (errorTween != null)
			{
				errorTween.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			return;
		}
		component.audioClip = okSound;
		if (okTween != null)
		{
			okTween.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		Singleton<AnalyticsManager>.Instance.LogReshufflePurchase();
	}

	private void Update()
	{
	}
}
