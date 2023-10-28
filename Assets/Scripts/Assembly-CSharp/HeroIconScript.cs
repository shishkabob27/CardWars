using System;
using UnityEngine;

public class HeroIconScript : MonoBehaviour
{
	public UISprite HeroSprite;

	private string lastSprite = string.Empty;

	private bool initialized;

	private void Update()
	{
		if (initialized)
		{
			Refresh();
			return;
		}
		SessionManager instance = SessionManager.GetInstance();
		if (instance.IsReady())
		{
			Refresh();
			initialized = true;
		}
	}

	private void Refresh()
	{
		try
		{
			Deck currentDeck = GetCurrentDeck();
			LeaderItem leaderItem = null;
			if (currentDeck != null)
			{
				leaderItem = currentDeck.Leader;
			}
			if (leaderItem != null && leaderItem.Form.SpriteName != lastSprite)
			{
				SQUtils.SetIcon(HeroSprite, leaderItem.Form.IconAtlas, leaderItem.Form.SpriteName);
				lastSprite = leaderItem.Form.SpriteName;
			}
		}
		catch (NullReferenceException)
		{
		}
	}

	private Deck GetCurrentDeck()
	{
		Deck result = null;
		if (CWDeckController.GetInstance() != null)
		{
			int currentDeck = CWDeckController.GetInstance().currentDeck;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			result = instance.DeckManager.Decks[currentDeck];
		}
		else if (GameState.Instance != null)
		{
			result = GameState.Instance.GetDeck(PlayerType.User);
		}
		return result;
	}
}
