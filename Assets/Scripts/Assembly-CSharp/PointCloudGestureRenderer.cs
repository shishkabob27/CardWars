using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PointCloudGestureRenderer : MonoBehaviour
{
	private LineRenderer lineRenderer;

	public PointCloudGestureTemplate GestureTemplate;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = false;
	}

	private void Start()
	{
		if ((bool)GestureTemplate)
		{
			Render(GestureTemplate);
		}
	}

	public void Blink()
	{
		GetComponent<Animation>().Stop();
		GetComponent<Animation>().Play();
	}

	public bool Render(PointCloudGestureTemplate template)
	{
		if (template.PointCount < 2)
		{
			return false;
		}
		lineRenderer.SetVertexCount(template.PointCount);
		for (int i = 0; i < template.PointCount; i++)
		{
			lineRenderer.SetPosition(i, template.GetPosition(i));
		}
		return true;
	}
}
