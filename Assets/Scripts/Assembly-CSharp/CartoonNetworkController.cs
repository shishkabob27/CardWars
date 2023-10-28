using UnityEngine;

public class CartoonNetworkController : MonoBehaviour
{
	public GameObject ButtonLayout;

	public GameObject ButtonLayoutUnderage;

	private void Start()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		bool flag = !(instance != null) || instance.IsUnderage;
		ButtonLayoutUnderage.SetActive(flag);
		ButtonLayout.SetActive(!flag);
	}

	private void Update()
	{
	}
}
