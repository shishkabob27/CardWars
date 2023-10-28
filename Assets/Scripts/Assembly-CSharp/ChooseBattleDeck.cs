using System.Collections.Generic;
using UnityEngine;

public class ChooseBattleDeck : MonoBehaviour
{
	public List<GameObject> On = new List<GameObject>();

	public List<GameObject> Off = new List<GameObject>();

	private bool activated = true;

	public bool IsActivated
	{
		get
		{
			return activated;
		}
		set
		{
		}
	}

	private void Start()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if ((bool)instance)
		{
			activated = instance.SelectedMPDeck == CWDeckBuildDeckController.GetInstance().currentDeck;
		}
		foreach (GameObject item in On)
		{
			item.SetActive(activated);
		}
		foreach (GameObject item2 in Off)
		{
			item2.SetActive(!activated);
		}
	}

	private void OnClick()
	{
		if (activated)
		{
			return;
		}
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		Deck deck = deckManager.Decks[CWDeckBuildDeckController.GetInstance().currentDeck];
		if (deck.CardCount() == 0)
		{
			return;
		}
		if (deck.Leader.Form.FCWorld)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FC_NotAllowedForBattleDeck);
			return;
		}
		foreach (GameObject item in On)
		{
			item.SetActive(!activated);
		}
		foreach (GameObject item2 in Off)
		{
			item2.SetActive(activated);
		}
		activated = !activated;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if ((bool)instance && activated)
		{
			instance.SelectedMPDeck = CWDeckBuildDeckController.GetInstance().currentDeck;
			instance.Save();
		}
	}

	public void SetToggle(bool toggled)
	{
		foreach (GameObject item in On)
		{
			item.SetActive(toggled);
		}
		foreach (GameObject item2 in Off)
		{
			item2.SetActive(!toggled);
		}
		activated = toggled;
	}
}
