using UnityEngine;

[AddComponentMenu("Game/UI/Button Key Binding")]
public class UIButtonKeyBinding : MonoBehaviour
{
	public KeyCode keyCode;

	private void Update()
	{
		if (!UICamera.inputHasFocus && keyCode != 0)
		{
			if (Input.GetKeyDown(keyCode))
			{
				SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
			}
			if (Input.GetKeyUp(keyCode))
			{
				SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
				SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
