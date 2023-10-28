using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using Multiplayer;
using UnityEngine;

public class RefreshMatch : MonoBehaviour
{
	public UILabel Timer;

	public UILabel YourName;

	public UILabel OpponentName;

	public UISprite OpponentPortrait;

	public UILabel LeaderLvl;

	public UILabel LeaderHP;

	public UILabel LeaderDesc;

	public UILabel TrophyWinVal;

	public UILabel TrophyLossVal;

	public GameObject SearchOpponent;

	public CWQuestLandscapes QuestLandscapes;

	public Animation IntroAnim;

	public GameObject PlayButton;

	public int Countdown;

	private int CountdownTimer;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if ((bool)SearchOpponent)
		{
			SearchOpponent.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
		StartCoroutine("Delay");
		OpponentPortrait.gameObject.SetActive(false);
	}

	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(0.5f);
		if ((bool)IntroAnim)
		{
			IntroAnim.Play();
		}
	}

	public void RefreshValues(MatchData aData)
	{
		CWMPMapController.MPData mLastMPData = CWMPMapController.GetInstance().mLastMPData;
		StopCoroutine("CountdownRoutine");
		CountdownTimer = Countdown;
		StartCoroutine("CountdownRoutine");
		OpponentName.text = aData.opponentName;
		LeaderLvl.text = "Lvl " + aData.opponentLeaderLevel;
		OpponentPortrait.gameObject.SetActive(true);
		OpponentPortrait.spriteName = LeaderManager.Instance.GetMPLeaderPortrait(aData.opponentLeader);
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.MPOpponentName = aData.opponentName;
		instance.MPWinTrophies = aData.wagerWin;
		instance.MPLossTrophies = aData.wagerLose;
		instance.WinStreak = aData.winStreak;
		instance.StreakBonus = aData.streakBonus;
		YourName.text = instance.MPPlayerName;
		mLastMPData.mLeaderLevel = aData.opponentLeaderLevel;
		mLastMPData.OpponentLeader = aData.opponentLeader;
		mLastMPData.mMatchID = aData.matchId;
		mLastMPData.OpponentPVPName = aData.opponentName;
		mLastMPData.PlayerPVPName = instance.MPPlayerName;
		mLastMPData.TrophyWin = aData.wagerWin;
		mLastMPData.TrophyLoss = aData.wagerLose;
		int mPLeaderHP = LeaderManager.Instance.GetMPLeaderHP(aData.opponentLeader, aData.opponentLeaderLevel);
		LeaderHP.text = mPLeaderHP.ToString();
		Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(aData.landscapes);
		if (dictionary != null)
		{
			string[] array = (string[])dictionary["landscape"];
			if ((bool)QuestLandscapes)
			{
				QuestLandscapes.UpdateMPPreview(array);
			}
			mLastMPData.mLandscapes = array;
		}
		string mPLeaderDesc = LeaderManager.Instance.GetMPLeaderDesc(aData.opponentLeader);
		LeaderDesc.text = mPLeaderDesc;
		TrophyWinVal.text = aData.wagerWin.ToString();
		TrophyLossVal.text = aData.wagerLose.ToString();
		UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren;
		foreach (UILabel uILabel in array2)
		{
			if (uILabel.name == "Stamina")
			{
				uILabel.text = RankManager.Instance.FindRank(instance.DeckManager.GetHighestLeaderRank()).PVPParticipationCost.ToString();
			}
		}
	}

	private void CountDownExpired()
	{
		StopCoroutine("CountdownRoutine");
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.GetSelectedDeckCopy().Leader.Form.FCWorld)
		{
			if (TutorialMonitor.Instance != null)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.FC_CantPlayAgainstNonFC);
			}
		}
		else if ((bool)PlayButton)
		{
			PlayButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	public IEnumerator CountdownRoutine()
	{
		while (true)
		{
			if ((bool)Timer)
			{
				Timer.text = CountdownTimer.ToString();
			}
			CountdownTimer--;
			if (CountdownTimer < 0)
			{
				CountdownTimer = 0;
				if ((bool)Timer)
				{
					Timer.text = CountdownTimer.ToString();
				}
				CountDownExpired();
				yield return null;
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
