using UnityEngine;

public class CWQuestStaminaOK : MonoBehaviour
{
	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.Gems--;
		instance.Stamina = Mathf.Max(instance.Stamina_Max, instance.Stamina);
		Singleton<AnalyticsManager>.Instance.LogRechargePurchase();
	}
}
