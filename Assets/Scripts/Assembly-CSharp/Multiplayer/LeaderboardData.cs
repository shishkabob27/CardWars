using System.Collections.Generic;

namespace Multiplayer
{
	public class LeaderboardData
	{
		public int rank;

		public int previousRank;

		public string name;

		public string icon;

		public string leader;

		public int leaderLevel;

		public int trophies;

		public int wins;

		public int losses;

		public LeaderboardData(Dictionary<string, object> dict)
		{
			rank = TFUtils.LoadInt(dict, "rank");
			previousRank = TFUtils.LoadInt(dict, "prev_rank");
			name = dict["player_name"].ToString();
			icon = dict["icon"].ToString();
			leader = dict["leader"].ToString();
			leaderLevel = TFUtils.LoadInt(dict, "leader_level");
			trophies = TFUtils.LoadInt(dict, "trophies");
			wins = TFUtils.LoadInt(dict, "wins");
			losses = TFUtils.LoadInt(dict, "losses");
		}

		public static List<LeaderboardData> ProcessList(List<object> data)
		{
			List<LeaderboardData> list = new List<LeaderboardData>(data.Count);
			foreach (object datum in data)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)datum;
				list.Add(new LeaderboardData(dict));
			}
			return list;
		}
	}
}
