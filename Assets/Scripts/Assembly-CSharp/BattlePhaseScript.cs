using UnityEngine;

public class BattlePhaseScript : MonoBehaviour
{
	public BattlePhase Phase;

	private void OnClick()
	{
		BattlePhaseManager instance = BattlePhaseManager.GetInstance();
		instance.Phase = Phase;
	}
}
