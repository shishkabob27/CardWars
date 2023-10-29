using System.Collections;
using UnityEngine;

public class CWSpawnObject : MonoBehaviour
{
	public string resourceName;

	public GameObject prefab;

	public Transform parentTr;

	public float delay;

	public bool onEnable;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (onEnable)
		{
			StartCoroutine(SpawnObject(parentTr));
		}
	}

	private void OnClick()
	{
		StartCoroutine(SpawnObject(parentTr));
	}

	private IEnumerator SpawnObject(Transform parentTr)
	{
		yield return new WaitForSeconds(delay);
		if (parentTr == null)
		{
			parentTr = base.transform;
		}
		//Object objData = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Props/" + resourceName);
		GameObject objData = Resources.Load("Props/" + resourceName, typeof(GameObject)) as GameObject;

        if (objData == null)
		{
            //objData = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Particles/" + resourceName);
            objData = Resources.Load("Particles/" + resourceName, typeof(GameObject)) as GameObject;
        }
		if (objData != null)
		{
			GameObject spawnObj = null;
			if (objData != null)
			{
				spawnObj = Instantiate(objData, parentTr.position, parentTr.rotation) as GameObject;
				spawnObj.transform.parent = parentTr;
			}
		}
		else if (prefab != null)
		{
			GameObject spawnObj3 = null;
			spawnObj3 = SLOTGame.InstantiateFX(prefab, parentTr.position, parentTr.rotation) as GameObject;
			spawnObj3.transform.parent = parentTr;
		}
		Animation[] anims = GetComponentsInChildren<Animation>();
		SummonScript summonScript = GetComponentInChildren<SummonScript>();
		bool wasPlaying = false;
		bool played = false;
		Animation[] array = anims;
		foreach (Animation ani in array)
		{
			if (summonScript != null)
			{
				if ((bool)ani.GetClip(summonScript.Intro))
				{
					wasPlaying = wasPlaying || ani.IsPlaying(summonScript.Intro);
					ani.Play(summonScript.Intro);
					played = played || ani.IsPlaying(summonScript.Intro);
				}
				if ((bool)ani.GetClip(summonScript.Idle))
				{
					wasPlaying = wasPlaying || ani.IsPlaying(summonScript.Idle);
					ani.PlayQueued(summonScript.Idle);
					played = played || ani.IsPlaying(summonScript.Idle);
				}
			}
			else
			{
				wasPlaying = wasPlaying || (ani.clip != null && ani.IsPlaying(ani.clip.name));
				ani.Play();
				played = played || (ani.clip != null && ani.IsPlaying(ani.clip.name));
			}
		}
		if (!wasPlaying && played && summonScript != null && summonScript.SpawnAudio != null && summonScript.GetComponent<AudioSource>() != null)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(summonScript.GetComponent<AudioSource>(), summonScript.SpawnAudio, true, false, SLOTAudioManager.AudioType.SFX);
		}
	}

	private void Update()
	{
	}
}
