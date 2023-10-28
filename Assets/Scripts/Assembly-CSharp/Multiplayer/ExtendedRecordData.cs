using System;
using System.Collections.Generic;

namespace Multiplayer
{
	public class ExtendedRecordData
	{
		public string defenderName;

		public string defenderIcon;

		public int outcome;

		public string attackerName;

		public string attackerIcon;

		public bool attackerWon;

		public DateTime datetime;

		public ExtendedRecordData(Dictionary<string, object> dict)
		{
			defenderName = dict["defender_name"].ToString();
			defenderIcon = dict["defender_icon"].ToString();
			outcome = TFUtils.LoadInt(dict, "outcome");
			attackerName = dict["attacker_name"].ToString();
			attackerIcon = dict["attacker_icon"].ToString();
			attackerWon = TFUtils.LoadBool(dict, "attacker_won", false);
			datetime = DateTime.Parse(dict["date"].ToString());
		}

		public static List<ExtendedRecordData> ProcessList(List<object> data)
		{
			List<ExtendedRecordData> list = new List<ExtendedRecordData>(data.Count);
			foreach (object datum in data)
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)datum;
				list.Add(new ExtendedRecordData(dict));
			}
			return list;
		}
	}
}
