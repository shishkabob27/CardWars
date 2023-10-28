using System;
using System.Collections.Generic;

namespace Multiplayer
{
	public class TournamentData
	{
		public int tournamentId;

		public DateTime endDate;

		public TournamentData(Dictionary<string, object> dict)
		{
			tournamentId = TFUtils.LoadInt(dict, "tournament_id");
			endDate = DateTime.Parse(dict["time"].ToString());
		}
	}
}
