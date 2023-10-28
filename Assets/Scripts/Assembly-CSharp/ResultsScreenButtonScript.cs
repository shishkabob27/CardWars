using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsScreenButtonScript : MonoBehaviour
{
	public GameObject WinnerEarnings;

	public GameObject LoserEarnings;

	public GameObject RewardCard;

	public ResultsCardsScript ResultsCards;

	public UILabel CoinsLabel;

	public UILabel HeaderLabel;

	public UILabel RewardLabel;

	public UILabel LoserRewardLabel;

	private ResultsDweebCardScript UnlockedCard;

	private GameDataScript GameData;

	public int Phase = 1;

	public CardItem CardData;

	private void Start()
	{
		NGUITools.SetActive(LoserEarnings, false);
		UnlockedCard = GameObject.Find("UnlockedCard").GetComponent<ResultsDweebCardScript>();
		GameData = GameDataScript.GetInstance();
		List<string> earnedCardsName = QuestEarningManager.GetInstance().earnedCardsName;
		string text = string.Empty;
		for (int i = 0; i < earnedCardsName.Count; i++)
		{
			text = text + "\n" + earnedCardsName[i];
		}
		RewardLabel.text = KFFLocalization.Get("!!CARDS_WON") + text;
		CoinsLabel.text = KFFLocalization.Get("!!COINS_EARNED") + GameData.P1_CoinsEarned;
		if (GameState.Instance.GetHealth(PlayerType.Opponent) <= 0)
		{
			Phase = 1;
			return;
		}
		Phase = 4;
		UpdateText();
	}

	private void OnClick()
	{
		Phase++;
		UpdateText();
	}

	private void UpdateText()
	{
		if (Phase == 2)
		{
			NGUITools.SetActive(WinnerEarnings, false);
			string text = ((GameData.UnlockCard == null) ? string.Empty : GameData.UnlockCard.Form.Name);
			RewardLabel.text = KFFLocalization.Get("!!YOU_UNLOCKED") + "\n" + text;
			UnlockedCard.CardData = GameData.UnlockCard;
			UnlockedCard.enabled = true;
			UnlockedCard.UpdateCard();
			UnlockedCard.Grow = true;
			ResultsCards.Shrink = true;
		}
		if (Phase == 3)
		{
			Resources.UnloadUnusedAssets();
			GlobalFlags.Instance.ReturnToMainMenu = true;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			QuestData currentQuest = instance.GetCurrentQuest();
			if (instance.GetQuestProgress(currentQuest) > 0)
			{
				GlobalFlags.Instance.NewlyCleared = true;
				instance.SetLastClearedQuest(currentQuest);
			}
			List<CardItem> earnedCards = QuestEarningManager.GetInstance().earnedCards;
			instance.DeckManager.AddCards(earnedCards);
			instance.Save();
			StartCoroutine(GoToMainMenu());
		}
		if (Phase == 4)
		{
			HeaderLabel.text = KFFLocalization.Get("!!DRINK_UP_DWEEB");
			List<CardItem> dweebCup = GameState.Instance.GetDweebCup();
			string text2 = string.Empty;
			for (int i = 0; i < dweebCup.Count; i++)
			{
				text2 = text2 + "\n" + dweebCup[i].Form.Name;
			}
			RewardLabel.text = KFFLocalization.Get("!!YOU_LOSE_YOU_DRANK") + text2;
			UnlockedCard.Grow = false;
			NGUITools.SetActive(LoserEarnings, true);
		}
		if (Phase == 5)
		{
			Resources.UnloadUnusedAssets();
			GlobalFlags.Instance.ReturnToMainMenu = true;
			PlayerInfoScript.GetInstance().Save();
			StartCoroutine(GoToMainMenu());
		}
	}

	private IEnumerator GoToMainMenu()
	{
		UICamera.useInputEnabler = true;
		float savedTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return Resources.UnloadUnusedAssets();
		Time.timeScale = savedTimeScale;
		UICamera.useInputEnabler = false;
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("AdventureTime");
	}
}
