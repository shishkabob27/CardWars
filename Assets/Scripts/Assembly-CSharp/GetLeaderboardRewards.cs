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
}
