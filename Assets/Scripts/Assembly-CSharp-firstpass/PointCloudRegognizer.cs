using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("FingerGestures/Gestures/PointCloud Recognizer")]
public class PointCloudRegognizer : DiscreteGestureRecognizer<PointCloudGesture>
{
	[Serializable]
	public struct Point
	{
		public int StrokeId;

		public Vector2 Position;

		public Point(int strokeId, Vector2 pos)
		{
			StrokeId = strokeId;
			Position = pos;
		}

		public Point(int strokeId, float x, float y)
		{
			StrokeId = strokeId;
			Position = new Vector2(x, y);
		}
	}

	private class NormalizedTemplate
	{
		public PointCloudGestureTemplate Source;

		public List<Point> Points;
	}

	private class GestureNormalizer
	{
		private List<Point> normalizedPoints;

		private List<Point> pointBuffer;

		public GestureNormalizer()
		{
			normalizedPoints = new List<Point>();
			pointBuffer = new List<Point>();
		}

		public List<Point> Apply(List<Point> inputPoints, int normalizedPointsCount)
		{
			normalizedPoints = Resample(inputPoints, normalizedPointsCount);
			Scale(normalizedPoints);
			TranslateToOrigin(normalizedPoints);
			return normalizedPoints;
		}

		private List<Point> Resample(List<Point> points, int normalizedPointsCount)
		{
			float num = PathLength(points) / (float)(normalizedPointsCount - 1);
			float num2 = 0f;
			Point item = default(Point);
			normalizedPoints.Clear();
			normalizedPoints.Add(points[0]);
			pointBuffer.Clear();
			pointBuffer.AddRange(points);
			for (int i = 1; i < pointBuffer.Count; i++)
			{
				Point point = pointBuffer[i - 1];
				Point point2 = pointBuffer[i];
				if (point.StrokeId == point2.StrokeId)
				{
					float num3 = Vector2.Distance(point.Position, point2.Position);
					if (num2 + num3 > num)
					{
						item.Position = Vector2.Lerp(point.Position, point2.Position, (num - num2) / num3);
						item.StrokeId = point.StrokeId;
						normalizedPoints.Add(item);
						pointBuffer.Insert(i, item);
						num2 = 0f;
					}
					else
					{
						num2 += num3;
					}
				}
			}
			if (normalizedPoints.Count == normalizedPointsCount - 1)
			{
				normalizedPoints.Add(pointBuffer[pointBuffer.Count - 1]);
			}
			return normalizedPoints;
		}

		private static float PathLength(List<Point> points)
		{
			float num = 0f;
			for (int i = 1; i < points.Count; i++)
			{
				if (points[i].StrokeId == points[i - 1].StrokeId)
				{
					num += Vector2.Distance(points[i - 1].Position, points[i].Position);
				}
			}
			return num;
		}

		private static void Scale(List<Point> points)
		{
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			Vector2 vector2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
			foreach (Point point in points)
			{
				vector.x = Mathf.Min(vector.x, point.Position.x);
				vector.y = Mathf.Min(vector.y, point.Position.y);
				vector2.x = Mathf.Max(vector2.x, point.Position.x);
				vector2.y = Mathf.Max(vector2.y, point.Position.y);
			}
			float num = Mathf.Max(vector2.x - vector.x, vector2.y - vector.y);
			float num2 = 1f / num;
			for (int i = 0; i < points.Count; i++)
			{
				Point value = points[i];
				value.Position = (value.Position - vector) * num2;
				points[i] = value;
			}
		}

		private static void TranslateToOrigin(List<Point> points)
		{
			Vector2 vector = Centroid(points);
			for (int i = 0; i < points.Count; i++)
			{
				Point value = points[i];
				value.Position -= vector;
				points[i] = value;
			}
		}

		private static Vector2 Centroid(List<Point> points)
		{
			Vector2 zero = Vector2.zero;
			foreach (Point point in points)
			{
				zero += point.Position;
			}
			return zero / points.Count;
		}
	}

	private const int NormalizedPointCount = 32;

	private const float gizmoSphereRadius = 0.01f;

	public float MinDistanceBetweenSamples = 5f;

	public float MaxMatchDistance = 3.5f;

	public List<PointCloudGestureTemplate> Templates;

	private GestureNormalizer normalizer;

	private List<NormalizedTemplate> normalizedTemplates;

	private static bool[] matched = new bool[32];

	private PointCloudGesture debugLastGesture;

	private NormalizedTemplate debugLastMatchedTemplate;

	protected override void Awake()
	{
		base.Awake();
		normalizer = new GestureNormalizer();
		normalizedTemplates = new List<NormalizedTemplate>();
		foreach (PointCloudGestureTemplate template in Templates)
		{
			AddTemplate(template);
		}
	}

	private NormalizedTemplate FindNormalizedTemplate(PointCloudGestureTemplate template)
	{
		return normalizedTemplates.Find((NormalizedTemplate t) => t.Source == template);
	}

	private List<Point> Normalize(List<Point> points)
	{
		return new List<Point>(normalizer.Apply(points, 32));
	}

	public bool AddTemplate(PointCloudGestureTemplate template)
	{
		if (FindNormalizedTemplate(template) != null)
		{
			return false;
		}
		List<Point> list = new List<Point>();
		for (int i = 0; i < template.PointCount; i++)
		{
			list.Add(new Point(template.GetStrokeId(i), template.GetPosition(i)));
		}
		NormalizedTemplate normalizedTemplate = new NormalizedTemplate();
		normalizedTemplate.Source = template;
		normalizedTemplate.Points = Normalize(list);
		normalizedTemplates.Add(normalizedTemplate);
		return true;
	}

	protected override void OnBegin(PointCloudGesture gesture, FingerGestures.IFingerList touches)
	{
		gesture.StartPosition = touches.GetAverageStartPosition();
		gesture.Position = touches.GetAveragePosition();
		gesture.RawPoints.Clear();
		gesture.RawPoints.Add(new Point(0, gesture.Position));
	}

	private bool RecognizePointCloud(PointCloudGesture gesture)
	{
		debugLastGesture = gesture;
		gesture.MatchDistance = 0f;
		gesture.MatchScore = 0f;
		gesture.RecognizedTemplate = null;
		gesture.NormalizedPoints.Clear();
		if (gesture.RawPoints.Count < 2)
		{
			return false;
		}
		gesture.NormalizedPoints.AddRange(normalizer.Apply(gesture.RawPoints, 32));
		float num = float.PositiveInfinity;
		foreach (NormalizedTemplate normalizedTemplate in normalizedTemplates)
		{
			float num2 = GreedyCloudMatch(gesture.NormalizedPoints, normalizedTemplate.Points);
			if (num2 < num)
			{
				num = num2;
				gesture.RecognizedTemplate = normalizedTemplate.Source;
				debugLastMatchedTemplate = normalizedTemplate;
			}
		}
		if (gesture.RecognizedTemplate != null)
		{
			gesture.MatchDistance = num;
			gesture.MatchScore = Mathf.Max((MaxMatchDistance - num) / MaxMatchDistance, 0f);
		}
		return gesture.MatchScore > 0f;
	}

	private float GreedyCloudMatch(List<Point> points, List<Point> refPoints)
	{
		float num = 0.5f;
		int num2 = Mathf.FloorToInt(Mathf.Pow(points.Count, 1f - num));
		float num3 = float.PositiveInfinity;
		for (int i = 0; i < points.Count; i += num2)
		{
			float num4 = CloudDistance(points, refPoints, i);
			float num5 = CloudDistance(refPoints, points, i);
			num3 = Mathf.Min(num3, num4, num5);
		}
		return num3;
	}

	private static float CloudDistance(List<Point> points1, List<Point> points2, int startIndex)
	{
		int count = points1.Count;
		ResetMatched(count);
		float num = 0f;
		int num2 = startIndex;
		do
		{
			int num3 = -1;
			float num4 = float.PositiveInfinity;
			for (int i = 0; i < count; i++)
			{
				if (!matched[i])
				{
					float num5 = Vector2.Distance(points1[num2].Position, points2[i].Position);
					if (num5 < num4)
					{
						num4 = num5;
						num3 = i;
					}
				}
			}
			matched[num3] = true;
			float num6 = 1 - (num2 - startIndex + points1.Count) % points1.Count / points1.Count;
			num += num6 * num4;
			num2 = (num2 + 1) % points1.Count;
		}
		while (num2 != startIndex);
		return num;
	}

	private static void ResetMatched(int count)
	{
		if (matched.Length < count)
		{
			matched = new bool[count];
		}
		for (int i = 0; i < count; i++)
		{
			matched[i] = false;
		}
	}

	protected override GestureRecognitionState OnRecognize(PointCloudGesture gesture, FingerGestures.IFingerList touches)
	{
		if (touches.Count != RequiredFingerCount)
		{
			if (touches.Count < RequiredFingerCount)
			{
				if (RecognizePointCloud(gesture))
				{
					return GestureRecognitionState.Ended;
				}
				return GestureRecognitionState.Failed;
			}
			return GestureRecognitionState.Failed;
		}
		gesture.Position = touches.GetAveragePosition();
		float adjustedPixelDistance = FingerGestures.GetAdjustedPixelDistance(MinDistanceBetweenSamples);
		Vector2 position = gesture.RawPoints[gesture.RawPoints.Count - 1].Position;
		if (Vector2.SqrMagnitude(gesture.Position - position) > adjustedPixelDistance * adjustedPixelDistance)
		{
			int strokeId = 0;
			gesture.RawPoints.Add(new Point(strokeId, gesture.Position));
		}
		return GestureRecognitionState.InProgress;
	}

	public override string GetDefaultEventMessageName()
	{
		return "OnCustomGesture";
	}

	public void OnDrawGizmosSelected()
	{
		if (debugLastMatchedTemplate != null)
		{
			Gizmos.color = Color.yellow;
			DrawNormalizedPointCloud(debugLastMatchedTemplate.Points, 15f);
		}
		if (debugLastGesture != null)
		{
			Gizmos.color = Color.green;
			DrawNormalizedPointCloud(debugLastGesture.NormalizedPoints, 15f);
		}
	}

	private void DrawNormalizedPointCloud(List<Point> points, float scale)
	{
		if (points.Count > 0)
		{
			Gizmos.DrawWireSphere(scale * points[0].Position, 0.01f);
			for (int i = 1; i < points.Count; i++)
			{
				Gizmos.DrawLine(scale * points[i - 1].Position, scale * points[i].Position);
				Gizmos.DrawWireSphere(scale * points[i].Position, 0.01f);
			}
		}
	}
}
