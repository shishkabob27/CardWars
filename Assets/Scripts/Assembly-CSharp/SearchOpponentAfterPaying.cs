using System;
using Multiplayer;
using UnityEngine;

public class SearchOpponentAfterPaying : AsyncData<string>
{
	public GameObject SearchOpponent;

	public UIButtonTween NoEnoughGems;

	private void OnEnable()
	{
		GameObject gameObject = GameObject.Find("O_5_NeedMoreGems_Show");
		if ((bool)gameObject)
		{
			NoEnoughGems = gameObject.GetComponent<UIButtonTween>();
		}
	}

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.Gems >= 1)
		{
			instance.Gems--;
			instance.Coins += RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPSearchCostCoins;
			instance.Save();
			Singleton<AnalyticsManager>.Instance.LogDeckFindWarOpponentPurchase(1, 0);
			global::Multiplayer.Multiplayer.GetRank(SessionManager.GetInstance().theSession, false, StringCallback);
			SearchOpponent.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		else if ((bool)NoEnoughGems)
		{
			NoEnoughGems.Play(true);
		}
	}

	public void StringCallback(string data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
			if (Asyncdata.MP_Data != null)
			{
				Singleton<AnalyticsManager>.Instance.LogPVPGemsSpentByRank(1, Convert.ToInt32(Asyncdata.MP_Data));
			}
		}
	}
}
