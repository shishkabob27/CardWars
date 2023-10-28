using UnityEngine;

public class CWDeckZoom : MonoBehaviour
{
	private PanelManagerDeck pManagerDeck;

	public GameObject blackPanel;

	public bool activateBlackPanel;

	public CardItem card;

	public string cardName;

	private UITexture texture;

	private void Start()
	{
		pManagerDeck = PanelManagerDeck.GetInstance();
	}

	private void OnEnable()
	{
		cardName = card.Form.Name;
		if (pManagerDeck == null)
		{
			pManagerDeck = PanelManagerDeck.GetInstance();
		}
		if ((bool)pManagerDeck)
		{
			pManagerDeck.FillCardInfo(base.gameObject, card);
		}
		TweenTransform component = GetComponent<TweenTransform>();
		if ((bool)component)
		{
			component.from = pManagerDeck.activeCard.transform;
		}
		texture = base.gameObject.GetComponentInChildren<UITexture>();
		GameObject gameObject = GetBlackPanel();
		if (gameObject != null && activateBlackPanel)
		{
			NGUITools.SetActive(gameObject, true);
			TweenAlpha component2 = gameObject.GetComponent<TweenAlpha>();
			component2.Play(true);
			component2.Reset();
		}
	}

	private void OnDisable()
	{
		CWDeckCard component = pManagerDeck.activeCard.GetComponent<CWDeckCard>();
		if (component != null)
		{
			component.isZoomed = false;
		}
		if (texture != null)
		{
			Texture mainTexture = texture.mainTexture;
			texture.mainTexture = null;
			Resources.UnloadAsset(mainTexture);
		}
	}

	private void OnClick()
	{
		GameObject gameObject = GetBlackPanel();
		if (!(gameObject == null))
		{
			TweenAlpha component = gameObject.GetComponent<TweenAlpha>();
			component.Play(false);
			component.Reset();
			Invoke("DeactivateBlack", component.duration);
		}
	}

	private void DeactivateBlack()
	{
		GameObject go = GetBlackPanel();
		if (NGUITools.GetActive(go))
		{
			NGUITools.SetActive(go, false);
		}
	}

	private GameObject GetBlackPanel()
	{
		if (blackPanel != null)
		{
			return blackPanel;
		}
		return pManagerDeck.blackPanel;
	}

	private void Update()
	{
	}
}
