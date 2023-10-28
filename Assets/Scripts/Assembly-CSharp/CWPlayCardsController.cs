using System.Collections.Generic;
using UnityEngine;

public class CWPlayCardsController : MonoBehaviour
{
	public GameObject[] cards;

	public GameObject playingCard;

	private GameState GameInstance;

	public int player;

	private List<CardItem> CurrentHand;

	private int currentHandCount;

	private int prevHandCount;

	private bool initialized;

	private void Refresh()
	{
		GameInstance = GameState.Instance;
		CurrentHand = GameInstance.GetHand(player);
	}

	private void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (!instance.IsReady())
			{
				return;
			}
			initialized = true;
			Refresh();
		}
		CurrentHand = GameInstance.GetHand(PlayerType.User);
		if (CurrentHand.Count != prevHandCount)
		{
			SetHoldingCardsVisibility();
			prevHandCount = CurrentHand.Count;
		}
	}

	public void SetHoldingCardsVisibility()
	{
		for (int i = 0; i < 7; i++)
		{
			bool active = ((i < CurrentHand.Count) ? true : false);
			cards[i].SetActive(active);
		}
	}
}
