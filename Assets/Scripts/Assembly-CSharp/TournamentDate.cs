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
}
