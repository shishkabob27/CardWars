using System;
using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class TournamentDate : AsyncData<TournamentData>
{
	public UILabel TopPlayerTournamentName;

	public UILabel TopPlayerEndDate;

	public UILabel YourRankTournamentName;

	public UILabel YourRankEndDate;

	public UIButtonTween UnderMaintenance;

	public GameObject[] RewardSlots;

	public UILabel[] RewardDesc;

	public UIButtonTween LoadingActivityShow;

	public UIButtonTween LoadingActivityHide;

	private DateTime TournamentEnd;

	private bool ReceivedDate;

	private void OnEnable()
	{
		if (Asyncdata.processed)
		{
			if ((bool)LoadingActivityShow)
			{
				LoadingActivityShow.Play(true);
			}
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			global::Multiplayer.Multiplayer.GetTournamentEndDate(SessionManager.GetInstance().theSession, instance.Cheater, TournamentDataCallback);
			StartCoroutine("Countdown");
		}
	}

	private void Ondisable()
	{
		StopCoroutine("Countdown");
	}

	private void PrintoutCountdown()
	{
		DateTime serverTime = TFUtils.ServerTime;
		string text;
		if (serverTime < TournamentEnd && ReceivedDate)
		{
			TimeSpan timeSpan = TournamentEnd - serverTime;
			text = timeSpan.Days + "d " + timeSpan.Hours + "h " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s";
		}
		else
		{
			text = KFFLocalization.Get("!!COMPUTING_REWARDS");
		}
		if ((bool)TopPlayerEndDate)
		{
			TopPlayerEndDate.text = text;
		}
		if ((bool)YourRankEndDate)
		{
			YourRankEndDate.text = text;
		}
	}

	private IEnumerator Countdown()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			PrintoutCountdown();
		}
	}

	public void TournamentDataCallback(TournamentData data, ResponseFlag flag)
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
		if ((bool)LoadingActivityHide)
		{
			LoadingActivityHide.Play(true);
		}
		if (Asyncdata.MP_Data != null)
		{
			ReceivedDate = true;
			string text = null;
			TournamentManager.Tournament currentTournament = Singleton<TournamentManager>.Instance.GetCurrentTournament(Asyncdata.MP_Data.tournamentId);
			if (currentTournament == null)
			{
				return;
			}
			text = currentTournament.Name;
			if ((bool)YourRankTournamentName)
			{
				YourRankTournamentName.text = text;
			}
			if ((bool)TopPlayerTournamentName)
			{
				TopPlayerTournamentName.text = text;
			}
			TournamentEnd = Convert.ToDateTime(Asyncdata.MP_Data.endDate);
			PrintoutCountdown();
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			List<TournamentManager.Reward> rewards = Singleton<TournamentManager>.Instance.GetRewards(currentTournament.ID, 1);
			int num = 0;
			{
				foreach (TournamentManager.Reward item in rewards)
				{
					switch (item.Type)
					{
					case TournamentManager.RewardType.Card:
					{
						GameObject gameObject2 = RewardSlots[num].transform.Find("Icon_Gem").gameObject;
						gameObject2.SetActive(false);
						GameObject gameObject3 = RewardSlots[num].transform.Find("Icon_Coin").gameObject;
						gameObject3.SetActive(false);
						PlayerDeckManager deckManager = instance.DeckManager;
						CardForm card = CardDataManager.Instance.GetCard(item.CardName);
						GameObject gameObject;
						if (card.Quality == Quality.Obsidian)
						{
							gameObject = RewardSlots[num].transform.Find("Icon_BlackCard").gameObject;
							RewardDesc[num].text = KFFLocalization.Get("!!BLACKCARD");
						}
						else if (card.Quality == Quality.Gold)
						{
							gameObject = RewardSlots[num].transform.Find("Icon_GoldCard").gameObject;
							RewardDesc[num].text = KFFLocalization.Get("!!GOLDCARD");
						}
						else
						{
							gameObject = RewardSlots[num].transform.Find("Icon_GoldCard").gameObject;
							RewardDesc[num].text = KFFLocalization.Get("!!CARD");
						}
						gameObject.SetActive(true);
						break;
					}
					case TournamentManager.RewardType.Gem:
					{
						GameObject gameObject = RewardSlots[num].transform.Find("DeckCard").gameObject;
						gameObject.SetActive(false);
						GameObject gameObject3 = RewardSlots[num].transform.Find("Icon_Coin").gameObject;
						gameObject3.SetActive(false);
						GameObject gameObject2 = RewardSlots[num].transform.Find("Icon_Gem").gameObject;
						gameObject2.SetActive(true);
						RewardDesc[num].text = KFFLocalization.Get("!!GEMS");
						break;
					}
					case TournamentManager.RewardType.Coin:
					{
						GameObject gameObject = RewardSlots[num].transform.Find("DeckCard").gameObject;
						gameObject.SetActive(false);
						GameObject gameObject2 = RewardSlots[num].transform.Find("Icon_Gem").gameObject;
						gameObject2.SetActive(false);
						GameObject gameObject3 = RewardSlots[num].transform.Find("Icon_Coin").gameObject;
						gameObject3.SetActive(true);
						RewardDesc[num].text = KFFLocalization.Get("!!COINS");
						break;
					}
					}
					num++;
				}
				return;
			}
		}
		if ((bool)UnderMaintenance)
		{
			UnderMaintenance.Play(true);
		}
	}
}
