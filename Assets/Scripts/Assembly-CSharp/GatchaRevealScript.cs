using UnityEngine;

public class GatchaRevealScript : MonoBehaviour
{
	private CardItem CardData;

	public bool Reveal;

	public GameObject CardObj;

	public float Rotation;

	public bool Premium;

	public TweenTransform prizeTween;

	public GameObject Flair1;

	public GameObject Flair2;

	public GameObject Flair3;

	public GameObject Flair4;

	public GameObject Flair5;

	public GameObject FlairTweenController;

	public Transform DisplayFlair;

	public Transform HiddenFlair;

	private bool revealDelay;

	private float interval;

	private float counter;

	private UITexture texture;

	public GameObject LeaderCardObj;

	public bool tweenPlayed;

	private void OnEnable()
	{
		Reveal = false;
		Rotation = 0f;
		tweenPlayed = false;
		if (Flair1 != null)
		{
			Flair1.transform.position = HiddenFlair.position;
			NGUITools.SetActive(Flair1, false);
		}
		if (Flair2 != null)
		{
			Flair2.transform.position = HiddenFlair.position;
			NGUITools.SetActive(Flair2, false);
		}
		if (Flair3 != null)
		{
			Flair3.transform.position = HiddenFlair.position;
			NGUITools.SetActive(Flair3, false);
		}
		if (Flair4 != null)
		{
			Flair4.transform.position = HiddenFlair.position;
			NGUITools.SetActive(Flair4, false);
		}
		if (Flair5 != null)
		{
			Flair5.transform.position = HiddenFlair.position;
			NGUITools.SetActive(Flair5, false);
		}
		if (CardObj != null)
		{
			UISprite[] componentsInChildren = CardObj.GetComponentsInChildren<UISprite>(true);
			UISprite[] array = componentsInChildren;
			foreach (UISprite uISprite in array)
			{
				uISprite.enabled = false;
			}
			UILabel[] componentsInChildren2 = CardObj.GetComponentsInChildren<UILabel>(true);
			UILabel[] array2 = componentsInChildren2;
			foreach (UILabel uILabel in array2)
			{
				uILabel.enabled = false;
			}
			TweenScale[] componentsInChildren3 = CardObj.GetComponentsInChildren<TweenScale>(true);
			TweenScale[] array3 = componentsInChildren3;
			foreach (TweenScale tweenScale in array3)
			{
				tweenScale.enabled = false;
			}
			UITexture[] componentsInChildren4 = CardObj.GetComponentsInChildren<UITexture>();
			UITexture[] array4 = componentsInChildren4;
			foreach (UITexture uITexture in array4)
			{
				uITexture.enabled = false;
			}
		}
		if (LeaderCardObj != null)
		{
			UISprite[] componentsInChildren5 = LeaderCardObj.GetComponentsInChildren<UISprite>(true);
			UISprite[] array5 = componentsInChildren5;
			foreach (UISprite uISprite2 in array5)
			{
				uISprite2.enabled = false;
			}
			UILabel[] componentsInChildren6 = LeaderCardObj.GetComponentsInChildren<UILabel>(true);
			UILabel[] array6 = componentsInChildren6;
			foreach (UILabel uILabel2 in array6)
			{
				uILabel2.enabled = false;
			}
			TweenScale[] componentsInChildren7 = LeaderCardObj.GetComponentsInChildren<TweenScale>(true);
			TweenScale[] array7 = componentsInChildren7;
			foreach (TweenScale tweenScale2 in array7)
			{
				tweenScale2.enabled = false;
			}
			UITexture[] componentsInChildren8 = LeaderCardObj.GetComponentsInChildren<UITexture>();
			UITexture[] array8 = componentsInChildren8;
			foreach (UITexture uITexture2 in array8)
			{
				uITexture2.enabled = false;
			}
		}
	}

	public void RevealCardAfterDelay(float delay)
	{
		counter = 0f;
		interval = delay;
		revealDelay = true;
	}

	public void RevealCard()
	{
		if (Reveal || tweenPlayed)
		{
			return;
		}
		Reveal = true;
		CWGachaController instance = CWGachaController.GetInstance();
		PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
		instance.canChooseChest = true;
		string text = instance.cardID;
		if (instance.OpenPremiumChestForFree)
		{
			instance.OpenPremiumChestForFree = false;
			text = "Building_ComfyCave";
		}
		if (instance.OpenPremiumChestWithKey)
		{
			instance.OpenPremiumChestWithKey = false;
		}
		CardForm card = CardDataManager.Instance.GetCard(text);
		CardData = new CardItem(card);
		if (SQUtils.StartsWith(text, "leader_"))
		{
			LeaderItem leaderItem = LeaderManager.Instance.AddNewLeader(text);
			if (leaderItem != null)
			{
				UpdateLeader(leaderItem);
			}
		}
		else
		{
			instance2.DeckManager.AddCard(CardData);
			UpdateCard();
		}
		TweenScale[] componentsInChildren = CardObj.GetComponentsInChildren<TweenScale>(true);
		TweenScale[] array = componentsInChildren;
		foreach (TweenScale tweenScale in array)
		{
			tweenScale.enabled = true;
		}
		componentsInChildren = LeaderCardObj.GetComponentsInChildren<TweenScale>(true);
		TweenScale[] array2 = componentsInChildren;
		foreach (TweenScale tweenScale2 in array2)
		{
			tweenScale2.enabled = true;
		}
		NGUITools.SetActive(prizeTween.gameObject, true);
		prizeTween.Play(true);
		prizeTween.Reset();
		tweenPlayed = true;
		TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.GatchaTutorialSave);
		PlayerInfoScript.GetInstance().Save();
	}

	private void OnClick()
	{
	}

	public void UpdateLeader(LeaderItem leader)
	{
		if (LeaderCardObj != null)
		{
			CWDeckHeroSelection.FillCardInfo(LeaderCardObj, leader);
		}
	}

	public void UpdateCard()
	{
		if (FlairTweenController != null)
		{
			UIButtonTween component = FlairTweenController.GetComponent<UIButtonTween>();
			if (component != null)
			{
				switch (CardData.Form.Rarity)
				{
				case 1:
					component.tweenTarget = Flair1;
					break;
				case 2:
					component.tweenTarget = Flair2;
					break;
				case 3:
					component.tweenTarget = Flair3;
					break;
				case 4:
					component.tweenTarget = Flair4;
					break;
				case 5:
					component.tweenTarget = Flair5;
					break;
				}
			}
		}
		if (CardObj != null)
		{
			FillCardInfo(CardObj, CardData);
		}
	}

	private void Update()
	{
		if (revealDelay)
		{
			counter += Time.deltaTime;
			if (counter > interval)
			{
				revealDelay = false;
				RevealCard();
			}
		}
	}

	public void FillCardInfo(GameObject obj, CardItem card)
	{
		UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
		FactionData factionData = FactionManager.Instance.GetFactionData(card.Form.Faction);
		Color factionColor = factionData.FactionColor;
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
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
				texture = uITexture;
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

	public void ShowBlankCard(GameObject obj, CardItem card)
	{
		UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
			case "Card_Art":
				uISprite.enabled = false;
				break;
			case "Card_Frame":
				uISprite.spriteName = "CardBack";
				break;
			case "Card_Frame_Combat":
				uISprite.enabled = false;
				break;
			case "Icon_ATK":
			case "Icon_DEF":
				uISprite.enabled = false;
				break;
			case "Frame_ATK":
			case "Frame_DEF":
				uISprite.enabled = false;
				break;
			case "Icon_FACT":
				uISprite.enabled = false;
				break;
			case "Icon_Magic":
				uISprite.enabled = false;
				break;
			case "BG_Name":
			case "BG_Type":
				uISprite.enabled = false;
				break;
			case "star1":
				uISprite.enabled = false;
				break;
			case "star2":
				uISprite.enabled = false;
				break;
			case "star3":
				uISprite.enabled = false;
				break;
			case "star4":
				uISprite.enabled = false;
				break;
			case "star5":
				uISprite.enabled = false;
				break;
			case "ATK_BG":
				uISprite.enabled = false;
				break;
			case "HPBarFront":
				uISprite.enabled = false;
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
				uILabel.text = string.Empty;
				break;
			case "Cost_Label":
				uILabel.text = string.Empty;
				break;
			case "DEF_Label":
				uILabel.text = string.Empty;
				break;
			case "Floop_Label":
				uILabel.text = string.Empty;
				break;
			case "Desc_Label":
				uILabel.text = string.Empty;
				break;
			case "Name_Label":
				uILabel.text = string.Empty;
				break;
			case "Faction_Label":
				uILabel.text = string.Empty;
				break;
			case "Type_Label":
				uILabel.text = string.Empty;
				break;
			case "LevelNum_Label":
				uILabel.text = string.Empty;
				break;
			case "DEF_sc_Label":
				uILabel.text = string.Empty;
				break;
			case "HP_sc_Label":
				uILabel.text = string.Empty;
				break;
			case "ATK_sc_Label":
				uILabel.text = string.Empty;
				break;
			case "slash":
				uILabel.text = string.Empty;
				break;
			case "Floop_Cost_Label":
				uILabel.text = string.Empty;
				break;
			}
		}
		UITexture[] componentsInChildren3 = obj.GetComponentsInChildren<UITexture>();
		UITexture[] array3 = componentsInChildren3;
		foreach (UITexture uITexture in array3)
		{
			uITexture.enabled = false;
		}
	}

	private void OnDisable()
	{
		if (texture != null)
		{
			Texture mainTexture = texture.mainTexture;
			texture.mainTexture = null;
			Resources.UnloadAsset(mainTexture);
		}
	}
}
