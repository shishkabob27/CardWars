using UnityEngine;

public class CWHandCardZoom : MonoBehaviour
{
	private PanelManagerBattle panelMgrBattle;

	public CardItem card;

	public string cardName;

	public CWTBTapToBringForward tapToBringForward;

	private UITexture texture;

	private void Start()
	{
		panelMgrBattle = PanelManagerBattle.GetInstance();
	}

	private void OnEnable()
	{
		cardName = card.Form.Name;
		if (panelMgrBattle == null)
		{
			panelMgrBattle = PanelManagerBattle.GetInstance();
		}
		PanelManagerBattle.FillCardInfo(base.gameObject, card, PlayerType.User);
		texture = base.gameObject.GetComponentInChildren<UITexture>();
		GetComponent<Collider>().enabled = true;
	}

	private void OnClick()
	{
		if (tapToBringForward != null)
		{
			tapToBringForward.PlayZoomTween(false);
			return;
		}
		UIButtonTween[] components = CWPlayerHandsController.GetInstance().tweenZoom.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			uIButtonTween.Play(false);
		}
		GetComponent<Collider>().enabled = false;
	}

	private void DeactivateBlack()
	{
		if (NGUITools.GetActive(panelMgrBattle.blackPanel))
		{
			NGUITools.SetActive(panelMgrBattle.blackPanel, false);
		}
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		if (texture != null)
		{
			Texture mainTexture = texture.mainTexture;
			texture.mainTexture = null;
			Resources.UnloadAsset(mainTexture);
		}
	}
}
