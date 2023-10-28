using UnityEngine;

public class CWDeckBuildDeckController : MonoBehaviour
{
	private static CWDeckBuildDeckController g_deckController;

	public CWDeckDeckCards Table;

	public CWDeckHeroPanel heroPanel;

	public CWDeckNameplate namePlate;

	public ChooseBattleDeck BattleDeckButton;

	private GameObject LandscapesButton;

	private bool initialized;

	public CardType currentTable;

	public int currentDeck
	{
		get
		{
			return PlayerInfoScript.GetInstance().SelectedDeck;
		}
		set
		{
			PlayerInfoScript.GetInstance().SelectedDeck = value;
		}
	}

	private void Awake()
	{
		if (g_deckController == null)
		{
			g_deckController = this;
		}
	}

	private void OnEnable()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!string.IsNullOrEmpty(instance.MPDeckLeaderID))
		{
			instance.DeckManager.Decks[instance.SelectedMPDeck].Leader = LeaderManager.Instance.GetLeader(instance.MPDeckLeaderID);
		}
	}

	public static CWDeckBuildDeckController GetInstance()
	{
		return g_deckController;
	}

	public void UpdateUI()
	{
		if (Table != null)
		{
			Table.OnEnable();
		}
		if (heroPanel != null)
		{
			heroPanel.Refresh();
		}
		if (namePlate != null)
		{
			namePlate.Refresh();
		}
		if (LandscapesButton == null)
		{
			LandscapesButton = base.transform.Find("BottomButtons/LandscapeButton").gameObject;
		}
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		Deck deck = deckManager.Decks[currentDeck];
		bool flag = deck.CardCount() > 0;
		SQUtils.SetGray(LandscapesButton, (!flag) ? 0.4f : 1f);
		LandscapesButton.GetComponent<Collider>().enabled = flag;
		UpdateBattleDeckButton();
	}

	private void UpdateBattleDeckButton()
	{
		if (!(BattleDeckButton == null))
		{
			BattleDeckButton.SetToggle(PlayerInfoScript.GetInstance().SelectedMPDeck == currentDeck);
		}
	}

	public void NextDeck()
	{
		currentDeck = (currentDeck + 1) % 5;
		UpdateUI();
	}

	public void PrevDeck()
	{
		currentDeck = ((currentDeck - 1 < 0) ? 4 : (currentDeck - 1));
		UpdateUI();
	}

	public void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (instance.IsReady())
			{
				UpdateUI();
				initialized = true;
			}
		}
	}
}
