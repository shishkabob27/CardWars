using UnityEngine;

public class CWBranchSummonCam : MonoBehaviour
{
	private BattlePhaseManager phaseMgr;

	private CWiTweenVantageCam vantageCam;

	public int player;

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
		vantageCam = CWiTweenVantageCam.GetInstance();
	}

	private void OnClick()
	{
		if (phaseMgr.prevPhase == BattlePhase.P1SetupActionRareCard || phaseMgr.prevPhase == BattlePhase.P2SetupActionRareCard)
		{
			vantageCam.CreatureSpawnCamera(player);
			SetTweenState(true);
		}
		else if (!phaseMgr.alreadySummonedCard.Contains(phaseMgr.currentCardID))
		{
			vantageCam.CreatureSpawnCamera(player);
			SetTweenState(true);
			if (phaseMgr.currentCardID != string.Empty)
			{
				phaseMgr.alreadySummonedCard.Add(phaseMgr.currentCardID);
			}
		}
		else
		{
			SetTweenState(false);
		}
	}

	private void SetTweenState(bool enable)
	{
		UIButtonTween[] components = GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label != "Persistent")
			{
				uIButtonTween.enabled = enable;
			}
		}
	}

	private void Update()
	{
	}
}
