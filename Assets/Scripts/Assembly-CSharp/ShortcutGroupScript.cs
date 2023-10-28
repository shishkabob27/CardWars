using UnityEngine;

public class ShortcutGroupScript : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.position = new Vector3(0f, base.transform.position.y, base.transform.position.z);
	}
}
