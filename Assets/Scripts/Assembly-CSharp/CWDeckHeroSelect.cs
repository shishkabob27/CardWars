using UnityEngine;

public class CWDeckHeroSelect : MonoBehaviour
{
	public GameObject tableObject;

	public CWDeckHeroPanel HeroPanel;

	public void OnClick()
	{
		CWDeckHero[] componentsInChildren = tableObject.GetComponentsInChildren<CWDeckHero>();
		CWDeckHero[] array = componentsInChildren;
		CWDeckHero cscript;
		for (int i = 0; i < array.Length; i++)
		{
			cscript = array[i];
			if (!cscript.Selected)
			{
				continue;
			}
			int num = LeaderManager.Instance.leaders.FindIndex((LeaderItem ldr) => ldr == cscript.leader);
			if (num >= 0)
			{
				PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
				int currentDeck = CWDeckController.GetInstance().currentDeck;
				Deck deck = deckManager.Decks[currentDeck];
				deck.SetLeaderForPlayer(num);
				PlayerInfoScript instance = PlayerInfoScript.GetInstance();
				if (instance.SelectedMPDeck == currentDeck)
				{
					instance.MPDeckLeaderID = cscript.leader.Form.ID;
				}
				HeroPanel.Refresh();
			}
			break;
		}
	}
}
