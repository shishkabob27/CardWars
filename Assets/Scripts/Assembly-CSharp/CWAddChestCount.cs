using System.Collections;
using UnityEngine;

public class CWAddChestCount : MonoBehaviour
{
	public float delay;

	private void OnClick()
	{
		CWBattleEndChestController instance = CWBattleEndChestController.GetInstance();
		instance.openedChestCount++;
		instance.earningHeader.Play(true);
	}

	private IEnumerator DelayDeactivate()
	{
		yield return new WaitForSeconds(delay);
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
	}
}
