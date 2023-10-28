using System.Collections;
using UnityEngine;

public class CWReturnToMainMenu : MonoBehaviour
{
	private void OnClick()
	{
		if (GameState.Instance.BattleResolver != null)
		{
			GameState.Instance.BattleResolver.SetResult(null);
		}
		if (GameState.Instance.BattleResolver == null || !GameState.Instance.BattleResolver.SkipRegularLogic())
		{
			Singleton<AnalyticsManager>.Instance.LogQuestQuit();
		}
		GlobalFlags.Instance.ReturnToMainMenu = true;
		PlayerInfoScript.GetInstance().Tutorial = false;
		StartCoroutine(LoadScene(0.5f));
	}

	private IEnumerator LoadScene(float delay)
	{
		UICamera.useInputEnabler = true;
		YieldInstruction waitYield = new WaitForSeconds(delay);
		AsyncOperation resUnloadAsync = Resources.UnloadUnusedAssets();
		yield return waitYield;
		float savedTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		while (!resUnloadAsync.isDone)
		{
			yield return null;
		}
		Time.timeScale = savedTimeScale;
		UICamera.useInputEnabler = false;
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("AdventureTime");
	}

	private void Update()
	{
	}
}
