using System.Collections;
using UnityEngine;

public class CWDeckConfirmSell : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public CWDeckCardList cardList;

	public UIDraggablePanel draggablePanel;

	private GameObject SellButton;

	public void OnEnable()
	{
		SellButton = base.transform.parent.parent.Find("Buttons/SellButton").gameObject;
		PlayerDeckManager.ResetSort();
		Sort();
	}

	public void Sort()
	{
		reset();
		StartCoroutine(FillTable());
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

	private IEnumerator FillTable()
	{
		if (SellButton != null)
		{
			SQUtils.SetGray(SellButton, 0.4f);
			SellButton.GetComponent<Collider>().enabled = false;
		}
		UIGrid grid = base.gameObject.GetComponent<UIGrid>();
		int curItemNum = 0;
		if (draggablePanel != null)
		{
			draggablePanel.ResetPosition();
		}
		foreach (CardItem card in cardList.chosenList)
		{
			CWDeckDeckCards.AddCard(card, base.gameObject, DeckCardPrefab, 0.55f, curItemNum, false, false);
			grid.Reposition();
			curItemNum++;
			yield return null;
		}
		if (SellButton != null)
		{
			SQUtils.SetGray(SellButton, 1f);
			SellButton.GetComponent<Collider>().enabled = true;
		}
		if (draggablePanel != null)
		{
			draggablePanel.ResetPosition();
		}
	}
}
