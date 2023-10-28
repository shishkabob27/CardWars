using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DungeonWorldItem : MonoBehaviour
{
	public GameObject WidgetHighlight;

	public GameObject WidgetLocked;

	public UILabel LabelName;

	public UILabel LabelUnlockTime;

	public UISprite Icon;

	private DateTime lockEventTime;

	public DungeonData Dungeon { get; private set; }

	public bool Locked { get; private set; }

	[method: MethodImpl(32)]
	public event Action<DungeonWorldItem> OnSelectEvent;

	public void SetData(DungeonData dungeon)
	{
		Dungeon = dungeon;
		LabelName.text = KFFLocalization.Get(dungeon.Name);
		Icon.spriteName = dungeon.IconName;
		UpdateLockStatus();
		SetHighlighted(false);
	}

	public void SetHighlighted(bool highlighted)
	{
		WidgetHighlight.SetActive(highlighted);
	}

	private void Update()
	{
		UpdateLockStatus();
	}

	private void OnClick()
	{
		if (this.OnSelectEvent != null)
		{
			this.OnSelectEvent(this);
		}
	}

	private void UpdateLockStatus()
	{
		DateTime serverTime = TFUtils.ServerTime;
		Locked = false;
		long timeToUnlock = Dungeon.GetTimeToUnlock(serverTime.Ticks);
		if (DebugFlagsScript.GetInstance().disableDungeonTimeLock || timeToUnlock == 0L)
		{
			WidgetLocked.SetActive(false);
			timeToUnlock = Dungeon.GetTimeToLock(serverTime.Ticks);
			if (timeToUnlock != long.MaxValue)
			{
				TimeSpan timeSpan = new TimeSpan(timeToUnlock);
				LabelUnlockTime.text = TFUtils.DurationToString((int)timeSpan.TotalSeconds);
			}
			else
			{
				LabelUnlockTime.text = string.Empty;
			}
		}
		else
		{
			WidgetLocked.SetActive(true);
			Locked = true;
			TimeSpan timeSpan2 = new TimeSpan(timeToUnlock);
			LabelUnlockTime.text = TFUtils.DurationToString((int)timeSpan2.TotalSeconds);
		}
	}
}
