using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

internal class CreditsUI : MonoBehaviour
{
	private class TSCreditInfo
	{
		[XmlAttribute("Text")]
		public string Text;

		[XmlAttribute("FontSize")]
		public float FontSize = 45f;

		[XmlAttribute("Texture")]
		public string Texture;

		[XmlAttribute("ColorR")]
		public float ColorR = 1f;

		[XmlAttribute("ColorG")]
		public float ColorG = 1f;

		[XmlAttribute("ColorB")]
		public float ColorB = 1f;

		[XmlAttribute("ColorA")]
		public float ColorA = 1f;

		[XmlAttribute("OutlineColorR")]
		public float OutlineColorR;

		[XmlAttribute("OutlineColorG")]
		public float OutlineColorG;

		[XmlAttribute("OutlineColorB")]
		public float OutlineColorB;

		[XmlAttribute("OutlineColorA")]
		public float OutlineColorA = 1f;

		[XmlAttribute("LineSpacing")]
		public float LineSpacing;

		[XmlAttribute("YOffset")]
		public float YOffset;

		[XmlAttribute("TextureWidth")]
		public float TextureWidth;

		[XmlAttribute("TextureHeight")]
		public float TextureHeight;

		public static TSCreditInfo FromXmlNode(XmlNode node)
		{
			TSCreditInfo tSCreditInfo = new TSCreditInfo();
			FieldInfo[] fields = typeof(TSCreditInfo).GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (node.Attributes[fieldInfo.Name] != null)
				{
					if (fieldInfo.FieldType == typeof(string))
					{
						fieldInfo.SetValue(tSCreditInfo, node.Attributes[fieldInfo.Name].InnerText);
					}
					else
					{
						fieldInfo.SetValue(tSCreditInfo, XmlBypass.GetFloat(node.Attributes[fieldInfo.Name], 0f));
					}
				}
			}
			return tSCreditInfo;
		}
	}

	[XmlRoot("CreditsList")]
	private class TSCreditsList
	{
		[XmlAttribute("ItemSpacing")]
		public float itemSpacing = 20f;

		[XmlArray("Credits")]
		[XmlArrayItem("Credit")]
		public List<TSCreditInfo> credits;

		private TSCreditsList()
		{
			credits = new List<TSCreditInfo>();
		}

		public static TSCreditsList FromXmlNode(XmlNode node)
		{
			TSCreditsList tSCreditsList = new TSCreditsList();
			if (node.Name != "CreditsList")
			{
				return null;
			}
			tSCreditsList.itemSpacing = XmlBypass.GetFloat(node.Attributes["ItemSpacing"], tSCreditsList.itemSpacing);
			XmlElement xmlElement = node["Credits"];
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode != null && !(childNode.Name != "Credit"))
				{
					tSCreditsList.credits.Add(TSCreditInfo.FromXmlNode(childNode));
				}
			}
			return tSCreditsList;
		}
	}

	private delegate void CreditsEndCallback();

	private const string CreditsFilename = "Credits/CreditsList";

	public float FIRST_ITEM_Y = -40f;

	public UILabel creditLabel;

	public UIDraggablePanel creditsLabelParent;

	public float scrollSpeed = 0.1f;

	private List<GameObject> creditsLabels = new List<GameObject>();

	private CreditsEndCallback creditsEndCallback;

	private bool closed;

	private float scrollHeight;

	private float lastY;

	private float topY;

	private static TSCreditsList creditsList;

	private void Awake()
	{
		topY = creditsLabelParent.transform.localPosition.y;
		LoadCreditsList("Credits/CreditsList");
	}

	private static void showobject(GameObject obj, bool show)
	{
		obj.SetActive(show);
	}

	private void ClearCreditsLabels()
	{
		foreach (GameObject creditsLabel in creditsLabels)
		{
			if (creditsLabel != null)
			{
				UnityEngine.Object.DestroyImmediate(creditsLabel);
			}
		}
		creditsLabels.Clear();
	}

	private void Start()
	{
		if (creditLabel != null)
		{
			creditLabel.gameObject.SetActive(false);
		}
		Setup(true, CreditsDoneCallback);
		if (Application.isPlaying)
		{
			NGUITools.SetActive(base.gameObject, false);
		}
	}

	private void OnEnable()
	{
		closed = false;
		SetScrollHeight();
		if (creditsLabelParent != null)
		{
			UIPanel component = creditsLabelParent.gameObject.GetComponent<UIPanel>();
			if (component != null && component.onChange != null)
			{
				creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y, 0f));
			}
		}
	}

	private void SetScrollHeight()
	{
		if (creditsLabelParent != null && creditsLabelParent.panel != null)
		{
			scrollHeight = Mathf.Abs(lastY) + creditsLabelParent.panel.clipRange.w * 2f;
		}
	}

	private float SetupLabel(GameObject instance, string txt, float fontsize, float r, float g, float b, float a, float or, float og, float ob, float oa, float linespacing, float x, float y)
	{
		Vector3 localScale = instance.transform.localScale;
		instance.transform.parent = ((!creditsLabelParent) ? base.gameObject.transform : creditsLabelParent.gameObject.transform);
		instance.transform.localScale = localScale;
		showobject(instance, true);
		UILabel uILabel = instance.GetComponent(typeof(UILabel)) as UILabel;
		if ((bool)uILabel)
		{
			uILabel.gameObject.transform.localScale = new Vector3(fontsize, fontsize, 1f);
			uILabel.color = new Color(r, g, b, a);
			uILabel.effectColor = new Color(or, og, ob, oa);
			uILabel.text = txt;
			uILabel.transform.localPosition = new Vector3(x, y, 0f);
			y -= NGUIMath.CalculateRelativeWidgetBounds((!creditsLabelParent) ? null : creditsLabelParent.transform, uILabel.transform).size.y + linespacing;
		}
		return y;
	}

	private float SetupTexture(GameObject instance, string texName, float w, float h, float r, float g, float b, float a, float linespacing, float x, float y)
	{
		Vector3 localScale = instance.transform.localScale;
		instance.transform.parent = ((!creditsLabelParent) ? base.gameObject.transform : creditsLabelParent.gameObject.transform);
		instance.transform.localScale = localScale;
		showobject(instance, true);
		Texture texture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(texName) as Texture;
		if (texture != null)
		{
			UITexture uITexture = instance.AddComponent(typeof(UITexture)) as UITexture;
			if (uITexture != null)
			{
				try
				{
					uITexture.shader = Shader.Find("Unlit/Transparent Colored");
					uITexture.pivot = UIWidget.Pivot.Top;
					uITexture.gameObject.transform.localScale = new Vector3((!(w > 0f) || !(h > 0f)) ? ((float)texture.width) : w, (!(w > 0f) || !(h > 0f)) ? ((float)texture.height) : h, 1f);
					uITexture.transform.localPosition = new Vector3(x, y, 0f);
					uITexture.color = new Color(r, g, b, a);
					uITexture.mainTexture = texture;
					y -= NGUIMath.CalculateRelativeWidgetBounds((!creditsLabelParent) ? null : creditsLabelParent.transform, uITexture.transform).size.y + linespacing;
				}
				catch (NullReferenceException)
				{
				}
			}
		}
		return y;
	}

	private void Setup()
	{
		if (creditsLabelParent != null)
		{
			creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y, 0f));
		}
		Setup(false, null);
	}

	private void Setup(bool backButtonShown, CreditsEndCallback callback)
	{
		creditsEndCallback = callback;
		ClearCreditsLabels();
		if (creditLabel != null)
		{
			float x = 0f;
			float num = FIRST_ITEM_Y;
			foreach (TSCreditInfo credit in creditsList.credits)
			{
				if ((credit.Texture != null && credit.Texture.Length > 0) || (credit.Text != null && credit.Text.Length > 0))
				{
					if (credit.Texture != null && credit.Texture.Length > 0)
					{
						GameObject gameObject = new GameObject();
						if (gameObject != null)
						{
							creditsLabels.Add(gameObject);
							num = SetupTexture(gameObject, credit.Texture, credit.TextureWidth, credit.TextureHeight, credit.ColorR, credit.ColorG, credit.ColorB, credit.ColorA, credit.LineSpacing, x, num - credit.YOffset);
						}
					}
					else
					{
						GameObject gameObject = SLOTGame.InstantiateFX(creditLabel.gameObject) as GameObject;
						if (gameObject != null)
						{
							creditsLabels.Add(gameObject);
							num = SetupLabel(gameObject, credit.Text, credit.FontSize, credit.ColorR, credit.ColorG, credit.ColorB, credit.ColorA, credit.OutlineColorR, credit.OutlineColorG, credit.OutlineColorB, credit.OutlineColorA, credit.LineSpacing, x, num - credit.YOffset);
						}
					}
				}
				else
				{
					num -= credit.LineSpacing;
				}
				num -= creditsList.itemSpacing;
			}
			lastY = num;
		}
		SetScrollHeight();
	}

	private void CloseScreen()
	{
		if (!closed)
		{
			closed = true;
			ClearCreditsLabels();
		}
	}

	private void backfadeoutcallback()
	{
		if (creditsEndCallback != null)
		{
			MonoBehaviour.print(" Calling credits end call back");
			creditsEndCallback();
		}
	}

	private void Update()
	{
		if (creditsLabelParent != null)
		{
			if (!creditsLabelParent.Pressed)
			{
				float y = scrollSpeed * Time.deltaTime;
				creditsLabelParent.MoveRelative(new Vector3(0f, y, 0f));
			}
			if (creditsLabelParent.transform.localPosition.y + topY > scrollHeight)
			{
				creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y, 0f));
			}
			else if (creditsLabelParent.transform.localPosition.y < 0f)
			{
				creditsLabelParent.MoveRelative(new Vector3(0f, 0f - creditsLabelParent.transform.localPosition.y - topY + scrollHeight, 0f));
			}
		}
	}

	private bool LoadCreditsList(string filename)
	{
		TextAsset textAsset = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(filename) as TextAsset;
		if (textAsset != null)
		{
			XmlDocument xmlDocument = XmlBypass.ParseString(textAsset.text);
			XmlNode node = xmlDocument["CreditsList"];
			creditsList = TSCreditsList.FromXmlNode(node);
			return true;
		}
		return false;
	}

	private void BackClicked()
	{
		CloseUI();
	}

	private void CloseUI()
	{
		ClearCreditsLabels();
		NGUITools.SetActive(base.gameObject, false);
	}

	private void CreditsDoneCallback()
	{
	}
}
