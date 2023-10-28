using UnityEngine;

public class HUDFPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private GUIText guiText;

	private TextMesh textMesh;

	private UILabel uiLabel;

	private void Start()
	{
		if (base.gameObject.GetComponent<GUIText>() != null)
		{
			guiText = base.gameObject.GetComponent<GUIText>();
		}
		if (base.gameObject.GetComponent<TextMesh>() != null)
		{
			textMesh = base.gameObject.GetComponent<TextMesh>();
		}
		if (base.gameObject.GetComponent<UILabel>() != null)
		{
			uiLabel = base.gameObject.GetComponent<UILabel>();
		}
		timeleft = updateInterval;
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if (!((double)timeleft <= 0.0))
		{
			return;
		}
		float num = accum / (float)frames;
		string text = string.Format("{0:F2} FPS", num);
		if (guiText != null)
		{
			guiText.text = text;
		}
		if (textMesh != null)
		{
			textMesh.text = text;
		}
		if (uiLabel != null)
		{
			uiLabel.text = text;
		}
		if (num < 30f)
		{
			if (guiText != null)
			{
				guiText.material.color = Color.yellow;
			}
			if (uiLabel != null)
			{
				uiLabel.color = Color.yellow;
			}
			return;
		}
		if (num < 10f)
		{
			if (guiText != null)
			{
				guiText.material.color = Color.red;
			}
			if (uiLabel != null)
			{
				uiLabel.color = Color.red;
			}
		}
		else
		{
			if (guiText != null)
			{
				guiText.material.color = Color.green;
			}
			if (uiLabel != null)
			{
				uiLabel.color = Color.green;
			}
		}
		timeleft = updateInterval;
		accum = 0f;
		frames = 0;
	}
}
