using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("FingerGestures/Finger Gestures Singleton")]
public class FingerGestures : MonoBehaviour
{
	public enum FingerPhase
	{
		None,
		Begin,
		Moving,
		Stationary
	}

	public class InputProviderEvent
	{
		public FGInputProvider inputProviderPrefab;
	}

	public class Finger
	{
		private int index;

		private FingerPhase phase;

		private FingerPhase prevPhase;

		private Vector2 pos = Vector2.zero;

		private Vector2 startPos = Vector2.zero;

		private Vector2 prevPos = Vector2.zero;

		private Vector2 deltaPos = Vector2.zero;

		private float startTime;

		private float lastMoveTime;

		private float distFromStart;

		private bool moved;

		private bool filteredOut = true;

		private Collider collider;

		private Collider prevCollider;

		private float elapsedTimeStationary;

		private List<GestureRecognizer> gestureRecognizers = new List<GestureRecognizer>();

		private Dictionary<string, object> extendedProperties = new Dictionary<string, object>();

		public int Index
		{
			get
			{
				return index;
			}
		}

		public bool IsDown
		{
			get
			{
				return phase != FingerPhase.None;
			}
		}

		public FingerPhase Phase
		{
			get
			{
				return phase;
			}
		}

		public FingerPhase PreviousPhase
		{
			get
			{
				return prevPhase;
			}
		}

		public bool WasDown
		{
			get
			{
				return prevPhase != FingerPhase.None;
			}
		}

		public bool IsMoving
		{
			get
			{
				return phase == FingerPhase.Moving;
			}
		}

		public bool WasMoving
		{
			get
			{
				return prevPhase == FingerPhase.Moving;
			}
		}

		public bool IsStationary
		{
			get
			{
				return phase == FingerPhase.Stationary;
			}
		}

		public bool WasStationary
		{
			get
			{
				return prevPhase == FingerPhase.Stationary;
			}
		}

		public bool Moved
		{
			get
			{
				return moved;
			}
		}

		public float StarTime
		{
			get
			{
				return startTime;
			}
		}

		public Vector2 StartPosition
		{
			get
			{
				return startPos;
			}
		}

		public Vector2 Position
		{
			get
			{
				return pos;
			}
		}

		public Vector2 PreviousPosition
		{
			get
			{
				return prevPos;
			}
		}

		public Vector2 DeltaPosition
		{
			get
			{
				return deltaPos;
			}
		}

		public float DistanceFromStart
		{
			get
			{
				return distFromStart;
			}
		}

		public bool IsFiltered
		{
			get
			{
				return filteredOut;
			}
		}

		public float TimeStationary
		{
			get
			{
				return elapsedTimeStationary;
			}
		}

		public List<GestureRecognizer> GestureRecognizers
		{
			get
			{
				return gestureRecognizers;
			}
		}

		public Dictionary<string, object> ExtendedProperties
		{
			get
			{
				return extendedProperties;
			}
		}

		public Finger(int index)
		{
			this.index = index;
		}

		public override string ToString()
		{
			return "Finger" + index;
		}

		internal void Update(bool newDownState, Vector2 newPos)
		{
			if (filteredOut && !newDownState)
			{
				filteredOut = false;
			}
			if (!IsDown && newDownState && !instance.ShouldProcessTouch(index, newPos))
			{
				filteredOut = true;
				newDownState = false;
			}
			prevPhase = phase;
			if (newDownState)
			{
				if (!WasDown)
				{
					phase = FingerPhase.Begin;
					pos = newPos;
					startPos = pos;
					prevPos = pos;
					deltaPos = Vector2.zero;
					moved = false;
					lastMoveTime = 0f;
					startTime = Time.time;
					elapsedTimeStationary = 0f;
					return;
				}
				prevPos = pos;
				pos = newPos;
				distFromStart = Vector3.Distance(startPos, pos);
				deltaPos = pos - prevPos;
				if (deltaPos.sqrMagnitude > 0f)
				{
					lastMoveTime = Time.time;
					phase = FingerPhase.Moving;
				}
				else if (!IsMoving || Time.time - lastMoveTime > 0.05f)
				{
					phase = FingerPhase.Stationary;
				}
				if (IsMoving)
				{
					moved = true;
				}
				else if (!WasStationary)
				{
					elapsedTimeStationary = 0f;
				}
				else
				{
					elapsedTimeStationary += Time.deltaTime;
				}
			}
			else
			{
				phase = FingerPhase.None;
			}
		}
	}

	public interface IFingerList : IEnumerable, IEnumerable<Finger>
	{
		Finger this[int index] { get; }

		int Count { get; }

		Vector2 GetAverageStartPosition();

		Vector2 GetAveragePosition();

		Vector2 GetAveragePreviousPosition();

		float GetAverageDistanceFromStart();

		Finger GetOldest();

		bool AllMoving();

		bool MovingInSameDirection(float tolerance);
	}

	[Serializable]
	public class FingerList : IEnumerable, IEnumerable<Finger>, IFingerList
	{
		public delegate T FingerPropertyGetterDelegate<T>(Finger finger);

		[SerializeField]
		private List<Finger> list;

		public Finger this[int index]
		{
			get
			{
				return list[index];
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public FingerList()
		{
			list = new List<Finger>();
		}

		public FingerList(List<Finger> list)
		{
			this.list = list;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Finger> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void Add(Finger touch)
		{
			list.Add(touch);
		}

		public bool Remove(Finger touch)
		{
			return list.Remove(touch);
		}

		public bool Contains(Finger touch)
		{
			return list.Contains(touch);
		}

		public void AddRange(IEnumerable<Finger> touches)
		{
			list.AddRange(touches);
		}

		public void Clear()
		{
			list.Clear();
		}

		public Vector2 AverageVector(FingerPropertyGetterDelegate<Vector2> getProperty)
		{
			Vector2 zero = Vector2.zero;
			if (Count > 0)
			{
				foreach (Finger item in list)
				{
					zero += getProperty(item);
				}
				zero /= (float)Count;
			}
			return zero;
		}

		public float AverageFloat(FingerPropertyGetterDelegate<float> getProperty)
		{
			float num = 0f;
			if (Count > 0)
			{
				foreach (Finger item in list)
				{
					num += getProperty(item);
				}
				num /= (float)Count;
			}
			return num;
		}

		private static Vector2 GetFingerStartPosition(Finger finger)
		{
			return finger.StartPosition;
		}

		private static Vector2 GetFingerPosition(Finger finger)
		{
			return finger.Position;
		}

		private static Vector2 GetFingerPreviousPosition(Finger finger)
		{
			return finger.PreviousPosition;
		}

		private static float GetFingerDistanceFromStart(Finger finger)
		{
			return finger.DistanceFromStart;
		}

		public Vector2 GetAverageStartPosition()
		{
			return AverageVector(GetFingerStartPosition);
		}

		public Vector2 GetAveragePosition()
		{
			return AverageVector(GetFingerPosition);
		}

		public Vector2 GetAveragePreviousPosition()
		{
			return AverageVector(GetFingerPreviousPosition);
		}

		public float GetAverageDistanceFromStart()
		{
			return AverageFloat(GetFingerDistanceFromStart);
		}

		public Finger GetOldest()
		{
			Finger finger = null;
			foreach (Finger item in list)
			{
				if (finger == null || item.StarTime < finger.StarTime)
				{
					finger = item;
				}
			}
			return finger;
		}

		public bool MovingInSameDirection(float tolerance)
		{
			if (Count < 2)
			{
				return true;
			}
			float num = Mathf.Max(0.1f, 1f - tolerance);
			Vector2 lhs = this[0].Position - this[0].StartPosition;
			lhs.Normalize();
			for (int i = 1; i < Count; i++)
			{
				Vector2 rhs = this[i].Position - this[i].StartPosition;
				rhs.Normalize();
				if (Vector2.Dot(lhs, rhs) < num)
				{
					return false;
				}
			}
			return true;
		}

		public bool AllMoving()
		{
			if (Count == 0)
			{
				return false;
			}
			foreach (Finger item in list)
			{
				if (!item.IsMoving)
				{
					return false;
				}
			}
			return true;
		}
	}

	[Flags]
	public enum SwipeDirection
	{
		Right = 1,
		Left = 2,
		Up = 4,
		Down = 8,
		UpperLeftDiagonal = 0x10,
		UpperRightDiagonal = 0x20,
		LowerRightDiagonal = 0x40,
		LowerLeftDiagonal = 0x80,
		None = 0,
		Vertical = 0xC,
		Horizontal = 3,
		Cross = 0xF,
		UpperDiagonals = 0x30,
		LowerDiagonals = 0xC0,
		Diagonals = 0xF0,
		All = 0xFF
	}

	public delegate bool GlobalTouchFilterDelegate(int fingerIndex, Vector2 position);

	public bool makePersistent = true;

	public bool detectUnityRemote = true;

	public FGInputProvider mouseInputProviderPrefab;

	public FGInputProvider touchInputProviderPrefab;

	private FingerClusterManager fingerClusterManager;

	private FGInputProvider inputProvider;

	private static List<GestureRecognizer> gestureRecognizers = new List<GestureRecognizer>();

	private float pixelDistanceScale = 1f;

	public bool adjustPixelScaleForRetinaDisplay = true;

	public float retinaPixelScale = 0.5f;

	private static FingerGestures instance;

	private Finger[] fingers;

	private FingerList touches;

	private GlobalTouchFilterDelegate globalTouchFilterFunc;

	private Transform[] fingerNodes;

	private static readonly SwipeDirection[] AngleToDirectionMap = new SwipeDirection[8]
	{
		SwipeDirection.Right,
		SwipeDirection.UpperRightDiagonal,
		SwipeDirection.Up,
		SwipeDirection.UpperLeftDiagonal,
		SwipeDirection.Left,
		SwipeDirection.LowerLeftDiagonal,
		SwipeDirection.Down,
		SwipeDirection.LowerRightDiagonal
	};

	public static FingerClusterManager DefaultClusterManager
	{
		get
		{
			return Instance.fingerClusterManager;
		}
	}

	public static FingerGestures Instance
	{
		get
		{
			return instance;
		}
	}

	public FGInputProvider InputProvider
	{
		get
		{
			return inputProvider;
		}
	}

	public int MaxFingers
	{
		get
		{
			return inputProvider.MaxSimultaneousFingers;
		}
	}

	public static IFingerList Touches
	{
		get
		{
			return instance.touches;
		}
	}

	public static List<GestureRecognizer> RegisteredGestureRecognizers
	{
		get
		{
			return gestureRecognizers;
		}
	}

	public static float PixelDistanceScale
	{
		get
		{
			return instance.pixelDistanceScale;
		}
		set
		{
			instance.pixelDistanceScale = value;
		}
	}

	public static GlobalTouchFilterDelegate GlobalTouchFilter
	{
		get
		{
			return instance.globalTouchFilterFunc;
		}
		set
		{
			instance.globalTouchFilterFunc = value;
		}
	}

	[method: MethodImpl(32)]
	public static event Gesture.EventHandler OnGestureEvent;

	[method: MethodImpl(32)]
	public static event FingerEventDetector<FingerEvent>.FingerEventHandler OnFingerEvent;

	internal static void FireEvent(Gesture gesture)
	{
		if (FingerGestures.OnGestureEvent != null)
		{
			FingerGestures.OnGestureEvent(gesture);
		}
	}

	internal static void FireEvent(FingerEvent eventData)
	{
		if (FingerGestures.OnFingerEvent != null)
		{
			FingerGestures.OnFingerEvent(eventData);
		}
	}

	private void Init()
	{
		if (adjustPixelScaleForRetinaDisplay && IsRetinaDisplay())
		{
			PixelDistanceScale = retinaPixelScale;
		}
		InitInputProvider();
		fingerClusterManager = GetComponent<FingerClusterManager>();
		if (!fingerClusterManager)
		{
			fingerClusterManager = base.gameObject.AddComponent<FingerClusterManager>();
		}
	}

	private void InitInputProvider()
	{
		InputProviderEvent inputProviderEvent = new InputProviderEvent();
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.Android:
			inputProviderEvent.inputProviderPrefab = touchInputProviderPrefab;
			break;
		default:
			inputProviderEvent.inputProviderPrefab = mouseInputProviderPrefab;
			break;
		}
		base.gameObject.SendMessage("OnSelectInputProvider", inputProviderEvent, SendMessageOptions.DontRequireReceiver);
		InstallInputProvider(inputProviderEvent.inputProviderPrefab);
	}

	public void InstallInputProvider(FGInputProvider inputProviderPrefab)
	{
		if ((bool)inputProviderPrefab)
		{
			if ((bool)inputProvider)
			{
				UnityEngine.Object.Destroy(inputProvider.gameObject);
			}
			inputProvider = UnityEngine.Object.Instantiate(inputProviderPrefab);
			inputProvider.name = inputProviderPrefab.name;
			inputProvider.transform.parent = base.transform;
			InitFingers(MaxFingers);
		}
	}

	public static Finger GetFinger(int index)
	{
		return instance.fingers[index];
	}

	public static void Register(GestureRecognizer recognizer)
	{
		if (!gestureRecognizers.Contains(recognizer))
		{
			gestureRecognizers.Add(recognizer);
		}
	}

	public static void Unregister(GestureRecognizer recognizer)
	{
		gestureRecognizers.Remove(recognizer);
	}

	public static float GetAdjustedPixelDistance(float rawPixelDistance)
	{
		return PixelDistanceScale * rawPixelDistance;
	}

	private bool IsRetinaDisplay()
	{
		return false;
	}

	private void Awake()
	{
		CheckInit();
	}

	private void Start()
	{
		if (makePersistent)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnEnable()
	{
		CheckInit();
	}

	private void CheckInit()
	{
		if (instance == null)
		{
			instance = this;
			Init();
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if ((bool)inputProvider)
		{
			UpdateFingers();
		}
	}

	private void InitFingers(int count)
	{
		fingers = new Finger[count];
		for (int i = 0; i < count; i++)
		{
			fingers[i] = new Finger(i);
		}
		touches = new FingerList();
	}

	private void UpdateFingers()
	{
		touches.Clear();
		Finger[] array = fingers;
		foreach (Finger finger in array)
		{
			Vector2 position = Vector2.zero;
			bool down = false;
			inputProvider.GetInputState(finger.Index, out down, out position);
			finger.Update(down, position);
			if (finger.IsDown)
			{
				touches.Add(finger);
			}
		}
	}

	protected bool ShouldProcessTouch(int fingerIndex, Vector2 position)
	{
		if (globalTouchFilterFunc != null)
		{
			return globalTouchFilterFunc(fingerIndex, position);
		}
		return true;
	}

	private Transform CreateNode(string name, Transform parent)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = parent;
		return gameObject.transform;
	}

	private void InitNodes()
	{
		int num = fingers.Length;
		if (fingerNodes != null)
		{
			Transform[] array = fingerNodes;
			foreach (Transform transform in array)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		fingerNodes = new Transform[num];
		for (int j = 0; j < fingerNodes.Length; j++)
		{
			fingerNodes[j] = CreateNode("Finger" + j, base.transform);
		}
	}

	public static SwipeDirection GetSwipeDirection(Vector2 dir, float tolerance)
	{
		float num = Mathf.Max(Mathf.Clamp01(tolerance) * 22.5f, 0.0001f);
		float num2 = NormalizeAngle360(57.29578f * Mathf.Atan2(dir.y, dir.x));
		if (num2 >= 337.5f)
		{
			num2 -= 360f;
		}
		for (int i = 0; i < 8; i++)
		{
			float num3 = 45f * (float)i;
			if (num2 <= num3 + 22.5f)
			{
				float num4 = num3 - num;
				float num5 = num3 + num;
				if (num2 >= num4 && num2 <= num5)
				{
					return AngleToDirectionMap[i];
				}
				break;
			}
		}
		return SwipeDirection.None;
	}

	public static SwipeDirection GetSwipeDirection(Vector2 dir)
	{
		return GetSwipeDirection(dir, 1f);
	}

	public static bool UsingUnityRemote()
	{
		return false;
	}

	public static bool AllFingersMoving(params Finger[] fingers)
	{
		if (fingers.Length == 0)
		{
			return false;
		}
		foreach (Finger finger in fingers)
		{
			if (!finger.IsMoving)
			{
				return false;
			}
		}
		return true;
	}

	public static bool FingersMovedInOppositeDirections(Finger finger0, Finger finger1, float minDOT)
	{
		float num = Vector2.Dot(finger0.DeltaPosition.normalized, finger1.DeltaPosition.normalized);
		return num < minDOT;
	}

	public static float SignedAngle(Vector2 from, Vector2 to)
	{
		float y = from.x * to.y - from.y * to.x;
		return Mathf.Atan2(y, Vector2.Dot(from, to));
	}

	public static float NormalizeAngle360(float angleInDegrees)
	{
		angleInDegrees %= 360f;
		if (angleInDegrees < 0f)
		{
			angleInDegrees += 360f;
		}
		return angleInDegrees;
	}
}
