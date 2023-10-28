using System.Collections;
using UnityEngine;

public class CWHeartAnimSequencer : MonoBehaviour
{
	public float enableTimer = 1f;

	public GameObject heartObj;

	public UILabel heartCountLabel;

	public int questNo;

	public string[] animNames;

	public float[] animTimers;

	public AudioClip[] animSounds;

	public GameObject[] heartFX;

	public Transform heartStartPosition;

	public GameObject vfx;

	public CWiTweenTrigger tweenTrigger;

	private bool _keyPressed;

	private void OnEnable()
	{
		ResetTween();
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData lastClearedQuest = instance.GetLastClearedQuest();
		heartCountLabel.text = "+" + lastClearedQuest.StaminaAwarded;
		if (lastClearedQuest.StaminaAwarded != 0)
		{
			PlayTween(0);
			StartCoroutine(PlaySequence());
		}
	}

	private IEnumerator WaitForKeyPress()
	{
		while (!_keyPressed)
		{
			if (Input.GetMouseButtonDown(0))
			{
				yield return null;
				break;
			}
			yield return 0;
		}
	}

	private void ResetTween()
	{
		iTween.Stop(heartObj);
		heartObj.transform.position = heartStartPosition.position;
		heartObj.transform.localScale = heartStartPosition.localScale;
	}

	private void PlayTween(int tweenGroup)
	{
		TweenPosition[] components = GetComponents<TweenPosition>();
		TweenPosition[] array = components;
		foreach (TweenPosition tweenPosition in array)
		{
			if (tweenPosition.tweenGroup == tweenGroup)
			{
				tweenPosition.Play(true);
			}
		}
	}

	private IEnumerator PlaySequence()
	{
		for (int i = 0; i < animNames.Length; i++)
		{
			yield return new WaitForSeconds(animTimers[i]);
			tweenTrigger.TriggerTweens(animNames[i]);
			if (heartFX[i] != null)
			{
				heartFX[i].SetActive(true);
			}
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), animSounds[i], true, false, SLOTAudioManager.AudioType.SFX);
			if (i == 0)
			{
				yield return StartCoroutine(WaitForKeyPress());
			}
		}
		PlayTween(1);
		yield return new WaitForSeconds(1f);
		MapControllerBase.GetInstance().resumeFlag = true;
	}

	private void EnableHeartObj()
	{
		heartObj.SetActive(true);
	}

	private void Update()
	{
	}
}
