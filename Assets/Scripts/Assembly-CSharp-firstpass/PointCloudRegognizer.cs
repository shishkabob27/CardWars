using System;
using UnityEngine;
using System.Collections.Generic;

public class PointCloudRegognizer : DiscreteGestureRecognizer<PointCloudGesture>
{
	[Serializable]
	public struct Point
	{
		public Point(int strokeId, Vector2 pos) : this()
		{
		}

		public int StrokeId;
		public Vector2 Position;
	}

	public float MinDistanceBetweenSamples;
	public float MaxMatchDistance;
	public List<PointCloudGestureTemplate> Templates;
}
