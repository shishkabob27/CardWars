using System;
using UnityEngine;

public class RestartOnResume : MonoBehaviour
{
	private static DateTime PauseBeginTime;

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			PauseBeginTime = DateTime.Now;
		}
		else if ((DateTime.Now - PauseBeginTime).TotalSeconds > (double)ParametersManager.Instance.Restart_OnResume_Time)
		{
			PlayerInfoScript.GetInstance().ReloadGame();
		}
	}
}
