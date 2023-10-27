using UnityEngine;

public class TBQuickSetup : MonoBehaviour
{
	public GameObject MessageTarget;
	public int MaxSimultaneousGestures;
	public FingerDownDetector FingerDown;
	public FingerUpDetector FingerUp;
	public FingerHoverDetector FingerHover;
	public FingerMotionDetector FingerMotion;
	public DragRecognizer Drag;
	public LongPressRecognizer LongPress;
	public SwipeRecognizer Swipe;
	public TapRecognizer Tap;
	public PinchRecognizer Pinch;
	public TwistRecognizer Twist;
	public TapRecognizer DoubleTap;
	public DragRecognizer TwoFingerDrag;
	public TapRecognizer TwoFingerTap;
	public SwipeRecognizer TwoFingerSwipe;
	public LongPressRecognizer TwoFingerLongPress;
}
