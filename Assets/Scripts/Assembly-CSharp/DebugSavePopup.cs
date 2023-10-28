using System.Collections;
using UnityEngine;

public class DebugSavePopup : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(WaitAndClose());
	}

	private IEnumerator WaitAndClose()
	{
		yield return new WaitForSeconds(1f);
		NGUITools.Destroy(base.gameObject);
	}
}
