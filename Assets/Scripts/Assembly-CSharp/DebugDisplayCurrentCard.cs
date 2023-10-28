using UnityEngine;

public class DebugDisplayCurrentCard : MonoBehaviour
{
	public GameObject currentCardObj;

	public UILabel currentCardLabel;

	public UILabel currentDecisionLabel;

	public GameObject currentCardParent;

	private DebugFlagsScript debugFlag;

	private bool setFlag;

	private GameObject obj;

	private BattlePhaseManager phaseMgr;

	private CWPlayerHandsController handCtrlr;

	private AIManager aiMgr;

	private CardItem prevCard;

	private CardItem currentCard;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (currentCardParent == null)
		{
			currentCardParent = debugFlag.GetParentObj();
		}
		phaseMgr = BattlePhaseManager.GetInstance();
		handCtrlr = CWPlayerHandsController.GetInstance();
		aiMgr = AIManager.Instance;
	}

	private void Update()
	{
		if (debugFlag.battleDisplay.currentCardDisplay && !setFlag)
		{
			if (obj == null)
			{
				if (currentCardParent != null)
				{
					obj = debugFlag.SpawnFPSObject(currentCardObj, currentCardParent);
				}
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
				UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>();
				UILabel[] array = componentsInChildren;
				foreach (UILabel uILabel in array)
				{
					if (uILabel.name == "Decision")
					{
						currentDecisionLabel = uILabel;
					}
					else
					{
						currentCardLabel = uILabel;
					}
				}
			}
			else
			{
				obj.SetActive(true);
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.currentCardDisplay && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
		if (currentCardLabel != null && currentDecisionLabel != null)
		{
			UpdateLabels();
		}
	}

	private void UpdateLabels()
	{
		if (phaseMgr.Phase == BattlePhase.P1Setup)
		{
			currentCard = handCtrlr.card;
			if (currentCard != null)
			{
				currentDecisionLabel.text = currentCard.Form.Type.ToString();
			}
		}
		else if (phaseMgr.Phase == BattlePhase.P2Setup)
		{
			string empty = string.Empty;
			if (aiMgr.Decision != null)
			{
				empty = (aiMgr.Decision.IsFloop ? "Floop" : ((!aiMgr.Decision.IsLeaderAbility) ? aiMgr.Decision.CardChoice.Form.Type.ToString() : "Leader"));
				currentDecisionLabel.text = empty;
			}
		}
		if (currentCard == null)
		{
			currentCardLabel.text = string.Empty;
		}
		else if (currentCard != prevCard)
		{
			currentCardLabel.text = currentCard.Form.Name;
			prevCard = currentCard;
		}
	}
}
