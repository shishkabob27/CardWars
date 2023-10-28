using UnityEngine;

public class BasicGesturesSample : SampleBase
{
	public GameObject longPressObject;

	public GameObject tapObject;

	public GameObject doubleTapObject;

	public GameObject swipeObject;

	public GameObject dragObject;

	private int dragFingerIndex = -1;

	private void OnLongPress(LongPressGesture gesture)
	{
		if (gesture.Selection == longPressObject)
		{
			SpawnParticles(longPressObject);
			base.UI.StatusText = "Performed a long-press with finger " + gesture.Fingers[0];
		}
	}

	private void OnTap(TapGesture gesture)
	{
		if (gesture.Selection == tapObject)
		{
			SpawnParticles(tapObject);
			base.UI.StatusText = "Tapped with finger " + gesture.Fingers[0];
		}
	}

	private void OnDoubleTap(TapGesture gesture)
	{
		if (gesture.Selection == doubleTapObject)
		{
			SpawnParticles(doubleTapObject);
			base.UI.StatusText = "Double-Tapped with finger " + gesture.Fingers[0];
		}
	}

	private void OnSwipe(SwipeGesture gesture)
	{
		GameObject startSelection = gesture.StartSelection;
		if (startSelection == swipeObject)
		{
			base.UI.StatusText = string.Concat("Swiped ", gesture.Direction, " with finger ", gesture.Fingers[0], " (velocity:", gesture.Velocity, ", distance: ", gesture.Move.magnitude, " )");
			SwipeParticlesEmitter componentInChildren = startSelection.GetComponentInChildren<SwipeParticlesEmitter>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Emit(gesture.Direction, gesture.Velocity);
			}
		}
	}

	private void OnDrag(DragGesture gesture)
	{
		FingerGestures.Finger finger = gesture.Fingers[0];
		if (gesture.Phase == ContinuousGesturePhase.Started)
		{
			if (!(gesture.Selection != dragObject))
			{
				base.UI.StatusText = "Started dragging with finger " + finger;
				dragFingerIndex = finger.Index;
				SpawnParticles(dragObject);
			}
		}
		else if (finger.Index == dragFingerIndex)
		{
			if (gesture.Phase == ContinuousGesturePhase.Updated)
			{
				dragObject.transform.position = SampleBase.GetWorldPos(gesture.Position);
				return;
			}
			base.UI.StatusText = "Stopped dragging with finger " + finger;
			dragFingerIndex = -1;
			SpawnParticles(dragObject);
		}
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates some of the supported single-finger gestures:\r\n\r\n- Drag: press the red sphere and move your finger to drag it around  \r\n\r\n- LongPress: keep your finger pressed on the cyan sphere for a few seconds\r\n\r\n- Tap: press & release the purple sphere \r\n\r\n- Double Tap: quickly press & release the green sphere twice in a row\r\n\r\n- Swipe: press the yellow sphere and move your finger in one of the four cardinal directions, then release. The speed of the motion is taken into account.";
	}

	private void SpawnParticles(GameObject obj)
	{
		ParticleEmitter componentInChildren = obj.GetComponentInChildren<ParticleEmitter>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit();
		}
	}
}
