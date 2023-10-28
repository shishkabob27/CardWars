using System.Collections;
using UnityEngine;

public class CWOutOfCardsApplyBleed : MonoBehaviour
{
	private GameState GameInstance;

	private void OnClick()
	{
		StartCoroutine(ApplyBleed());
	}

	private IEnumerator ApplyBleed()
	{
		yield return new WaitForSeconds(0.6f);
		GameInstance = GameState.Instance;
		GameInstance.DoResultBleed(PlayerType.User);
	}

	private void Update()
	{
	}
}
