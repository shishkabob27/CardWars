using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NisLaunchHelper))]
public class CWResultTriggerPostQuest : MonoBehaviour
{
	public GameObject endTween;

	private bool playing;

	public void OnClick()
	{
		StartCoroutine(CoroutinePostQuest());
	}

	private IEnumerator CoroutinePostQuest()
	{
		if (playing)
		{
			yield break;
		}
		QuestData qData = GameState.Instance.ActiveQuest;
		bool playNis = qData.NisWinPostBattle != null;
		if (playNis && !qData.NisPlayAlways)
		{
			int numStars = ((GameState.Instance.BattleResolver == null) ? PlayerInfoScript.GetInstance().GetQuestProgress(qData) : GameState.Instance.BattleResolver.questStars);
			playNis = GlobalFlags.Instance.NewlyCleared && numStars <= 1;
		}
		if (!playNis)
		{
			TriggerEndTween();
			yield break;
		}
		playing = true;
		NisLaunchHelper nisLauncher = GetComponent<NisLaunchHelper>();
		nisLauncher.LaunchNis(qData.NisWinPostBattle);
		while (nisLauncher.isPlaying)
		{
			yield return null;
		}
		playing = false;
		TriggerEndTween();
	}

	private void TriggerEndTween()
	{
		if (endTween != null)
		{
			endTween.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
