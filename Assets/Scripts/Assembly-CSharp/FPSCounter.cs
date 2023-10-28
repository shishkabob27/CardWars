using System.Collections;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public float frequency = 0.5f;

	public UISprite bar;

	private GUIText guiText;

	private UILabel uiLabel;

	private TextMesh textMesh;

	public float FramesPerSec { get; protected set; }

	private void OnEnable()
	{
		guiText = base.gameObject.GetComponent<GUIText>();
		textMesh = base.gameObject.GetComponent<TextMesh>();
		uiLabel = base.gameObject.GetComponent<UILabel>();
		StartCoroutine(FPS());
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;
			FramesPerSec = (float)frameCount / timeSpan;
			if (guiText != null)
			{
				guiText.text = string.Format("{0:F1} FPS", FramesPerSec);
			}
			if (textMesh != null)
			{
				textMesh.text = string.Format("{0:F1} FPS", FramesPerSec);
			}
			if (uiLabel != null)
			{
				uiLabel.text = string.Format("{0:F1} FPS", FramesPerSec);
			}
			bar.fillAmount = FramesPerSec / 60f;
			if (FramesPerSec < 30f)
			{
				if (guiText != null)
				{
					guiText.material.color = Color.yellow;
				}
				if (uiLabel != null)
				{
					uiLabel.color = Color.yellow;
				}
				if (bar != null)
				{
					bar.color = Color.yellow;
				}
			}
			else if (FramesPerSec < 10f)
			{
				if (guiText != null)
				{
					guiText.material.color = Color.red;
				}
				if (uiLabel != null)
				{
					uiLabel.color = Color.red;
				}
				if (bar != null)
				{
					bar.color = Color.red;
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
				if (bar != null)
				{
					bar.color = Color.green;
				}
			}
		}
	}
}
