using System.Collections;
using UnityEngine;

public class CWBattleRingTapDelegate : MonoBehaviour
{
	public GameObject buttonObj;

	public string functionName;

	public bool disableFlag = true;

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private IEnumerator WaitTweenToEnd()
	{
		TweenTransform tw = GetComponent<TweenTransform>();
		float waitTime = tw.duration + tw.delay;
		yield return new WaitForSeconds(waitTime);
		disableFlag = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !disableFlag)
		{
			buttonObj.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
			disableFlag = true;
		}
	}
}
