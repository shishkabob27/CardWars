using UnityEngine;

[AddComponentMenu("FingerGestures/Gestures/Tap Recognizer")]
public class TapRecognizer : DiscreteGestureRecognizer<TapGesture>
{
	public int RequiredTaps = 1;

	public float MoveTolerance = 20f;

	public float MaxDuration;

	public float MaxDelayBetweenTaps = 0.5f;

	private bool IsMultiTap
	{
		get
		{
			return RequiredTaps > 1;
		}
	}

	private bool HasTimedOut(TapGesture gesture)
	{
		if (MaxDuration > 0f && gesture.ElapsedTime > MaxDuration)
		{
			return true;
		}
		if (IsMultiTap && MaxDelayBetweenTaps > 0f && Time.time - gesture.LastTapTime > MaxDelayBetweenTaps)
		{
			return true;
		}
		return false;
	}

	protected override void Reset(TapGesture gesture)
	{
		gesture.Taps = 0;
		gesture.Down = false;
		gesture.WasDown = false;
		base.Reset(gesture);
	}

	protected override TapGesture MatchActiveGestureToCluster(FingerClusterManager.Cluster cluster)
	{
		if (IsMultiTap)
		{
			TapGesture tapGesture = FindClosestPendingGesture(cluster.Fingers.GetAveragePosition());
			if (tapGesture != null)
			{
				return tapGesture;
			}
		}
		return base.MatchActiveGestureToCluster(cluster);
	}

	private TapGesture FindClosestPendingGesture(Vector2 center)
	{
		TapGesture result = null;
		float num = float.PositiveInfinity;
		foreach (TapGesture gesture in base.Gestures)
		{
			if (gesture.State == GestureRecognitionState.InProgress && !gesture.Down)
			{
				float num2 = Vector2.SqrMagnitude(center - gesture.Position);
				if (num2 < MoveTolerance * MoveTolerance && num2 < num)
				{
					result = gesture;
					num = num2;
				}
			}
		}
		return result;
	}

	private GestureRecognitionState RecognizeSingleTap(TapGesture gesture, FingerGestures.IFingerList touches)
	{
		if (touches.Count != RequiredFingerCount)
		{
			if (touches.Count == 0)
			{
				return GestureRecognitionState.Ended;
			}
			return GestureRecognitionState.Failed;
		}
		if (HasTimedOut(gesture))
		{
			return GestureRecognitionState.Failed;
		}
		float num = Vector3.SqrMagnitude(touches.GetAveragePosition() - gesture.StartPosition);
		if (num >= MoveTolerance * MoveTolerance)
		{
			return GestureRecognitionState.Failed;
		}
		return GestureRecognitionState.InProgress;
	}

	private GestureRecognitionState RecognizeMultiTap(TapGesture gesture, FingerGestures.IFingerList touches)
	{
		gesture.WasDown = gesture.Down;
		gesture.Down = false;
		if (touches.Count == RequiredFingerCount)
		{
			gesture.Down = true;
			gesture.LastDownTime = Time.time;
		}
		else if (touches.Count == 0)
		{
			gesture.Down = false;
		}
		else if (touches.Count < RequiredFingerCount)
		{
			if (Time.time - gesture.LastDownTime > 0.25f)
			{
				return GestureRecognitionState.Failed;
			}
		}
		else if (!Young(touches))
		{
			return GestureRecognitionState.Failed;
		}
		if (HasTimedOut(gesture))
		{
			return GestureRecognitionState.Failed;
		}
		if (gesture.Down)
		{
			float num = Vector3.SqrMagnitude(touches.GetAveragePosition() - gesture.StartPosition);
			if (num >= MoveTolerance * MoveTolerance)
			{
				return GestureRecognitionState.Failed;
			}
		}
		if (gesture.WasDown != gesture.Down && !gesture.Down)
		{
			gesture.Taps++;
			gesture.LastTapTime = Time.time;
			if (gesture.Taps >= RequiredTaps)
			{
				return GestureRecognitionState.Ended;
			}
		}
		return GestureRecognitionState.InProgress;
	}

	public override string GetDefaultEventMessageName()
	{
		return "OnTap";
	}

	protected override void Begin(TapGesture gesture, int clusterId, FingerGestures.IFingerList touches)
	{
		base.Begin(gesture, clusterId, touches);
		if (!(gesture.Hit.collider != null))
		{
			return;
		}
		bool flag = true;
		if (UICamera.useInputEnabler)
		{
			UIInputEnabler component = gesture.Hit.collider.gameObject.GetComponent<UIInputEnabler>();
			if (component == null || !component.inputEnabled)
			{
				flag = false;
			}
		}
		if (flag)
		{
			gesture.Hit.collider.gameObject.SendMessage("OnBeginTap", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	protected override void OnBegin(TapGesture gesture, FingerGestures.IFingerList touches)
	{
		gesture.Position = touches.GetAveragePosition();
		gesture.StartPosition = gesture.Position;
		gesture.LastTapTime = Time.time;
	}

	protected override GestureRecognitionState OnRecognize(TapGesture gesture, FingerGestures.IFingerList touches)
	{
		return (!IsMultiTap) ? RecognizeSingleTap(gesture, touches) : RecognizeMultiTap(gesture, touches);
	}
}
