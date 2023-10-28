using UnityEngine;

public class PauseButton : MonoBehaviour
{
	public GameObject onClickTarget;

	private void OnEnable()
	{
		if (GlobalFlags.Instance.InMPMode)
		{
			NGUITools.SetActive(base.gameObject, false);
		}
	}

	private void OnClick()
	{
		if (onClickTarget != null && !TutorialMonitor.Instance.PopupActive && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1Win && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1Lose && BattlePhaseManager.GetInstance().Phase != BattlePhase.P1SetupLaneOpp && BattlePhaseManager.GetInstance().Phase != BattlePhase.P1SetupLanePlyr && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1Defeated && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P2Defeated && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1WinPanel && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1LosePanel && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1MPWinPanel && BattlePhaseManager.GetInstance().Phase != BattlePhase.Result_P1MPLosePanel)
		{
			onClickTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
