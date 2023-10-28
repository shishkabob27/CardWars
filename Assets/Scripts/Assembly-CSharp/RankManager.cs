using System.Collections;
using System.Collections.Generic;

public class RankManager : ILoadable
{
	public class RankEntry
	{
		public int DeckMaxSize;

		public int PVPSearchCostCoins;

		public int PVPSearchCostGems;

		public int PVPParticipationCost;
	}

	private Dictionary<int, RankEntry> ranks;

	private static RankManager instance;

	public static RankManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new RankManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] Rankings = SQUtils.ReadJSONData("db_RankUp.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		ranks = new Dictionary<int, RankEntry>();
		Dictionary<string, object>[] array = Rankings;
		foreach (Dictionary<string, object> rk in array)
		{
			RankEntry r = new RankEntry();
			int ix = TFUtils.LoadInt(rk, "LeaderRank", 0);
			r.DeckMaxSize = TFUtils.LoadInt(rk, "DeckMaxSize", 0);
			r.PVPSearchCostCoins = TFUtils.LoadInt(rk, "PvP_Search_Coins", 0);
			r.PVPSearchCostGems = TFUtils.LoadInt(rk, "PvP_Search_Gems", 0);
			r.PVPParticipationCost = TFUtils.LoadInt(rk, "PvP_Entry", 0);
			ranks.Add(ix, r);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public void Destroy()
	{
		instance = null;
	}

	public RankEntry FindRank(int r)
	{
		if (ranks == null)
		{
			return null;
		}
		if (!ranks.ContainsKey(r))
		{
			return null;
		}
		return ranks[r];
	}
}
