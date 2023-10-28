using System.Collections;
using UnityEngine;

public class CWResultFlyCard : MonoBehaviour
{
	public GameObject flyingObj;

	public Transform parentTr;

	public Transform dest;

	public float time;

	public Vector3OrTransform start;

	private PanelManagerBattle panelMgrBattle;

	public GameObject objectToDestroy;

	private void Start()
	{
		panelMgrBattle = PanelManagerBattle.GetInstance();
	}

	private void OnClick()
	{
		StartCoroutine(FlyingCard());
	}

	private IEnumerator FlyingCard()
	{
		yield return new WaitForSeconds(0.5f);
		Vector3 pos = panelMgrBattle.newCamera.GetComponent<Camera>().WorldToViewportPoint(objectToDestroy.transform.position);
		Vector3 newPos = panelMgrBattle.uiCamera.ViewportToWorldPoint(pos);
		TweenAlpha[] tweens = objectToDestroy.GetComponentsInChildren<TweenAlpha>();
		TweenAlpha[] array = tweens;
		foreach (TweenAlpha tw in array)
		{
			if (tw != null)
			{
				tw.enabled = true;
				tw.Play(true);
			}
		}
		yield return new WaitForSeconds(0.2f);
		GameObject spawnObj = GetSpawnObj(newPos);
		iTween.MoveTo(spawnObj, iTween.Hash("position", dest, "time", time));
		Vector3 sc = new Vector3(0.001f, 0.001f, 0.001f);
		iTween.ScaleTo(spawnObj, iTween.Hash("scale", sc, "time", time));
		yield return new WaitForSeconds(0.5f);
		Object.DestroyImmediate(objectToDestroy);
		yield return new WaitForSeconds(0.5f);
		CWBattleEndPlayerStats battleStats = CWBattleEndPlayerStats.GetInstance();
		battleStats.inventoryLabel.text = (int.Parse(battleStats.inventoryLabel.text) + 1).ToString();
	}

	private GameObject GetSpawnObj(Vector3 startPos)
	{
		GameObject result = null;
		if (parentTr == null)
		{
			parentTr = base.transform;
		}
		if (flyingObj != null)
		{
			result = SLOTGame.InstantiateFX(flyingObj, startPos, Quaternion.Euler(Vector3.zero)) as GameObject;
		}
		return result;
	}

	private void Update()
	{
	}
}
