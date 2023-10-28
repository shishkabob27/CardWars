using UnityEngine;

public class CWCreatureStatsFloorDisplay : MonoBehaviour
{
	public int playerType;

	public int lane;

	public UILabel atkLabel;

	public GameObject floopButton;

	public BoxCollider creatureInfoCol;

	private CreatureScript script;

	public bool displayFlag;

	private float oribinalSize;

	private GameState GameInstance;

	private BattlePhaseManager phaseMgr;

	private CWFloopActionManager floopMgr;

	private GameObject FloopButtonIdleFx;

	private bool alwaysUpdateFlag = true;

	private void Start()
	{
		GameInstance = GameState.Instance;
		floopMgr = CWFloopActionManager.GetInstance();
		if (phaseMgr == null)
		{
			phaseMgr = BattlePhaseManager.GetInstance();
		}
	}

	private void OnEnable()
	{
		if (floopButton != null)
		{
			oribinalSize = floopButton.transform.localScale.z;
		}
		if (GameInstance == null)
		{
			GameInstance = GameState.Instance;
		}
		UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "ATKLabel")
			{
				atkLabel = uILabel;
			}
		}
		phaseMgr = BattlePhaseManager.GetInstance();
		displayFlag = true;
		script = GameInstance.GetCreature(playerType, lane - 1);
	}

	private void Update()
	{
		if (atkLabel != null)
		{
			atkLabel.text = script.ATK.ToString();
		}
		script = GameInstance.GetCreature(playerType, lane - 1);
		if (script != null)
		{
			displayFlag = true;
		}
		if (displayFlag)
		{
			script = GameInstance.GetCreature(playerType, lane - 1);
			if (script == null)
			{
				displayFlag = false;
				return;
			}
			bool enable = GameInstance.CanFloopCard(playerType, script);
			if (script.IsSummoning)
			{
				UpdateFloopButtonState(false);
			}
			int num = -1;
			if (phaseMgr.Phase == BattlePhase.P1Setup)
			{
				num = 0;
			}
			else if (phaseMgr.Phase == BattlePhase.P2Setup)
			{
				num = 1;
			}
			if (num != playerType)
			{
				alwaysUpdateFlag = false;
			}
			else
			{
				alwaysUpdateFlag = true;
			}
			if (alwaysUpdateFlag)
			{
				UpdateFloopButtonState(enable);
			}
			else
			{
				UpdateFloopButtonState(false);
			}
			if (script.Health == 0)
			{
				displayFlag = false;
				NGUITools.SetActive(base.gameObject, false);
			}
		}
		else
		{
			floopButton.SetActive(false);
		}
	}

	private void SetVisibilityOnWidgets(bool enable)
	{
		UISprite[] componentsInChildren = GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			uISprite.enabled = enable;
		}
		UILabel[] componentsInChildren2 = GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			uILabel.enabled = enable;
		}
	}

	private void PushFloop()
	{
		alwaysUpdateFlag = false;
		UpdateFloopButtonState(false);
	}

	private void UnPushFloop()
	{
		alwaysUpdateFlag = true;
	}

	private void SetVisualStateOnFloopButton(bool enable)
	{
		if (!(floopButton == null))
		{
			Renderer[] componentsInChildren = floopButton.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.material.color = ((!enable) ? Color.grey : Color.white);
			}
			float z = ((!enable) ? 0.3f : oribinalSize);
			floopButton.transform.localScale = new Vector3(floopButton.transform.localScale.x, floopButton.transform.localScale.y, z);
		}
	}

	private void UpdateFloopButtonState(bool enable)
	{
		if (!(floopButton == null))
		{
			if (FloopButtonIdleFx == null)
			{
				FloopButtonIdleFx = floopMgr.GetSpawnedFX(floopMgr.floopButtonFXIdle.name, floopButton, true);
			}
			FloopButtonIdleFx.SetActive(enable);
			SetVisualStateOnFloopButton(enable);
			if (phaseMgr == null)
			{
				phaseMgr = BattlePhaseManager.GetInstance();
			}
			if (floopMgr == null)
			{
				floopMgr = CWFloopActionManager.GetInstance();
			}
			if (floopButton.GetComponent<Collider>() != null)
			{
				floopButton.GetComponent<Collider>().enabled = enable;
			}
			if (!(creatureInfoCol != null))
			{
			}
		}
	}
}
