using UnityEngine;

public class CWBattleLongPressZoom : CWFingerGestureBase
{
	public GameObject tapObject;

	public GameObject doubleTapObject;

	private PanelManagerBattle pmgrBtl;

	private void Start()
	{
		pmgrBtl = PanelManagerBattle.GetInstance();
	}

	private void OnLongPress(LongPressGesture gesture)
	{
		GameObject zoomCard = pmgrBtl.zoomCard;
		zoomCard.GetComponent<CWDeckZoom>().card = GetComponent<CWDeckCard>().card;
		SetTween();
	}

	private void FillZoomCard()
	{
	}

	private void OnTap(TapGesture gesture)
	{
		tapObject = gesture.Selection;
	}

	private void OnDoubleTap(TapGesture gesture)
	{
		if (!(gesture.Selection == doubleTapObject))
		{
		}
	}

	private void SetTween()
	{
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		instance.activeCard = base.gameObject;
		NGUITools.SetActive(instance.zoomCard, true);
		TweenTransform component = instance.zoomCard.GetComponent<TweenTransform>();
		component.Play(true);
		component.Reset();
		NGUITools.SetActive(instance.blackPanel, true);
		TweenAlpha component2 = instance.blackPanel.GetComponent<TweenAlpha>();
		component2.Play(true);
		component2.Reset();
	}
}
