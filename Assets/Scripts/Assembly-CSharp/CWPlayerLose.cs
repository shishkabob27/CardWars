using System;
using Multiplayer;
using UnityEngine;

public class CWPlayerLose : AsyncData<string>
{
	public GameObject TweenController;

	private void OnEnable()
	{
		if (GlobalFlags.Instance.InMPMode && Asyncdata.processed)
		{
			global::Multiplayer.Multiplayer.MatchFinish(SessionManager.GetInstance().theSession, CWMPMapController.GetInstance().mLastMPData.mMatchID, true, StringCallback);
		}
	}

	private void StringCallback(string data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			if (Asyncdata.MP_Data != null)
			{
				instance.TotalTrophies = Convert.ToInt32(Asyncdata.MP_Data);
				instance.Save();
			}
		}
	}

	private void OnClick()
	{
		if (TweenController != null)
		{
			TweenController.SendMessage("OnClick");
		}
		Singleton<AnalyticsManager>.Instance.LogQuestLoss();
	}
}
