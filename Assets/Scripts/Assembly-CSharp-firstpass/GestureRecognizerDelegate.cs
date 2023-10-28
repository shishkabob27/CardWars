using UnityEngine;

public abstract class GestureRecognizerDelegate : MonoBehaviour
{
	public abstract bool CanBegin(Gesture gesture, FingerGestures.IFingerList touches);
}
