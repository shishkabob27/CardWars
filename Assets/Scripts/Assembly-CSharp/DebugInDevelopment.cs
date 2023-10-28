using UnityEngine;

public class DebugInDevelopment : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
