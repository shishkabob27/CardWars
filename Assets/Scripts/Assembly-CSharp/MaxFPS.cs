using UnityEngine;

public class MaxFPS : MonoBehaviour
{
	public int maxFPS = 60;

	private void OnEnable()
	{
		Application.targetFrameRate = maxFPS;
		base.enabled = false;
	}
}
