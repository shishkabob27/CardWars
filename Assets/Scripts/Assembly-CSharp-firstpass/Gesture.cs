using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Gesture
{
	public delegate void EventHandler(Gesture gesture);

	internal int ClusterId;

	private GestureRecognizer recognizer;

	private float startTime;

	private Vector2 startPosition = Vector2.zero;

	private Vector2 position = Vector2.zero;

	private GestureRecognitionState state;

	private GestureRecognitionState prevState;

	private FingerGestures.FingerList fingers = new FingerGestures.FingerList();

	private GameObject startSelection;

	private GameObject selection;

	private RaycastHit lastHit = default(RaycastHit);

	public FingerGestures.FingerList Fingers
	{
		get
		{
			return fingers;
		}
		internal set
		{
			fingers = value;
		}
	}

	public GestureRecognizer Recognizer
	{
		get
		{
			return recognizer;
		}
		internal set
		{
			recognizer = value;
		}
	}

	public float StartTime
	{
		get
		{
			return startTime;
		}
		internal set
		{
			startTime = value;
		}
	}

	public Vector2 StartPosition
	{
		get
		{
			return startPosition;
		}
		internal set
		{
			startPosition = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public GestureRecognitionState State
	{
		get
		{
			return state;
		}
		set
		{
			if (state != value)
			{
				prevState = state;
				state = value;
				if (this.OnStateChanged != null)
				{
					this.OnStateChanged(this);
				}
			}
		}
	}

	public GestureRecognitionState PreviousState
	{
		get
		{
			return prevState;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return Time.time - StartTime;
		}
	}

	public GameObject StartSelection
	{
		get
		{
			return startSelection;
		}
		internal set
		{
			startSelection = value;
		}
	}

	public GameObject Selection
	{
		get
		{
			return selection;
		}
		internal set
		{
			selection = value;
		}
	}

	public RaycastHit Hit
	{
		get
		{
			return lastHit;
		}
		internal set
		{
			lastHit = value;
		}
	}

	[method: MethodImpl(32)]
	public event EventHandler OnStateChanged;

	public GameObject PickObject(ScreenRaycaster raycaster, Vector2 screenPos)
	{
		if (!raycaster || !raycaster.enabled)
		{
			return null;
		}
		if (!raycaster.Raycast(screenPos, out lastHit))
		{
			return null;
		}
		if (UICamera.useInputEnabler)
		{
			UIInputEnabler component = lastHit.collider.gameObject.GetComponent<UIInputEnabler>();
			if (component == null || !component.inputEnabled)
			{
				return null;
			}
		}
		return lastHit.collider.gameObject;
	}

	internal void PickStartSelection(ScreenRaycaster raycaster)
	{
		StartSelection = PickObject(raycaster, StartPosition);
		Selection = StartSelection;
	}

	internal void PickSelection(ScreenRaycaster raycaster)
	{
		Selection = PickObject(raycaster, Position);
	}
}
