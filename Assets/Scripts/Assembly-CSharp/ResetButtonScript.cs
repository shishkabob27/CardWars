using UnityEngine;

public class ResetButtonScript : MonoBehaviour
{
	private void OnClick()
	{
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("BattleScene");
	}
}
