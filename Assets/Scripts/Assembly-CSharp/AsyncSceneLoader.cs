using UnityEngine;

public class AsyncSceneLoader : MonoBehaviour
{
	public GameObject destroyRoot;

	public string nextScene;

	private AsyncOperation async;

	private void Start()
	{
		async = SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevelAdditiveAsync(nextScene);
		SLOTGame.GetInstance().ShowBusyIcon(true);
	}

	private void Update()
	{
		if (async.isDone)
		{
			GameObject obj = ((!(destroyRoot == null)) ? destroyRoot : base.gameObject);
			Object.Destroy(obj);
			SLOTGame.GetInstance().ShowBusyIcon(false);
		}
	}
}
