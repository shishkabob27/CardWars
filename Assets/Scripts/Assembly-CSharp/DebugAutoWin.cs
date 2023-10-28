using UnityEngine;

public class DebugAutoWin : MonoBehaviour
{
	private DebugFlagsScript debugFlag;

	private BattlePhaseManager phaseMgr;

	private bool autoWinFlag;

	private bool autoLoseFlag;

	private bool quickWinFlag;

	private bool quickLooseFlag;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
		if (debugFlag.QuickWin)
		{
			GameState.Instance.SetHealth(1000, 1);
		}
	}

	private void Update()
	{
		if (debugFlag.autoWin && !autoWinFlag)
		{
			GameState.Instance.SetHealth(PlayerType.Opponent, 0);
			phaseMgr.Phase = BattlePhase.Result_P2Defeated;
			autoWinFlag = true;
		}
		if (debugFlag.autoLose && !autoLoseFlag)
		{
			GameState.Instance.SetHealth(PlayerType.User, 0);
			phaseMgr.Phase = BattlePhase.Result_P1Defeated;
			autoLoseFlag = true;
		}
		if (!debugFlag.autoLose && autoLoseFlag)
		{
			autoLoseFlag = false;
		}
		if (debugFlag.QuickWin && !quickWinFlag)
		{
			GameState.Instance.SetHealth(PlayerType.Opponent, 1);
			quickWinFlag = true;
		}
		if (!debugFlag.QuickWin && quickWinFlag)
		{
			quickWinFlag = false;
		}
		if (debugFlag.quickLoose && !quickLooseFlag)
		{
			GameState.Instance.SetHealth(PlayerType.User, 1);
			quickLooseFlag = true;
		}
		if (!debugFlag.quickLoose && quickLooseFlag)
		{
			quickLooseFlag = false;
		}
		if (debugFlag.recoverPlayerHP)
		{
			GameState.Instance.SetHealth(PlayerType.User, GameState.Instance.GetMaxHealth(PlayerType.User));
			debugFlag.recoverPlayerHP = false;
		}
		if (debugFlag.recoverOpponentHP)
		{
			GameState.Instance.SetHealth(PlayerType.Opponent, GameState.Instance.GetMaxHealth(PlayerType.Opponent));
			debugFlag.recoverOpponentHP = false;
		}
	}
}
