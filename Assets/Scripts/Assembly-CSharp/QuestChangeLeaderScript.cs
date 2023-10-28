using System.Collections.Generic;
using UnityEngine;

public class QuestChangeLeaderScript : MonoBehaviour
{
	public bool Increment;

	public LeaderSelectController LeaderSelect;

	private void OnClick()
	{
		List<LeaderItem> leaders = LeaderManager.Instance.leaders;
		int num = leaders.IndexOf(LeaderSelect.SelectedLeader);
		int count = leaders.Count;
		LeaderSelect.SetSelectedLeader(leaders[num]);
		num = ((!Increment) ? ((num == 0) ? (num = count - 1) : (--num)) : ((num + 1) % count));
		LeaderSelect.SetSelectedLeader(leaders[num]);
	}
}
