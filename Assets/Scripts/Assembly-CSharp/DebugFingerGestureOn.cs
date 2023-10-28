using UnityEngine;

public class DebugFingerGestureOn : MonoBehaviour
{
	private DebugFlagsScript debugFlag;

	public GameObject[] targetTweens;

	public GameObject oldCards;

	public GameObject newCards;

	private bool fingureGestureOn;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
	}

	private void setTweenStatus(bool enable)
	{
		GameObject[] array = targetTweens;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			UIButtonTween[] components = gameObject.GetComponents<UIButtonTween>();
			UIButtonTween[] array2 = components;
			foreach (UIButtonTween uIButtonTween in array2)
			{
				if (uIButtonTween.tweenTarget == oldCards)
				{
					uIButtonTween.enabled = !enable;
				}
				if (uIButtonTween.tweenTarget == newCards)
				{
					uIButtonTween.enabled = enable;
				}
			}
		}
	}

	private void Update()
	{
		if (debugFlag.fingerGesture && !fingureGestureOn)
		{
			setTweenStatus(true);
			fingureGestureOn = true;
		}
		if (!debugFlag.fingerGesture && fingureGestureOn)
		{
			setTweenStatus(false);
			fingureGestureOn = false;
		}
	}
}
