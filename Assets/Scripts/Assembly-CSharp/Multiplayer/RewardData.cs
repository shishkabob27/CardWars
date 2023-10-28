using System.Collections.Generic;

namespace Multiplayer
{
	public class RewardData
	{
		public int tournamentId;

		public int rank;

		public RewardData(Dictionary<string, object> dict)
		{
			tournamentId = TFUtils.LoadInt(dict, "tournament_id");
			rank = TFUtils.LoadInt(dict, "outcome");
		}

		public static List<RewardData> ProcessList(List<object> data)
		{
			List<RewardData> list = new List<RewardData>(data.Count);
			foreach (object datum in data)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)datum;
				list.Add(new RewardData(dict));
			}
			return list;
		}
	}
}
