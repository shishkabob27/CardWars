using UnityEngine;

public class CalendarGiftController : MonoBehaviour
{
	public UISprite IconReward;
	public UISprite CalendarFrame;
	public UISprite CountdownBG;
	public UISprite HeaderBG;
	public UILabel GiftRewardText;
	public GameObject Card;
	public GameObject[] Days;
	public GameObject Special;
	public GameObject CalendarButtonClose;
	public UIButtonTween CalendarHide;
	public UIButtonTween CalendarRewardShow;
	public UIButtonTween CalendarCatchupShow;
	public UIButtonTween CalendarCatchupHide;
	public UILabel CalendarCatchupText;
	public UIButtonTween NotEnoughGemsShow;
	public AudioClip RewardClip;
	public AudioClip ClaimDayClip;
	public UILabel TimeLeftText;
	public GameObject FirstTimePopupTemplate;
	public GameObject GachaButton;
	public PopupTapDelegate FirstGachaKeyPopup;
	public UIButtonTween FirstGachaKeyShow;
	public UILabel DaysClaimedText;
}
