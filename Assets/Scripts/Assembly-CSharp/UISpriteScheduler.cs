using UnityEngine;
using System;
using System.Collections.Generic;

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
	public List<UISpriteScheduler.Variation> variations;
}
