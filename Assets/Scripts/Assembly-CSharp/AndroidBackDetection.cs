using UnityEngine;

public class AndroidBackDetection : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			AndroidUiStack.Instance.ActivateEscape();
		}
	}
}
