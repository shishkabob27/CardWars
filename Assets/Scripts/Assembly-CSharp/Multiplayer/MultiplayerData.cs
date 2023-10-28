using System.Collections.Generic;

namespace Multiplayer
{
	public class MultiplayerData
	{
		public string name;

		public string icon;

		public string leader;

		public int leaderLevel;

		public int trophies;

		public bool rename;

		public MultiplayerData(Dictionary<string, object> dict)
		{
			name = dict["name"].ToString();
			icon = dict["icon"].ToString();
			leader = dict["leader"].ToString();
			leaderLevel = TFUtils.LoadInt(dict, "level");
			trophies = TFUtils.LoadInt(dict, "trophies");
			if (dict.ContainsKey("needs_rename"))
			{
				rename = (bool)dict["needs_rename"];
			}
		}
	}
}
