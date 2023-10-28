using UnityEngine;

public class ReloadButton : MonoBehaviour
{
	private void OnClick()
	{
		PlayerInfoScript.GetInstance().ReloadGame();
	}
}
