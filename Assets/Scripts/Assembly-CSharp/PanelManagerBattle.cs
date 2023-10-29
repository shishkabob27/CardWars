using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManagerBattle : MonoBehaviour
{
	private GameState GameInstance;

	private BattlePhaseManager phaseMgr;

	public Camera uiCamera;

	public GameObject newCamera;

	public GameObject newCameraTarget;

	public GameObject hexGrid;

	public GameObject debugWindow;

	public GameObject zoomCard;

	public GameObject tweenP1Damage;

	public GameObject tweenP2Damage;

	public CWCreatureStatsFloorDisplay[] P1FloorDisplays;

	public CWCreatureStatsFloorDisplay[] P2FloorDisplays;

	public GameObject blackPanel;

	public GameObject[] lootObjects;

	public GameObject coinLootObject;

	public GameObject[] tombstoneObjs;

	public GameObject flyingCard;

	public GameObject flyingCardDestination;

	public CWDisableLaneCollider battleLaneColController;

	public GameObject floopPanelCameraTarget;

	public GameObject floopPanelCameraLookAtTarget;

	public GameObject currentCardObj;

	public UILabel QuestStatus;

	public UILabel QuestInfo;

	public UILabel QuestCondition;

	public List<UISlicedSprite> Stars = new List<UISlicedSprite>();

	public bool hpBarOnTheGround;

	private static PanelManagerBattle g_panelManager;

	private void Awake()
	{
		g_panelManager = this;
	}

	public static PanelManagerBattle GetInstance()
	{
		return g_panelManager;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public static void FillCardInfo(GameObject obj, CardItem card, int bonusATK, int bonusDEF)
	{
		UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			switch (uILabel.name)
			{
			case "BonusATK_Label":
				uILabel.text = ((bonusATK != 0) ? bonusATK.ToString() : string.Empty);
				break;
			case "BonusDEF_Label":
				uILabel.text = ((bonusDEF != 0) ? bonusDEF.ToString() : string.Empty);
				break;
			}
		}
		FillCardInfo(obj, card, PlayerType.User);
	}

	public static void FillCardInfo(GameObject obj, CardScript script, PlayerType player)
	{
		UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "Cost_Label")
			{
				uILabel.color = new Color(83f / 85f, 0.83137256f, 1f);
				uILabel.text = script.Data.Form.DetermineCost(player).ToString();
				break;
			}
			if (uILabel.name == "Floop_Cost_Label")
			{
				uILabel.text = ((script.Data.Form.Type != 0) ? string.Empty : script.DetermineFloopCost().ToString());
			}
		}
		FillCardInfo(obj, script.Data);
	}

	public static void FillCardInfo(GameObject obj, CardItem card, PlayerType player)
	{
		UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "Cost_Label")
			{
				uILabel.color = new Color(83f / 85f, 0.83137256f, 1f);
				uILabel.text = card.Form.DetermineCost(player).ToString();
				break;
			}
			if (uILabel.name == "Floop_Cost_Label")
			{
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.Form.FloopCost.ToString());
			}
		}
		FillCardInfo(obj, card);
	}

	public static void FillCardInfo(GameObject obj, CardItem card, FloopActionType actionType)
	{
		UILabel[] componentsInChildren = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "Footer_Label")
			{
				switch (actionType)
				{
				case FloopActionType.Floop:
					uILabel.text = KFFLocalization.Get("!!BS_Q_3_FLOOPABILITY");
					break;
				case FloopActionType.Building:
					uILabel.text = KFFLocalization.Get("!!BS_Q_6_BUILDINGABILITY");
					break;
				case FloopActionType.Spell:
					uILabel.text = KFFLocalization.Get("!!BS_Q_7_SPELLABILITY");
					break;
				case FloopActionType.Hero:
					uILabel.text = KFFLocalization.Get("!!BS_Q_9_HEROABILITY");
					break;
				}
				break;
			}
		}
		FillCardInfo(obj, card);
	}

	public static void FillCardInfo(GameObject obj, CardItem card)
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
				uISprite.enabled = true;
				SQUtils.SetIcon(uISprite.gameObject, card.Form.IconAtlas, card.Form.SpriteName, Color.white);
				break;
			case "Card_Frame_Gold":
				uISprite.color = Color.white;
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
				uISprite.color = Color.white;
				uISprite.enabled = ((card.Form.Type == CardType.Creature) ? true : false);
				break;
			case "Icon_Magic":
				uISprite.color = Color.white;
				uISprite.enabled = true;
				break;
			case "Icon_FACT":
				uISprite.color = Color.white;
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
				uISprite.color = Color.white;
				break;
			case "star2":
				uISprite.enabled = card.Form.Rarity >= 2;
				uISprite.color = Color.white;
				break;
			case "star3":
				uISprite.enabled = card.Form.Rarity >= 3;
				uISprite.color = Color.white;
				break;
			case "star4":
				uISprite.enabled = card.Form.Rarity >= 4;
				uISprite.color = Color.white;
				break;
			case "star5":
				uISprite.enabled = card.Form.Rarity >= 5;
				uISprite.color = Color.white;
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
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.ATK.ToString());
				break;
			case "Floop_Desc_Label":
				uILabel.text = ((card.Form.Type != 0) ? string.Empty : card.Form.RawDescription.ToString());
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
			case "Faction_Label":
				uILabel.text = card.Form.Faction.ToString();
				break;
			case "Type_Label":
				uILabel.text = KFFLocalization.Get("!!" + card.Form.Type.ToString().ToUpper());
				break;
			case "LevelNum_Label":
				uILabel.text = card.Level.ToString();
				break;
			case "Cost_Label":
				uILabel.text = card.Form.Cost.ToString();
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
			case "Card_Art_Zoomed":
			{
				Texture2D mainTexture2 = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Textures/CardArtZoomed/" + text + card.Form.SpriteName) as Texture2D;
				uITexture.mainTexture = mainTexture2;
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
	}

	public void HPBarUpdate(GameObject obj, PlayerType player, int lane)
	{
		CreatureScript creature = GameInstance.GetCreature(player, lane);
		if (creature == null)
		{
			return;
		}
		UIFilledSprite[] componentsInChildren = obj.GetComponentsInChildren<UIFilledSprite>(true);
		UIFilledSprite[] array = componentsInChildren;
		foreach (UIFilledSprite uIFilledSprite in array)
		{
			if (uIFilledSprite.name == "CreatureHPBarFront")
			{
				uIFilledSprite.fillAmount = creature.GetHealthPct();
			}
		}
		UILabel[] componentsInChildren2 = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			switch (uILabel.name)
			{
			case "DEF_sc_Label":
				uILabel.text = ((creature.DEF != 0) ? creature.DEF.ToString() : string.Empty);
				break;
			case "HP_sc_Label":
				uILabel.text = creature.Health.ToString();
				break;
			case "ATK_sc_Label":
				uILabel.text = ((creature.ATK != 0) ? creature.ATK.ToString() : string.Empty);
				break;
			}
		}
	}

	public IEnumerator PlaySpellFx(CardItem card, PlayerType player)
	{
		if (phaseMgr.Phase != BattlePhase.P1SetupActionSpell && phaseMgr.Phase != BattlePhase.P2SetupActionSpell)
		{
			phaseMgr.Phase = ((player != PlayerType.User) ? BattlePhase.P2SetupActionSpell : BattlePhase.P1SetupActionSpell);
		}
		yield return new WaitForSeconds(0.5f);
		//Object particleData = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Particles/" + ((SpellCard)card.Form).ParticleName);
		GameObject particleData = Resources.Load("Particles/" + ((SpellCard)card.Form).ParticleName, typeof(GameObject)) as GameObject;
		Transform parentTr = CWFloopActionManager.GetInstance().spawnFXCameraCenter.transform;
		if (particleData != null)
		{
			GameObject particleObj = Instantiate(particleData, parentTr.position, parentTr.rotation) as GameObject;
			particleObj.transform.parent = parentTr;
		}
		yield return new WaitForSeconds(1.5f);
		GameInstance.CastSpell(player, card);
		CardManagerScript.GetInstance().CardSelected = false;
	}

	public void QuestStatusUpdate()
	{
		string text = null;
		if (GameState.Instance.BattleResolver != null)
		{
			text = GameState.Instance.BattleResolver.questConditionId;
		}
		else
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			QuestData currentQuest = instance.GetCurrentQuest();
			int questProgress = instance.GetQuestProgress(currentQuest);
			text = ((questProgress >= currentQuest.Condition.Length) ? null : currentQuest.Condition[questProgress]);
		}
		string status;
		string passFail;
		string condition;
		Color fontColor;
		QuestConditionManager.Instance.QuestConditionToStatus(text, out status, out passFail, out condition, out fontColor);
		QuestCondition.text = condition;
		QuestStatus.text = status;
		QuestInfo.text = passFail;
		QuestInfo.color = fontColor;
	}

	public void QuestStatusInit()
	{
		QuestStatusUpdate();
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		int num = Mathf.Max(0, (GameState.Instance.BattleResolver == null) ? instance.GetQuestProgress(instance.GetCurrentQuest()) : GameState.Instance.BattleResolver.questStars);
		for (int num2 = 2; num2 >= num; num2--)
		{
			Stars[num2].spriteName = "UI_Star_Empty";
		}
	}
}
