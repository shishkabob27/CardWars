using UnityEngine;

public class CWTutorialNext : MonoBehaviour
{
	public GameObject target;

	public string functionName;

	public bool useOnPress;

	private bool done;

	private void OnClick()
	{
		if (base.enabled && !useOnPress && !done)
		{
			done = true;
			Send();
			Object.Destroy(this);
		}
	}

	private void OnPress(bool pressed)
	{
		if (base.enabled && useOnPress && pressed && !done)
		{
			done = true;
			Send();
			Object.Destroy(this);
		}
	}

	private void Send()
	{
		if (target != null && !string.IsNullOrEmpty(functionName))
		{
			UIButtonSound uIButtonSound = target.GetComponent(typeof(UIButtonSound)) as UIButtonSound;
			bool flag = false;
			if (uIButtonSound != null)
			{
				flag = uIButtonSound.enabled;
				uIButtonSound.enabled = false;
			}
			target.SendMessage(functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
			if (uIButtonSound != null)
			{
				uIButtonSound.enabled = flag;
			}
		}
	}
}
