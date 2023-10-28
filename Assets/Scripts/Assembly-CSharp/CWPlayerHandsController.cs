using System.Collections.Generic;
using UnityEngine;

public class CWPlayerHandsController : MonoBehaviour
{
	public BoxCollider[] laneColliders;

	public BoxCollider[] oppLaneColliders;

	public GameObject[] playerHands;

	public GameObject replaceWindow;

	public GameObject tweenZoom;

	private PanelManagerBattle panelMgrBattle;

	private GameState GameInstance;

	private CardManagerScript cardMgr;

	public GameObject spellParticleEffect;

	public AudioClip errorSound;

	public AudioClip cardMoveSound;

	public AudioClip cardZoomSound;

	public AudioClip cardUnzoomSound;

	public AudioClip cardPickSound;

	public AudioClip cardDragSound;

	public AudioClip[] cardPlaceSounds;

	public AudioClip spellSound;

	public AudioClip cardDrawSound;

	public CardItem currentCard;

	public bool spinStart;

	public GameObject replaceTweenTarget;

	public int lane;

	public CardItem card;

	public string cardName;

	public GameObject endOfSpinningCardFX;

	public Vector3 endOfSpinningCardFXOffset;

	private BattlePhaseManager phaseMgr;

	private CWFloopActionManager floopMgr;

	private static CWPlayerHandsController g_handsController;

	private List<UITexture> DynamicHiRezTextures = new List<UITexture>();

	private float spinSpeed;

	private float prevSpinSpeed;

	private void Awake()
	{
		g_handsController = this;
	}

	public static CWPlayerHandsController GetInstance()
	{
		return g_handsController;
	}

	private void Start()
	{
		cardMgr = CardManagerScript.GetInstance();
		GameInstance = GameState.Instance;
		panelMgrBattle = PanelManagerBattle.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public void HandStart()
	{
		UpdateCards(GameInstance.GetHand(PlayerType.User));
	}

	public void RegisterDynamicTexture(UITexture aTexture)
	{
		if (!DynamicHiRezTextures.Contains(aTexture))
		{
			DynamicHiRezTextures.Add(aTexture);
		}
	}

	public void FlushDynamicTextures()
	{
		foreach (UITexture dynamicHiRezTexture in DynamicHiRezTextures)
		{
			Resources.UnloadAsset(dynamicHiRezTexture.mainTexture);
			dynamicHiRezTexture.mainTexture = null;
		}
		DynamicHiRezTextures.Clear();
	}

	public void UpdateCards(List<CardItem> cards)
	{
		for (int num = 6; num >= 0; num--)
		{
			if (num >= cards.Count)
			{
				UITexture[] componentsInChildren = playerHands[num].gameObject.GetComponentsInChildren<UITexture>();
				UITexture[] array = componentsInChildren;
				foreach (UITexture uITexture in array)
				{
					switch (uITexture.name)
					{
					case "Card_Art":
						if (uITexture.mainTexture != null)
						{
							RegisterDynamicTexture(uITexture);
						}
						break;
					}
				}
			}
			playerHands[num].gameObject.SetActive(num < cards.Count);
			if (num < cards.Count)
			{
				if (cards[num] == null)
				{
					break;
				}
				PanelManagerBattle.FillCardInfo(playerHands[num], cards[num], PlayerType.User);
				CWTBTapToBringForward component = playerHands[num].GetComponent<CWTBTapToBringForward>();
				component.card = cards[num];
				component.cardName = cards[num].Form.Name;
				PlayDrawTween(playerHands[num].gameObject, true);
				int num2 = cards[num].Form.DetermineCost(PlayerType.User);
				bool enable = num2 <= GameInstance.GetMagicPoints(PlayerType.User);
				UpdateHandCardStatus(playerHands[num].gameObject, enable, num2);
			}
			else
			{
				PlayDrawTween(playerHands[num].gameObject, false);
			}
		}
	}

	private void UpdateHandCardStatus(GameObject cardObj, bool enable, int cost)
	{
		UISprite[] componentsInChildren = cardObj.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			Color color = uISprite.color;
			uISprite.color = ((!enable) ? Color.Lerp(Color.black, color, 0.5f) : color);
		}
		UITexture[] componentsInChildren2 = cardObj.GetComponentsInChildren<UITexture>(true);
		UITexture[] array2 = componentsInChildren2;
		foreach (UITexture uITexture in array2)
		{
			uITexture.color = ((!enable) ? Color.Lerp(Color.black, Color.white, 0.5f) : Color.white);
		}
		UILabel[] componentsInChildren3 = cardObj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array3 = componentsInChildren3;
		foreach (UILabel uILabel in array3)
		{
			Color color2 = uILabel.color;
			uILabel.color = ((!enable) ? Color.Lerp(Color.black, color2, 0.5f) : color2);
			if (uILabel.name == "Cost_Label")
			{
				uILabel.text = cost.ToString();
			}
		}
	}

	public void ResetCardPos()
	{
		int cardsInHand = GameInstance.GetCardsInHand(PlayerType.User);
		CWTBDragToMove[] componentsInChildren = GetComponentsInChildren<CWTBDragToMove>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			CWTBDragToMove cWTBDragToMove = componentsInChildren[i];
			if (i < cardsInHand)
			{
				cWTBDragToMove.transform.position = cWTBDragToMove.originalPos.position;
			}
		}
	}

	public void PlayDrawTween(GameObject tweenTarget, bool enable)
	{
		TweenTransform[] componentsInChildren = tweenTarget.GetComponentsInChildren<TweenTransform>(true);
		TweenTransform[] array = componentsInChildren;
		foreach (TweenTransform tweenTransform in array)
		{
			if (tweenTransform.tweenGroup == 1)
			{
				tweenTransform.enabled = enable;
				tweenTransform.Play(enable);
			}
		}
	}

	public void PlayDiscardTween(GameObject tweenTarget, bool enable)
	{
		TweenTransform[] componentsInChildren = tweenTarget.GetComponentsInChildren<TweenTransform>(true);
		TweenTransform[] array = componentsInChildren;
		foreach (TweenTransform tweenTransform in array)
		{
			if (tweenTransform.tweenGroup == 1)
			{
				tweenTransform.enabled = enable;
				tweenTransform.Play(enable);
			}
		}
	}

	public void ResetLaneColor()
	{
		for (int i = 0; i < laneColliders.Length; i++)
		{
			laneColliders[i].gameObject.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f);
		}
	}

	public void PlayCard(int activeLane, CardItem card)
	{
		for (int i = 0; i < laneColliders.Length; i++)
		{
			if (activeLane == i)
			{
				PlayCardToLane(i, card);
			}
		}
	}

	public bool CanPlay()
	{
		return (BattlePhaseManager.GetInstance().Phase == BattlePhase.P1Setup || BattlePhaseManager.GetInstance().Phase == BattlePhase.P1SetupAction) && !spinStart;
	}

	public void PlayCardToLane(int lane, CardItem card)
	{
		phaseMgr.AddCardToHistory(card.Form.ID);
		currentCard = card;
		SetCardToFloopPanel(card);
		if (CanPlay() && currentCard.Form.CanPlay(PlayerType.User, lane))
		{
			TutorialMonitor.Instance.HidePlayersFirstTurnLabel();
			if (currentCard.Form.Type == CardType.Spell)
			{
				TriggerSpell(PlayerType.User, card);
			}
			else if (!GameInstance.LaneHasCard(PlayerType.User, lane, currentCard.Form.Type))
			{
				Summon(lane, card);
			}
			else
			{
				replaceTweenTarget.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void SetCardToFloopPanel(CardItem card)
	{
		if (floopMgr == null)
		{
			floopMgr = CWFloopActionManager.GetInstance();
		}
		floopMgr.lane = lane;
		floopMgr.card = card;
		floopMgr.player = 0;
	}

	public void TriggerSpell(PlayerType player, CardItem card)
	{
		if (card.Form.Rarity >= 5)
		{
			phaseMgr.Phase = ((player != PlayerType.User) ? BattlePhase.P2SetupActionRareCard : BattlePhase.P1SetupActionRareCard);
			CWCharacterAnimController.GetInstance().DoEffectPlayCardSpell(player, lane, card);
		}
		else
		{
			PreCastSpell(player, false, card);
		}
	}

	public void PreCastSpell(PlayerType player, bool rareFlag, CardItem card)
	{
		spinSpeed = 0f;
		prevSpinSpeed = 0f;
		if (player == PlayerType.User && !rareFlag)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), spellSound);
			spinStart = true;
		}
		else
		{
			spinStart = false;
			StartCoroutine(panelMgrBattle.PlaySpellFx(card, player));
		}
	}

	public void Summon(int lane, CardItem card)
	{
		cardMgr.CardSelected = false;
		GameInstance.Summon(PlayerType.User, lane, card);
	}

	public GameObject SpawnHighlightFX(string fxName, GameObject target)
	{
		Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Particles/" + fxName);
		GameObject result = null;
		if ((bool)@object)
		{
			result = SLOTGame.InstantiateFX(@object, target.transform.position, Quaternion.identity) as GameObject;
		}
		return result;
	}

	private void Update()
	{
		if (!spinStart)
		{
			return;
		}
		if (panelMgrBattle.currentCardObj != null)
		{
			Collider component = panelMgrBattle.currentCardObj.GetComponent<Collider>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		AnimateSpin(panelMgrBattle.currentCardObj);
	}

	private void AnimateSpin(GameObject cardObj)
	{
		spinSpeed += Time.deltaTime * 3000f;
		cardObj.transform.localEulerAngles = new Vector3(0f, cardObj.transform.localEulerAngles.y + Time.deltaTime * spinSpeed, 0f);
		if (spinSpeed > 3000f && prevSpinSpeed <= 3000f && endOfSpinningCardFX != null)
		{
			GameObject gameObject = SLOTGame.InstantiateFX(endOfSpinningCardFX) as GameObject;
			if (gameObject != null)
			{
				Camera camera = NGUITools.FindCameraForLayer(cardObj.layer);
				Camera camera2 = NGUITools.FindCameraForLayer(gameObject.layer);
				if (camera != null && camera2 != null)
				{
					Vector3 position = cardObj.transform.position;
					position = camera.WorldToScreenPoint(position);
					position = camera2.ScreenToWorldPoint(position);
					gameObject.transform.position = position + endOfSpinningCardFXOffset;
				}
			}
		}
		prevSpinSpeed = spinSpeed;
		if (spinSpeed > 4000f)
		{
			Collider component = cardObj.GetComponent<Collider>();
			if (component != null)
			{
				component.enabled = true;
			}
			cardObj.SetActive(false);
			StartCoroutine(panelMgrBattle.PlaySpellFx(currentCard, PlayerType.User));
			spinStart = false;
			spinSpeed = 0f;
			prevSpinSpeed = 0f;
		}
	}
}
