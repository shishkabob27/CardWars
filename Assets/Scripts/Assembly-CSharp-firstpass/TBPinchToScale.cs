using UnityEngine;

[AddComponentMenu("FingerGestures/Toolbox/Pinch To Scale")]
public class TBPinchToScale : MonoBehaviour
{
	public Vector3 scaleWeights = Vector3.one;

	public float minScaleAmount = 0.5f;

	public float maxScaleAmount = 2f;

	public float sensitivity = 1f;

	public float smoothingSpeed = 12f;

	private float idealScaleAmount = 1f;

	private float scaleAmount = 1f;

	private Vector3 baseScale = Vector3.one;

	public float ScaleAmount
	{
		get
		{
			return scaleAmount;
		}
		set
		{
			value = Mathf.Clamp(value, minScaleAmount, maxScaleAmount);
			if (value != scaleAmount)
			{
				scaleAmount = value;
				Vector3 localScale = scaleAmount * baseScale;
				localScale.x *= scaleWeights.x;
				localScale.y *= scaleWeights.y;
				localScale.z *= scaleWeights.z;
				base.transform.localScale = localScale;
			}
		}
	}

	public float IdealScaleAmount
	{
		get
		{
			return idealScaleAmount;
		}
		set
		{
			idealScaleAmount = Mathf.Clamp(value, minScaleAmount, maxScaleAmount);
		}
	}

	private void Start()
	{
		baseScale = base.transform.localScale;
		IdealScaleAmount = ScaleAmount;
	}

	private void Update()
	{
		if (smoothingSpeed > 0f)
		{
			ScaleAmount = Mathf.Lerp(ScaleAmount, IdealScaleAmount, Time.deltaTime * smoothingSpeed);
		}
		else
		{
			ScaleAmount = IdealScaleAmount;
		}
	}

	private void OnPinch(PinchGesture gesture)
	{
		IdealScaleAmount += 0.01f * sensitivity * gesture.Delta;
	}
}
