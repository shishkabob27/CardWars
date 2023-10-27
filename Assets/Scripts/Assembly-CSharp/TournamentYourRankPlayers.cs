using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class TournamentYourRankPlayers : AsyncData<List<LeaderboardData>>
{
	public GameObject YourRankTemplate;
	public UIButtonTween LoadingActivityShow;
	public UIButtonTween LoadingActivityHide;
}
