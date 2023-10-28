using UnityEngine;

public class CWDeckOpenPanel : MonoBehaviour
{
	public GameObject CreatureControl;

	public GameObject BuildingControl;

	public GameObject SpellsControl;

	public CardType PanelType;

	public static CardType openPanel;

	private CWDeckController deckCtlr;

	public void Awake()
	{
		deckCtlr = CWDeckController.GetInstance();
	}

	public void OnEnable()
	{
	}

	public void OnClick()
	{
		deckCtlr = CWDeckController.GetInstance();
		if (openPanel != PanelType)
		{
			OpenMyself();
		}
	}

	public void OpenMyself()
	{
		CWDeckControlPanel component = CreatureControl.GetComponent<CWDeckControlPanel>();
		CWDeckControlPanel component2 = BuildingControl.GetComponent<CWDeckControlPanel>();
		CWDeckControlPanel component3 = SpellsControl.GetComponent<CWDeckControlPanel>();
		switch (PanelType)
		{
		case CardType.Creature:
			component.OpenPanel();
			component.RaiseBar();
			component2.ClosePanel();
			component2.LowerPanel();
			component2.LowerBar();
			component3.ClosePanel();
			component3.LowerBar();
			break;
		case CardType.Building:
			component.ClosePanel();
			component.RaiseBar();
			component2.OpenPanel();
			if (deckCtlr.buildingBarPos == BuildingPos.Down)
			{
				component2.RaisePanel();
			}
			else
			{
				component2.PlacePanelHigh();
			}
			component2.RaiseBar();
			component3.ClosePanel();
			component3.LowerBar();
			break;
		case CardType.Spell:
			component.ClosePanel();
			component.RaiseBar();
			component2.ClosePanel();
			component2.RaiseBar();
			component3.OpenPanel();
			component3.RaiseBar();
			break;
		}
		openPanel = PanelType;
	}
}
