#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CalendarGiftController : MonoBehaviour
{
	private class UpsellInfo
	{
		public int NumGiftsTotalLastMonth;

		public int NumGiftsClaimedLastMonth;

		public int CatchupDayIndex;

		public int UnclaimedDays;

		public int TotalCost;

		public UpsellInfo()
		{
			Clear();
		}

		public void Clear()
		{
			NumGiftsTotalLastMonth = 0;
			NumGiftsClaimedLastMonth = 0;
			CatchupDayIndex = -1;
			UnclaimedDays = 0;
			TotalCost = 0;
		}
	}

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

	private UpsellInfo upsellInfo;

	private bool mWasKeyAwarded;

	private void Start()
	{
	}

	private void OnEnable()
	{
		Init();
		PopupGift();
	}

	private void Update()
	{
		UpdateTimeLeft();
	}

	private void PopupGift()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!(null != instance))
		{
			return;
		}
		if (instance.NeedsCalendarReset())
		{
			InitUpsellInfo();
			if (upsellInfo != null && upsellInfo.TotalCost > 0)
			{
				StartCoroutine(CalendarUpsell());
				return;
			}
			ResetCalendar();
		}
		if (instance.HasUnclaimedCalendarGift())
		{
			if (null != CalendarButtonClose)
			{
				CalendarButtonClose.SetActive(false);
			}
			StartCoroutine(GiftAndShowReward());
		}
	}

	private void Init()
	{
		List<CalendarGift> calendarGifts = CalendarGiftDataManager.Instance.GetCalendarGifts();
		if (calendarGifts.Any())
		{
			CalendarGift calendarGift = calendarGifts[0];
			if (null != HeaderBG && calendarGift.HeaderBG != null)
			{
				HeaderBG.spriteName = calendarGift.HeaderBG;
			}
			if (null != CountdownBG && calendarGift.CountdownBG != null)
			{
				CountdownBG.spriteName = calendarGift.CountdownBG;
			}
			if (null != CalendarFrame && calendarGift.CalendarFrame != null)
			{
				CalendarFrame.spriteName = calendarGift.CalendarFrame;
			}
			int num = 0;
			GameObject[] days = Days;
			foreach (GameObject gameObject in days)
			{
				TFUtils.Assert(calendarGifts.Count > num, "Something is wrong with the data, not enough days defined in schedule");
				if (num > calendarGifts.Count)
				{
					break;
				}
				calendarGift = calendarGifts[num];
				Transform transform = gameObject.transform.Find("CellContent");
				if (!(transform != null))
				{
					continue;
				}
				Transform transform2 = transform.transform.Find("Label_Day");
				UILabel uILabel = ((!(null != transform2)) ? null : transform2.GetComponent<UILabel>());
				if (null != uILabel)
				{
					uILabel.text = KFFLocalization.Get("!!DAY") + " " + (num + 1);
				}
				if (calendarGift.IconBorder != null)
				{
					Transform transform3 = transform.transform.Find("Background");
					if (null != transform3)
					{
						UISprite component = transform3.gameObject.transform.Find("BG").GetComponent<UISprite>();
						component.spriteName = calendarGift.IconBorder;
					}
				}
				if (calendarGift.GiftType.Contains("Key"))
				{
					Transform transform4 = transform.transform.Find("StarsKey");
					if (transform4 != null)
					{
						transform4.gameObject.SetActive(true);
					}
				}
				if (calendarGift.Icon != null)
				{
					Transform transform5 = transform.transform.Find("Reward");
					if (null != transform5)
					{
						UISprite component2 = transform5.gameObject.transform.Find("Icon_Reward").GetComponent<UISprite>();
						component2.spriteName = calendarGift.Icon;
					}
				}
				num++;
			}
		}
		SetDaysClaimed();
	}

	private void InitUpsellInfo()
	{
		upsellInfo = null;
		CalendarGiftDataManager instance = CalendarGiftDataManager.Instance;
		ScheduleData previousCalendar = instance.GetPreviousCalendar();
		if (previousCalendar != null)
		{
			PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
			int numCalendarGifts = instance.GetNumCalendarGifts(previousCalendar);
			int numCalendarGiftsClaimed = instance2.GetNumCalendarGiftsClaimed(previousCalendar);
			int num = instance.GetCatchupDayIndex(previousCalendar);
			if (numCalendarGiftsClaimed > num)
			{
				num = numCalendarGifts - 1;
			}
			int unclaimedDays = num + 1 - numCalendarGiftsClaimed;
			int num2 = 0;
			List<CalendarGift> calendarGifts = instance.GetCalendarGifts(previousCalendar);
			for (int i = numCalendarGiftsClaimed; i <= num; i++)
			{
				num2 += calendarGifts[i].CatchupCost;
			}
			upsellInfo = new UpsellInfo();
			upsellInfo.NumGiftsTotalLastMonth = numCalendarGifts;
			upsellInfo.NumGiftsClaimedLastMonth = numCalendarGiftsClaimed;
			upsellInfo.CatchupDayIndex = num;
			upsellInfo.UnclaimedDays = unclaimedDays;
			upsellInfo.TotalCost = num2;
		}
	}

	private void ResetCalendar()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (null != instance)
		{
			instance.ResetCalendar();
			instance.Save();
		}
		Init();
	}

	public void OnNoCatchup()
	{
		ResetCalendar();
		PopupGift();
	}

	public void OnYesCatchup()
	{
		TFUtils.Assert(null != upsellInfo, "Upsell info should not be null at this point");
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (upsellInfo.TotalCost > instance.Gems)
		{
			if (null != NotEnoughGemsShow)
			{
				if (null != CalendarHide)
				{
					CalendarHide.Play(true);
				}
				NotEnoughGemsShow.Play(true);
			}
		}
		else
		{
			ScheduleData previousCalendar = CalendarGiftDataManager.Instance.GetPreviousCalendar();
			if (previousCalendar != null)
			{
				instance.Gems -= upsellInfo.TotalCost;
				instance.Save();
				Singleton<AnalyticsManager>.Instance.LogCalendarPrizePurchase(upsellInfo.TotalCost);
				StartCoroutine(GiftAndShowAfterUpsell(previousCalendar, upsellInfo.CatchupDayIndex));
			}
		}
		if (null != CalendarCatchupHide)
		{
			CalendarCatchupHide.Play(true);
		}
	}

	public void OnYesBuyMoreGems()
	{
		if (null != CalendarHide)
		{
			CalendarHide.Play(true);
		}
	}

	public void OnNoBuyMoreGems()
	{
		ResetCalendar();
		PopupGift();
	}

	public void OnCloseReward()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.HasFirstTimeKeyAwarded() && mWasKeyAwarded)
		{
			instance.GatchaKeyAwarded();
			FirstGachaKeyShow.Play(true);
			if (FirstGachaKeyPopup != null)
			{
				CalendarHide.Play(true);
				FirstGachaKeyPopup.Init(KFFLocalization.Get("!!KEY_TUTORIAL"), new GameObject[1] { GachaButton }, null, "OnClick");
			}
		}
		if (instance.NeedsCalendarReset())
		{
			ResetCalendar();
		}
		PopupGift();
	}

	private void UpdateTimeLeft()
	{
		if (null != TimeLeftText)
		{
			TimeSpan timeLeftInCalendar = CalendarGiftDataManager.Instance.GetTimeLeftInCalendar();
			TimeLeftText.text = TFUtils.DurationToString((int)timeLeftInCalendar.TotalSeconds);
		}
		if (DaysClaimedText != null)
		{
			int numCalendarGiftsClaimed = PlayerInfoScript.GetInstance().GetNumCalendarGiftsClaimed();
			DaysClaimedText.text = KFFLocalization.Get("!!DAYS_CLAIMED") + ": " + numCalendarGiftsClaimed;
		}
	}

	private void SetDaysClaimed(int maxIndex = -1)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		bool[] calendarGiftClaimHistory = instance.GetCalendarGiftClaimHistory();
		if (maxIndex == -1)
		{
			maxIndex = Days.Length - 1;
		}
		for (int i = 0; i <= maxIndex; i++)
		{
			SetDayClaimed(i, calendarGiftClaimHistory[i]);
		}
	}

	private void SetDayClaimed(int dayIndex, bool claimed, bool playAudio = false)
	{
		if (dayIndex >= Days.Length)
		{
			return;
		}
		GameObject gameObject = Days[dayIndex];
		GameObject gameObject2 = gameObject.transform.Find("Clear").gameObject;
		if (gameObject2 != null)
		{
			gameObject2.SetActive(claimed);
			if (claimed)
			{
				UITweener component = gameObject2.GetComponent<UITweener>();
				if (null != component)
				{
					component.Play(true);
				}
			}
		}
		GameObject gameObject3 = ((!(gameObject.transform.Find("CellContent") != null)) ? null : gameObject.transform.Find("CellContent").gameObject);
		if (gameObject3 != null)
		{
			UITweener[] componentsInChildren = gameObject3.GetComponentsInChildren<UITweener>();
			UITweener[] array = componentsInChildren;
			foreach (UITweener uITweener in array)
			{
				uITweener.Play(claimed);
			}
			Transform transform = gameObject3.transform.Find("StarsKey");
			if (transform != null && transform.gameObject.activeInHierarchy)
			{
				transform.gameObject.SetActive(!claimed);
			}
		}
		if (playAudio && null != ClaimDayClip)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), ClaimDayClip);
		}
	}

	private IEnumerator CalendarUpsell()
	{
		if (upsellInfo != null)
		{
			if (null != CalendarCatchupText)
			{
				string text = KFFLocalization.Get("!!I_7_CALENDARCATCHUP", new string[2] { "<Val1>", "<Val2>" }, new string[2]
				{
					string.Empty + upsellInfo.UnclaimedDays,
					string.Empty + upsellInfo.TotalCost
				});
				CalendarCatchupText.text = text;
			}
			if (null != CalendarCatchupShow)
			{
				CalendarCatchupShow.Play(true);
			}
		}
		yield return null;
	}

	private IEnumerator GiftAndShowAfterUpsell(ScheduleData calendar, int giftIndex)
	{
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(GiftReward(calendar, giftIndex));
		yield return StartCoroutine(ShowReward());
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator GiftAndShowReward()
	{
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(GiftReward());
		yield return StartCoroutine(ShowReward());
	}

	private IEnumerator GiftReward(ScheduleData calendar = null, int giftIndex = -1)
	{
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		if (giftIndex == -1)
		{
			giftIndex = pinfo.GetCurrentCalendarGiftIndex();
		}
		List<CalendarGift> gifts = CalendarGiftDataManager.Instance.GetCalendarGifts(calendar);
		CalendarGift gift = gifts[giftIndex];
		if (null != Card)
		{
			Card.SetActive(false);
			mWasKeyAwarded = false;
			if (gift.GiftType.StartsWith("Card"))
			{
				Card.SetActive(true);
				string cardId = gift.GiftType.Substring("Card".Length);
				cardId = cardId.TrimStart(' ');
				switch (cardId)
				{
				case "normal":
					cardId = GachaManager.Instance.PickColumn("DailyGiftStandard");
					break;
				case "gold":
					cardId = GachaManager.Instance.PickColumn("DailyGiftGold");
					break;
				case "obsidian":
					cardId = GachaManager.Instance.PickColumn("DailyGiftObsidian");
					break;
				case "halloween":
					cardId = GachaManager.Instance.PickColumn("DailyGiftHalloween");
					break;
				}
				CardItem cardItem = new CardItem(CardDataManager.Instance.GetCard(cardId));
				PanelManagerBattle.FillCardInfo(Card, cardItem);
				CWDeckCard script = Card.GetComponent<CWDeckCard>();
				if (null != script)
				{
					script.card = cardItem;
				}
				if (null != GiftRewardText)
				{
					GiftRewardText.text = KFFLocalization.Get(gift.Name);
				}
				pinfo.DeckManager.AddCard(cardItem);
			}
			else if (gift.GiftType.StartsWith("Key"))
			{
				string keyId2 = gift.GiftType.Substring("Key".Length);
				keyId2 = keyId2.TrimStart(' ');
				KeyRingItem key = KeyRingDataManager.Instance.GetKeyRingItem(keyId2);
				if (key != null)
				{
					if (null != GiftRewardText)
					{
						GiftRewardText.text = KFFLocalization.Get(key.Name);
					}
					if (null != IconReward)
					{
						IconReward.spriteName = gift.Icon;
					}
					pinfo.GachaKeys.AddKey(key.Type);
					mWasKeyAwarded = true;
				}
			}
			else
			{
				if (null != GiftRewardText)
				{
					GiftRewardText.text = gift.Quantity + " " + KFFLocalization.Get(gift.Name);
				}
				if (null != IconReward)
				{
					IconReward.spriteName = gift.Icon;
				}
				switch (gift.GiftType)
				{
				case "Hearts":
					pinfo.Stamina += gift.Quantity;
					break;
				case "Gems":
					pinfo.Gems += gift.Quantity;
					break;
				case "Coins":
					pinfo.Coins += gift.Quantity;
					break;
				}
			}
		}
		pinfo.ClaimCalendarGiftDay(giftIndex);
		pinfo.Save();
		if (giftIndex != 0)
		{
			SetDaysClaimed(giftIndex - 1);
		}
		yield return new WaitForSeconds(1f);
		SetDayClaimed(giftIndex, true, true);
	}

	private IEnumerator ShowReward()
	{
		yield return new WaitForSeconds(1.5f);
		if (null != CalendarButtonClose)
		{
			CalendarButtonClose.SetActive(true);
		}
		if (null != CalendarRewardShow)
		{
			if (null != RewardClip)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), RewardClip);
			}
			CalendarRewardShow.Play(true);
			DebugFlagsScript debug = DebugFlagsScript.GetInstance();
			if (null != debug && debug.autoIncrementCalendar)
			{
				DateTime now = TFUtils.ServerTime;
				if (!string.IsNullOrEmpty(debug.specifyCalendarDate))
				{
					try
					{
						now = Convert.ToDateTime(debug.specifyCalendarDate);
					}
					catch (FormatException)
					{
					}
				}
				DateTime tomorrow = now.AddDays(1.0);
				debug.specifyCalendarDate = string.Format("{0}/{1}/{2}", tomorrow.Month, tomorrow.Day, tomorrow.Year);
			}
			if (null != CalendarRewardShow.tweenTarget)
			{
				TweenPosition tweenPos = CalendarRewardShow.tweenTarget.GetComponent<TweenPosition>();
				float delay = ((!(null != tweenPos)) ? 5.5f : (tweenPos.delay + 0.5f));
				yield return new WaitForSeconds(delay);
				OnCloseReward();
			}
		}
		yield return null;
	}
}
