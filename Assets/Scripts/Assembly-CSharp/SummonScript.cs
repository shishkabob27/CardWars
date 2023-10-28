using System.Collections;
using UnityEngine;

public class SummonScript : MonoBehaviour
{
	public GameObject SpawnEffect;

	public AudioClip SpawnAudio;

	public float SpawnAudioDelaySecs;

	public string Intro;

	public string Idle;

	public float IntroLength;

	public CardScript cardScript;

	public GameObject hpBar;

	private void OnEnable()
	{
		IntroLength = GetIntroLength();
		StartCoroutine(SummonAction());
	}

	public void Summon()
	{
		IntroLength = GetIntroLength();
		StartCoroutine(SummonAction());
	}

	private float GetIntroLength()
	{
		float result = 2.5f;
		Animation component = GetComponent<Animation>();
		if (component != null && component[Intro] != null)
		{
			result = component[Intro].length;
		}
		return result;
	}

	private IEnumerator SummonAction()
	{
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 10f, base.transform.position.z);
		bool finishedSummoning = false;
		BattlePhaseManager phaseMgr = BattlePhaseManager.GetInstance();
		bool alreadySummoned = phaseMgr.alreadySummonedCard.Contains(phaseMgr.currentCardID);
		yield return new WaitForSeconds(1f);
		StartCoroutine(PlaySpawnAudio());
		if (SpawnEffect != null)
		{
			SLOTGame.InstantiateFX(SpawnEffect, new Vector3(base.transform.position.x, -2.1f, base.transform.position.z), Quaternion.identity);
		}
		if (Intro != string.Empty)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 10f, base.transform.position.z);
			GetComponent<Animation>().Play(Intro);
		}
		else
		{
			base.transform.localRotation = Quaternion.identity;
			iTween.MoveTo(base.gameObject, iTween.Hash("position", base.transform.parent, "time", 2f));
		}
		if (hpBar != null)
		{
			hpBar.SetActive(true);
		}
		if (!finishedSummoning && alreadySummoned && cardScript != null && cardScript.Owner == PlayerType.User)
		{
			cardScript.FinishSummoning();
			finishedSummoning = true;
		}
		yield return new WaitForSeconds(IntroLength);
		if (Idle != string.Empty)
		{
			GetComponent<Animation>().Play(Idle);
		}
		GetComponent<Animation>()["Derez"].speed = 1f;
		if (!finishedSummoning || cardScript.Owner != PlayerType.User)
		{
			cardScript.FinishSummoning();
		}
	}

	private IEnumerator PlaySpawnAudio()
	{
		if (SpawnAudioDelaySecs > 0f)
		{
			yield return new WaitForSeconds(SpawnAudioDelaySecs);
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), SpawnAudio, true, false, SLOTAudioManager.AudioType.SFX);
	}

	private void Update()
	{
	}
}
