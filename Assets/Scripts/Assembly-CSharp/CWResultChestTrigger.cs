using System.Collections;
using AnimationOrTween;
using UnityEngine;

public class CWResultChestTrigger : MonoBehaviour
{
	public CardItem card;

	public bool hasCardFlag;

	public string cardName;

	private CWBattleEndChestController endChestCtrlr;

	private PanelManagerBattle panelMgrBattle;

	private DebugFlagsScript debugFlag;

	public GameObject openFXGroup;

	private QuestEarningManager earningMgr;

	private void Start()
	{
		endChestCtrlr = CWBattleEndChestController.GetInstance();
		panelMgrBattle = PanelManagerBattle.GetInstance();
		debugFlag = DebugFlagsScript.GetInstance();
		earningMgr = QuestEarningManager.GetInstance();
	}

	private void OnEnable()
	{
		Animation componentInChildren = GetComponentInChildren<Animation>();
		componentInChildren.Play("LootChest_In");
		componentInChildren.PlayQueued("LootChest_Idle");
	}

	private void OnClick()
	{
		UICamera.useInputEnabler = true;
		Object.DestroyImmediate(GetComponent<Collider>());
		GameObject gameObject = endChestCtrlr.vfxRarity[card.Form.Rarity - 1];
		int num = 1;
		if (gameObject.name.EndsWith("Low"))
		{
			num = 0;
		}
		else if (gameObject.name.EndsWith("Med"))
		{
			num = 1;
		}
		else if (gameObject.name.EndsWith("High"))
		{
			num = 2;
		}
		float cardTime = endChestCtrlr.cardTimes[num];
		float fxTime = endChestCtrlr.fxTimes[num];
		openFXGroup.SetActive(true);
		if (card != null)
		{
			if (earningMgr.cardHistory.Contains(card.Form.ID))
			{
				hasCardFlag = true;
			}
			else
			{
				earningMgr.cardHistory.Add(card.Form.ID);
			}
			if (debugFlag.battleDisplay.forceNewCardLoot == ForceLoot.Yes)
			{
				hasCardFlag = false;
			}
			else if (debugFlag.battleDisplay.forceNewCardLoot == ForceLoot.No)
			{
				hasCardFlag = true;
			}
			UIButtonPlayAnimation component = GetComponent<UIButtonPlayAnimation>();
			if (!hasCardFlag)
			{
				endChestCtrlr.PlaceCamForLoot(base.transform.position);
				endChestCtrlr.earningHeader.Play(false);
				component.clipName = "LootChest_NewCard_In";
				StartCoroutine(DelaySequenceAfterLootOpen(fxTime, cardTime, true));
			}
			else
			{
				component.clipName = "LootChest_Out";
				StartCoroutine(DelaySequenceAfterLootOpen(0f, 2f, false));
				UICamera.useInputEnabler = false;
			}
		}
	}

	private IEnumerator DelaySequenceAfterLootOpen(float fxTime, float cardTime, bool newCard)
	{
		Transform chestParentTr = base.transform.parent.parent;
		Transform bannerParentTr = endChestCtrlr.bannerParentTr;
		if (newCard)
		{
			GameObject prefabObj2 = ((card.Form.Rarity < 3) ? endChestCtrlr.normalChestFX : endChestCtrlr.premiumChestFX);
			GetSpawnObject(prefabObj2, chestParentTr, chestParentTr.rotation);
			GetSpawnObject(endChestCtrlr.bannerObjects[card.Form.Rarity - 1], bannerParentTr, bannerParentTr.rotation);
			yield return new WaitForSeconds(fxTime);
			prefabObj2 = endChestCtrlr.vfxRarity[card.Form.Rarity - 1];
			GetSpawnObject(prefabObj2, bannerParentTr, chestParentTr.rotation);
		}
		yield return new WaitForSeconds(cardTime);
		GameObject cardObj = GetSpawnObject(quat: Quaternion.Euler(new Vector3(0f, -90f, 0f)), prefab: endChestCtrlr.resultCard, parentTr: chestParentTr);
		Transform cardInChest = base.transform.Find("Chest_Card");
		if (cardInChest != null)
		{
			cardInChest.gameObject.SetActive(false);
		}
		PanelManagerBattle.FillCardInfo(cardObj, card, PlayerType.User);
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(endChestCtrlr.revealCardSound);
		yield return new WaitForSeconds(2f);
		if (newCard)
		{
			endChestCtrlr.tapAnywherePanel.SetActive(true);
			UIButtonPlayAnimation animPlay = endChestCtrlr.battleEndTapDelegate.GetComponent<UIButtonPlayAnimation>();
			animPlay.target = GetComponent<Animation>();
			CWDestroyObjectOnClick destroyScript = endChestCtrlr.battleEndTapDelegate.GetComponent<CWDestroyObjectOnClick>();
			destroyScript.obj = base.transform.parent.gameObject;
			CWResultFlyCard flyingCardScript2 = endChestCtrlr.battleEndTapDelegate.GetComponent<CWResultFlyCard>();
			flyingCardScript2.objectToDestroy = cardObj;
			endChestCtrlr.battleEndTapDelegate.disableFlag = false;
		}
		else
		{
			CWResultFlyCard flyingCardScript = base.transform.parent.parent.GetComponent<CWResultFlyCard>();
			flyingCardScript.objectToDestroy = cardObj;
			SetChestOpened(base.transform.parent.gameObject);
		}
		switch (card.Form.Type)
		{
		case CardType.Building:
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.LootAwarded1);
			break;
		case CardType.Spell:
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.LootAwarded2);
			break;
		}
		UICamera.useInputEnabler = false;
	}

	private void SetupResultCard(GameObject cardObj)
	{
		CWResultZoomCardSet component = cardObj.GetComponent<CWResultZoomCardSet>();
		component.card = card;
		UIButtonTween uIButtonTween = cardObj.AddComponent<UIButtonTween>();
		uIButtonTween.tweenTarget = panelMgrBattle.zoomCard;
		uIButtonTween.ifDisabledOnPlay = EnableCondition.EnableThenPlay;
	}

	private GameObject GetSpawnObject(GameObject prefab, Transform parentTr, Quaternion quat)
	{
		GameObject gameObject = null;
		gameObject = SLOTGame.InstantiateFX(prefab, parentTr.position, quat) as GameObject;
		gameObject.transform.parent = parentTr;
		return gameObject;
	}

	public void SetChestOpened(GameObject obj)
	{
		endChestCtrlr.hasCardTween.SendMessage("OnClick");
		base.transform.parent.parent.SendMessage("OnClick");
		Object.DestroyImmediate(obj);
	}

	private void Update()
	{
		cardName = card.Form.ID;
	}
}
