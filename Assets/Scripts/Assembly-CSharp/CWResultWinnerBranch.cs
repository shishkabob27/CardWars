using System.Collections;
using UnityEngine;

public class CWResultWinnerBranch : MonoBehaviour
{
	public BattlePhase targetPhase;

	public float delay;

	private void OnClick()
	{
		StartCoroutine(ResultBranchTimer(delay));
	}

	private IEnumerator ResultBranchTimer(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		BattlePhaseManager phaseMgr = BattlePhaseManager.GetInstance();
		GameState GameInstance = GameState.Instance;
		phaseMgr.Phase = targetPhase;
		if (GameInstance.GetHealth(PlayerType.User) <= 0)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.Lost);
			yield break;
		}
		GameState.Instance.AwardStaticLoot();
		TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.Won);
		VOManager.Instance.PlayEvent(PlayerType.User, VOEvent.Win);
	}

	private void Update()
	{
	}
}
