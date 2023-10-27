using UnityEngine;

public class AnalyticsShopContext : MonoBehaviour
{
	public enum Operation
	{
		OnClick = 0,
		OnEnable = 1,
	}

	public string Context;
	public Operation Activation;
}
