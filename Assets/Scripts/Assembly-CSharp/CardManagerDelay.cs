using UnityEngine;

public class CardManagerDelay : MonoBehaviour
{
	private void Update()
	{
		if (AIDeckManager.Instance.Loaded)
		{
			CardManagerScript component = base.gameObject.GetComponent<CardManagerScript>();
			if (component != null)
			{
				component.enabled = true;
			}
			Object.Destroy(this);
		}
	}
}
