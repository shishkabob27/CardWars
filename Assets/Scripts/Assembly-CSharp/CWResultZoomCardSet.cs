using UnityEngine;

public class CWResultZoomCardSet : MonoBehaviour
{
	public CardItem card;

	private PanelManagerBattle panelMgrBattle;

	private void Start()
	{
		panelMgrBattle = PanelManagerBattle.GetInstance();
	}

	private void OnClick()
	{
		PanelManagerBattle.FillCardInfo(panelMgrBattle.zoomCard, card);
	}

	private void Update()
	{
	}
}
