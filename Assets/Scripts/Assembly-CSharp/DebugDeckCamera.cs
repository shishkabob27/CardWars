using UnityEngine;

public class DebugDeckCamera : MonoBehaviour
{
	public GameObject FlyDeckButton;

	public GameObject TblDeckButton;

	public bool useFly;

	private void OnEnable()
	{
		GameObject gameObject = null;
		gameObject = (useFly ? TblDeckButton : FlyDeckButton);
		if (gameObject != null)
		{
			NGUITools.SetActive(gameObject, false);
		}
	}
}
