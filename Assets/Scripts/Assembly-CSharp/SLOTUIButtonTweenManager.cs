using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

public class SLOTUIButtonTweenManager : MonoBehaviour
{
	public GameObject[] buttons;

	public GameObject[] buttonMessageHandlers;

	private List<UITweener> activeTweeners = new List<UITweener>();

	public void OnButtonClick(SLOTUIButton button)
	{
		if (!(button != null) || IsBusy())
		{
			return;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			if (!(buttons[i] == button.gameObject))
			{
				continue;
			}
			if (i >= buttonMessageHandlers.Length || !(buttonMessageHandlers[i] != null))
			{
				break;
			}
			buttonMessageHandlers[i].SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			UIButtonTween[] componentsInChildren = buttonMessageHandlers[i].GetComponentsInChildren<UIButtonTween>();
			UIButtonTween[] array = componentsInChildren;
			foreach (UIButtonTween uIButtonTween in array)
			{
				if (uIButtonTween != null)
				{
					UITweener[] tweenersFromButtonTween = GetTweenersFromButtonTween(uIButtonTween);
					if (tweenersFromButtonTween != null && tweenersFromButtonTween.Length > 0)
					{
						activeTweeners.AddRange(tweenersFromButtonTween);
					}
				}
			}
			break;
		}
	}

	private UITweener[] GetTweenersFromButtonTween(UIButtonTween bt)
	{
		GameObject gameObject = ((!(bt.tweenTarget == null)) ? bt.tweenTarget : bt.gameObject);
		if (!NGUITools.GetActive(gameObject) && bt.ifDisabledOnPlay != EnableCondition.EnableThenPlay)
		{
			return null;
		}
		return (!bt.includeChildren) ? gameObject.GetComponents<UITweener>() : gameObject.GetComponentsInChildren<UITweener>();
	}

	public bool IsBusy()
	{
		int num = activeTweeners.Count - 1;
		while (num >= 0)
		{
			UITweener uITweener = activeTweeners[num];
			if (uITweener == null || !uITweener.enabled)
			{
				activeTweeners.RemoveAt(num);
				num--;
				continue;
			}
			return true;
		}
		return false;
	}
}
