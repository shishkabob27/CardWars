using UnityEngine;

public class CWDeckConfirmButton : AsyncData<string>
{
	public CWDeckAddCards DeckAddCards;
	public UILabel SizeLabel;
	public UIButtonTween closeTween;
	public AudioClip[] placeCardSounds;
	public CWDeckManagerShowHideBackButton showhide;
}
