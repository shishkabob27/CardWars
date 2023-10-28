using UnityEngine;

public class CWBattleEndRewardLoser : MonoBehaviour
{
	public UILabel TrophiesLost;

	public UILabel TotalTrophies;

	private void OnClick()
	{
		if (GameState.Instance.BattleResolver != null)
		{
			GameState.Instance.BattleResolver.SetResult(PlayerType.Opponent);
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if ((bool)TrophiesLost)
		{
			TrophiesLost.text = instance.MPLossTrophies.ToString();
		}
		if ((bool)TotalTrophies)
		{
			TotalTrophies.text = instance.TotalTrophies.ToString();
		}
		if (GameState.Instance.BattleResolver == null && !GlobalFlags.Instance.InMPMode)
		{
			QuestData activeQuest = GameState.Instance.ActiveQuest;
			instance.mQuestMatchStats[activeQuest.QuestType].Losses++;
		}
		SideQuestManager.Instance.OnBattleEndLoser(GameState.Instance.ActiveQuest);
	}
}
