using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Component")]
public class UICheckboxControlledComponent : MonoBehaviour
{
	public MonoBehaviour target;

	public bool inverse;

	private bool mUsingDelegates;

	private void Start()
	{
		UICheckbox component = GetComponent<UICheckbox>();
		if (component != null)
		{
			mUsingDelegates = true;
			component.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(component.onStateChange, new UICheckbox.OnStateChange(OnActivateDelegate));
		}
	}

	private void OnActivateDelegate(bool isActive)
	{
		if (base.enabled && target != null)
		{
			target.enabled = ((!inverse) ? isActive : (!isActive));
		}
	}

	private void OnActivate(bool isActive)
	{
		if (!mUsingDelegates)
		{
			OnActivateDelegate(isActive);
		}
	}
}
