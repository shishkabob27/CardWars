using UnityEngine;

public class CWFastForwardButtonScript : MonoBehaviour
{
	private BattlePhase currentPhase;

	private BattlePhaseManager phaseMgr;

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	private void OnClick()
	{
		Time.timeScale = 5f;
	}

	private void Update()
	{
		if (phaseMgr.currentPhase != currentPhase && Time.timeScale > 0f)
		{
			currentPhase = phaseMgr.currentPhase;
			Time.timeScale = 1f;
		}
	}
}
