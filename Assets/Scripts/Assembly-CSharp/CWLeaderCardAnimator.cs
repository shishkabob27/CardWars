using System.Collections;
using UnityEngine;

public class CWLeaderCardAnimator : MonoBehaviour
{
	private void OnEnable()
	{
		GetWaitTimeFromPlayTween(0);
		StartCoroutine(WaitThenMoveAway());
	}

	private IEnumerator WaitThenMoveAway()
	{
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(WaitForKeyPress());
		float waitTime = GetWaitTimeFromPlayTween(1);
		yield return new WaitForSeconds(waitTime);
		MapControllerBase.GetInstance().resumeFlag = true;
		base.gameObject.SetActive(false);
	}

	private IEnumerator WaitForKeyPress()
	{
		bool keyPressed = false;
		while (!keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				yield return null;
				break;
			}
			yield return 0;
		}
	}

	private float GetWaitTimeFromPlayTween(int tweenGroup)
	{
		float result = 0f;
		TweenPosition[] components = GetComponents<TweenPosition>();
		TweenPosition[] array = components;
		foreach (TweenPosition tweenPosition in array)
		{
			if (tweenPosition.tweenGroup == tweenGroup)
			{
				tweenPosition.Play(true);
				tweenPosition.Reset();
				result = tweenPosition.duration;
			}
		}
		return result;
	}

	private void Update()
	{
	}
}
