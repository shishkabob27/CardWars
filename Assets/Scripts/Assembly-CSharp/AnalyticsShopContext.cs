using UnityEngine;

public class AnalyticsShopContext : MonoBehaviour
{
	public enum Operation
	{
		OnClick,
		OnEnable
	}

	public string Context = string.Empty;

	public Operation Activation;

	public static string ContextLast { get; private set; }

	private void OnEnable()
	{
		if (Activation == Operation.OnEnable && !string.IsNullOrEmpty(Context))
		{
			ContextLast = Context;
		}
	}

	private void OnClick()
	{
		if (Activation == Operation.OnClick && !string.IsNullOrEmpty(Context))
		{
			ContextLast = Context;
		}
	}
}
