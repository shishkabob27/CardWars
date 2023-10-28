using UnityEngine;

public class ActivateObjectScript : MonoBehaviour
{
	public GameObject GameObj;

	public bool Activate;

	private void OnClick()
	{
		if (GameObj != null)
		{
			GameObj.SetActive(Activate);
		}
	}
}
