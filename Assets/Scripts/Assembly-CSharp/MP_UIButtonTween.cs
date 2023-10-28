using UnityEngine;

public class MP_UIButtonTween : MonoBehaviour
{
	public UIButtonTween PVPInfoShow;

	public UIButtonTween PVPInfoHide;

	public TweenPositionAnchored tweenPositionAnchored;

	private bool TweenHideEnabled;

	private void OnEnable()
	{
		if (GlobalFlags.Instance.InMPMode && (bool)PVPInfoShow)
		{
			PVPInfoShow.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		TweenHideEnabled = false;
	}

	private void Update()
	{
		if ((bool)tweenPositionAnchored && !TweenHideEnabled && tweenPositionAnchored.amountPerDelta < 0f)
		{
			TweenHideEnabled = true;
			PVPInfoHide.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
