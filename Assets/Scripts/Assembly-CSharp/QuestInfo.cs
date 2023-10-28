using UnityEngine;

public class QuestInfo : MonoBehaviour
{
	private float savedTimeScale;

	private void OnEnable()
	{
		savedTimeScale = Time.timeScale;
		Time.timeScale = 0f;
	}

	private void QuestInfoDone()
	{
		Time.timeScale = savedTimeScale;
	}
}
