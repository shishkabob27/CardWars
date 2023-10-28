using UnityEngine;

public class CWShowPostgame : MonoBehaviour
{
	public GameObject LoseFlow;

	public GameObject WinFlow;

	public GameObject EmptyObj;

	private void OnEnable()
	{
	}

	private void OnClick()
	{
		BattlePhaseManager instance = BattlePhaseManager.GetInstance();
		GameState instance2 = GameState.Instance;
		instance.Phase = BattlePhase.GameOver;
		UIButtonTween component = base.gameObject.GetComponent<UIButtonTween>();
		if (component != null)
		{
			if (instance2.GetHealth(PlayerType.User) <= 0)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.Lost);
				component.tweenTarget = LoseFlow;
				return;
			}
			GameState.Instance.AwardStaticLoot();
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.Won);
			VOManager.Instance.PlayEvent(PlayerType.User, VOEvent.Win);
			component.tweenTarget = WinFlow;
		}
	}
}
