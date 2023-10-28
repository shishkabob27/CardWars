using UnityEngine;

public class CWDiscardPileSet : MonoBehaviour
{
	private static CWDiscardPileSet[] g_instances = new CWDiscardPileSet[2];

	public CWDiscardPileFillTable fillTable;

	public int type;

	private PlayerType playerType;

	private BattlePhaseManager phaseMgr;

	private BoxCollider col;

	private bool colActive;

	private void Awake()
	{
		playerType = ((type != 0) ? PlayerType.Opponent : PlayerType.User);
		g_instances[(int)playerType] = this;
	}

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
		col = GetComponent<BoxCollider>();
	}

	public static CWDiscardPileSet GetInstance(PlayerType type)
	{
		return g_instances[(int)type];
	}

	public void SetFilterScript(CardScript script)
	{
		fillTable.type = playerType;
		fillTable.SetFilterScript(script);
	}

	private void OnClick()
	{
		fillTable.type = playerType;
	}

	private void Update()
	{
		if (phaseMgr.Phase == BattlePhase.P1Setup && !colActive)
		{
			col.enabled = true;
			colActive = true;
		}
		if (phaseMgr.Phase != BattlePhase.P1Setup && colActive)
		{
			col.enabled = false;
			colActive = false;
		}
	}
}
