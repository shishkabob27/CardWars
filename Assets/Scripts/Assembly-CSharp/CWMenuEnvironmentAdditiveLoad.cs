using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWMenuEnvironmentAdditiveLoad : AsyncLoader
{
	public static CWMenuEnvironmentAdditiveLoad Instance;

	public GameObject[] environmentDefaultObjects;

	private void OnEnable()
	{
		Instance = this;
	}

	private void OnDisable()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private IEnumerator Start()
	{
		while (!SessionManager.GetInstance().IsLoadDataDone())
		{
			yield return null;
		}
		AsyncOperation asyncOp = null;
		List<ScheduleData> specialEnvironments = ScheduleDataManager.Instance.GetItemsAvailableAndUnlocked("menu_environment", TFUtils.ServerTime.Ticks);
		foreach (ScheduleData specialEnvironment in specialEnvironments)
		{
			asyncOp = SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevelAdditiveAsync(specialEnvironment.ID);
			if (asyncOp != null)
			{
				break;
			}
		}
		if (asyncOp != null)
		{
			yield return asyncOp;
			if (environmentDefaultObjects != null)
			{
				GameObject[] array = environmentDefaultObjects;
				foreach (GameObject environmentDefaultObj in array)
				{
					Object.Destroy(environmentDefaultObj);
				}
			}
		}
		base.IsReady = true;
	}
}
