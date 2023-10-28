using UnityEngine;

public class NcDrawFpsText : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private void Start()
	{
		if (!GetComponent<GUIText>())
		{
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string text = string.Format("{0:F2} FPS", num);
			GetComponent<GUIText>().text = text;
			if (num < 30f)
			{
				GetComponent<GUIText>().material.color = Color.yellow;
			}
			else if (num < 10f)
			{
				GetComponent<GUIText>().material.color = Color.red;
			}
			else
			{
				GetComponent<GUIText>().material.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
