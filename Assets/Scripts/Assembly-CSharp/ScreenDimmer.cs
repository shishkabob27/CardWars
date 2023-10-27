using UnityEngine;

public class ScreenDimmer : MonoBehaviour
{
	public enum StartupType
	{
		FadeIn = 0,
		FadeOut = 1,
		FadedIn = 2,
		FadedOut = 3,
	}

	public float fadeDuration;
	public float alpha;
	public StartupType startupType;
}
