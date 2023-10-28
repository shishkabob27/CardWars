using UnityEngine;

public class RestorePurchasesButtonScript : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	private void OnClick()
	{
		Singleton<PurchaseManager>.Instance.RestorePurchases();
	}
}
