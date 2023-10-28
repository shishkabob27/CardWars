using UnityEngine;

public class PanelManagerDeck : MonoBehaviour
{
	public Camera uiCamera;

	public GameObject activeCard;

	public GameObject blackPanel;

	public GameObject zoomCard;

	public GameObject zoomLeaderCard;

	public GameObject defaultPage;

	public GameObject backToMenuTweenTarget;

	private static PanelManagerDeck g_panelManager;

	private void Awake()
	{
		g_panelManager = this;
	}

	private void Start()
	{
		if (MenuController.GetInstance() == null)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (!instance.isInitialized)
			{
				instance.Login();
			}
		}
		DeckManagerSceneSetup();
	}

	private void DeckManagerSceneSetup()
	{
		CameraManager.DeactivateCamera(uiCamera);
	}

	public static PanelManagerDeck GetInstance()
	{
		return g_panelManager;
	}

	public void FillCardInfo(GameObject obj, CardForm form, bool showSortInfo = false)
	{
		CardItem card = new CardItem(form);
		FillCardInfo(obj, card, showSortInfo);
	}

	public void FillCardInfo(GameObject obj, CardItem card, bool showSortInfo = false)
	{
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
			case "BG_Name":
			case "BG_Type":
			{
				Color color2 = Color.Lerp(factionColor, Color.white, 0.5f);
				uISprite.color = color2;
				break;
			}
			case "Icon_ATK":
			case "Icon_DEF":
				uISprite.enabled = ((card.Form.Type == CardType.Creature) ? true : false);
				break;
			case "Icon_FACT":
				uISprite.spriteName = factionData.LandscapeIcon;
				uISprite.enabled = uISprite.spriteName.Length > 0;
				break;
			case "Frame_ATK":
			case "Frame_DEF":
			{
				Color color = Color.Lerp(factionColor, Color.white, 0.5f);
				uISprite.color = color;
				uISprite.enabled = ((card.Form.Type == CardType.Creature) ? true : false);
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
			}
		}
		CWDeckCard.ShowNewFlag(obj, card.IsNew);
		UILabel[] componentsInChildren2 = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			switch (uILabel.name)
			{
			case "ATK_Label":
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.ATK.ToString());
				break;
			case "Cost_Label":
				uILabel.text = card.Form.Cost.ToString();
				break;
			case "DEF_Label":
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.DEF.ToString());
				break;
			case "Floop_Label":
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.Form.CostDescription);
				break;
			case "Desc_Label":
				uILabel.text = card.Description;
				break;
			case "Name_Label":
				uILabel.text = card.Form.Name;
				break;
			case "Type_Label":
				uILabel.text = KFFLocalization.Get("!!" + card.Form.Type.ToString().ToUpper());
				break;
			case "BonusATK_Label":
				uILabel.text = (card.ATK - card.Form.BaseATK).ToString();
				break;
			case "BonusDEF_Label":
				uILabel.text = (card.DEF - card.Form.BaseDEF).ToString();
				break;
			case "LevelNum_Label":
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
				Texture2D mainTexture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Textures/CardArt/" + text + card.Form.SpriteName) as Texture2D;
				uITexture.mainTexture = mainTexture;
				break;
			}
			case "Card_Art_Glimmer":
			case "Card_Glimmer":
				if (card != null && card.Form != null && card.Form.HasGlimmer())
				{
					uITexture.gameObject.SetActive(true);
				}
				else
				{
					uITexture.gameObject.SetActive(false);
				}
				break;
			}
		}
		if (!showSortInfo)
		{
			return;
		}
		Transform transform = obj.transform.Find("Panel/Labels/Sort_Label");
		if (!(transform != null))
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		SortType primarySort = PlayerDeckManager.GetPrimarySort();
		GameObject go = obj.transform.Find("Panel/Labels/Desc_Label").gameObject;
		if (primarySort != 0)
		{
			NGUITools.SetActive(go, false);
			NGUITools.SetActive(gameObject, true);
			string value = string.Empty;
			switch (primarySort)
			{
			case SortType.ATK:
				value = "ATK " + card.ATK;
				break;
			case SortType.DEF:
				value = "DEF " + card.DEF;
				break;
			case SortType.RARE:
				value = "Rarity " + card.Form.Rarity;
				break;
			case SortType.TYPE:
				value = GetCardTypeName(card.Form.Type);
				break;
			case SortType.FACT:
				value = GetFactionName(card.Form.Faction);
				break;
			case SortType.MP:
				value = "MP " + card.Form.Cost;
				break;
			}
			SQUtils.SetLabel(gameObject, value);
		}
		else
		{
			NGUITools.SetActive(go, true);
			NGUITools.SetActive(gameObject, false);
		}
	}

	private string GetFactionName(Faction t)
	{
		string result = t.ToString();
		switch (t)
		{
		case Faction.Corn:
			result = KFFLocalization.Get("!!FACTION_CORN");
			break;
		case Faction.Cotton:
			result = KFFLocalization.Get("!!FACTION_COTTON");
			break;
		case Faction.Plains:
			result = KFFLocalization.Get("!!FACTION_PLAINS");
			break;
		case Faction.Sand:
			result = KFFLocalization.Get("!!FACTION_SAND");
			break;
		case Faction.Swamp:
			result = KFFLocalization.Get("!!FACTION_SWAMP");
			break;
		case Faction.Universal:
			result = KFFLocalization.Get("!!FACTION_UNIVERSAL");
			break;
		}
		return result;
	}

	private string GetCardTypeName(CardType t)
	{
		string result = t.ToString();
		switch (t)
		{
		case CardType.Creature:
			result = KFFLocalization.Get("!!CREATURE");
			break;
		case CardType.Building:
			result = KFFLocalization.Get("!!BUILDING");
			break;
		case CardType.Spell:
			result = KFFLocalization.Get("!!SPELL");
			break;
		}
		return result;
	}
}
