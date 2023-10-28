using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PointCloudRegognizer))]
public class PointCloudGestureSample : SampleBase
{
	public PointCloudGestureRenderer GestureRendererPrefab;

	public float GestureScale = 8f;

	public Vector2 GestureSpacing = new Vector2(1.25f, 1f);

	public int MaxGesturesPerRaw = 2;

	private List<PointCloudGestureRenderer> gestureRenderers;

	protected override void Start()
	{
		base.Start();
		RenderGestureTemplates();
	}

	private void OnCustomGesture(PointCloudGesture gesture)
	{
		string text = (gesture.MatchScore * 100f).ToString("N2");
		base.UI.StatusText = "Matched " + gesture.RecognizedTemplate.name + " (score: " + text + "% distance:" + gesture.MatchDistance.ToString("N2") + ")";
		PointCloudGestureRenderer pointCloudGestureRenderer = FindGestureRenderer(gesture.RecognizedTemplate);
		if ((bool)pointCloudGestureRenderer)
		{
			pointCloudGestureRenderer.Blink();
		}
	}

	private void OnFingerDown(FingerDownEvent e)
	{
		base.UI.StatusText = string.Empty;
	}

	private void RenderGestureTemplates()
	{
		gestureRenderers = new List<PointCloudGestureRenderer>();
		Transform transform = new GameObject("Gesture Templates").transform;
		transform.parent = base.transform;
		transform.localScale = GestureScale * Vector3.one;
		PointCloudRegognizer component = GetComponent<PointCloudRegognizer>();
		Vector3 zero = Vector3.zero;
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		foreach (PointCloudGestureTemplate template in component.Templates)
		{
			PointCloudGestureRenderer pointCloudGestureRenderer = Object.Instantiate(GestureRendererPrefab, transform.position, transform.rotation) as PointCloudGestureRenderer;
			pointCloudGestureRenderer.GestureTemplate = template;
			pointCloudGestureRenderer.name = template.name;
			pointCloudGestureRenderer.transform.parent = transform;
			pointCloudGestureRenderer.transform.localPosition = zero;
			pointCloudGestureRenderer.transform.localScale = Vector3.one;
			zero.x += GestureSpacing.x;
			num3 = Mathf.Max(num3, zero.x);
			if (++num >= MaxGesturesPerRaw)
			{
				zero.y += GestureSpacing.y;
				zero.x = 0f;
				num = 0;
				num2++;
			}
			gestureRenderers.Add(pointCloudGestureRenderer);
		}
		Vector3 zero2 = Vector3.zero;
		zero2.x -= GestureScale * 0.5f * (num3 - GestureSpacing.x);
		if (num2 > 0)
		{
			zero2.y -= GestureScale * 0.5f * (zero.y - GestureSpacing.y);
		}
		transform.localPosition = zero2;
	}

	private PointCloudGestureRenderer FindGestureRenderer(PointCloudGestureTemplate template)
	{
		return gestureRenderers.Find((PointCloudGestureRenderer gr) => gr.GestureTemplate == template);
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates how to use the PointCloudGestureRecognizer to recognize custom gestures from a list of templates";
	}
}
