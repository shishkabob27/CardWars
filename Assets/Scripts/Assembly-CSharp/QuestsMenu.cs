using System.Collections;
using UnityEngine;

public class QuestsMenu : MonoBehaviour
{
	public GameObject OfflineStatus;

	private void TweenDone()
	{
		StartCoroutine(TweenDoneCoroutine());
	}

	private IEnumerator TweenDoneCoroutine()
	{
		yield return null;
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			GameObject obj = GameObject.Find("Menu Main Camera");
			if (obj != null && obj.activeInHierarchy && obj.GetComponent<Camera>() != null && obj.GetComponent<Camera>().enabled)
			{
				base.gameObject.SendMessage("TriggerTutorial", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void Update()
	{
		if (null != OfflineStatus)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (null != instance)
			{
				OfflineStatus.SetActive(!SocialManager.Instance.IsPlayerAuthenticated());
			}
		}
	}
}
