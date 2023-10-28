using UnityEngine;

public class CWLeaderLongPressZoom : CWFingerGestureBase
{
	private void OnLongPress(LongPressGesture gesture)
	{
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		GameObject zoomLeaderCard = instance.zoomLeaderCard;
		CWDeckHero component = GetComponent<CWDeckHero>();
		if (component != null)
		{
			zoomLeaderCard.GetComponent<CWLeaderZoom>().leader = component.leader;
		}
		SetTween();
	}

	private void OnTap(TapGesture gesture)
	{
	}

	private void OnDoubleTap(TapGesture gesture)
	{
	}

	private void SetTween()
	{
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		instance.activeCard = base.gameObject;
		NGUITools.SetActive(instance.zoomLeaderCard, true);
		TweenTransform component = instance.zoomLeaderCard.GetComponent<TweenTransform>();
		component.Play(true);
		component.Reset();
		NGUITools.SetActive(instance.blackPanel, true);
		TweenAlpha component2 = instance.blackPanel.GetComponent<TweenAlpha>();
		component2.Play(true);
		component2.Reset();
	}
}
