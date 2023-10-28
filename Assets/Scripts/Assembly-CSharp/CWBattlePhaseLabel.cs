using System.Collections.Generic;
using UnityEngine;

public class CWBattlePhaseLabel : MonoBehaviour
{
	public string instructionID;

	private bool initialized;

	private void Refresh()
	{
		Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_InstructionLabel.json");
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			if ((string)dictionary["InstructionID"] == instructionID)
			{
				UILabel component = GetComponent<UILabel>();
				if (component != null)
				{
					component.text = KFFLocalization.Get((string)dictionary["Text"]);
				}
				break;
			}
		}
	}

	private void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (instance.IsReady())
			{
				Refresh();
				initialized = true;
			}
		}
	}
}
