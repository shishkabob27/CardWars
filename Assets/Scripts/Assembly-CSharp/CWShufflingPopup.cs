using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWShufflingPopup : MonoBehaviour
{
	public AudioClip cardShufflePanelSound;

	public AudioClip cardShuffleSound;

	public int playerInteger;

	private PlayerType player;

	private GameState GameInstance;

	public CWUpdatePlayerData PlayerData;

	public void Reset()
	{
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
	}

	public void OnClick()
	{
		player = playerInteger;
		UICamera.useInputEnabler = true;
		GameDataScript.GetInstance().stopUpdateFlag = true;
		StartCoroutine(ProcessCards());
		if (player == PlayerType.User)
		{
			GameInstance.SetHealth(player, GameInstance.GetMaxHealth(player));
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(cardShufflePanelSound);
		if (player == PlayerType.User)
		{
			PlayerInfoScript.GetInstance().Gems--;
			PlayerInfoScript.GetInstance().Save();
		}
		if (PlayerData != null)
		{
			PlayerData.Resurrect();
		}
	}

	private IEnumerator ProcessActionPoints()
	{
		int points = GameState.Instance.GetMagicPoints(player);
		int count = GameState.Instance.DiscardCount(player) + 5;
		float delay = (float)count / 50f;
		while (points > 0)
		{
			GameState.Instance.AddMagicPoints(player, -1);
			points = GameState.Instance.GetMagicPoints(player);
			yield return new WaitForSeconds(delay);
		}
	}

	private IEnumerator ProcessCards()
	{
		List<CardItem> currentHand = GameInstance.GetHand(player);
		while (currentHand.Count > 0)
		{
			CardItem card = GameInstance.GetCardInHand(player, currentHand.Count - 1);
			GameInstance.RemoveCardFromHand(player, card);
			GameInstance.DiscardCard(player, card);
		}
		List<CardItem> Pile = GameInstance.GetDiscardPile(player);
		Deck CurrentDeck = GameInstance.GetDeck(player);
		while (Pile.Count > 0)
		{
			CardItem item = Pile[0];
			Pile.RemoveAt(0);
			CurrentDeck.AddCard(item);
			yield return new WaitForSeconds(0.1f);
		}
		GameState.Instance.Reshuffle(player);
		for (int i = 0; i < 5; i++)
		{
			GameState.Instance.DrawCard(player);
			yield return new WaitForSeconds(0.5f);
		}
		UICamera.useInputEnabler = false;
		GameDataScript.GetInstance().stopUpdateFlag = false;
	}

	private void Update()
	{
	}
}
