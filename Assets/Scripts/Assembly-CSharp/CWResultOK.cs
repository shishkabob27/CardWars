using System.Collections;
using UnityEngine;

public class CWResultOK : MonoBehaviour
{
	public bool GoToBuildDeck;

	private void Start()
	{
	}

	private void OnClick()
	{
		if (GoToBuildDeck)
		{
			GlobalFlags.Instance.ReturnToBuildDeck = true;
		}
		else
		{
			GlobalFlags.Instance.ReturnToMainMenu = true;
		}
		PlayerInfoScript.GetInstance().Save();
		StartCoroutine(GoToMainMenu());
	}

	private void Update()
	{
	}

	private IEnumerator GoToMainMenu()
	{
		UICamera.useInputEnabler = true;
		float savedTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return Resources.UnloadUnusedAssets();
		Time.timeScale = savedTimeScale;
		UICamera.useInputEnabler = false;
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("AdventureTime");
	}
}
