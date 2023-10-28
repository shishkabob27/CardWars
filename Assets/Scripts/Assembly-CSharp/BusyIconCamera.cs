using UnityEngine;

public class BusyIconCamera : MonoBehaviour
{
	public enum Placement
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public Placement placement;

	private void Start()
	{
		SetViewportRect();
	}

	private void SetViewportRect()
	{
		Camera component = GetComponent<Camera>();
		if (component != null)
		{
			float num = (float)Mathf.Min(Screen.width, Screen.height) * 0.125f;
			float num2 = 0f;
			float num3 = 0f;
			float x = num2;
			float y = num3;
			switch (placement)
			{
			case Placement.TopLeft:
				x = num2;
				y = (float)Screen.height - num - num3;
				break;
			case Placement.TopRight:
				x = (float)Screen.width - num - num2;
				y = (float)Screen.height - num - num3;
				break;
			case Placement.BottomLeft:
				x = num2;
				y = num3;
				break;
			case Placement.BottomRight:
				x = (float)Screen.width - num - num2;
				y = num3;
				break;
			}
			component.pixelRect = new Rect(x, y, num, num);
		}
	}
}
