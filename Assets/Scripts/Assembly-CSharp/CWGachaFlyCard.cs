using System.Collections;
using UnityEngine;

public class CWGachaFlyCard : MonoBehaviour
{
	public GameObject flyingObj;

	public Transform parentTr;

	public Transform startTr;

	public Transform destTr;

	public float time;

	public AudioClip earnSound;

	private PanelManager panelMgr;

	private CWUpdatePlayerStats playerStats;

	private PlayerInfoScript pInfo;

	private void Start()
	{
		panelMgr = PanelManager.GetInstance();
		playerStats = CWUpdatePlayerStats.GetInstance();
		pInfo = PlayerInfoScript.GetInstance();
	}

	private void OnClick()
	{
		playerStats.holdUpdateFlag = true;
		playerStats.inventoryLabel.text = pInfo.DeckManager.GetSortedInventory().Count - 1 + "/" + pInfo.MaxInventory;
		StartCoroutine(FlyingCard());
	}

	private GameObject debugCube(Transform tr, Vector3 pos)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.transform.parent = tr;
		gameObject.transform.localPosition = pos * 100f;
		return gameObject;
	}

	private IEnumerator FlyingCard()
	{
		yield return new WaitForSeconds(0.5f);
		Vector3 pos = panelMgr.newCamera.GetComponent<Camera>().WorldToViewportPoint(startTr.position);
		Vector3 newPos = panelMgr.uiCamera.ViewportToWorldPoint(pos);
		GameObject spawnObj = GetSpawnObj(newPos);
		iTween.MoveTo(spawnObj, iTween.Hash("position", destTr, "time", time));
		Vector3 sc = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(spawnObj, iTween.Hash("scale", sc, "time", time));
		yield return new WaitForSeconds(time);
		playerStats.holdUpdateFlag = false;
		iTweenEvent tweenEvent = iTweenEvent.GetEvent(destTr.gameObject, "PunchScale");
		if (tweenEvent != null)
		{
			tweenEvent.Play();
		}
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(earnSound);
	}

	private GameObject GetSpawnObj(Vector3 startPos)
	{
		GameObject gameObject = null;
		if (parentTr == null)
		{
			parentTr = base.transform;
		}
		if (flyingObj != null)
		{
			gameObject = SLOTGame.InstantiateFX(flyingObj, startPos, Quaternion.Euler(Vector3.zero)) as GameObject;
			gameObject.transform.parent = parentTr;
			gameObject.layer = parentTr.gameObject.layer;
		}
		return gameObject;
	}

	private void Update()
	{
	}
}
