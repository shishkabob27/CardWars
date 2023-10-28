using UnityEngine;

public class ImmediateUnColliderScript : MonoBehaviour
{
	private void OnEnable()
	{
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = true;
		}
	}

	private void OnClick()
	{
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = false;
		}
	}
}
