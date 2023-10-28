using UnityEngine;

public class DebugDisplayQuestCondition : MonoBehaviour
{
	public UILabel qcLabel;

	public UILabel qcNumLabel;

	public GameObject qcObj;

	public GameObject qcParent;

	private DebugFlagsScript debugFlag;

	private bool setFlag;

	public GameObject obj;

	private QuestConditionManager qcMgr;

	private string desc;

	private int numStars;

	private bool initialized;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
		if (qcParent == null)
		{
			qcParent = debugFlag.GetParentObj();
		}
	}

	private void Refresh()
	{
		qcMgr = QuestConditionManager.Instance;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData questData;
		if (GameState.Instance.BattleResolver != null)
		{
			numStars = GameState.Instance.BattleResolver.questStars;
			questData = GameState.Instance.BattleResolver.questData;
		}
		else
		{
			questData = instance.GetCurrentQuest();
			numStars = instance.GetQuestProgress(questData);
		}
		string empty = string.Empty;
		desc = string.Empty;
		if (numStars > 0)
		{
			empty = questData.Condition[numStars - 1];
			desc = qcMgr.ConditionDescription(empty);
		}
	}

	private void Update()
	{
		SessionManager instance = SessionManager.GetInstance();
		if (!initialized && instance.IsReady())
		{
			Refresh();
			initialized = true;
		}
		if (debugFlag.battleDisplay.DisplayQuestCondition && !setFlag)
		{
			if (obj == null)
			{
				if (qcParent != null)
				{
					obj = debugFlag.SpawnFPSObject(qcObj, qcParent);
				}
				obj.SetActive(true);
				obj.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				obj.SetActive(true);
				Refresh();
			}
			Refresh();
			if (qcLabel == null)
			{
				qcLabel = FindLabel(obj, "QuestCondition");
			}
			if (qcNumLabel == null)
			{
				qcNumLabel = FindLabel(obj, "QuestConditionNum");
			}
			if (qcLabel != null)
			{
				qcLabel.text = desc;
			}
			if (qcNumLabel != null)
			{
				qcNumLabel.text = numStars.ToString();
			}
			setFlag = true;
		}
		if (!debugFlag.battleDisplay.DisplayQuestCondition && setFlag)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			setFlag = false;
		}
	}

	private UILabel FindLabel(GameObject obj, string lbName)
	{
		UILabel result = null;
		UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == lbName)
			{
				result = uILabel;
				break;
			}
		}
		return result;
	}
}
