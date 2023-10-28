using UnityEngine;

public class RestartSceneScript : MonoBehaviour
{
	public delegate void ResetGameCallback();

	public static ResetGameCallback resetGameCallback;

	public void OnClick()
	{
		if (resetGameCallback != null)
		{
			resetGameCallback();
		}
	}
}
