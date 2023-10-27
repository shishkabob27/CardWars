using Multiplayer;
using UnityEngine;

public class SearchOpponent : AsyncData<MatchData>
{
	public RefreshMatch refreshMatch;
	public UILabel CostValue;
	public UIButtonTween SearchStatus;
	public UIButtonTween NotEnoughMoney;
	public UIButtonTween CouldNotFindMatch;
	public UIButtonTween ConnectionFailed;
	public UILabel NotEnoughMoneyDesc;
	public GameObject GoButton;
}
