using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("NGUI/Interaction/Button Keys")]
public class UIButtonKeys : MonoBehaviour
{
	public bool startsSelected;

	public UIButtonKeys selectOnClick;

	public UIButtonKeys selectOnUp;

	public UIButtonKeys selectOnDown;

	public UIButtonKeys selectOnLeft;

	public UIButtonKeys selectOnRight;

	private void OnEnable()
	{
		if (startsSelected && UICamera.selectedObject == null)
		{
			if (!NGUITools.GetActive(UICamera.selectedObject))
			{
				UICamera.selectedObject = base.gameObject;
			}
			else
			{
				UICamera.Notify(base.gameObject, "OnHover", true);
			}
		}
	}

	private void OnKey(KeyCode key)
	{
		if (!base.enabled || !NGUITools.GetActive(base.gameObject))
		{
			return;
		}
		switch (key)
		{
		case KeyCode.LeftArrow:
			if (selectOnLeft != null)
			{
				UICamera.selectedObject = selectOnLeft.gameObject;
			}
			break;
		case KeyCode.RightArrow:
			if (selectOnRight != null)
			{
				UICamera.selectedObject = selectOnRight.gameObject;
			}
			break;
		case KeyCode.UpArrow:
			if (selectOnUp != null)
			{
				UICamera.selectedObject = selectOnUp.gameObject;
			}
			break;
		case KeyCode.DownArrow:
			if (selectOnDown != null)
			{
				UICamera.selectedObject = selectOnDown.gameObject;
			}
			break;
		case KeyCode.Tab:
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				if (selectOnLeft != null)
				{
					UICamera.selectedObject = selectOnLeft.gameObject;
				}
				else if (selectOnUp != null)
				{
					UICamera.selectedObject = selectOnUp.gameObject;
				}
				else if (selectOnDown != null)
				{
					UICamera.selectedObject = selectOnDown.gameObject;
				}
				else if (selectOnRight != null)
				{
					UICamera.selectedObject = selectOnRight.gameObject;
				}
			}
			else if (selectOnRight != null)
			{
				UICamera.selectedObject = selectOnRight.gameObject;
			}
			else if (selectOnDown != null)
			{
				UICamera.selectedObject = selectOnDown.gameObject;
			}
			else if (selectOnUp != null)
			{
				UICamera.selectedObject = selectOnUp.gameObject;
			}
			else if (selectOnLeft != null)
			{
				UICamera.selectedObject = selectOnLeft.gameObject;
			}
			break;
		}
	}

	private void OnClick()
	{
		if (base.enabled && selectOnClick != null)
		{
			UICamera.selectedObject = selectOnClick.gameObject;
		}
	}
}
