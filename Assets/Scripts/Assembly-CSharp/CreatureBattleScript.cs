using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBattleScript : MonoBehaviour
{
	private GameObject BattleHitEffect;

	public GameObject Tombstone;

	public float Timer;

	public string Idle;

	public bool RemoveTombstone;

	public bool DamageDealt;

	public bool Flooped;

	public bool Crash;

	private GameState GameInstance;

	private PanelManagerBattle panelMgr;

	private DamageScript[] DamageScripts = new DamageScript[2];

	public PlayerType Player { get; set; }

	public int LaneIndex { get; set; }

	private void Start()
	{
		panelMgr = PanelManagerBattle.GetInstance();
		GameInstance = GameState.Instance;
		for (int i = 0; i < 2; i++)
		{
			GameObject gameObject = GameObject.Find("R_P" + (i + 1) + "Damage");
			if (gameObject != null)
			{
				DamageScripts[i] = gameObject.GetComponent<DamageScript>();
			}
			else
			{
				DamageScripts[i] = null;
			}
		}
	}

	private void Update()
	{
		if (Crash)
		{
			Timer += Time.deltaTime;
			if (Timer > 0.75f && !DamageDealt)
			{
				DamageDealt = true;
			}
		}
		if (Flooped)
		{
			Timer += Time.deltaTime;
		}
	}

	public void DealDamage()
	{
		CreatureScript creature = GameInstance.GetCreature(Player, LaneIndex);
		Lane lane = GameInstance.GetLane(Player, LaneIndex);
		int index = lane.OpponentLane.Index;
		if (GameInstance.LaneHasCreature(!Player, index) && !GameInstance.GetCreature(!Player, index).Helpless)
		{
			CreatureScript creature2 = GameInstance.GetCreature(!Player, index);
			int faction = (int)creature.Data.Form.Faction;
			int faction2 = (int)creature2.Data.Form.Faction;
			DamageScripts[(int)(!Player)].UpdateLabel(creature.ATK);
			int aTK = creature.ATK;
			aTK = (int)((float)aTK + (float)aTK * RPSMatrix.Instance.Factors[faction, faction2]);
			if (aTK < 0)
			{
				aTK = 0;
			}
			CWBattleSequenceController instance = CWBattleSequenceController.GetInstance();
			BattlePhaseManager instance2 = BattlePhaseManager.GetInstance();
			if (instance.result == "Crit" && instance2.Phase == BattlePhase.P1Battle)
			{
				aTK = (int)((float)aTK * instance.damageModifierCrit);
			}
			creature2.TakeDamage(creature, aTK);
		}
		else
		{
			GameInstance.DealDamage(!Player, creature.ATK - GameInstance.GetATKPenalty(Player), lane.Index);
		}
	}

	public void CheckForDefeat()
	{
		if (GameInstance.IsMarkedForDeath(Player, LaneIndex))
		{
			StartCoroutine(DeathActions());
		}
	}

	private bool CanSideQuestDropItem(SideQuestData sqd)
	{
		if (sqd != null && !string.IsNullOrEmpty(sqd.CollectiblePrefab) && DropProfileDataManager.Instance.GetDropProfile(sqd.DropProfileID) != null)
		{
			bool flag = true;
			bool flag2 = true;
			if (!string.IsNullOrEmpty(sqd.ValidLeaderID))
			{
				LeaderItem leader = GameState.Instance.GetLeader(PlayerType.User);
				if (leader != null && leader.Form != null && leader.Form.ID != sqd.ValidLeaderID)
				{
					flag = false;
				}
			}
			if (sqd.ValidQuestIDs.Count > 0)
			{
				QuestData activeQuest = GameState.Instance.ActiveQuest;
				if (activeQuest != null && !sqd.ValidQuestIDs.Contains(activeQuest.iQuestID))
				{
					flag2 = false;
				}
			}
			if (flag && flag2)
			{
				QuestData activeQuest2 = GameState.Instance.ActiveQuest;
				PlayerInfoScript instance = PlayerInfoScript.GetInstance();
				SideQuestProgress sideQuestProgress = instance.GetSideQuestProgress(sqd);
				if (sideQuestProgress != null && sideQuestProgress.State == SideQuestProgress.SideQuestState.InProgress)
				{
					if (DebugFlagsScript.GetInstance().battleDisplay.forceItemDrop)
					{
						return true;
					}
					QuestEarningManager instance2 = QuestEarningManager.GetInstance();
					int numEarnedItems = instance2.GetNumEarnedItems(sqd);
					int num = sideQuestProgress.Collected + numEarnedItems;
					if (num < sqd.NumCollectibles)
					{
						int num2 = numEarnedItems;
						if (activeQuest2 != null && sideQuestProgress.CollectedPerQuestnode.ContainsKey(activeQuest2.iQuestID))
						{
							num2 += sideQuestProgress.CollectedPerQuestnode[activeQuest2.iQuestID];
						}
						if (num2 < sqd.MaxCollectiblesPerQuest)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private SideQuestData DetermineSideQuestToDropItem()
	{
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		List<SideQuestData> activeSideQuests = SideQuestManager.Instance.GetActiveSideQuests(activeQuest.QuestType);
		if (activeSideQuests != null)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			List<SideQuestData> list = new List<SideQuestData>();
			for (int i = 0; i < activeSideQuests.Count; i++)
			{
				SideQuestData sideQuestData = activeSideQuests[i];
				if (CanSideQuestDropItem(sideQuestData))
				{
					list.Add(sideQuestData);
				}
			}
			SideQuestData sideQuestData2 = null;
			if (list.Count == 1)
			{
				sideQuestData2 = list[0];
			}
			else if (list.Count > 1)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				sideQuestData2 = list[index];
			}
			if (sideQuestData2 != null)
			{
				DropProfile dropProfile = DropProfileDataManager.Instance.GetDropProfile(sideQuestData2.DropProfileID);
				if (dropProfile != null)
				{
					int numEarnedItems = QuestEarningManager.GetInstance().GetNumEarnedItems(sideQuestData2);
					int num = Math.Min(numEarnedItems, dropProfile.ItemDropPercentages.Count - 1);
					if (num >= 0)
					{
						float num2 = dropProfile.ItemDropPercentages[num];
						if (DebugFlagsScript.GetInstance().battleDisplay.forceItemDrop)
						{
							num2 = 1f;
						}
						float num3 = UnityEngine.Random.Range(0f, 1f);
						if (num3 <= num2)
						{
							return sideQuestData2;
						}
					}
				}
			}
		}
		return null;
	}

	private bool CanDropCoins()
	{
		int count = QuestEarningManager.GetInstance().earnedCards.Count;
		int maxLootDrops = GameState.Instance.ActiveQuest.MaxLootDrops;
		if (count >= maxLootDrops)
		{
			return false;
		}
		float num = 0f;
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		if (activeQuest.UseNewCardDropSystem)
		{
			DropProfile dropProfile = DropProfileDataManager.Instance.GetDropProfile(activeQuest.DropProfileID);
			if (dropProfile.CoinDropPercentages.Count <= count)
			{
				return false;
			}
			num = dropProfile.CoinDropPercentages[count];
			if (num == 0f)
			{
				return false;
			}
		}
		else
		{
			CardItem card = GameInstance.GetCard(Player, LaneIndex, CardType.Creature);
			num = card.CoinDropRate;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f);
		return num2 <= num;
	}

	private bool CanDropCard()
	{
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		int count = QuestEarningManager.GetInstance().earnedCards.Count;
		int maxLootDrops = activeQuest.MaxLootDrops;
		float num = 0f;
		if (count >= maxLootDrops)
		{
			return false;
		}
		if (activeQuest.UseNewCardDropSystem)
		{
			DropProfile dropProfile = DropProfileDataManager.Instance.GetDropProfile(activeQuest.DropProfileID);
			if (dropProfile.ChestDropPercentages.Count <= count)
			{
				return false;
			}
			num = dropProfile.ChestDropPercentages[count];
			if (num == 0f)
			{
				return false;
			}
		}
		else
		{
			CardItem card = GameInstance.GetCard(Player, LaneIndex, CardType.Creature);
			num = card.DropRate;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f);
		return num2 <= num;
	}

	private void EarnCard(CardItem card)
	{
		CardScript creature = GameInstance.GetCreature(Player, LaneIndex);
		TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.Looted);
		QuestEarningManager.GetInstance().earnedCards.Add(card);
		QuestEarningManager.GetInstance().earnedCardsName.Add(card.Form.ID);
		QuestEarningManager.GetInstance().hasCardFlag.Add(PlayerInfoScript.GetInstance().DeckManager.HasCard(card.Form.ID));
		CWLootingSequencer instance = CWLootingSequencer.GetInstance();
		instance.chestLanes.Add(creature);
		instance.holdLootFlag = true;
		BattlePhaseManager instance2 = BattlePhaseManager.GetInstance();
		if (instance2.Phase == BattlePhase.P1Battle || instance2.Phase == BattlePhase.P2Battle)
		{
			instance.LootInBattle = true;
		}
		else
		{
			instance.LootInBattle = false;
		}
		GameObject lootChestObj = GetLootChestObj(card);
		CWTombstoneScript component = base.transform.parent.GetComponent<CWTombstoneScript>();
		component.tombStone = lootChestObj;
		CWLootingSequenceTrigger[] componentsInChildren = lootChestObj.GetComponentsInChildren<CWLootingSequenceTrigger>(true);
		CWLootingSequenceTrigger[] array = componentsInChildren;
		foreach (CWLootingSequenceTrigger cWLootingSequenceTrigger in array)
		{
			cWLootingSequenceTrigger.lane = LaneIndex;
		}
		QuestEarningManager.GetInstance().dropedThisBattle = true;
	}

	private IEnumerator DeathActions()
	{
		if (!GameInstance.IsMarkedForDeath(Player, LaneIndex))
		{
			yield break;
		}
		CWTombstoneScript tombStoneScript = base.transform.parent.GetComponent<CWTombstoneScript>();
		VOManager.Instance.PlayEvent(Player, VOEvent.CreatureDestroyed);
		VOManager.Instance.PlayEvent(!Player, VOEvent.DestroyCreature);
		tombStoneScript.tombStone = panelMgr.tombstoneObjs[(int)PlayerType.Opponent];
		if (Player == PlayerType.Opponent && !QuestEarningManager.GetInstance().dropedThisBattle)
		{
			QuestData qd = GameState.Instance.ActiveQuest;
			bool dropCard = CanDropCard() || DebugFlagsScript.GetInstance().battleDisplay.forceLootDrop;
			CardItem card = null;
			dropCard = dropCard && !DebugFlagsScript.GetInstance().battleDisplay.forceItemDrop;
			BattleResolver battleResolver = GameState.Instance.BattleResolver;
			if (battleResolver != null)
			{
				battleResolver.GetOverrideDropCard(ref dropCard, ref card);
			}
			if (dropCard)
			{
				if (card == null)
				{
					if (qd.UseNewCardDropSystem)
					{
						card = qd.GetWeightedCard();
					}
					else
					{
						Deck CurrentDeck = GameInstance.GetDeck(Player);
						Deck WholeDeck = AIDeckManager.Instance.GetDeck(CurrentDeck.Name);
						card = WholeDeck.GetWeightedCard();
					}
				}
				if (card != null)
				{
					EarnCard(card);
				}
				else
				{
					TFUtils.DebugLog("No card was dropped");
					dropCard = false;
				}
			}
			if (!dropCard)
			{
				GameObject droppedItem = null;
				SideQuestData sqdToDrop = DetermineSideQuestToDropItem();
				if (sqdToDrop != null)
				{
					droppedItem = SideQuestManager.Instance.GetSideQuestDropObject(sqdToDrop);
					if (droppedItem != null)
					{
						QuestEarningManager qeMgr = QuestEarningManager.GetInstance();
						qeMgr.IncNumEarnedItems(sqdToDrop);
						qeMgr.dropedThisBattle = true;
						tombStoneScript.tombStone = droppedItem;
						CardScript sc = GameInstance.GetCreature(Player, LaneIndex);
						CWLootingSequencer lootSqcr = CWLootingSequencer.GetInstance();
						lootSqcr.chestLanes.Add(sc);
						lootSqcr.holdLootFlag = true;
						BattlePhaseManager phaseMgr = BattlePhaseManager.GetInstance();
						if (phaseMgr.Phase == BattlePhase.P1Battle || phaseMgr.Phase == BattlePhase.P2Battle)
						{
							lootSqcr.LootInBattle = true;
						}
						else
						{
							lootSqcr.LootInBattle = false;
						}
						CWLootingSequenceTrigger[] loots = droppedItem.GetComponentsInChildren<CWLootingSequenceTrigger>(true);
						CWLootingSequenceTrigger[] array = loots;
						foreach (CWLootingSequenceTrigger loot in array)
						{
							loot.lane = LaneIndex;
						}
					}
				}
				if (droppedItem == null && CanDropCoins())
				{
					int coinAmount2 = 0;
					if (qd.UseNewCardDropSystem)
					{
						coinAmount2 = qd.GetWeightedCoins();
					}
					else
					{
						CardItem creatureCard = GameInstance.GetCard(Player, LaneIndex, CardType.Creature);
						coinAmount2 = creatureCard.CoinReward;
					}
					if (coinAmount2 > 0)
					{
						QuestEarningManager.GetInstance().earnedCoin += coinAmount2;
						tombStoneScript.tombStone = panelMgr.coinLootObject;
						QuestEarningManager.GetInstance().dropedThisBattle = true;
					}
				}
			}
		}
		GetComponent<Animation>().Play("Death");
		tombStoneScript.SpawnTombStone();
		yield return new WaitForSeconds(1f);
		CardItem dyingCard = GameInstance.GetCard(Player, LaneIndex, CardType.Creature);
		GameInstance.RemoveCardFromPlay(Player, LaneIndex, CardType.Creature);
		if (GameState.Instance.LaneHasBuilding(Player, LaneIndex))
		{
			BuildingScript Script = GameInstance.GetBuilding(Player, LaneIndex);
			Script.OnCreatureDied(dyingCard);
		}
		Lane lane = GameInstance.GetLane(Player, LaneIndex);
		if (lane.OpponentLane.HasCreature())
		{
			CreatureScript script = lane.OpponentLane.GetCreature();
			script.OnCreatureDied(dyingCard);
		}
	}

	private GameObject GetLootChestObj(CardItem card)
	{
		return PanelManagerBattle.GetInstance().lootObjects[card.Form.Rarity - 1];
	}
}
