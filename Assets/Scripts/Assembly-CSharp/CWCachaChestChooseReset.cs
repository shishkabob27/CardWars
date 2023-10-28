using UnityEngine;

public class CWCachaChestChooseReset : MonoBehaviour
{
	private void OnClick()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		if ((bool)instance)
		{
			instance.canChooseChest = true;
		}
	}
}
