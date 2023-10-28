using UnityEngine;

public class CWP1SetupBranch : MonoBehaviour
{
	public GameObject tweenToWarning;

	public GameObject tweenToBattle;

	public GameObject tweenToP2Setup;

	private GameState GameInstance;

	private BattlePhaseManager phaseMgr;

	private VoiceoverScript Voiceover;

	private Collider col;

	private void Start()
	{
		GameInstance = GameState.Instance;
		phaseMgr = BattlePhaseManager.GetInstance();
		Voiceover = VoiceoverScript.GetInstance();
		col = GetComponent<Collider>();
	}

	public void Execute()
	{
		if (GameInstance.GetMagicPoints(PlayerType.User) == 0 || !GameInstance.HasLegalMove(PlayerType.User))
		{
			Advance();
		}
		else
		{
			tweenToWarning.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Advance()
	{
		if (GameDataScript.GetInstance().Turn <= 1)
		{
			BattleManagerScript.GetInstance().P1BattleFinished();
		}
		else
		{
			phaseMgr.Phase = BattlePhase.P1BattleBanner;
		}
		Voiceover.P1BattlePhase();
	}

	private void Update()
	{
		col.enabled = phaseMgr.Phase == BattlePhase.P1Setup;
	}
}
