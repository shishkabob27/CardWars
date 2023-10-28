using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class GetLeaderboardRewards : AsyncData<List<RewardData>>
{
	public UIButtonTween IntroPopup;

	public UIButtonTween RewardPopup;

	public GameObject CardObj;

	public GameObject GemObj;

	public GameObject CoinObj;

	public UILabel Desc;

	public CompleteTournamentRewards CompleteTournament;

	private List<TournamentManager.Reward>.Enumerator RewardEnumerator;

	private bool ShowNextReward = true;

	private string TournamentRank;

	private void OnEnable()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (Asyncdata.processed && (bool)instance && instance.MPPlayerName != null && instance.MPPlayerName != string.Empty)
		{
			global::Multiplayer.Multiplayer.TournamentReward(SessionManager.GetInstance().theSession, RewardsCallback);
		}
	}

	public void RewardsCallback(List<RewardData> data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (Asyncdata.processed)
		{
			return;
		}
		Asyncdata.processed = true;
		if (Asyncdata.MP_Data == null)
		{
			return;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		foreach (RewardData mP_Datum in Asyncdata.MP_Data)
		{
			if (mP_Datum.rank <= 50)
			{
				SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA_50);
			}
			if (mP_Datum.rank <= 10)
			{
				SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA_10);
			}
			if (mP_Datum.rank <= 1)
			{
				SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ARENA_1);
			}
			TournamentRank = mP_Datum.rank.ToString();
			List<TournamentManager.Reward> rewards = Singleton<TournamentManager>.Instance.GetRewards(mP_Datum.tournamentId, mP_Datum.rank);
			if (rewards != null)
			{
				foreach (TournamentManager.Reward item in rewards)
				{
					switch (item.Type)
					{
					case TournamentManager.RewardType.Card:
					{
						PlayerDeckManager deckManager = instance.DeckManager;
						CardForm card = CardDataManager.Instance.GetCard(item.CardName);
						CardItem card2 = new CardItem(card);
						deckManager.AddCard(card2);
						break;
					}
					case TournamentManager.RewardType.Gem:
						instance.Gems += item.Amount;
						instance.Save();
						break;
					case TournamentManager.RewardType.Coin:
						instance.Coins += item.Amount;
						instance.Save();
						break;
					}
				}
			}
			if ((bool)CompleteTournament)
			{
				CompleteTournament.CompleteTournamenentReward(mP_Datum.tournamentId);
			}
			if ((bool)IntroPopup)
			{
				IntroPopup.Play(true);
			}
			rewards = Singleton<TournamentManager>.Instance.GetRewards(mP_Datum.tournamentId, mP_Datum.rank);
			RewardEnumerator = rewards.GetEnumerator();
			RewardEnumerator.MoveNext();
			ShowNextReward = rewards.Count > 0;
		}
	}

	public void ShowReward()
	{
		if (!ShowNextReward || RewardEnumerator.Current == null)
		{
			return;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		switch (RewardEnumerator.Current.Type)
		{
		case TournamentManager.RewardType.Card:
		{
			GemObj.SetActive(false);
			CoinObj.SetActive(false);
			CardObj.SetActive(true);
			PlayerDeckManager deckManager = instance.DeckManager;
			CardForm card = CardDataManager.Instance.GetCard(RewardEnumerator.Current.CardName);
			CardItem card2 = new CardItem(card);
			PanelManagerDeck.GetInstance().FillCardInfo(CardObj, card2);
			if ((bool)Desc)
			{
				Desc.text = KFFLocalization.Get("!!7_TOURNAMENTREWARD_1");
				Desc.text = Desc.text.Replace("<val1>", TournamentRank);
				if (card.Quality == Quality.Obsidian)
				{
					Desc.text = Desc.text.Replace("<val2>", "[000000]" + KFFLocalization.Get("!!BLACKCARD") + "[-]");
				}
				else if (card.Quality == Quality.Gold)
				{
					Desc.text = Desc.text.Replace("<val2>", "[ffcc33]" + KFFLocalization.Get("!!GOLDCARD") + "[-]");
				}
				else
				{
					Desc.text = Desc.text.Replace("<val2>", "[ffffff]" + KFFLocalization.Get("!!CARD") + "[-]");
				}
			}
			break;
		}
		case TournamentManager.RewardType.Gem:
			GemObj.SetActive(true);
			CoinObj.SetActive(false);
			CardObj.SetActive(false);
			if ((bool)Desc)
			{
				Desc.text = KFFLocalization.Get("!!7_TOURNAMENTREWARD_3");
				Desc.text = Desc.text.Replace("<val1>", TournamentRank);
				Desc.text = Desc.text.Replace("<val2>", RewardEnumerator.Current.Amount.ToString());
			}
			break;
		case TournamentManager.RewardType.Coin:
			GemObj.SetActive(false);
			CoinObj.SetActive(true);
			CardObj.SetActive(false);
			if ((bool)Desc)
			{
				Desc.text = KFFLocalization.Get("!!7_TOURNAMENTREWARD_4");
				Desc.text = Desc.text.Replace("<val1>", TournamentRank);
				Desc.text = Desc.text.Replace("<val2>", RewardEnumerator.Current.Amount.ToString());
			}
			break;
		}
		if ((bool)RewardPopup)
		{
			RewardPopup.Play(true);
		}
		ShowNextReward = RewardEnumerator.MoveNext();
	}
}
