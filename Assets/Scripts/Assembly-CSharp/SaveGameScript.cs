using UnityEngine;

public class SaveGameScript : MonoBehaviour
{
	private void OnClick()
	{
		PlayerInfoScript.GetInstance().Save();
	}
}
