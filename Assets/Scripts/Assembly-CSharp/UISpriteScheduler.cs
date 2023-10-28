using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class UISpriteScheduler : MonoBehaviour
{
	[Serializable]
	public class Variation
	{
		public string id;

		public UIAtlas atlas;

		public string spriteName;
	}

	public string defaultVariation;

	public string scheduleCategory;

	public List<Variation> variations;

	private void OnEnable()
	{
		RefreshSprite();
	}

	private void RefreshSprite()
	{
		List<ScheduleData> itemsAvailableAndUnlocked = ScheduleDataManager.Instance.GetItemsAvailableAndUnlocked(scheduleCategory, TFUtils.ServerTime.Ticks);
		string targetVariation = defaultVariation;
		if (itemsAvailableAndUnlocked.Count > 0)
		{
			targetVariation = itemsAvailableAndUnlocked[0].ID;
		}
		if (targetVariation != null)
		{
			UpdateSprite(variations.Find((Variation elem) => elem.id == targetVariation));
		}
	}

	private void UpdateSprite(Variation variation)
	{
		if (variation != null)
		{
			UISprite component = GetComponent<UISprite>();
			if (!(component == null))
			{
				component.atlas = variation.atlas;
				component.spriteName = variation.spriteName;
			}
		}
	}
}
