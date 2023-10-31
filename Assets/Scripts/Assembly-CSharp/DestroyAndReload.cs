using System;
using Prime31;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyAndReload : MonoBehaviour
{
	private bool complete;

	private void Start()
	{
		if (LoadingManager.Instance != null)
		{
			LoadingManager.Instance.Clear();
		}
		SessionManager instance = SessionManager.GetInstance();
		if (instance != null)
		{
			instance.Clear();
		}
		SQUtils.FlushCache();
		UICamera.useInputEnabler = false;
		GameObject prime31ManagerGameObject = AbstractManager.getPrime31ManagerGameObject();
		object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		object[] array2 = array;
		foreach (object obj in array2)
		{
			if (base.gameObject != obj)
			{
				GameObject gameObject = obj as GameObject;
				if (!(gameObject == null) && !(gameObject == prime31ManagerGameObject) && !(gameObject.transform.parent == prime31ManagerGameObject.transform) && (gameObject.hideFlags & HideFlags.DontSave) == 0 && !(gameObject.name == "UnityFacebookSDKPlugin") && !(gameObject.name == "_Kochava Analytics") && !(gameObject.GetComponent<UpsightManager>() != null) && gameObject.activeInHierarchy)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}
		GameState.FullReset();
		GlobalFlags.FullReset();
		GC.Collect();
		complete = true;
	}

	private void Update()
	{
		if (complete)
		{
			SceneManager.LoadScene("StartupScene");
			complete = false;
		}
	}
}
