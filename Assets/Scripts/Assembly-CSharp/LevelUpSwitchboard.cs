using UnityEngine;

public class LevelUpSwitchboard : MonoBehaviour
{
	public GameObject TweenToLevelUp;

	public GameObject TweenToLoot;

	private GameObject target;

	private static LevelUpSwitchboard switchboard;

	private void Awake()
	{
		switchboard = this;
	}

	public static LevelUpSwitchboard GetInstance()
	{
		return switchboard;
	}

	public void SetSwitchboard()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
		int rank = leader.Rank;
		if (leader != null)
		{
			leader.XP += activeQuest.XPRewarded;
		}
		int rank2 = leader.Rank;
		if (rank2 != rank)
		{
			target = TweenToLevelUp;
		}
		else
		{
			target = TweenToLoot;
		}
	}

	private void OnClick()
	{
		if (target != null)
		{
			target.SendMessage("OnClick");
		}
	}
}
