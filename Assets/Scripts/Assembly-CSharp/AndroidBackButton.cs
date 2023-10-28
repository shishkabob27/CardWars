using UnityEngine;
using UnityEngine.EventSystems;

public class AndroidBackButton : MonoBehaviour, IAndroidBackActivator
{
	public bool TryActivate()
	{
		if (UICamera.useInputEnabler)
		{
			UIInputEnabler component = GetComponent<UIInputEnabler>();
			if (component == null || !component.inputEnabled)
			{
				return false;
			}
		}
		else if (GetComponent<Collider>() != null && !GetComponent<Collider>().enabled)
		{
			return false;
		}
		if (!ExecuteEvents.Execute(base.gameObject, new PointerEventData(EventSystem.current)
		{
			clickCount = 1
		}, ExecuteEvents.pointerClickHandler))
		{
			SendMessage("OnClick");
		}
		return true;
	}

	public void OnEnable()
	{
		AndroidUiStack.Instance.Add(this);
	}

	public void OnDisable()
	{
		AndroidUiStack.Instance.Remove(this);
	}
}
