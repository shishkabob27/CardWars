using UnityEngine;

public class StartAndStopTween : MonoBehaviour
{
	public GameObject target;

	private void OnGUI()
	{
		if (GUILayout.Button("Start Bounce"))
		{
			iTweenEvent.GetEvent(target, "Bounce").Play();
		}
		if (GUILayout.Button("Stop Bounce"))
		{
			iTweenEvent.GetEvent(target, "Bounce").Stop();
		}
		if (GUILayout.Button("Start Color Fade"))
		{
			iTweenEvent.GetEvent(target, "Color Fade").Play();
		}
		if (GUILayout.Button("Stop Color Fade"))
		{
			iTweenEvent.GetEvent(target, "Color Fade").Stop();
		}
	}
}
