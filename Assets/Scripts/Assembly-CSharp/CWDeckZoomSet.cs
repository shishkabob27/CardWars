using UnityEngine;

public class CWDeckZoomSet : MonoBehaviour
{
	private PanelManagerDeck pManager;

	private void Start()
	{
		pManager = PanelManagerDeck.GetInstance();
	}

	private void OnClick()
	{
		if (pManager == null)
		{
			pManager = PanelManagerDeck.GetInstance();
		}
		pManager.activeCard = base.gameObject;
	}

	private void Update()
	{
	}
}
