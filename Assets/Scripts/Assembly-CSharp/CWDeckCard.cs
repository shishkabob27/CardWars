using System;
using System.Collections.Generic;
using UnityEngine;

public class CWDeckCard : MonoBehaviour
{
	public delegate bool ClickedCallbackFunc(CWDeckCard deckCard);

	private bool selected;

	private bool xed;

	private bool grayed;

	private bool inuse;

	private bool responsive = true;

	public bool SelectShowsX;

	public GameObject RadioParentObject;

	public CWDeckCardList SelectList;

	public CardItem card;

	public AudioClip pickSound;

	public bool isZoomed;

	private int SelectionListLimit = 10;

	private CWDeckLongPressZoom zoom;

	public ClickedCallbackFunc clickedCallback;

	public bool Responsive
	{
		get
		{
			return responsive;
		}
		set
		{
			responsive = value;
		}
	}

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			xed = false;
			if (selected)
			{
				UnsetAllSiblings();
			}
			if ((bool)SelectList)
			{
				ReLabelSequenceNums();
				return;
			}
			SetEnabled("Panel/Panel/Selected", selected);
			SetEnabled("Panel/Panel/Xed", xed);
		}
	}

	public bool Xed
	{
		get
		{
			return xed;
		}
		set
		{
			selected = false;
			xed = value;
			if (xed)
			{
				UnsetAllSiblings();
			}
			if ((bool)SelectList)
			{
				ReLabelSequenceNums();
				return;
			}
			SetEnabled("Panel/Panel/Selected", selected);
			SetEnabled("Panel/Panel/Xed", xed);
		}
	}

	public bool Grayed
	{
		get
		{
			return grayed;
		}
		set
		{
			grayed = value;
			SetGray(grayed);
		}
	}

	public bool InUse
	{
		get
		{
			return inuse;
		}
		set
		{
			inuse = value;
			SQUtils.SetActive(base.gameObject, "Panel/Panel/InUse", inuse);
		}
	}

	private void Start()
	{
		zoom = base.gameObject.GetComponent<CWDeckLongPressZoom>();
	}

	public void OnClick()
	{
		if (isZoomed || card == null)
		{
			return;
		}
		if (card.IsNew)
		{
			card.IsNew = false;
			ShowNewFlag(base.gameObject, false);
		}
		if (Grayed || !Responsive)
		{
			return;
		}
		if (clickedCallback == null || !clickedCallback(this))
		{
			if (SelectShowsX)
			{
				Xed = !Xed;
			}
			else
			{
				Selected = !Selected;
			}
		}
		if (pickSound != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayGUISound(pickSound);
		}
	}

	public void OnDoubleClick()
	{
		if (!(zoom == null) && clickedCallback == null && !isZoomed && card != null)
		{
			if (card.IsNew)
			{
				card.IsNew = false;
				ShowNewFlag(base.gameObject, false);
			}
			if (!Grayed)
			{
				zoom.gameObject.SendMessage("TriggerZoomCard", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void SetGray(bool isGray)
	{
		Color factionColor = FactionManager.Instance.GetFactionData(card.Form.Faction).FactionColor;
		Color color = Color.Lerp(factionColor, new Color(0.2f, 0.2f, 0.2f, 1f), 0.5f);
		UISprite[] componentsInChildren = base.gameObject.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			Color color2 = Color.white;
			if (!(uISprite.name == "InUse_BG") && !(uISprite.name == "Sequence_BG"))
			{
				if (uISprite.name == "Card_Frame")
				{
					color2 = factionColor;
				}
				else if (uISprite.name == "BG_Name" || uISprite.name == "BG_Type" || uISprite.name == "Frame_ATK" || uISprite.name == "Frame_DEF")
				{
					color2 = Color.Lerp(factionColor, Color.white, 0.5f);
				}
				uISprite.color = ((!isGray) ? color2 : color);
			}
		}
	}

	private void SetEnabled(string name, bool isEnabled)
	{
		Transform transform = base.transform.Find(name);
		if (!(transform == null))
		{
			UISprite component = transform.gameObject.GetComponent<UISprite>();
			if (!(component == null))
			{
				component.enabled = isEnabled;
			}
		}
	}

	private void UnsetAllSiblings()
	{
		if (RadioParentObject == null)
		{
			return;
		}
		CWDeckCard[] componentsInChildren = RadioParentObject.GetComponentsInChildren<CWDeckCard>(true);
		CWDeckCard[] array = componentsInChildren;
		foreach (CWDeckCard cWDeckCard in array)
		{
			if (!(cWDeckCard == this))
			{
				cWDeckCard.Selected = false;
				cWDeckCard.Xed = false;
			}
		}
	}

	public static void ShowNewFlag(GameObject obj, bool isNew)
	{
		Transform transform = obj.transform.Find("Panel/Panel/New");
		if (transform != null)
		{
			NGUITools.SetActive(transform.gameObject, isNew);
		}
	}

	public void SetSequenceNum(int ix)
	{
		Transform transform = base.transform.Find("Panel/Panel/SequenceNum");
		NGUITools.SetActive(transform.gameObject, true);
		SQUtils.SetLabel(transform, "Sequence_Label", ix.ToString());
		if (SelectShowsX)
		{
			xed = true;
		}
		else
		{
			selected = true;
		}
	}

	public void ClearSequenceNum()
	{
		Transform transform = base.transform.Find("Panel/Panel/SequenceNum");
		NGUITools.SetActive(transform.gameObject, false);
		xed = false;
		selected = false;
	}

	public void GrayIfTooManySelected()
	{
		Grayed = SelectList.chosenList.Count >= SelectionListLimit && !Selected && !Xed;
	}

	private void ReLabelSequenceNums()
	{
		if (SelectList == null)
		{
			return;
		}
		List<CardItem> chosenList = SelectList.chosenList;
		chosenList.Remove(card);
		CWDeckCard[] componentsInChildren = SelectList.gameObject.GetComponentsInChildren<CWDeckCard>();
		int num = 1;
		CardItem dcard;
		foreach (CardItem chosen in SelectList.chosenList)
		{
			dcard = chosen;
			CWDeckCard cWDeckCard = Array.Find(componentsInChildren, (CWDeckCard p) => p.card == dcard);
			if (cWDeckCard != null)
			{
				cWDeckCard.SetSequenceNum(num);
			}
			num++;
		}
		if (selected || xed)
		{
			chosenList.Add(card);
			SetSequenceNum(num);
		}
		else
		{
			ClearSequenceNum();
		}
		CWDeckCard[] array = componentsInChildren;
		foreach (CWDeckCard cWDeckCard2 in array)
		{
			cWDeckCard2.GrayIfTooManySelected();
		}
	}
}
