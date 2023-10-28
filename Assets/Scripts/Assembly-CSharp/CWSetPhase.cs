using UnityEngine;

public class CWSetPhase : MonoBehaviour
{
	public BattlePhase setPhase;

	public float delay;

	private BattlePhaseManager phaseMgr;

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	private void OnClick()
	{
		phaseMgr.SetPhase(delay, setPhase);
	}

	private void Update()
	{
	}
}
