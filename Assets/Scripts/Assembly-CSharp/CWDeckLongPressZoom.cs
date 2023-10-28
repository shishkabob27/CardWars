using UnityEngine;

public class CWDeckLongPressZoom : CWFingerGestureBase
{
	public GameObject tapObject;

	public GameObject doubleTapObject;

	private PanelManagerDeck pmgrDeck;

	public bool useTapFlag;

	private void Start()
	{
		pmgrDeck = PanelManagerDeck.GetInstance();
	}

	private void OnLongPress(LongPressGesture gesture)
	{
		if (!useTapFlag)
		{
			TriggerZoomCard();
		}
	}

	private void TriggerZoomCard()
	{
		if (!(UICamera.lastHit.collider != GetComponent<Collider>()))
		{
			GameObject zoomCard = pmgrDeck.zoomCard;
			CWDeckCard component = GetComponent<CWDeckCard>();
			if (component != null)
			{
				component.isZoomed = true;
				zoomCard.GetComponent<CWDeckZoom>().card = component.card;
			}
			else
			{
				CWFuseCard component2 = GetComponent<CWFuseCard>();
				zoomCard.GetComponent<CWDeckZoom>().card = component2.card;
			}
			SetTween();
		}
	}

	private void FillZoomCard()
	{
	}

	private void OnTap(TapGesture gesture)
	{
		tapObject = gesture.Selection;
		if (useTapFlag)
		{
			TriggerZoomCard();
		}
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
