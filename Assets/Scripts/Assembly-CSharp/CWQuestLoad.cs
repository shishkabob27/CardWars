using System.Collections;
using UnityEngine;

[RequireComponent(typeof(QuestLaunchHelper))]
public class CWQuestLoad : MonoBehaviour
{
	public UIButtonTween ShowStaminaError;

	public UIButtonTween ShowTooManyCardsError;

	public UIButtonTween ShowFCLeaderPopup;

	public AudioClip errorSound;

	public AudioClip okSound;

	public string questContext;

	public LeaderSelectController LeaderSelect;

	private bool stopFlag;

	private void Start()
	{
		if (null == LeaderSelect)
		{
			LeaderSelect = new LeaderSelectController();
		}
	}

	private void OnPress(bool enable)
	{
		if (enable)
		{
			iTweenEvent @event = iTweenEvent.GetEvent(base.gameObject, "Go");
			if (@event != null)
			{
				@event.Play();
			}
		}
	}

	public bool CanStartQuest()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData currentQuest = instance.GetCurrentQuest();
		Deck selectedDeckCopy = instance.GetSelectedDeckCopy();
		LeaderItem selectedLeader = LeaderSelect.SelectedLeader;
		CharacterData characterData = CharacterDataManager.Instance.GetCharacterData(selectedLeader.Form.CharacterID);
		CharacterData opponent = currentQuest.Opponent;
		if (selectedDeckCopy.CardCount() < ParametersManager.Instance.Min_Cards_In_Deck)
		{
			if (TutorialMonitor.Instance != null)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.TooFewCards);
			}
			return false;
		}
		if (selectedDeckCopy.CardCount() > selectedLeader.RankValues.DeckMaxSize)
		{
			if (TutorialMonitor.Instance != null)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.TooManyCards);
			}
			return false;
		}
		if (instance.DeckManager.CardCount() > instance.MaxInventory)
		{
			ShowTooManyCardsError.Play(true);
			return false;
		}
		bool flag = LeaderManager.Instance.IsLeaderFromFC(currentQuest.LeaderID);
		if (selectedLeader.Form.FCWorld != flag)
		{
			if (TutorialMonitor.Instance != null)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FC_CantPlayAgainstNonFC);
			}
			return false;
		}
		int num;
		if (!GlobalFlags.Instance.InMPMode)
		{
			if (characterData.ID == opponent.ID)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.CantPlayYourself);
				return false;
			}
			if ((characterData.ID == "BMO" && opponent.ID == "Jake") || (characterData.ID == "Jake" && opponent.ID == "BMO"))
			{
				if (TutorialMonitor.Instance != null)
				{
					TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.JakeCantPlayBMO);
				}
				return false;
			}
			num = currentQuest.StaminaCost;
		}
		else
		{
			num = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPParticipationCost;
		}
		if (instance.Stamina < num)
		{
			ShowStaminaError.Play(true);
			return false;
		}
		switch (currentQuest.LeaderRestriction)
		{
		case QuestData.LeaderRestrictionRules.ALLOW:
		{
			bool flag2 = false;
			for (int j = 0; j < currentQuest.RestrictedLeaders.Length; j++)
			{
				if (currentQuest.RestrictedLeaders[j] == selectedLeader.Form.ID)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				if (TutorialMonitor.Instance != null && currentQuest.LeaderErrTrigger != 0)
				{
					TutorialMonitor.Instance.TriggerTutorial(currentQuest.LeaderErrTrigger);
				}
				return false;
			}
			break;
		}
		case QuestData.LeaderRestrictionRules.NOTALLOW:
		{
			for (int i = 0; i < currentQuest.RestrictedLeaders.Length; i++)
			{
				if (currentQuest.RestrictedLeaders[i] == selectedLeader.Form.ID)
				{
					if (TutorialMonitor.Instance != null && currentQuest.LeaderErrTrigger != 0)
					{
						TutorialMonitor.Instance.TriggerTutorial(currentQuest.LeaderErrTrigger);
					}
					return false;
				}
			}
			break;
		}
		}
		return true;
	}

	private void OnClick()
	{
		StartCoroutine(OnClickHelper());
	}

	private IEnumerator OnClickHelper()
	{
		stopFlag = false;
		PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
		QuestData qd = pinfo.GetCurrentQuest();
		if (GlobalFlags.Instance.InMPMode)
		{
			int questToLoad = (pinfo.CurrentMPQuest = ((pinfo.NumMPGamesPlayed >= 2) ? Random.Range(0, QuestManager.Instance.MPquests.Count - 1) : (pinfo.NumMPGamesPlayed + 8)));
			qd = QuestManager.Instance.GetMPQuest(questToLoad);
		}
		if (!CanStartQuest())
		{
			stopFlag = true;
		}
		else
		{
			if (stopFlag)
			{
				yield break;
			}
			GlobalFlags Flags = GlobalFlags.Instance;
			int EntryFee = (Flags.InMPMode ? RankManager.Instance.FindRank(pinfo.DeckManager.GetHighestLeaderRank()).PVPParticipationCost : qd.StaminaCost);
			ElFistoController efc = GetComponent<ElFistoController>();
			bool elFistoMode = null != efc && efc.ShouldShowElFisto();
			if (elFistoMode)
			{
				qd = efc.GetElFistoQuestData();
			}
			pinfo.StartMatch(qd, EntryFee);
			qd.LoadingScreenTextureName = MapControllerBase.GetInstance().LoadingScreenTextureName;
			LeaderSelect.SaveSelectedLeaderToPlayerDeck();
			pinfo.Save();
			GameObject sqhud = GameObject.Find("SideQuest_Hud");
			if (sqhud != null)
			{
				sqhud.SetActive(false);
			}
			Singleton<AnalyticsManager>.Instance.LogStaminaConsumed(questContext, EntryFee, pinfo.Stamina, pinfo.Stamina_Max, int.Parse(qd.QuestID));
			UIButtonTween tween = GetComponent<UIButtonTween>();
			if (tween != null)
			{
				tween.enabled = true;
			}
			Deck opponentDeck = null;
			BattleResolver battleResolver = null;
			if (Flags.InMPMode)
			{
				CWMPMapController.MPData matchData = CWMPMapController.GetInstance().mLastMPData;
				opponentDeck = AIDeckManager.Instance.GetMPDeck(matchData.mLandscapes, matchData.mCards, matchData.OpponentLeader, matchData.mLeaderLevel);
			}
			else if (qd.IsQuestType("fc"))
			{
				QuestData firstQuest = QuestManager.Instance.GetFirstQuest("fc");
				if (firstQuest.QuestID == qd.QuestID && pinfo.GetQuestProgress(qd) <= 0)
				{
					battleResolver = new FCSpecialDropBattleResolver(ParametersManager.Instance.FC_CardReward_FirstTime);
				}
			}
			if (elFistoMode)
			{
				if (null != tween)
				{
					UITweener tweener = tween.tweenTarget.GetComponent<UITweener>();
					if (null != tweener)
					{
						yield return new WaitForSeconds(tweener.duration + tweener.delay);
					}
				}
				yield return StartCoroutine(efc.ShowElFistoIntro());
				efc.ResetElFisto();
				battleResolver = new ElFistoBattleResolver(efc);
			}
			GetComponent<QuestLaunchHelper>().LaunchQuest(qd.QuestID, pinfo.GetSelectedDeckCopy(), opponentDeck, battleResolver);
		}
	}
}
