using UnityEngine;

public class CWCommandCardSet : MonoBehaviour
{
	public CWCommandCardFill cardFillScript;

	public int playerType;

	public int lane;

	public bool creatureFlag;

	public GameObject creatureObj;

	private GameState GameInstance;

	public bool debugMode;

	private GameObject commandCard;

	public GameObject floopButton;

	private CWFloopActionManager floopActionMgr;

	private PanelManagerBattle panelMgrBattle;

	private void Start()
	{
		GameInstance = GameState.Instance;
		floopActionMgr = CWFloopActionManager.GetInstance();
		panelMgrBattle = PanelManagerBattle.GetInstance();
	}

	private void OnEnable()
	{
	}

	private void OnClick()
	{
		if (creatureFlag && GameInstance.LaneHasCreature(playerType, lane - 1))
		{
			CreatureScript creature = GameInstance.GetCreature(playerType, lane - 1);
			SetCardValue(creature.Data, creature.ATK - creature.Data.ATK, creature.DEF - creature.Data.DEF);
			floopActionMgr.player = playerType;
			floopActionMgr.lane = lane - 1;
			floopActionMgr.card = creature.Data;
			floopActionMgr.anim = creatureObj.GetComponent<Animation>();
			if (BattlePhaseManager.GetInstance().Phase != BattlePhase.P1Setup)
			{
				return;
			}
			bool flag = GameInstance.CanFloopCard(playerType, creature);
			if (playerType == (int)PlayerType.User && flag)
			{
				SetTweenAndSoundOnCreature(false);
				floopButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				SetTweenAndSoundOnCreature(true);
			}
		}
		else if (!creatureFlag && GameInstance.LaneHasBuilding(playerType, lane - 1))
		{
			BuildingScript building = GameInstance.GetBuilding(playerType, lane - 1);
			SetCardValue(building.Data, 0, 0);
		}
		panelMgrBattle.floopPanelCameraLookAtTarget.transform.position = base.transform.position;
		panelMgrBattle.floopPanelCameraTarget.transform.position = new Vector3(base.transform.position.x - floopActionMgr.detailCameraOffsetX, base.transform.position.y + floopActionMgr.detailCameraOffsetY, base.transform.position.z);
		if (NGUITools.GetActive(cardFillScript.gameObject))
		{
			cardFillScript.Refresh();
		}
	}

	private void SetTweenAndSoundOnCreature(bool enable)
	{
		UIButtonTween[] components = GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			uIButtonTween.enabled = enable;
		}
		UIButtonSound component = GetComponent<UIButtonSound>();
		if (component != null)
		{
			component.enabled = enable;
		}
	}

	private void SetCardValue(CardItem card, int bonusATK, int bonusDEF)
	{
		cardFillScript.bonusATK = bonusATK;
		cardFillScript.bonusDEF = bonusDEF;
		SetCardValue(card);
	}

	private void SetCardValue(CardItem card)
	{
		cardFillScript.card = card;
		cardFillScript.playerType = playerType;
		cardFillScript.lane = lane;
		cardFillScript.creatureFlag = creatureFlag;
		cardFillScript.canFloop = IsFloopable();
		cardFillScript.canAct = IsActionable();
		cardFillScript.creatureObj = creatureObj;
		SetButtonActiveState(card);
	}

	private void SetButtonActiveState(CardItem card)
	{
		UIButtonActivate[] components = GetComponents<UIButtonActivate>();
		UIButtonActivate[] array = components;
		int num = 0;
		if (num < array.Length)
		{
			UIButtonActivate uIButtonActivate = array[num];
			if (playerType == 1)
			{
				uIButtonActivate.state = false;
			}
			else
			{
				uIButtonActivate.state = true;
			}
		}
	}

	public bool IsFloopable()
	{
		bool result = false;
		PlayerType player = ((playerType != 0) ? PlayerType.Opponent : PlayerType.User);
		if (GameInstance.IsFloopingEnabled(player) && creatureFlag && GameInstance.LaneHasCreature(playerType, lane - 1))
		{
			CreatureScript creature = GameInstance.GetCreature(playerType, lane - 1);
			result = GameInstance.CanFloopCard(player, creature);
		}
		return result;
	}

	public bool IsActionable()
	{
		return false;
	}

	private void Update()
	{
	}
}
