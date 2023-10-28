using UnityEngine;

[RequireComponent(typeof(EmitParticles))]
public class EmitParticlesOnSwipe : MonoBehaviour
{
	public bool constrained;

	private EmitParticles emitter;

	private void Awake()
	{
		emitter = GetComponent<EmitParticles>();
	}

	private void OnSwipe(SwipeGesture gesture)
	{
		if (constrained)
		{
			switch (gesture.Direction)
			{
			case FingerGestures.SwipeDirection.Up:
				emitter.EmitUp();
				break;
			case FingerGestures.SwipeDirection.Right:
				emitter.EmitRight();
				break;
			case FingerGestures.SwipeDirection.Down:
				emitter.EmitDown();
				break;
			case FingerGestures.SwipeDirection.Left:
				emitter.EmitLeft();
				break;
			case FingerGestures.SwipeDirection.Horizontal:
			case FingerGestures.SwipeDirection.Right | FingerGestures.SwipeDirection.Up:
			case FingerGestures.SwipeDirection.Left | FingerGestures.SwipeDirection.Up:
			case FingerGestures.SwipeDirection.Horizontal | FingerGestures.SwipeDirection.Up:
				break;
			}
		}
		else
		{
			Vector3 dir = new Vector3(gesture.Move.x, gesture.Move.y, 0f);
			emitter.Emit(dir);
		}
	}
}
