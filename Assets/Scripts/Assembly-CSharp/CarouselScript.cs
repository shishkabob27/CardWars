using UnityEngine;

public class CarouselScript : MonoBehaviour
{
	public float Rotation;

	public float Limit = 105f;

	public float Destination;

	public float Threshold;

	public float Speed;

	public float StartPosition;

	public float TouchStart;

	public float TouchCurrent;

	public float TouchDelta;

	public int Target;

	public bool IgnoreClick;

	public bool Lerp;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Lerp = false;
			TouchStart = Input.mousePosition.x;
		}
		if (Input.GetMouseButton(0))
		{
			TouchCurrent = Input.mousePosition.x;
			TouchDelta = (TouchStart - TouchCurrent) * Speed;
			Rotation = StartPosition + TouchDelta;
			if (Rotation > Limit)
			{
				Rotation = Limit;
				StartPosition = Rotation;
				TouchStart = Input.mousePosition.x;
			}
			if (Rotation < Limit * -1f)
			{
				Rotation = Limit * -1f;
				StartPosition = Rotation;
				TouchStart = Input.mousePosition.x;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			StartPosition = Rotation;
			Lerp = true;
			Recenter();
		}
		if (Lerp)
		{
			StartPosition = Mathf.Lerp(Rotation, Destination, Time.deltaTime * 10f);
			Rotation = Mathf.Lerp(Rotation, Destination, Time.deltaTime * 10f);
		}
		base.transform.eulerAngles = new Vector3(0f, Rotation, 0f);
	}

	private void Recenter()
	{
		if (!IgnoreClick)
		{
			if (Rotation < Threshold * -5f)
			{
				Destination = Threshold * -6f;
			}
			if (Rotation >= Threshold * -5f && Rotation < Threshold * -3f)
			{
				Destination = Threshold * -4f;
			}
			if (Rotation >= Threshold * -3f && Rotation < Threshold * -1f)
			{
				Destination = Threshold * -2f;
			}
			if (Rotation >= Threshold * -1f && Rotation <= Threshold)
			{
				Destination = 0f;
			}
			if (Rotation > Threshold * 1f && Rotation <= Threshold * 3f)
			{
				Destination = Threshold * 2f;
			}
			if (Rotation > Threshold * 3f && Rotation <= Threshold * 5f)
			{
				Destination = Threshold * 4f;
			}
			if (Rotation > Threshold * 5f)
			{
				Destination = Threshold * 6f;
			}
			return;
		}
		if (Target == 1)
		{
			Destination = Threshold * -6f;
		}
		if (Target == 2)
		{
			Destination = Threshold * -4f;
		}
		if (Target == 3)
		{
			Destination = Threshold * -2f;
		}
		if (Target == 4)
		{
			Destination = Threshold * 0f;
		}
		if (Target == 5)
		{
			Destination = Threshold * 2f;
		}
		if (Target == 6)
		{
			Destination = Threshold * 4f;
		}
		if (Target == 7)
		{
			Destination = Threshold * 6f;
		}
		IgnoreClick = false;
	}
}
