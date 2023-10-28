using System.Collections;
using UnityEngine;

public class CWDeckDeckCards : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public GameObject SortButton;

	public UIDraggablePanel draggablePanel;

	public GameObject AddPanel;

	public GameObject SortPanel;

	public UILabel CreaturesStack;

	public UILabel SpellsStack;

	public UILabel BuildingsStack;

	private float scale = 0.75f;

	private CardType filterType = CardType.None;

	public void OnEnable()
	{
		CWDeckSortButton component = SortButton.GetComponent<CWDeckSortButton>();
		if (component == null)
		{
			PlayerDeckManager.SetSort(SortType.TYPE, SortType.FACT);
		}
		filterType = CardType.None;
		Sort();
	}

	public void Sort(CardType filter)
	{
		filterType = filter;
		Sort();
	}

	public void Sort()
	{
		if (AddPanel.activeInHierarchy && SortPanel.activeInHierarchy)
		{
			AddPanel.BroadcastMessage("Sort", null, SendMessageOptions.DontRequireReceiver);
			return;
		}
		reset();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(FillTable());
		}
	}

	public void reset()
	{
		StopAllCoroutines();
		UIDraggablePanel component = base.transform.parent.GetComponent<UIDraggablePanel>();
		if (component != null)
		{
			component.ResetPosition();
		}
		foreach (Transform item in base.gameObject.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public static void FillCard(CardItem card, GameObject itemObj, float scale, bool selectShowsX, bool responsive = true, CWDeckCardList cardList = null)
	{
		itemObj.transform.localScale = new Vector3(scale, scale, 1f);
		CWDeckCard component = itemObj.GetComponent<CWDeckCard>();
		if ((bool)component)
		{
			component.card = card;
			component.SelectShowsX = selectShowsX;
			component.Responsive = responsive;
			component.SelectList = cardList;
			component.InUse = false;
			int num = 0;
			if ((bool)cardList)
			{
				num = cardList.GetNum(card);
			}
			if (num > 0)
			{
				component.SetSequenceNum(num);
			}
			else
			{
				component.ClearSequenceNum();
			}
		}
		bool showSortInfo = PlayerDeckManager.GetSecondarySort() != SortType.FACT;
		if (PlayerDeckManager.GetPrimarySort() == SortType.NAME)
		{
			showSortInfo = false;
		}
		PanelManagerDeck instance = PanelManagerDeck.GetInstance();
		instance.FillCardInfo(itemObj, card, showSortInfo);
	}

	public static GameObject AddCard(CardItem card, GameObject parent, GameObject prefab, float scale, int curItemNum, bool selectShowsX, bool responsive = true, CWDeckCardList cardList = null)
	{
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		SQUtils.SetLayer(gameObject, parent.layer);
		gameObject.name = string.Format("DeckCard{0:D3}", curItemNum);
		gameObject.GetComponent<Collider>().enabled = true;
		UIPanel component = gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		FillCard(card, gameObject, scale, selectShowsX, responsive, cardList);
		return gameObject;
	}

	private IEnumerator FillTable()
	{
		UIGrid grid = base.gameObject.GetComponent<UIGrid>();
		PlayerDeckManager deckMgr = PlayerInfoScript.GetInstance().DeckManager;
		int currentDeck = CWDeckController.GetInstance().currentDeck;
		Deck deck = deckMgr.GetSortedDeck(currentDeck);
		if (deck != null)
		{
			deck.PrecacheCounts();
			CreaturesStack.text = deck.GetCreatureCount().ToString();
			SpellsStack.text = deck.GetSpellCount().ToString();
			BuildingsStack.text = deck.GetBuildingCount().ToString();
			int curItemNum = 0;
			for (int ix = 0; ix < deck.CardCount(); ix++)
			{
				CardItem card = deck.GetCard(ix);
				if (filterType == CardType.None || card.Form.Type == filterType)
				{
					AddCard(card, base.gameObject, DeckCardPrefab, scale, curItemNum, false, false);
					grid.Reposition();
					curItemNum++;
					yield return null;
				}
			}
		}
		if (draggablePanel != null)
		{
			draggablePanel.ResetPosition();
		}
	}
}
