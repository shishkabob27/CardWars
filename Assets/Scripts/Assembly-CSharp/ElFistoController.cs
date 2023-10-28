#define ASSERTS_ON
using System.Collections;
using UnityEngine;

public class ElFistoController : MonoBehaviour
{
	public enum ElFistoModes
	{
		Invalid = -1,
		Off,
		On,
		Always,
		Count
	}

	private const string ElFistoMatchLapse = "ElFistoMatchLapse";

	private const string ElFistoProgress = "ElFistoProgress";

	public ElFistoModes ElFistoMode;

	public AudioClip IntroSound;

	public UIButtonTween ShowElFistoIntroTween;

	public UIButtonTween HideElFistoIntroTween;

	public float ElFistoShowIntroDelay = 2f;

	public string ElFistoNIS = "FcFistoIntro";

	public string ElFistoCardNIS = "FcFistoIntroCard";

	public string RoundTextKey = "!!FC_EL_FISTO_ROUND";

	public int GetCurrentRound()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		int occuranceCounter = instance.GetOccuranceCounter("ElFistoProgress");
		return occuranceCounter + 1;
	}

	public void IncRound()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		instance.IncOccuranceCounter("ElFistoProgress");
	}

	public ElFisto GetCurrentElFisto()
	{
		return ElFistoDataManager.Instance.GetElFistoByRound(GetCurrentRound());
	}

	public QuestData GetElFistoQuestData()
	{
		ElFisto currentElFisto = GetCurrentElFisto();
		TFUtils.Assert(null != currentElFisto, "Invalid ElFisto quest?");
		return QuestManager.Instance.GetQuestByID("elfisto", currentElFisto.QuestID);
	}

	public void ResetElFisto()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		int totalMatchesPlayed = GetTotalMatchesPlayed();
		instance.SetOccuranceCounter("ElFistoMatchLapse", totalMatchesPlayed);
	}

	public bool ShouldAward()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		int num = 0;
		if (instance.mQuestMatchStats.ContainsKey("elfisto"))
		{
			num = instance.mQuestMatchStats["elfisto"].Wins;
		}
		return num == GetCurrentRound();
	}

	public IEnumerator DisplayAward(GameObject youGotThis)
	{
		TFUtils.Assert(null != youGotThis, "Can't award without YouGotThis game object");
		if (ShouldAward() && null != youGotThis)
		{
			YouGotThisController youGotThisController = youGotThis.GetComponent<YouGotThisController>();
			TFUtils.Assert(null != youGotThisController, "Can't award without youGotThisController");
			if (null == youGotThisController)
			{
				yield break;
			}
			ElFisto ef = GetCurrentElFisto();
			TFUtils.DebugLog("Going to DisplayAward for ElFisto: " + ef.RewardType + ", " + ef.RewardName);
			IncRound();
			yield return StartCoroutine(youGotThisController.AwardItem(ef.RewardType, ef.RewardName, ef.RewardIcon, ef.RewardQuantity));
		}
		yield return null;
	}

	public IEnumerator DisplayElFisto(UIButtonTween victory, UIButtonTween complete)
	{
		if (HasCompletedElFisto())
		{
			if (null != complete)
			{
				complete.Play(true);
				yield return new WaitForSeconds(3.5f);
				complete.Play(false);
				yield return new WaitForSeconds(0.5f);
			}
		}
		else if (null != victory)
		{
			victory.Play(true);
			yield return new WaitForSeconds(3.5f);
			victory.Play(false);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void SetupElFisto()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (null != instance && instance.GetOccuranceCounter("ElFistoMatchLapse") == 0)
		{
			ResetElFisto();
		}
	}

	private int GetTotalMatchesPlayed()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		return instance.mQuestMatchStats["fc"].Attempts + instance.mQuestMatchStats["main"].Attempts;
	}

	public bool HasCompletedElFisto()
	{
		return GetCurrentRound() > ElFistoDataManager.Instance.GetNumRounds();
	}

	public bool ShouldShowElFisto()
	{
		if (ElFistoMode == ElFistoModes.On)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			QuestData currentQuest = instance.GetCurrentQuest();
			if (!currentQuest.IsQuestType("fc"))
			{
				return false;
			}
			if (HasCompletedElFisto())
			{
				return false;
			}
			if (0 == QuestManager.Instance.CountQuestsWithState("elfisto", QuestData.QuestState.PLAYABLE))
			{
				return false;
			}
			ElFisto elFistoByRound = ElFistoDataManager.Instance.GetElFistoByRound(GetCurrentRound());
			int occuranceCounter = instance.GetOccuranceCounter("ElFistoMatchLapse");
			int num = GetTotalMatchesPlayed() - occuranceCounter;
			return num > elFistoByRound.BattlesNeeded && elFistoByRound.ChanceToAppear >= Random.Range(0f, 1f);
		}
		return false;
	}

	public IEnumerator ShowElFistoIntro()
	{
		UGuiTextReplacement.Instance.Set(RoundTextKey, KFFLocalization.Get(RoundTextKey, "<val>", string.Empty + GetCurrentRound()));
		bool nisComplete = false;
		NisLaunchHelper nisLauncher = GetComponent<NisLaunchHelper>();
		nisLauncher.endFadeSecsOverride = 0f;
		nisLauncher.OnceComplete(delegate
		{
			nisComplete = true;
		});
		ElFisto elFisto = GetCurrentElFisto();
		bool willAwardCard = GetCurrentElFisto().RewardType.StartsWith("Card");
		if (willAwardCard)
		{
			nisLauncher.LaunchNis(ElFistoCardNIS);
		}
		else
		{
			nisLauncher.LaunchNis(ElFistoNIS);
		}
		while (!nisComplete)
		{
			yield return null;
		}
		float timeToWait = ((!willAwardCard) ? 2.5f : 5f);
		yield return new WaitForSeconds(timeToWait);
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		ElFistoMode = ElFistoModes.On;
		SetupElFisto();
	}
}
