using UnityEngine;

[AddComponentMenu("FingerGestures/Toolbox/Quick Setup")]
public class TBQuickSetup : MonoBehaviour
{
	public GameObject MessageTarget;

	public int MaxSimultaneousGestures = 2;

	private ScreenRaycaster screenRaycaster;

	[HideInInspector]
	public FingerDownDetector FingerDown;

	[HideInInspector]
	public FingerUpDetector FingerUp;

	[HideInInspector]
	public FingerHoverDetector FingerHover;

	[HideInInspector]
	public FingerMotionDetector FingerMotion;

	[HideInInspector]
	public DragRecognizer Drag;

	[HideInInspector]
	public LongPressRecognizer LongPress;

	[HideInInspector]
	public SwipeRecognizer Swipe;

	[HideInInspector]
	public TapRecognizer Tap;

	[HideInInspector]
	public PinchRecognizer Pinch;

	[HideInInspector]
	public TwistRecognizer Twist;

	[HideInInspector]
	public TapRecognizer DoubleTap;

	[HideInInspector]
	public DragRecognizer TwoFingerDrag;

	[HideInInspector]
	public TapRecognizer TwoFingerTap;

	[HideInInspector]
	public SwipeRecognizer TwoFingerSwipe;

	[HideInInspector]
	public LongPressRecognizer TwoFingerLongPress;

	private GameObject CreateChildNode(string name)
	{
		GameObject gameObject = new GameObject(name);
		Transform transform = gameObject.transform;
		transform.parent = base.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return gameObject;
	}

	private void Start()
	{
		if (!MessageTarget)
		{
			MessageTarget = base.gameObject;
		}
		screenRaycaster = GetComponent<ScreenRaycaster>();
		if (!screenRaycaster)
		{
			screenRaycaster = base.gameObject.AddComponent<ScreenRaycaster>();
		}
		if (!FingerGestures.Instance)
		{
			base.gameObject.AddComponent<FingerGestures>();
		}
		GameObject node = CreateChildNode("Finger Event Detectors");
		FingerDown = AddFingerEventDetector<FingerDownDetector>(node);
		FingerUp = AddFingerEventDetector<FingerUpDetector>(node);
		FingerMotion = AddFingerEventDetector<FingerMotionDetector>(node);
		FingerHover = AddFingerEventDetector<FingerHoverDetector>(node);
		GameObject node2 = CreateChildNode("Single Finger Gestures");
		Drag = AddSingleFingerGesture<DragRecognizer>(node2);
		Tap = AddSingleFingerGesture<TapRecognizer>(node2);
		Swipe = AddSingleFingerGesture<SwipeRecognizer>(node2);
		LongPress = AddSingleFingerGesture<LongPressRecognizer>(node2);
		DoubleTap = AddSingleFingerGesture<TapRecognizer>(node2);
		DoubleTap.RequiredTaps = 2;
		DoubleTap.EventMessageName = "OnDoubleTap";
		GameObject node3 = CreateChildNode("Two-Finger Gestures");
		Pinch = AddTwoFingerGesture<PinchRecognizer>(node3);
		Twist = AddTwoFingerGesture<TwistRecognizer>(node3);
		TwoFingerDrag = AddTwoFingerGesture<DragRecognizer>(node3, "OnTwoFingerDrag");
		TwoFingerTap = AddTwoFingerGesture<TapRecognizer>(node3, "OnTwoFingerTap");
		TwoFingerSwipe = AddTwoFingerGesture<SwipeRecognizer>(node3, "OnTwoFingerSwipe");
		TwoFingerLongPress = AddTwoFingerGesture<LongPressRecognizer>(node3, "OnTwoFingerLongPress");
	}

	private T AddFingerEventDetector<T>(GameObject node) where T : FingerEventDetector
	{
		T val = node.AddComponent<T>();
		val.Raycaster = screenRaycaster;
		val.MessageTarget = MessageTarget;
		return val;
	}

	private T AddGesture<T>(GameObject node) where T : GestureRecognizer
	{
		T val = node.AddComponent<T>();
		val.Raycaster = screenRaycaster;
		val.EventMessageTarget = MessageTarget;
		if (val.SupportFingerClustering)
		{
			val.MaxSimultaneousGestures = MaxSimultaneousGestures;
		}
		return val;
	}

	private T AddSingleFingerGesture<T>(GameObject node) where T : GestureRecognizer
	{
		T result = AddGesture<T>(node);
		result.RequiredFingerCount = 1;
		return result;
	}

	private T AddTwoFingerGesture<T>(GameObject node) where T : GestureRecognizer
	{
		T result = AddGesture<T>(node);
		result.RequiredFingerCount = 2;
		return result;
	}

	private T AddTwoFingerGesture<T>(GameObject node, string eventName) where T : GestureRecognizer
	{
		T val = AddTwoFingerGesture<T>(node);
		val.EventMessageName = eventName;
		return val;
	}
}
