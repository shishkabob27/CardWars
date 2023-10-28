using UnityEngine;

public class CWSendFunctionToAIManager : MonoBehaviour
{
	private void OnClick()
	{
		AIManager.Instance.MakeDecision();
	}

	private void Update()
	{
	}
}
