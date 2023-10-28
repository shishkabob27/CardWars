using UnityEngine;

public class UIInputEnabler : MonoBehaviour
{
	public bool permanent;

	private bool inputenabled = true;

	private bool removeOnPress;

	private bool removeOnClick = true;

	public bool inputEnabled
	{
		get
		{
			return inputenabled;
		}
		set
		{
			inputenabled = value;
		}
	}

	public bool RemoveOnPress
	{
		get
		{
			return removeOnPress;
		}
		set
		{
			removeOnPress = value;
			if (removeOnPress)
			{
				removeOnClick = false;
			}
		}
	}

	public bool RemoveOnClick
	{
		get
		{
			return removeOnClick;
		}
		set
		{
			removeOnClick = value;
			if (removeOnClick)
			{
				removeOnPress = false;
			}
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed && removeOnPress)
		{
			removeOnPress = false;
			RemoveInputEnabler(base.gameObject);
		}
	}

	private void OnClick()
	{
		if (removeOnClick)
		{
			removeOnClick = false;
			RemoveInputEnabler(base.gameObject);
		}
	}

	private void RemoveInputEnabler(GameObject o)
	{
		if (!(o != null))
		{
			return;
		}
		Object[] components = o.GetComponents(typeof(UIInputEnabler));
		Object[] array = components;
		foreach (Object @object in array)
		{
			UIInputEnabler uIInputEnabler = @object as UIInputEnabler;
			bool flag = true;
			if (uIInputEnabler != null)
			{
				if (uIInputEnabler.permanent)
				{
					flag = false;
				}
				else
				{
					uIInputEnabler.inputEnabled = false;
					uIInputEnabler.enabled = false;
				}
			}
			if (@object != null && flag)
			{
				Object.DestroyImmediate(@object);
			}
		}
	}
}
