using UnityEngine;

public class PinchTwistSample : SampleBase
{
	public enum InputMode
	{
		PinchOnly,
		TwistOnly,
		PinchAndTwist
	}

	public Transform target;

	public Material twistMaterial;

	public Material pinchMaterial;

	public Material pinchAndTwistMaterial;

	public float pinchScaleFactor = 0.02f;

	private bool rotating;

	private bool pinching;

	private Material originalMaterial;

	private bool Rotating
	{
		get
		{
			return rotating;
		}
		set
		{
			if (rotating != value)
			{
				rotating = value;
				UpdateTargetMaterial();
			}
		}
	}

	private bool Pinching
	{
		get
		{
			return pinching;
		}
		set
		{
			if (pinching != value)
			{
				pinching = value;
				UpdateTargetMaterial();
			}
		}
	}

	private void OnTwist(TwistGesture gesture)
	{
		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
			base.UI.StatusText = "Twist gesture started";
			Rotating = true;
		}
		else if (gesture.Phase == ContinuousGesturePhase.Updated)
		{
			if (Rotating)
			{
				base.UI.StatusText = "Rotation updated by " + gesture.DeltaRotation + " degrees";
				target.Rotate(0f, 0f, gesture.DeltaRotation);
			}
		}
		else if (Rotating)
		{
			base.UI.StatusText = "Rotation gesture ended. Total rotation: " + gesture.TotalRotation;
			Rotating = false;
		}
	}

	private void OnPinch(PinchGesture gesture)
	{
		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
			Pinching = true;
		}
		else if (gesture.Phase == ContinuousGesturePhase.Updated)
		{
			if (Pinching)
			{
				target.transform.localScale += gesture.Delta * pinchScaleFactor * Vector3.one;
			}
		}
		else if (Pinching)
		{
			Pinching = false;
		}
	}

	private void UpdateTargetMaterial()
	{
		Material sharedMaterial = ((pinching && rotating) ? pinchAndTwistMaterial : (pinching ? pinchMaterial : ((!rotating) ? originalMaterial : twistMaterial)));
		target.GetComponent<Renderer>().sharedMaterial = sharedMaterial;
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates how to use the two-fingers Pinch and Rotation gesture events to control the scale and orientation of a rectangle on the screen\r\n\r\n- Pinch: move two fingers closer or further apart to change the scale of the rectangle (mousewheel on desktop)\r\n- Rotation: twist two fingers in a circular motion to rotate the rectangle (CTRL+mousewheel on desktop)\r\n\r\n";
	}

	protected override void Start()
	{
		base.Start();
		base.UI.StatusText = "Use two fingers anywhere on the screen to rotate and scale the green object.";
		originalMaterial = target.GetComponent<Renderer>().sharedMaterial;
	}
}
