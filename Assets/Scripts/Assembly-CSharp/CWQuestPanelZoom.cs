using UnityEngine;

public class CWQuestPanelZoom : MonoBehaviour
{
	public CardItem card;

	public GameObject zoomCard;

	private void Start()
	{
	}

	private void OnClick()
	{
		if (!(null == zoomCard) && !zoomCard.activeInHierarchy)
		{
			zoomCard.GetComponent<CWDeckZoom>().card = card;
			PanelManagerDeck.GetInstance().activeCard = base.gameObject;
			NGUITools.SetActive(zoomCard, true);
			TweenTransform component = zoomCard.GetComponent<TweenTransform>();
			component.Play(true);
			component.Reset();
		}
	}

	private void Update()
	{
	}
}
