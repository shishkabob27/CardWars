using System;
using UnityEngine;

public class FGMouseInputProvider : FGInputProvider
{
	public int maxButtons = 3;

	public string pinchAxis = "Mouse ScrollWheel";

	public float pinchAxisScale = 100f;

	public float pinchResetTimeDelay = 0.15f;

	public float initialPinchDistance = 150f;

	public string twistAxis = "Mouse ScrollWheel";

	public float twistAxisScale = 100f;

	public KeyCode twistKey = KeyCode.LeftControl;

	public float twistResetTimeDelay = 0.15f;

	public KeyCode twistAndPinchKey = KeyCode.LeftShift;

	private Vector2 pivot = Vector2.zero;

	private Vector2[] pos = new Vector2[2]
	{
		Vector2.zero,
		Vector2.zero
	};

	private bool pinching;

	private float pinchResetTime;

	private float pinchDistance;

	private bool twisting;

	private float twistAngle;

	private float twistResetTime;

	public override int MaxSimultaneousFingers
	{
		get
		{
			return maxButtons;
		}
	}

	private void Start()
	{
		pinchDistance = initialPinchDistance;
	}

	private void Update()
	{
		UpdatePinchEmulation();
		UpdateTwistEmulation();
		if (pinching || twisting)
		{
			pivot = Input.mousePosition;
			float f = 0f;
			float num = initialPinchDistance;
			if (pinching && twisting && Input.GetKey(twistAndPinchKey))
			{
				f = (float)Math.PI / 180f * twistAngle;
				num = pinchDistance;
			}
			else if (twisting)
			{
				f = (float)Math.PI / 180f * twistAngle;
			}
			else if (pinching)
			{
				num = pinchDistance;
			}
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f);
			pos[0].x = pivot.x - 0.5f * num * num2;
			pos[0].y = pivot.y - 0.5f * num * num3;
			pos[1].x = pivot.x + 0.5f * num * num2;
			pos[1].y = pivot.y + 0.5f * num * num3;
		}
	}

	private void UpdatePinchEmulation()
	{
		float num = pinchAxisScale * Input.GetAxis(pinchAxis);
		if (Mathf.Abs(num) > 0.0001f)
		{
			if (!pinching)
			{
				pinching = true;
				pinchDistance = initialPinchDistance;
			}
			pinchResetTime = Time.time + pinchResetTimeDelay;
			pinchDistance = Mathf.Max(5f, pinchDistance + num);
		}
		else if (pinchResetTime <= Time.time)
		{
			pinching = false;
			pinchDistance = initialPinchDistance;
		}
	}

	private void UpdateTwistEmulation()
	{
		float num = twistAxisScale * Input.GetAxis(twistAxis);
		if (twistKey != 0 && Input.GetKey(twistKey) && Mathf.Abs(num) > 0.0001f)
		{
			if (!twisting)
			{
				twisting = true;
				twistAngle = 0f;
			}
			twistResetTime = Time.time + twistResetTimeDelay;
			twistAngle += num;
		}
		else if (twistResetTime <= Time.time)
		{
			twisting = false;
			twistAngle = 0f;
		}
	}

	public override void GetInputState(int fingerIndex, out bool down, out Vector2 position)
	{
		down = Input.GetMouseButton(fingerIndex);
		position = Input.mousePosition;
		if ((pinching || twisting) && (fingerIndex == 0 || fingerIndex == 1))
		{
			down = true;
			position = pos[fingerIndex];
		}
	}
}
