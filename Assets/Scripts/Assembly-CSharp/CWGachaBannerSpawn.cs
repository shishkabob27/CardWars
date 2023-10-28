using System.Collections;
using UnityEngine;

public class CWGachaBannerSpawn : MonoBehaviour
{
	public GameObject[] bannerObjects;

	public Transform parentTr;

	public float PremiumDelay;

	public float NormalDelay;

	public float delay;

	public float bannerScale;

	public bool debugRarityFlag;

	public int debugRarity;

	private void SpawnBanner()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		if (SQUtils.StartsWith(instance.cardID, "leader_"))
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(instance.SFX_Rarity[instance.SFX_Rarity.Length - 1]);
			delay = ((instance.activeChest != CWGachaController.ChestType.Premium) ? NormalDelay : PremiumDelay);
			StartCoroutine(delaySpawnHeroFX());
		}
		else
		{
			CardForm card = CardDataManager.Instance.GetCard(instance.cardID);
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(instance.SFX_Rarity[card.Rarity - 1]);
			delay = ((instance.activeChest != CWGachaController.ChestType.Premium) ? NormalDelay : PremiumDelay);
			StartCoroutine(delaySpawnFX(card));
		}
	}

	private IEnumerator delaySpawnFX(CardForm card)
	{
		yield return new WaitForSeconds(delay);
		if (!debugRarityFlag)
		{
			SpawnFX(parentTr, bannerObjects[card.Rarity - 1]);
		}
		else
		{
			SpawnFX(parentTr, bannerObjects[debugRarity - 1]);
		}
	}

	private IEnumerator delaySpawnHeroFX()
	{
		yield return new WaitForSeconds(delay);
		if (!debugRarityFlag)
		{
			SpawnFX(parentTr, bannerObjects[bannerObjects.Length - 1]);
		}
		else
		{
			SpawnFX(parentTr, bannerObjects[debugRarity - 1]);
		}
	}

	private void SpawnFX(Transform parentTr, GameObject fxObj)
	{
		if (fxObj != null)
		{
			GameObject gameObject = SLOTGame.InstantiateFX(fxObj, parentTr.position, parentTr.rotation) as GameObject;
			gameObject.transform.localScale *= bannerScale;
			gameObject.transform.parent = parentTr;
		}
	}

	private void Update()
	{
	}
}
