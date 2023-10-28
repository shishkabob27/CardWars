using UnityEngine;

public class CWDisplayGemCount : MonoBehaviour
{
	public UILabel gemCount;

	private void OnEnable()
	{
		gemCount = GetComponent<UILabel>();
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		gemCount.text = instance.Gems.ToString();
	}

	private void Update()
	{
	}
}
