using System.Text.RegularExpressions;
using UnityEngine;

public class NotificationTicker : MonoBehaviour
{
	private const float TICKER_SPEED = 2f;

	public UILabel tickerLabel;

	private Vector2 textSize;

	private float tickerClipWidth;

	private Vector3 tickerLabelPos;

	private Vector3 tickerLabelScale;

	private bool gotTickerLabelPos;

	private void Awake()
	{
		GetTickerLabelPos();
	}

	private void OnEnable()
	{
		UpdateTextSize();
		tickerLabelPos.x = tickerClipWidth * 0.5f;
	}

	public string GetText()
	{
		if (tickerLabel != null)
		{
			return tickerLabel.text;
		}
		return null;
	}

	public void SetText(string txt, bool resetPosition = true, params string[] Params)
	{
		GetTickerLabelPos();
		if (!(tickerLabel != null))
		{
			return;
		}
		txt = ConvertTextForTicker(txt);
		if (Params != null)
		{
			txt = string.Format(txt, Params);
		}
		if (tickerLabel.text != txt)
		{
			tickerLabel.text = txt;
			UpdateTextSize();
			if (resetPosition)
			{
				tickerLabelPos.x = tickerClipWidth * 0.5f;
			}
		}
	}

	private void UpdateTextSize()
	{
		if (tickerLabel != null && tickerLabel.font != null)
		{
			string text = tickerLabel.text;
			textSize = (string.IsNullOrEmpty(text) ? Vector2.zero : tickerLabel.font.CalculatePrintedSize(text, tickerLabel.supportEncoding, tickerLabel.symbolStyle));
			if (tickerLabelScale.x != 0f)
			{
				textSize.x *= tickerLabelScale.x;
			}
			if (tickerLabelScale.y != 0f)
			{
				textSize.y *= tickerLabelScale.y;
			}
		}
	}

	private void Update()
	{
		if (Application.isPlaying && tickerLabel != null && !string.IsNullOrEmpty(tickerLabel.text))
		{
			tickerLabelPos.x -= 2f * Time.deltaTime;
			float num = textSize.x + tickerClipWidth * 0.5f;
			if (tickerLabelPos.x < 0f - num)
			{
				tickerLabelPos.x = tickerClipWidth * 0.5f;
			}
			tickerLabel.transform.position = tickerLabelPos;
		}
	}

	private string ConvertTextForTicker(string txt)
	{
		return Regex.Replace(txt, "\\r\\n?|\\n", "     ");
	}

	private void GetTickerLabelPos()
	{
		if (!gotTickerLabelPos)
		{
			if (tickerLabel != null)
			{
				Vector3 position = tickerLabel.transform.position;
				tickerLabelPos.y = position.y;
				tickerLabelPos.z = position.z;
				tickerLabelScale = tickerLabel.transform.lossyScale;
			}
			UIPanel componentInChildren = SLOTGame.GetComponentInChildren<UIPanel>(base.gameObject, true);
			if (componentInChildren != null)
			{
				Vector3 scale = SLOTGame.GetScale(componentInChildren.transform);
				tickerClipWidth = componentInChildren.clipRange.z * scale.x;
			}
			gotTickerLabelPos = true;
		}
	}
}
