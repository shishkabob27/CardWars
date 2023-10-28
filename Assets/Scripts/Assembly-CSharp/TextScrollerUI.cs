using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class TextScrollerUI : MonoBehaviour
{
	private class FormatDefaults
	{
		public const float FontSize = 30f;

		public const float ColorR = 1f;

		public const float ColorG = 1f;

		public const float ColorB = 1f;

		public const float ColorA = 1f;

		public const float OutlineColorR = 0f;

		public const float OutlineColorG = 0f;

		public const float OutlineColorB = 0f;

		public const float OutlineColorA = 1f;

		public const float LineSpacing = 0f;

		public const float YOffset = 2f;

		public const float TextureWidth = 0f;

		public const float TextureHeight = 0f;

		public const float ItemSpacing = 20f;
	}

	private const int bufferSize = 200;

	public UILabel templateLabel;

	public UIDraggablePanel scrollerPanel;

	public float scrollSpeed;

	public string LocStringOrFilename;

	public bool isFilename = true;

	public bool allowWrap;

	public bool mustScrollToEnd;

	private bool hasReachedEnd;

	public GameObject acceptButton;

	private List<GameObject> labels = new List<GameObject>();

	private List<string> stringLines = new List<string>();

	private float scrollHeight;

	private float lastY;

	private float topY;

	private bool initialized;

	private bool next_update_delay;

	private bool populating;

	private void ClearLabels()
	{
		foreach (GameObject label in labels)
		{
			if (label != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(label);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(label);
				}
			}
		}
		labels.Clear();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		initialized = false;
		hasReachedEnd = false;
		populating = false;
		next_update_delay = false;
		topY = scrollerPanel.transform.localPosition.y;
		LoadText(LocStringOrFilename);
		if (templateLabel != null)
		{
			templateLabel.gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		ClearLabels();
		if (stringLines != null)
		{
			stringLines.Clear();
			stringLines = null;
		}
		scrollHeight = (lastY = (topY = 0f));
	}

	private void SetScrollHeight()
	{
		if (scrollerPanel != null && scrollerPanel.panel != null)
		{
			scrollHeight = Mathf.Abs(lastY);
		}
	}

	private float SetupLabel(GameObject instance, string txt, float fontSize, float x, float y)
	{
		return SetupLabel(instance, txt, fontSize, 1f, 1f, 1f, 1f, 0f, 0f, 0f, 1f, 0f, x, y);
	}

	private float SetupLabel(GameObject instance, string txt, float fontsize, float r, float g, float b, float a, float or, float og, float ob, float oa, float linespacing, float x, float y)
	{
		Vector3 localScale = instance.transform.localScale;
		instance.transform.parent = ((!scrollerPanel) ? base.gameObject.transform : scrollerPanel.gameObject.transform);
		instance.transform.localScale = localScale;
		instance.SetActive(true);
		UILabel uILabel = instance.GetComponent(typeof(UILabel)) as UILabel;
		if ((bool)uILabel)
		{
			uILabel.gameObject.transform.localScale = new Vector3(fontsize, fontsize, 1f);
			uILabel.color = new Color(r, g, b, a);
			uILabel.effectColor = new Color(or, og, ob, oa);
			uILabel.text = txt;
			uILabel.transform.localPosition = new Vector3(x, y, 0f);
			y -= NGUIMath.CalculateRelativeWidgetBounds((!scrollerPanel) ? null : scrollerPanel.transform, uILabel.transform).size.y + linespacing;
		}
		instance.SetActive(false);
		return y;
	}

	public IEnumerator Setup()
	{
		yield return new WaitForSeconds(0.2f);
		ClearLabels();
		int i = 0;
		if (templateLabel != null)
		{
			float x = templateLabel.transform.localPosition.x;
			float y = 0f;
			float fontSize = templateLabel.transform.localScale.y;
			int ithLine = 0;
			int maxLineProcessed = Math.Min((int)((double)stringLines.Count * 0.1), 20);
			foreach (string line in stringLines)
			{
				if (!string.IsNullOrEmpty(line) && line.Length > 0)
				{
					GameObject instance = SLOTGame.InstantiateFX(templateLabel.gameObject) as GameObject;
					if (instance != null)
					{
						y = SetupLabel(instance, line, fontSize, x, y - 2f);
						instance.SetActive(false);
						labels.Add(instance);
					}
				}
				else
				{
					y = y;
				}
				y = (lastY = y - 20f);
				SetScrollHeight();
				CheckBounds();
				if (ithLine++ > maxLineProcessed)
				{
					ithLine = 0;
					yield return null;
				}
			}
		}
		initialized = true;
	}

	private void Update()
	{
		if (!initialized && !populating)
		{
			populating = true;
			StartCoroutine(Setup());
		}
		SetScrollHeight();
		if (null != acceptButton)
		{
			acceptButton.GetComponent<Collider>().enabled = !mustScrollToEnd || hasReachedEnd;
		}
		foreach (GameObject label in labels)
		{
			UILabel component = label.GetComponent<UILabel>();
			float num = component.transform.localScale.y * component.relativeSize.y;
			float y = component.transform.localPosition.y;
			float num2 = component.transform.localPosition.y - num;
			bool flag = scrollerPanel.transform.localPosition.y + num2 - 200f > scrollerPanel.panel.clipRange.w;
			bool flag2 = scrollerPanel.transform.localPosition.y + y + 200f < 0f;
			label.SetActive(!flag && !flag2);
		}
		CheckBounds();
	}

	private void CheckBounds()
	{
		if (!(scrollerPanel != null))
		{
			return;
		}
		if (!scrollerPanel.Pressed)
		{
			float y = scrollSpeed * Time.deltaTime;
			scrollerPanel.MoveRelative(new Vector3(0f, y, 0f));
		}
		if (allowWrap)
		{
			if (scrollerPanel.transform.localPosition.y + topY > scrollHeight)
			{
				hasReachedEnd = true;
				scrollerPanel.MoveRelative(new Vector3(0f, 0f - scrollerPanel.transform.localPosition.y, 0f));
			}
			else if (scrollerPanel.transform.localPosition.y < 0f)
			{
				scrollerPanel.MoveRelative(new Vector3(0f, 0f - scrollerPanel.transform.localPosition.y - topY + scrollHeight, 0f));
			}
			return;
		}
		if (scrollerPanel.transform.localPosition.y < scrollerPanel.panel.clipRange.w)
		{
			scrollerPanel.MoveRelative(new Vector3(0f, scrollerPanel.panel.clipRange.w - scrollerPanel.transform.localPosition.y, 0f));
		}
		if (scrollerPanel.transform.localPosition.y - scrollHeight - scrollerPanel.panel.clipRange.w / 3f > 0f)
		{
			hasReachedEnd = true;
			scrollerPanel.MoveRelative(new Vector3(0f, 0f - (scrollerPanel.transform.localPosition.y - scrollHeight - scrollerPanel.panel.clipRange.w / 3f), 0f));
		}
	}

	private bool LoadText(string text_or_filename)
	{
		string text = null;
		if (isFilename)
		{
			TextAsset textAsset = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(text_or_filename) as TextAsset;
			if (null != textAsset)
			{
				text = textAsset.text;
			}
		}
		else
		{
			text = Localization.instance.Get(text_or_filename);
		}
		if (!string.IsNullOrEmpty(text))
		{
			char[] separator = new char[1] { '\n' };
			string[] array = text.Split(separator);
			stringLines = new List<string>();
			string[] array2 = array;
			foreach (string item in array2)
			{
				stringLines.Add(item);
			}
			return true;
		}
		return false;
	}
}
