using UnityEngine;

public class CWDeckNameplate : MonoBehaviour
{
	public UILabel DeckNumber;

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		int currentDeck = CWDeckController.GetInstance().currentDeck;
		if (DeckNumber != null)
		{
			DeckNumber.text = (currentDeck + 1).ToString();
		}
	}
}
