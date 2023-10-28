using System.Collections.Generic;

namespace Multiplayer
{
	public class RecentNotification
	{
		public int wins;

		public int losses;

		public int rank;

		public RecentNotification(Dictionary<string, object> dict)
		{
			wins = TFUtils.LoadInt(dict, "wins");
			losses = TFUtils.LoadInt(dict, "losses");
			rank = TFUtils.LoadInt(dict, "rank");
		}
	}
}
