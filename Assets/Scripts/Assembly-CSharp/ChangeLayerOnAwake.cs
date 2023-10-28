using UnityEngine;

public class ChangeLayerOnAwake : MonoBehaviour
{
	public string ChangeToLayer;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (ChangeToLayer != null)
		{
			base.gameObject.SetLayerRecursively(ChangeToLayer);
		}
	}
}
