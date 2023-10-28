using UnityEngine;

public class RemoveBackgroundScript : MonoBehaviour
{
	private GameObject Background;

	private void Start()
	{
		Background = GameObject.Find("Background_Panel");
		Object.Destroy(Background);
		Object.Destroy(this);
	}
}
