using UnityEngine;
using System.Collections.Generic;

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
	public Color defaultColor;
}
