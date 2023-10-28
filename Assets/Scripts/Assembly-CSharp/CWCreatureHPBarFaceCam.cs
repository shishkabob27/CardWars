using UnityEngine;

public class CWCreatureHPBarFaceCam : MonoBehaviour
{
	private Camera gameCamera;

	public int player;

	public int lane;

	public UISprite hpBarSprite;

	public Transform hpBar;

	public GameObject target;

	private CreatureScript creatureScript;

	private Bounds barBounds;

	private PanelManagerBattle panelMgrBattle;

	private CWBattleSequenceController battleSqCtrlr;

	private CWFloopActionManager floopMgr;

	public PlayerType playerType(int player)
	{
		return (player != 0) ? PlayerType.Opponent : PlayerType.User;
	}

	private void Start()
	{
		barBounds = SLOTGame.GetUIBoundsRecursive(base.transform);
		battleSqCtrlr = CWBattleSequenceController.GetInstance();
		panelMgrBattle = PanelManagerBattle.GetInstance();
		floopMgr = CWFloopActionManager.GetInstance();
		gameCamera = panelMgrBattle.newCamera.GetComponent<Camera>();
		if (panelMgrBattle.hpBarOnTheGround)
		{
			base.transform.localPosition = ((player != 0) ? new Vector3(50f, 0f, 0f) : new Vector3(-50f, 0f, 0f));
			Vector3 euler = ((player != 0) ? new Vector3(-90f, 0f, 0f) : new Vector3(-90f, 180f, 0f));
			base.transform.localRotation = Quaternion.Euler(euler);
		}
	}

	private Color GetFactionColor(Faction faction)
	{
		return FactionManager.Instance.GetFactionData(faction).FactionColor;
	}

	private void Update()
	{
		if (!panelMgrBattle.hpBarOnTheGround)
		{
			if (target != null)
			{
				Bounds meshRendererBoundsRecursive = SLOTGame.GetMeshRendererBoundsRecursive(target.transform);
				Vector3 center = meshRendererBoundsRecursive.center;
				center.y += meshRendererBoundsRecursive.extents.y + barBounds.extents.y;
				base.transform.position = center;
			}
			if (floopMgr.usingSpellCamera && floopMgr.SpellCamera == null)
			{
				gameCamera = panelMgrBattle.newCamera.GetComponent<Camera>();
			}
			if (gameCamera != null)
			{
				base.transform.LookAt(base.transform.position + gameCamera.transform.rotation * Vector3.back, gameCamera.transform.rotation * Vector3.up);
			}
		}
		if (battleSqCtrlr.camAlignFlag)
		{
			gameCamera = ((player != 0) ? battleSqCtrlr.battleCamP2.GetComponent<Camera>() : battleSqCtrlr.battleCamP1.GetComponent<Camera>());
		}
		else if (floopMgr.usingSpellCamera && floopMgr.SpellCamera != null)
		{
			gameCamera = floopMgr.SpellCamera;
		}
		else
		{
			gameCamera = panelMgrBattle.newCamera.GetComponent<Camera>();
		}
		if (GameState.Instance.IsMarkedForDeath(player, lane))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Bounds meshRendererBoundsRecursive = SLOTGame.GetMeshRendererBoundsRecursive(target.transform);
		Gizmos.DrawWireCube(meshRendererBoundsRecursive.center, meshRendererBoundsRecursive.size);
	}
}
