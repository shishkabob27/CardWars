using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class TournamentYourRankPlayers : AsyncData<List<LeaderboardData>>
{
	public GameObject YourRankTemplate;

	public UIButtonTween LoadingActivityShow;

	public UIButtonTween LoadingActivityHide;

	private Quaternion GoUp;

	private Quaternion GoDown;

	private Quaternion NoChange;

	private void OnEnable()
	{
		if (Asyncdata.processed)
		{
			if ((bool)LoadingActivityShow)
			{
				LoadingActivityShow.Play(true);
			}
			global::Multiplayer.Multiplayer.NearbyLeaderboard(SessionManager.GetInstance().theSession, LeaderboardCallback);
			GoUp = Quaternion.Euler(0f, 0f, 90f);
			GoDown = Quaternion.Euler(0f, 0f, -90f);
			NoChange = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void LeaderboardCallback(List<LeaderboardData> data)
	{
		ResponseFlag a_flag = ResponseFlag.Success;
		Asyncdata.Set(a_flag, data);
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
		if (Asyncdata.MP_Data == null)
		{
			return;
		}
		UIFastGrid component = base.gameObject.GetComponent<UIFastGrid>();
		List<object> list = new List<object>();
		foreach (LeaderboardData mP_Datum in Asyncdata.MP_Data)
		{
			list.Add(mP_Datum);
		}
		if ((bool)component)
		{
			component.Initialize(list, pickPrefab, fillItem);
		}
	}

	private GameObject pickPrefab(object data)
	{
		return YourRankTemplate;
	}

	private void fillItem(GameObject aLeaderboardItemObj, object data)
	{
		LeaderboardData leaderboardData = data as LeaderboardData;
		LeaderboardItem component = aLeaderboardItemObj.GetComponent<LeaderboardItem>();
		component.Name.text = leaderboardData.name;
		component.Rank.text = leaderboardData.rank.ToString();
		component.wins.text = leaderboardData.wins.ToString();
		component.losses.text = leaderboardData.losses.ToString();
		component.Trophies.text = leaderboardData.trophies.ToString();
		int num = leaderboardData.rank - leaderboardData.previousRank;
		if (leaderboardData.name == PlayerInfoScript.GetInstance().MPPlayerName)
		{
			component.BG.color = Color.green;
		}
		else
		{
			component.BG.color = new Color(0.77f, 1f, 1f);
		}
		if (leaderboardData.rank <= 3)
		{
			Color[] array = new Color[3]
			{
				new Color(1f, 0.752f, 0f),
				new Color(0.525f, 0.752f, 0.8f),
				new Color(0.8f, 0.517f, 0.105f)
			};
			component.RankBG.GetComponent<UISlicedSprite>().color = array[leaderboardData.rank - 1];
		}
		else
		{
			component.RankBG.GetComponent<UISlicedSprite>().color = Color.white;
		}
		if (num > 0)
		{
			component.RankTrend.text = (leaderboardData.rank - leaderboardData.previousRank).ToString();
			component.RankTrendArrow.SetActive(true);
			component.RankTrendArrow.transform.rotation = GoDown;
			component.RankTrendArrow.GetComponent<UISlicedSprite>().color = Color.red;
			component.RankTrend.text = num.ToString();
			component.RankTrend.color = Color.red;
		}
		else if (num < 0)
		{
			component.RankTrendArrow.SetActive(true);
			component.RankTrendArrow.transform.rotation = GoUp;
			component.RankTrendArrow.GetComponent<UISlicedSprite>().color = Color.green;
			component.RankTrend.text = (-num).ToString();
			component.RankTrend.color = Color.green;
		}
		else
		{
			component.RankTrendArrow.SetActive(false);
			component.RankTrend.text = string.Empty;
		}
	}
}
