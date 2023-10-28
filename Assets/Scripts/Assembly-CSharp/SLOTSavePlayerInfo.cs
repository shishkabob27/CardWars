using UnityEngine;

public class SLOTSavePlayerInfo : MonoBehaviour
{
	private void OnClick()
	{
		PlayerInfoScript.GetInstance().Save();
	}
}
