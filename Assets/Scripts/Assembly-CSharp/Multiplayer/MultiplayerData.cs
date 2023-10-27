using System.Collections.Generic;

namespace Multiplayer
{
	public class MultiplayerData
	{
		public MultiplayerData(Dictionary<string, object> dict)
		{
		}

		public string name;
		public string icon;
		public string leader;
		public int leaderLevel;
		public int trophies;
		public bool rename;
	}
}
