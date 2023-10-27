using System.Collections.Generic;

namespace Multiplayer
{
	public class LeaderboardData
	{
		public LeaderboardData(Dictionary<string, object> dict)
		{
		}

		public int rank;
		public int previousRank;
		public string name;
		public string icon;
		public string leader;
		public int leaderLevel;
		public int trophies;
		public int wins;
		public int losses;
	}
}
