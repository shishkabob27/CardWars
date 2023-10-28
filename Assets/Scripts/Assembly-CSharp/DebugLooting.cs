using UnityEngine;

public class DebugLooting : MonoBehaviour
{
	private QuestEarningManager earningMgr;

	private PlayerInfoScript pInfo;

	private Deck deck;

	public bool getItem;

	private GameState GameInstance;

	private bool initialized;

	private void Start()
	{
		pInfo = PlayerInfoScript.GetInstance();
		earningMgr = QuestEarningManager.GetInstance();
		GameInstance = GameState.Instance;
	}

	private void FillIn()
	{
		QuestData currentQuest = pInfo.GetCurrentQuest();
		deck = ((currentQuest != null) ? AIDeckManager.Instance.GetDeckCopy(currentQuest.OpponentDeckID) : null);
		deck = GameInstance.GetDeck(PlayerType.Opponent);
	}

	private CardItem GetRandomCardItem(Deck cards)
	{
		int idx = NGUITools.RandomRange(0, cards.CardCount() - 1);
		return cards.GetCard(idx);
	}

	private void AddCardsToEarning()
	{
		CardItem cardItem = null;
		if (deck == null)
		{
			deck = GameInstance.GetDeck(PlayerType.Opponent);
		}
		if (deck != null)
		{
			cardItem = GetRandomCardItem(deck);
		}
		if (cardItem != null && earningMgr.earnedCards.Count < 10)
		{
			earningMgr.earnedCards.Add(cardItem);
			earningMgr.earnedCardsName.Add(cardItem.Form.ID);
			earningMgr.hasCardFlag.Add(pInfo.DeckManager.HasCard(cardItem.Form.ID));
		}
	}

	private void Update()
	{
		if (getItem)
		{
			AddCardsToEarning();
			getItem = false;
		}
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (instance.IsReady())
			{
				FillIn();
				initialized = true;
			}
		}
	}
}
