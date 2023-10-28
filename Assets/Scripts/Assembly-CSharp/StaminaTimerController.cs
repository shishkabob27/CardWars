using System;
using UnityEngine;

public class StaminaTimerController : MonoBehaviour
{
	private DateTime lastDelta;

	private bool Initialized;

	private bool needsUpdate;

	private TimeSpan intervalTimeSpan = TimeSpan.Zero;

	private static StaminaTimerController mController;

	public int Interval
	{
		get
		{
			return ParametersManager.Instance.Stamina_Restoration_Rate;
		}
	}

	private void Awake()
	{
		Initialized = false;
		if (mController == null)
		{
			mController = this;
		}
	}

	public static StaminaTimerController GetInstance()
	{
		return mController;
	}

	public void Initiate()
	{
		if (TFUtils.IsServerTimeValid())
		{
			Initiate(TFUtils.ServerTime.ToString());
		}
		else
		{
			lastDelta = DateTime.Now;
		}
	}

	public void Initiate(string lastTimestamp)
	{
		if (!TFUtils.IsServerTimeValid())
		{
			return;
		}
		Initialized = true;
		DateTime dateTime;
		try
		{
			dateTime = DateTime.Parse(lastTimestamp);
		}
		catch (Exception)
		{
			dateTime = DateTime.Today;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (dateTime != DateTime.Today)
		{
			double totalDays = TFUtils.ServerTime.Subtract(dateTime).TotalDays;
			if (totalDays > 1.0)
			{
				instance.Stamina = Mathf.Max(instance.Stamina, instance.Stamina_Max);
				lastDelta = TFUtils.ServerTime;
			}
			else
			{
				double num = TFUtils.ServerTime.Subtract(dateTime).TotalMilliseconds;
				int stamina_Restoration_Rate = ParametersManager.Instance.Stamina_Restoration_Rate;
				while (num >= (double)stamina_Restoration_Rate && instance.Stamina < instance.Stamina_Max)
				{
					instance.Stamina++;
					num -= (double)stamina_Restoration_Rate;
					dateTime = dateTime.AddMilliseconds(stamina_Restoration_Rate);
				}
				if (instance.Stamina == instance.Stamina_Max)
				{
					lastDelta = TFUtils.ServerTime;
				}
				else
				{
					lastDelta = dateTime;
				}
			}
			instance.LastTimestamp = lastDelta.ToString();
		}
		else
		{
			lastDelta = TFUtils.ServerTime;
			instance.Stamina = Mathf.Max(instance.Stamina, instance.Stamina_Max);
			instance.LastTimestamp = lastDelta.ToString();
		}
	}

	private void Update()
	{
		if (!Initialized)
		{
			return;
		}
		if (TFUtils.IsServerTimeValid())
		{
			needsUpdate = false;
			int stamina_Restoration_Rate = ParametersManager.Instance.Stamina_Restoration_Rate;
			if (TFUtils.ServerTime.Subtract(lastDelta).TotalMilliseconds >= (double)stamina_Restoration_Rate)
			{
				lastDelta = lastDelta.AddMilliseconds(stamina_Restoration_Rate);
				PlayerInfoScript instance = PlayerInfoScript.GetInstance();
				instance.LastTimestamp = lastDelta.ToString();
				if (instance.Stamina < instance.Stamina_Max)
				{
					instance.Stamina++;
					instance.Save();
				}
			}
		}
		else if (!needsUpdate)
		{
			needsUpdate = true;
			SessionManager.GetInstance().TestConnectivity();
		}
	}

	public void SetTimestampNow()
	{
		lastDelta = TFUtils.ServerTime;
	}

	private DateTime getCurrentTime()
	{
		return TFUtils.ServerTime;
	}

	public double GetTotalSecondsRemaining()
	{
		return GetTimeSpanRemaining().TotalSeconds;
	}

	public TimeSpan GetTimeSpanRemaining()
	{
		return GetTargetTime().Subtract(TFUtils.ServerTime);
	}

	public DateTime GetTargetTime()
	{
		if (intervalTimeSpan == TimeSpan.Zero)
		{
			intervalTimeSpan = TimeSpan.FromMilliseconds(ParametersManager.Instance.Stamina_Restoration_Rate);
		}
		return lastDelta.Add(intervalTimeSpan);
	}
}
