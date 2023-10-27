using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick = 0,
		OnMouseOver = 1,
		OnMouseOut = 2,
		OnPress = 3,
		OnRelease = 4,
	}

	public AudioClip audioClip;
	public Trigger trigger;
	public float volume;
	public float pitch;
}
