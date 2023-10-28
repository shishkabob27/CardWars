using UnityEngine;

public class CWDetermineDweeb : MonoBehaviour
{
	private BattlePhaseManager phaseMgr;

	private GameState GameInstance;

	public GameObject TweenController;

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
		GameInstance = GameState.Instance;
	}

	private void OnClick()
	{
		phaseMgr.Phase = BattlePhase.GameOver;
		if (GameInstance.GetHealth(PlayerType.User) <= 0)
		{
			OfferResurrection();
		}
	}

	private void OfferResurrection()
	{
		if (TweenController != null)
		{
			TweenController.SendMessage("OnClick");
			NGUITools.SetActive(base.gameObject.transform.parent.gameObject, false);
		}
	}

	private void Update()
	{
	}
}
