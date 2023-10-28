using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FingerDownDetector))]
[RequireComponent(typeof(ScreenRaycaster))]
[RequireComponent(typeof(FingerUpDetector))]
[RequireComponent(typeof(FingerMotionDetector))]
public class FingerEventsSamplePart2 : SampleBase
{
	private class PathRenderer
	{
		private LineRenderer lineRenderer;

		private List<Vector3> points = new List<Vector3>();

		private List<GameObject> markers = new List<GameObject>();

		public PathRenderer(int index, LineRenderer lineRendererPrefab)
		{
			lineRenderer = Object.Instantiate(lineRendererPrefab);
			lineRenderer.name = lineRendererPrefab.name + index;
			lineRenderer.enabled = true;
			UpdateLines();
		}

		public void Reset()
		{
			points.Clear();
			UpdateLines();
			foreach (GameObject marker in markers)
			{
				Object.Destroy(marker);
			}
			markers.Clear();
		}

		public void AddPoint(Vector2 screenPos)
		{
			AddPoint(screenPos, null);
		}

		public void AddPoint(Vector2 screenPos, GameObject markerPrefab)
		{
			Vector3 worldPos = SampleBase.GetWorldPos(screenPos);
			if ((bool)markerPrefab)
			{
				AddMarker(worldPos, markerPrefab);
			}
			points.Add(worldPos);
			UpdateLines();
		}

		private GameObject AddMarker(Vector2 pos, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
			gameObject.name = prefab.name + "(" + markers.Count + ")";
			markers.Add(gameObject);
			return gameObject;
		}

		private void UpdateLines()
		{
			lineRenderer.SetVertexCount(points.Count);
			for (int i = 0; i < points.Count; i++)
			{
				lineRenderer.SetPosition(i, points[i]);
			}
		}
	}

	public LineRenderer lineRendererPrefab;

	public GameObject fingerDownMarkerPrefab;

	public GameObject fingerMoveBeginMarkerPrefab;

	public GameObject fingerMoveEndMarkerPrefab;

	public GameObject fingerUpMarkerPrefab;

	private PathRenderer[] paths;

	protected override void Start()
	{
		base.Start();
		base.UI.StatusText = "Drag your fingers anywhere on the screen";
		paths = new PathRenderer[FingerGestures.Instance.MaxFingers];
		for (int i = 0; i < paths.Length; i++)
		{
			paths[i] = new PathRenderer(i, lineRendererPrefab);
		}
	}

	protected override string GetHelpText()
	{
		return "This sample lets you visualize the FingerDown, FingerMoveBegin, FingerMove, FingerMoveEnd and FingerUp events.\r\n\r\nINSTRUCTIONS:\r\nMove your finger accross the screen and observe what happens.\r\n\r\nLEGEND:\r\n- Red Circle = FingerDown position\r\n- Yellow Square = FingerMoveBegin position\r\n- Green Sphere = FingerMoveEnd position\r\n- Blue Circle = FingerUp position";
	}

	private void OnFingerDown(FingerDownEvent e)
	{
		PathRenderer pathRenderer = paths[e.Finger.Index];
		pathRenderer.Reset();
		pathRenderer.AddPoint(e.Finger.Position, fingerDownMarkerPrefab);
	}

	private void OnFingerMove(FingerMotionEvent e)
	{
		PathRenderer pathRenderer = paths[e.Finger.Index];
		if (e.Phase == FingerMotionPhase.Started)
		{
			base.UI.StatusText = "Started moving " + e.Finger;
			pathRenderer.AddPoint(e.Position, fingerMoveBeginMarkerPrefab);
		}
		else if (e.Phase == FingerMotionPhase.Updated)
		{
			pathRenderer.AddPoint(e.Position);
		}
		else
		{
			base.UI.StatusText = "Stopped moving " + e.Finger;
			pathRenderer.AddPoint(e.Position, fingerMoveEndMarkerPrefab);
		}
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		PathRenderer pathRenderer = paths[e.Finger.Index];
		pathRenderer.AddPoint(e.Finger.Position, fingerUpMarkerPrefab);
		base.UI.StatusText = string.Concat("Finger ", e.Finger, " was held down for ", e.TimeHeldDown.ToString("N2"), " seconds");
	}
}
