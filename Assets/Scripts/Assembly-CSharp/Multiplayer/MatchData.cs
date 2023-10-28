using System;
using System.Collections.Generic;

namespace Multiplayer
{
	public class MatchData
	{
		public string matchId;

		public string opponentName;

		public string opponentIcon;

		public string opponentLeader;

		public string landscapes;

		public int opponentLeaderLevel;

		public int wagerWin;

		public int wagerLose;

		public DateTime expiration;

		public int winStreak;

		public int streakBonus;

		public MatchData(Dictionary<string, object> dict)
		{
			matchId = dict["match_id"].ToString();
			opponentName = dict["name"].ToString();
			opponentIcon = dict["icon"].ToString();
			landscapes = dict["landscapes"].ToString();
			opponentLeader = dict["leader"].ToString();
			opponentLeaderLevel = TFUtils.LoadInt(dict, "leader_level");
			wagerWin = TFUtils.LoadInt(dict, "wager_win");
			wagerLose = TFUtils.LoadInt(dict, "wager_lose");
			winStreak = TFUtils.LoadInt(dict, "streak");
			streakBonus = TFUtils.LoadInt(dict, "streak_bonus");
			expiration = DateTime.Parse(dict["expiration_date"].ToString());
		}
	}
}
