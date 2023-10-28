using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NisLaunchHelper))]
public class QuestLaunchHelper : MonoBehaviour
{
	private string launchingQuestId;

	public bool isLaunching
	{
		get
		{
			return launchingQuestId != null;
		}
	}

	public void LaunchQuest(string questId, Deck playerDeckCopy, Deck opponentDeckCopy = null, BattleResolver questBattleResolver = null)
	{
		StartCoroutine(CoroutineLaunchQuest(questId, playerDeckCopy, opponentDeckCopy, questBattleResolver));
	}

	private IEnumerator CoroutineLaunchQuest(string questId, Deck playerDeckCopy, Deck opponentDeckCopy, BattleResolver questBattleResolver)
	{
		if (launchingQuestId != null)
		{
			TFUtils.DebugLog("Busy launching quest " + launchingQuestId + ". Ignoring request to launch quest " + questId);
			yield break;
		}
		QuestData qd = QuestManager.Instance.GetQuest(questId);
		if (qd == null)
		{
			TFUtils.DebugLog("Failed to launch quest " + questId);
			yield break;
		}
		launchingQuestId = questId;
		if (qd.NisPreLaunch != null && (qd.NisPlayAlways || PlayerInfoScript.GetInstance().GetQuestProgress(qd) <= 0))
		{
			bool nisComplete = false;
			NisLaunchHelper nisLauncher = GetComponent<NisLaunchHelper>();
			nisLauncher.OnceComplete(delegate
			{
				nisComplete = true;
			});
			nisLauncher.LaunchNis(qd.NisPreLaunch);
			while (!nisComplete)
			{
				yield return null;
			}
		}
		if (DebugFlagsScript.GetInstance().stopTutorial)
		{
			GlobalFlags.Instance.stopTutorial = true;
		}
		GameState gameState = GameState.Instance;
		gameState.ResetFromQuestData(qd, playerDeckCopy, opponentDeckCopy);
		gameState.BattleResolver = questBattleResolver;
		yield return StartCoroutine(PlayVO(gameState.GetCharacter(PlayerType.Opponent)));
		yield return Resources.UnloadUnusedAssets();
		SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevel("LoadingScreen");
		yield return new WaitForSeconds(0.05f);
		CleanupNisHelper();
		launchingQuestId = null;
	}

	private IEnumerator PlayVO(CharacterData charDataVO)
	{
		UICamera.useInputEnabler = true;
		AudioClip charVO = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("GoVO/" + charDataVO.GoVO) as AudioClip;
		if (charVO == null)
		{
			charVO = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("VO/GoVO/" + charDataVO.GoVO) as AudioClip;
		}
		if (charVO != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayVO(GetComponent<AudioSource>(), charVO);
			yield return new WaitForSeconds(1.7f);
		}
		UICamera.useInputEnabler = false;
	}

	private void CleanupNisHelper()
	{
		GetComponent<NisLaunchHelper>().Reset();
	}
}
