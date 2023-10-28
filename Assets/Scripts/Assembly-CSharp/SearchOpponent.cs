using System.Collections;
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

	private bool SearchTimerExpired;

	private int SearchFeesCoins;

	private int SearchFeesGems;

	private void Start()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		CostValue.text = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPSearchCostCoins.ToString();
	}

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		RankManager.RankEntry rankEntry = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank());
		if (rankEntry != null)
		{
			SearchFeesCoins = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPSearchCostCoins;
			SearchFeesGems = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPSearchCostGems;
		}
		else
		{
			SearchFeesCoins = 1;
			SearchFeesGems = 1;
		}
		if (instance.Coins < SearchFeesCoins && (bool)NotEnoughMoney)
		{
			NotEnoughMoney.Play(true);
		}
		else if (Asyncdata.processed)
		{
			if ((bool)SearchStatus)
			{
				SearchStatus.Play(true);
			}
			StartCoroutine("SearchTimer");
			global::Multiplayer.Multiplayer.MatchMake(SessionManager.GetInstance().theSession, instance.DeckManager.GetHighestLeaderRank(), MatchDataCallback);
		}
	}

	private IEnumerator SearchTimer()
	{
		SearchTimerExpired = false;
		yield return new WaitForSeconds(2f);
		SearchTimerExpired = true;
		yield return null;
	}

	public void MatchDataCallback(MatchData data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (Asyncdata.processed || !SearchTimerExpired)
		{
			return;
		}
		Asyncdata.processed = true;
		SearchTimerExpired = false;
		if ((bool)SearchStatus)
		{
			SearchStatus.Play(false);
		}
		if (Asyncdata.MP_Data != null)
		{
			if ((bool)refreshMatch)
			{
				Asyncdata.MP_Data.opponentLeader = ValidateOpponentLeader(Asyncdata.MP_Data.opponentLeader);
				refreshMatch.RefreshValues(Asyncdata.MP_Data);
			}
			if ((bool)GoButton)
			{
				GoButton.SetActive(true);
			}
			UICamera.UnlockInput();
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if ((bool)CostValue && (bool)instance)
			{
				instance.Coins -= SearchFeesCoins;
				instance.Save();
				Singleton<AnalyticsManager>.Instance.LogDeckFindWarOpponentPurchase(0, SearchFeesCoins);
			}
		}
		else
		{
			CouldNotFindMatch.Play(true);
			ConnectionFailed.Play(true);
			SearchTimerExpired = false;
		}
	}

	private string ValidateOpponentLeader(string leader)
	{
		if (LeaderManager.Instance.IsLeaderFromFC(leader))
		{
			return "Leader_Finn";
		}
		return leader;
	}
}
