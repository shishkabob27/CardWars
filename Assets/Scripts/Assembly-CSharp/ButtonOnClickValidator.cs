using UnityEngine;

public class ButtonOnClickValidator : MonoBehaviour
{
	public GameObject onClickTarget;

	private void OnClick()
	{
		if (onClickTarget != null && IsClickValid())
		{
			onClickTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	public bool IsClickValid()
	{
		return GetComponent<Collider>() != null && GetComponent<Collider>().enabled && UICamera.lastHit.collider == GetComponent<Collider>();
	}
}
