using System.Collections;
using UnityEngine;

public class CWTapDelegate : MonoBehaviour
{
	public GameObject buttonObj;

	public string functionName;

	public bool ketchup;

	public bool disableFlag = true;

	private void OnEnable()
	{
		if (!ketchup)
		{
			if (GetComponent<Collider>() != null)
			{
				GetComponent<Collider>().enabled = false;
			}
			StartCoroutine(WaitTweenToEnd());
		}
	}

	private IEnumerator WaitTweenToEnd()
	{
		TweenTransform tw = GetComponent<TweenTransform>();
		float waitTime = tw.duration + tw.delay;
		yield return new WaitForSeconds(waitTime);
		disableFlag = false;
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = true;
		}
	}

	private void OnClick()
	{
		if (!disableFlag && Time.timeScale != 0f)
		{
			UIInputEnabler uIInputEnabler = base.gameObject.GetComponent(typeof(UIInputEnabler)) as UIInputEnabler;
			if (!UICamera.useInputEnabler || (uIInputEnabler != null && uIInputEnabler.inputEnabled))
			{
				buttonObj.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
				disableFlag = true;
			}
		}
	}
}
