using System.Collections;
using UnityEngine;

public class CWLootingSequenceTrigger : MonoBehaviour
{
	public int lane;

	private CWLootingSequencer lootingSqcr;

	public GameObject openFXGroup;

	public bool awarded;

	private void Start()
	{
		lootingSqcr = CWLootingSequencer.GetInstance();
	}

	private void OnEnable()
	{
		Animation componentInChildren = GetComponentInChildren<Animation>();
		componentInChildren.Play("LootChest_In");
		componentInChildren.PlayQueued("LootChest_Idle");
	}

	private void OnClick()
	{
		bool flag;
		lock (this)
		{
			flag = !awarded;
			awarded = true;
		}
		if (flag)
		{
			GetComponent<Collider>().enabled = false;
			lootingSqcr.playedChest.Add(lane);
			openFXGroup.SetActive(true);
			Animation componentInChildren = GetComponentInChildren<Animation>();
			float length = componentInChildren.GetClip("LootChest_Select").length;
			StartCoroutine(DelaySequence(length));
		}
	}

	private IEnumerator DelaySequence(float waitTime)
	{
		yield return new WaitForSeconds(1f);
		int tmpCount = int.Parse(lootingSqcr.lootCountLabel.text);
		lootingSqcr.lootCountLabel.text = (tmpCount + 1).ToString();
		GameObject spawnObj = SLOTGame.InstantiateFX(lootingSqcr.fxPrefab, lootingSqcr.lootCountLabel.transform.position, Quaternion.identity) as GameObject;
		spawnObj.transform.parent = lootingSqcr.lootCountLabel.transform.parent;
		spawnObj.layer = spawnObj.transform.parent.gameObject.layer;
		yield return new WaitForSeconds(waitTime - 1f);
		lootingSqcr.PlayLootingSequence();
		Object.DestroyImmediate(base.transform.parent.gameObject);
	}

	private void Update()
	{
	}
}
