using System;
using UnityEngine;

public class DailyGiftController : MonoBehaviour
{
	public GameObject[] Wheel;

	public UIButtonTween ShowGiftTween;

	public UIButtonTween AreYouSureTween;

	public UIButtonTween NotEnoughGemsTween;

	public GameObject RefreshTimeBox;

	public UILabel RefreshTime;

	public UILabel GiftLabel;

	public UISprite GiftIcon;

	public UILabel ButtonText;

	public UILabel RetryCost;

	public AudioClip WheelClip;

	public AudioClip RewardClip;

	public float WheelClipPitchRange;

	public float startSpeed = 0.05f;

	public float endSpeed = 0.5f;

	public float speedDuration = 9f;

	public int numCells = 12;

	public GameObject CardObj;

	public GameObject ActivateButton;

	public static int refreshDurationSec = 86400;

	private WheelState State = WheelState.Ready;

	private int CurrentCell;

	private float timer;

	private float currentSpeed;

	private int dailyGiftID;

	private DailyGift dailyGift;

	private int numFreeGifts;

	private int secondsLeft;

	private AudioSource WheelAudioSource;

	private float AudioPitch;

	private string ObsidianCardID;

	private string GoldCardID;

	private string StandardCardID;

	private string ActivateButtonString;

	private bool MetricsGemsSpent;

	private void Start()
	{
		numFreeGifts = InitNumFreeGifts();
		Reset();
		WheelAudioSource = GetComponent<AudioSource>();
		AudioPitch = WheelAudioSource.pitch;
		ActivateButtonString = KFFLocalization.Get(ButtonText.text);
	}

	private void Update()
	{
		if (null != ActivateButton)
		{
			ActivateButton.GetComponent<Collider>().enabled = State == WheelState.Ready || State == WheelState.Inactive;
		}
		switch (State)
		{
		case WheelState.Active:
		{
			timer -= Time.deltaTime;
			float num = Time.deltaTime * (endSpeed - startSpeed) / speedDuration;
			currentSpeed += num;
			if (currentSpeed > endSpeed)
			{
				currentSpeed = endSpeed;
			}
			if (timer <= 0f)
			{
				if (CurrentCell > -1)
				{
					Wheel[CurrentCell].transform.Find("Highlight").gameObject.SetActive(false);
				}
				CurrentCell = (CurrentCell + 1) % numCells;
				Wheel[CurrentCell].transform.Find("Highlight").gameObject.SetActive(true);
				if ((bool)WheelAudioSource && (bool)WheelClip)
				{
					WheelAudioSource.pitch = AudioPitch + UnityEngine.Random.Range(0f - WheelClipPitchRange, WheelClipPitchRange);
					SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(WheelAudioSource, WheelClip);
				}
				timer = currentSpeed;
			}
			if (CurrentCell == dailyGiftID && currentSpeed == endSpeed)
			{
				Invoke("FinishWheel", 0.5f);
				State = WheelState.Finished;
			}
			break;
		}
		}
		if ((bool)ButtonText)
		{
			ButtonText.text = ActivateButtonString.Replace("<Val1>", numFreeGifts.ToString());
		}
		if ((bool)RetryCost)
		{
			RetryCost.text = RetryCost.text.Replace("<Val1>", DailyGiftDataManager.Instance.RetyGemCost.ToString());
		}
		UpdateTime();
	}

	private void UpdateTime()
	{
		DateTime lastGiftTimestamp = PlayerInfoScript.GetInstance().LastGiftTimestamp;
		int num = (int)(TFUtils.ServerTime - lastGiftTimestamp).TotalSeconds;
		int num2 = refreshDurationSec - num;
		if (secondsLeft > 0 && num2 <= 0)
		{
			numFreeGifts = 1;
		}
		secondsLeft = num2;
		if (num2 < 0)
		{
			RefreshTimeBox.SetActive(false);
			return;
		}
		TimeSpan timeSpan = new TimeSpan((long)num2 * 10000000L);
		if (RefreshTime != null)
		{
			RefreshTime.text = timeSpan.ToString();
		}
		RefreshTimeBox.SetActive(true);
	}

	public static int TimeToNextDailyGift()
	{
		DateTime lastGiftTimestamp = PlayerInfoScript.GetInstance().LastGiftTimestamp;
		int num = (int)(TFUtils.ServerTime - lastGiftTimestamp).TotalSeconds;
		int val = refreshDurationSec - num;
		return Math.Max(0, val);
	}

	private int InitNumFreeGifts()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		DateTime lastGiftTimestamp = instance.LastGiftTimestamp;
		if (lastGiftTimestamp != DateTime.MinValue)
		{
			return ((int)(TFUtils.ServerTime - lastGiftTimestamp).TotalSeconds / refreshDurationSec > 0) ? 1 : 0;
		}
		instance.DailyGiftTimestamp = TFUtils.ServerTime;
		instance.NumUsedFreeGifts = -1;
		instance.Save();
		return 1;
	}

	private void Reset()
	{
		State = WheelState.Ready;
		CurrentCell = -1;
		currentSpeed = startSpeed;
		timer = currentSpeed;
		GachaManager instance = GachaManager.Instance;
		ObsidianCardID = instance.PickDailyGift(Quality.Obsidian);
		GoldCardID = instance.PickDailyGift(Quality.Gold);
		StandardCardID = instance.PickDailyGift(Quality.Standard);
		int num = 0;
		GameObject[] wheel = Wheel;
		foreach (GameObject gameObject in wheel)
		{
			gameObject.transform.Find("Highlight").gameObject.SetActive(false);
			UISprite component = gameObject.transform.Find("Icon").GetComponent<UISprite>();
			UILabel component2 = gameObject.transform.Find("Label_Item_Val").GetComponent<UILabel>();
			DailyGift gift = DailyGiftDataManager.Instance.GetGift(num);
			component.spriteName = gift.Icon;
			component2.text = gift.Quantity.ToString();
			num++;
		}
		PickGift();
	}

	private void FinishWheel()
	{
		if ((bool)CardObj)
		{
			if (dailyGift.Type.StartsWith("Card"))
			{
				CardObj.SetActive(true);
			}
			else
			{
				CardObj.SetActive(false);
			}
		}
		if (GiftLabel != null)
		{
			GiftLabel.text = dailyGift.Quantity + " " + KFFLocalization.Get(dailyGift.Name);
		}
		if (GiftIcon != null)
		{
			GiftIcon.spriteName = dailyGift.Icon;
		}
		if (ShowGiftTween != null)
		{
			if ((bool)WheelAudioSource && (bool)RewardClip)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(WheelAudioSource, RewardClip);
			}
			ShowGiftTween.Play(true);
		}
		GrantGift();
		if (CurrentCell > -1)
		{
			Wheel[CurrentCell].transform.Find("Highlight").gameObject.SetActive(false);
		}
		State = WheelState.Inactive;
	}

	public void FillCardInfo(GameObject obj, CardItem card)
	{
		obj.SetActive(true);
		UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
		FactionData factionData = FactionManager.Instance.GetFactionData(card.Form.Faction);
		Color factionColor = factionData.FactionColor;
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
			case "Card_Art":
				SQUtils.SetIcon(uISprite.gameObject, card.Form.IconAtlas, card.Form.SpriteName, Color.white);
				break;
			case "Card_Frame_Gold":
				uISprite.spriteName = card.Form.GetCardQualityFrameSprite();
				uISprite.enabled = uISprite.spriteName.Length > 0;
				break;
			case "Card_Frame":
				uISprite.enabled = true;
				SQUtils.SetIcon(uISprite.gameObject, factionData.CardFrameAtlas, factionData.CardFrameSprite, factionData.CardFrameColor);
				break;
			case "Icon_ATK":
			case "Icon_DEF":
				uISprite.enabled = ((card.Form.Type == CardType.Creature) ? true : false);
				break;
			case "Frame_ATK":
			case "Frame_DEF":
			{
				Color color2 = Color.Lerp(factionColor, Color.white, 0.5f);
				uISprite.color = color2;
				uISprite.enabled = ((card.Form.Type == CardType.Creature) ? true : false);
				break;
			}
			case "Icon_FACT":
				uISprite.spriteName = factionData.LandscapeIcon;
				uISprite.enabled = uISprite.spriteName.Length > 0;
				break;
			case "Icon_Magic":
				uISprite.enabled = true;
				break;
			case "BG_Name":
			case "BG_Type":
			{
				uISprite.enabled = true;
				Color color = Color.Lerp(factionColor, Color.white, 0.5f);
				uISprite.color = color;
				break;
			}
			case "star1":
				uISprite.enabled = card.Form.Rarity >= 1;
				break;
			case "star2":
				uISprite.enabled = card.Form.Rarity >= 2;
				break;
			case "star3":
				uISprite.enabled = card.Form.Rarity >= 3;
				break;
			case "star4":
				uISprite.enabled = card.Form.Rarity >= 4;
				break;
			case "star5":
				uISprite.enabled = card.Form.Rarity >= 5;
				break;
			case "ATK_BG":
				uISprite.enabled = card.Form.Type == CardType.Creature;
				break;
			case "HPBarFront":
				uISprite.enabled = card.Form.Type == CardType.Creature;
				break;
			}
		}
		UILabel[] componentsInChildren2 = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			switch (uILabel.name)
			{
			case "ATK_Label":
				uILabel.enabled = true;
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.ATK.ToString());
				break;
			case "Cost_Label":
				uILabel.color = new Color(0.9098039f, 0.6156863f, 0.99607843f);
				uILabel.enabled = true;
				uILabel.text = card.Form.Cost.ToString();
				break;
			case "Floop_Cost_Label":
				uILabel.enabled = true;
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.Form.FloopCost.ToString());
				break;
			case "Floop_Desc_Label":
				uILabel.enabled = true;
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.Form.RawDescription.ToString());
				break;
			case "DEF_Label":
				uILabel.enabled = true;
				uILabel.text = ((card.DEF != 0) ? card.DEF.ToString() : string.Empty);
				break;
			case "Floop_Label":
				uILabel.enabled = true;
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.Form.CostDescription);
				break;
			case "Desc_Label":
				uILabel.enabled = true;
				uILabel.text = card.Description;
				break;
			case "Name_Label":
				uILabel.enabled = true;
				uILabel.text = card.Form.Name;
				break;
			case "Faction_Label":
				uILabel.enabled = true;
				uILabel.text = card.Form.Faction.ToString();
				break;
			case "Type_Label":
				uILabel.enabled = true;
				uILabel.text = KFFLocalization.Get("!!" + card.Form.Type.ToString().ToUpper());
				break;
			case "LevelNum_Label":
				uILabel.enabled = true;
				uILabel.text = card.Level.ToString();
				break;
			}
		}
		UITexture[] componentsInChildren3 = obj.GetComponentsInChildren<UITexture>(true);
		string text = ((card.Form.Type == CardType.Creature) ? ("Creatures/" + card.Form.Faction.ToString() + "/") : ((card.Form.Type == CardType.Building) ? "Buildings/" : ((card.Form.Type != CardType.Spell) ? string.Empty : "Spells/")));
		UITexture[] array3 = componentsInChildren3;
		foreach (UITexture uITexture in array3)
		{
			switch (uITexture.name)
			{
			case "Card_Art":
			{
				uITexture.enabled = true;
				Texture2D mainTexture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Textures/CardArt/" + text + card.Form.SpriteName) as Texture2D;
				uITexture.mainTexture = mainTexture;
				break;
			}
			case "Card_Art_Glimmer":
			case "Card_Glimmer":
				if (card != null && card.Form != null && card.Form.HasGlimmer())
				{
					uITexture.enabled = true;
					uITexture.gameObject.SetActive(true);
				}
				else
				{
					uITexture.gameObject.SetActive(false);
				}
				break;
			}
		}
	}

	private void PickGift()
	{
		dailyGiftID = DailyGiftDataManager.Instance.PickGift();
		dailyGift = DailyGiftDataManager.Instance.GetGift(dailyGiftID);
	}

	private void GrantGift()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		CWUpdatePlayerStats instance2 = CWUpdatePlayerStats.GetInstance();
		if (instance2 != null)
		{
			instance2.holdUpdateFlag = false;
		}
		CardObj.SetActive(false);
		switch (dailyGift.Type)
		{
		case "Coins":
			instance.Coins += dailyGift.Quantity;
			break;
		case "Gems":
			instance.Gems += dailyGift.Quantity;
			break;
		case "Trophies":
			instance.TotalTrophies += dailyGift.Quantity;
			break;
		case "Hearts":
			instance.Stamina += dailyGift.Quantity;
			break;
		case "Card normal":
		{
			CardItem card = new CardItem(CardDataManager.Instance.GetCard(StandardCardID));
			FillCardInfo(CardObj, card);
			instance.DeckManager.AddCard(card);
			break;
		}
		case "Card gold":
		{
			CardItem card = new CardItem(CardDataManager.Instance.GetCard(GoldCardID));
			FillCardInfo(CardObj, card);
			instance.DeckManager.AddCard(card);
			break;
		}
		case "Card obsidian":
		{
			CardItem card = new CardItem(CardDataManager.Instance.GetCard(ObsidianCardID));
			FillCardInfo(CardObj, card);
			instance.DeckManager.AddCard(card);
			break;
		}
		}
		instance.Save();
		if (MetricsGemsSpent)
		{
			MetricsGemsSpent = false;
			Singleton<AnalyticsManager>.Instance.LogExtraDailySpinPurchase(DailyGiftDataManager.Instance.RetyGemCost);
		}
	}

	private void OnConfirm()
	{
		if (State != 0)
		{
			if (numFreeGifts > 0)
			{
				OnActivate();
			}
			else if (AreYouSureTween != null)
			{
				AreYouSureTween.Play(true);
			}
		}
	}

	public void CheckGems()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.Gems - DailyGiftDataManager.Instance.RetyGemCost < 0)
		{
			if (NotEnoughGemsTween != null)
			{
				NotEnoughGemsTween.Play(true);
			}
		}
		else
		{
			OnActivate();
		}
	}

	private void OnActivate()
	{
		Reset();
		State = WheelState.Active;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (numFreeGifts > 0)
		{
			numFreeGifts--;
			instance.LastGiftTimestamp = TFUtils.ServerTime;
			instance.NumUsedFreeGifts++;
		}
		else
		{
			instance.Gems -= DailyGiftDataManager.Instance.RetyGemCost;
			MetricsGemsSpent = true;
		}
	}
}
