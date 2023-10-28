using System.Collections.Generic;
using UnityEngine;

public class CardManagerScript : MonoBehaviour
{
	private GameState GameInstance;

	public bool CardSelected;

	public int CardsDiscarded;

	public bool Discarding;

	public bool GnomeSnot;

	public bool UpdateNow;

	public List<int> ChosenCardIDs;

	public List<int> ChosenCardTypes;

	public GameObject[] CardObjects;

	private static CardManagerScript g_cardManager;

	private bool isInitialized;

	public bool IsInitialized
	{
		get
		{
			return isInitialized;
		}
	}

	private void Awake()
	{
		g_cardManager = this;
	}

	public static CardManagerScript GetInstance()
	{
		return g_cardManager;
	}

	private void Initialize()
	{
		GameInstance = GameState.Instance;
		for (int i = 0; i < 7; i++)
		{
		}
		GameInstance.CardManager = this;
		GameInstance.Setup();
		isInitialized = true;
	}

	public void NewCard(int idx)
	{
	}

	public void HideOrShowCards()
	{
		List<CardItem> hand = GameInstance.GetHand(PlayerType.User);
		for (int i = 0; i < 5; i++)
		{
			if (i < hand.Count)
			{
				NGUITools.SetActive(CardObjects[i], true);
			}
			else
			{
				NGUITools.SetActive(CardObjects[i], false);
			}
		}
	}

	private void DebugDrawCard(PlayerType player)
	{
		if (GameInstance.GetCardsInDeck(player).Count != 0)
		{
			GameInstance.DrawCard(player);
		}
	}

	private void DebugRemoveCard(PlayerType player)
	{
		List<CardItem> hand = GameInstance.GetHand(player);
		CardSelected = false;
		for (int i = 0; i < 7; i++)
		{
		}
		if (hand.Count > 0)
		{
			CardItem cardInHand = GameInstance.GetCardInHand(player, hand.Count - 1);
			GameInstance.RemoveCardFromHand(player, cardInHand);
			GameInstance.DiscardCard(player, cardInHand);
		}
	}

	private void DebugDiscardCard(PlayerType player)
	{
		List<CardItem> hand = GameInstance.GetHand(player);
		CardSelected = false;
		for (int i = 0; i < 7; i++)
		{
		}
		if (hand.Count > 0)
		{
			CardItem cardInHand = GameInstance.GetCardInHand(player, hand.Count - 1);
			GameInstance.RemoveCardFromHand(player, cardInHand);
			GameInstance.DiscardCard(player, cardInHand);
		}
	}

	private void Update()
	{
		if (!isInitialized && SessionManager.GetInstance().IsReady())
		{
			Initialize();
		}
		if (Input.GetKeyDown("a"))
		{
			DebugDrawCard(PlayerType.User);
		}
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			DebugDrawCard(PlayerType.Opponent);
		}
		if (Input.GetKeyDown("r"))
		{
			DebugRemoveCard(PlayerType.User);
		}
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			DebugRemoveCard(PlayerType.Opponent);
		}
		if (!Input.GetKeyDown("d"))
		{
		}
	}
}
