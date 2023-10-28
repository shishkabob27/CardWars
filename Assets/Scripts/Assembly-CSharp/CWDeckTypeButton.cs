using System.Collections.Generic;
using UnityEngine;

public class CWDeckTypeButton : MonoBehaviour
{
	public CardType cardType;

	public CWDeckAddCards DeckAddCards;

	public UILabel TextLabel;

	public UILabel TextLabel2;

	public UILabel StackLabel;

	public List<CWDeckTypeButton> otherButtons;

	public UISprite ButtonBG;

	public UISprite ButtonFrame;

	public UISprite ButtonIcon;

	public UITweener selectedTweener;

	private bool selected;

	public Color defaultColor;

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			if (selected)
			{
				foreach (CWDeckTypeButton otherButton in otherButtons)
				{
					otherButton.Selected = false;
				}
				DeckAddCards.Filter = cardType;
			}
			else if (DeckAddCards.Filter == cardType)
			{
				DeckAddCards.Filter = CardType.None;
			}
			if (selectedTweener != null)
			{
				SLOTUI.EnableTweener(selectedTweener, selected);
			}
		}
	}

	private void Start()
	{
		TextLabel.color = Color.white;
		TextLabel2.color = Color.white;
		if (ButtonBG != null)
		{
			ButtonBG.color = defaultColor;
		}
		if (ButtonFrame != null)
		{
			ButtonFrame.color = Color.white;
		}
		if (ButtonIcon != null)
		{
			ButtonIcon.color = Color.white;
		}
	}

	private void OnEnable()
	{
		if (selectedTweener == null)
		{
			selectedTweener = FindTweener();
		}
		Selected = cardType == CardType.Creature;
	}

	private UITweener FindTweener()
	{
		Transform transform = base.transform.Find("ScaleRoot");
		UITweener uITweener = null;
		if (transform != null)
		{
			uITweener = transform.gameObject.GetComponent(typeof(UITweener)) as UITweener;
			if (uITweener == null)
			{
				TweenScale tweenScale = transform.gameObject.AddComponent(typeof(TweenScale)) as TweenScale;
				if (tweenScale != null)
				{
					tweenScale.style = UITweener.Style.PingPong;
					tweenScale.duration = 0.25f;
					tweenScale.tweenGroup = 123;
					tweenScale.from = Vector3.one;
					tweenScale.to = new Vector3(1.1f, 1.1f, 1.1f);
					uITweener = tweenScale;
				}
			}
		}
		return uITweener;
	}

	private void OnClick()
	{
		if (!Selected)
		{
			Selected = true;
		}
	}

	public void Update()
	{
		if (DeckAddCards.chosenCounts != null && DeckAddCards.chosenCounts.Length > (int)cardType)
		{
			StackLabel.text = DeckAddCards.chosenCounts[(int)cardType].ToString();
		}
	}
}
