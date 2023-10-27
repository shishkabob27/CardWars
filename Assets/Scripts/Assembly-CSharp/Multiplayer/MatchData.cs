using System.Collections.Generic;

namespace Multiplayer
{
	public class MatchData
	{
		public MatchData(Dictionary<string, object> dict)
		{
		}

		public string matchId;
		public string opponentName;
		public string opponentIcon;
		public string opponentLeader;
		public string landscapes;
		public int opponentLeaderLevel;
		public int wagerWin;
		public int wagerLose;
		public int winStreak;
		public int streakBonus;
	}
}
