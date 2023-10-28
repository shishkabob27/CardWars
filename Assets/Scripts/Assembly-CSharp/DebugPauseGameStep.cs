using System.Collections;
using UnityEngine;

public class DebugPauseGameStep : MonoBehaviour
{
	public UISprite sprite;

	private int clickCount;

	private void OnClick()
	{
		StartCoroutine(StepForward());
	}

	private IEnumerator StepForward()
	{
		Time.timeScale = 0.1f;
		yield return new WaitForSeconds(Time.deltaTime);
		Time.timeScale = 0f;
	}

	private void Update()
	{
	}
}
