using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class TournamentTopPlayers : AsyncData<List<LeaderboardData>>
{
	public GameObject TopPlayerTemplate;
	public UIButtonTween LoadingActivityShow;
	public UIButtonTween LoadingActivityHide;
}
