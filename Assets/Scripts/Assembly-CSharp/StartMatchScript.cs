using System.Collections;
using UnityEngine;

public class StartMatchScript : MonoBehaviour
{
	private void OnClick()
	{
		StartCoroutine(GoToBattleScene());
	}

	private IEnumerator GoToBattleScene()
	{
		UICamera.useInputEnabler = true;
		yield return Resources.UnloadUnusedAssets();
		UICamera.useInputEnabler = false;
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("LoadingScreen");
	}
}
