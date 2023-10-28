using System.Collections.Generic;
using UnityEngine;

public class RestoreStaminaScript : MonoBehaviour
{
	public UIButtonTween increase;

	public UIButtonTween store;

	public UIButtonTween full;

	private int Cost = 1;

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(instance != null))
		{
			return;
		}
		if (instance.Stamina >= instance.Stamina_Max)
		{
			full.enabled = true;
			full.Play(true);
			return;
		}
		GetCost();
		if (instance.Gems >= Cost)
		{
			instance.Gems -= Cost;
			instance.Stamina = Mathf.Max(instance.Stamina_Max, instance.Stamina);
			instance.Save();
			Singleton<AnalyticsManager>.Instance.LogRechargePurchase();
			increase.enabled = true;
			increase.Play(true);
		}
		else
		{
			store.enabled = true;
			store.Play(true);
		}
	}

	private void GetCost()
	{
		Dictionary<string, object>[] array = SQUtils.ReadJSONData("db_Cost.json");
		Dictionary<string, object>[] array2 = array;
		foreach (Dictionary<string, object> dictionary in array2)
		{
			string text = TFUtils.LoadString(dictionary, "CostID", string.Empty);
			if (text == "StaminaRecover")
			{
				Cost = TFUtils.LoadInt(dictionary, "Cost", 1);
			}
		}
	}
}
