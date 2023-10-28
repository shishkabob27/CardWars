using UnityEngine;

public class CWKetchupBottleCollider : MonoBehaviour
{
	public CWKetchupBottleScript bottle;

	private void OnPress(bool isDown)
	{
		if (bottle != null)
		{
			bottle.OnPress(isDown);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (bottle != null)
		{
			bottle.OnDrag(delta);
		}
	}
}
