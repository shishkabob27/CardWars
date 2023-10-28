using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HowOldAreYou : MonoBehaviour
{
	private const int mMinAge = 1;

	private const int mMaxAge = 122;

	public UIButtonTween ShowTween;

	public UIButtonTween HideTween;

	public GameObject OKButton;

	private UIInput mAgeInput;

	private BoxCollider mOKButtonCollider;

	[method: MethodImpl(32)]
	public static event Action<int> AgeGateDone;

	private void Start()
	{
		mAgeInput = base.gameObject.GetComponentInChildren<UIInput>();
		if (null != OKButton)
		{
			mOKButtonCollider = OKButton.GetComponent<BoxCollider>();
		}
	}

	private void Update()
	{
		if (null != mOKButtonCollider && null != mAgeInput)
		{
			mOKButtonCollider.enabled = false;
			int result;
			if (int.TryParse(mAgeInput.text, out result))
			{
				mOKButtonCollider.enabled = 1 <= result && result <= 122;
			}
		}
	}

	public void OnClick()
	{
	}

	public void OnCancel()
	{
		if (HowOldAreYou.AgeGateDone != null)
		{
			HowOldAreYou.AgeGateDone(-1);
		}
	}

	public void OnSubmit()
	{
		UIInput componentInChildren = GetComponentInChildren<UIInput>();
		if (null != componentInChildren)
		{
			OnSubmit(componentInChildren.text);
		}
	}

	public void OnSubmit(string inputString)
	{
		int obj = -1;
		int result;
		if (!string.IsNullOrEmpty(inputString) && int.TryParse(inputString, out result))
		{
			obj = ((result >= 0) ? result : (-result));
		}
		if (HowOldAreYou.AgeGateDone != null)
		{
			HowOldAreYou.AgeGateDone(obj);
		}
		if ((bool)HideTween)
		{
			HideTween.Play(true);
		}
	}
}
