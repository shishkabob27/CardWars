using UnityEngine;

public class ScreenColorScript : MonoBehaviour
{
	public UISprite Overlay;

	public bool Red;

	public float TimeLimit;

	public float ColorTimer;

	public float Timer;

	private void Update()
	{
		if (!Red)
		{
			return;
		}
		if (Timer < TimeLimit)
		{
			Timer += Time.deltaTime;
			ColorTimer += Time.deltaTime;
			if (ColorTimer > 1f)
			{
				ColorTimer = 1f;
			}
		}
		else
		{
			ColorTimer -= Time.deltaTime;
			if (ColorTimer < 0f)
			{
				ColorTimer = 0f;
				TimeLimit = 0f;
				Red = false;
				Timer = 0f;
			}
		}
		Overlay.color = new Color(ColorTimer, 0f, 0f, ColorTimer * 0.4f);
	}
}
