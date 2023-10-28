using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class GestureRecognizer<T> : GestureRecognizer where T : Gesture, new()
{
	public delegate void GestureEventHandler(T gesture);

	private List<T> gestures;

	private static FingerGestures.FingerList tempTouchList = new FingerGestures.FingerList();

	public List<T> Gestures
	{
		get
		{
			return gestures;
		}
	}

	[method: MethodImpl(32)]
	public event GestureEventHandler OnGesture;

	protected override void Start()
	{
		base.Start();
		InitGestures();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	private void InitGestures()
	{
		if (gestures == null)
		{
			gestures = new List<T>();
			for (int i = 0; i < MaxSimultaneousGestures; i++)
			{
				T item = CreateGesture();
				item.OnStateChanged += OnStateChanged;
				item.Recognizer = this;
				gestures.Add(item);
			}
		}
	}

	protected virtual bool CanBegin(T gesture, FingerGestures.IFingerList touches)
	{
		if (touches.Count != RequiredFingerCount)
		{
			return false;
		}
		if (IsExclusive && FingerGestures.Touches.Count != RequiredFingerCount)
		{
			return false;
		}
		if ((bool)Delegate && !Delegate.CanBegin(gesture, touches))
		{
			return false;
		}
		if (UICamera.useInputEnabler)
		{
			UIInputEnabler uIInputEnabler = ((gesture != null && !(gesture.Hit.collider == null)) ? gesture.Hit.collider.gameObject.GetComponent<UIInputEnabler>() : null);
			if (uIInputEnabler == null || !uIInputEnabler.inputEnabled)
			{
				return false;
			}
		}
		return true;
	}

	protected abstract void OnBegin(T gesture, FingerGestures.IFingerList touches);

	protected abstract GestureRecognitionState OnRecognize(T gesture, FingerGestures.IFingerList touches);

	protected virtual GameObject GetDefaultSelectionForSendMessage(T gesture)
	{
		return gesture.Selection;
	}

	protected virtual T CreateGesture()
	{
		return new T();
	}

	public override Type GetGestureType()
	{
		return typeof(T);
	}

	protected virtual void OnStateChanged(Gesture gesture)
	{
	}

	protected virtual T FindGestureByCluster(FingerClusterManager.Cluster cluster)
	{
		return gestures.Find((T g) => g.ClusterId == cluster.Id);
	}

	protected virtual T MatchActiveGestureToCluster(FingerClusterManager.Cluster cluster)
	{
		return (T)null;
	}

	protected virtual T FindFreeGesture()
	{
		return gestures.Find((T g) => g.State == GestureRecognitionState.Ready);
	}

	protected virtual void Reset(T gesture)
	{
		ReleaseFingers(gesture);
		gesture.ClusterId = 0;
		gesture.Fingers.Clear();
		gesture.State = GestureRecognitionState.Ready;
	}

	public virtual void Update()
	{
		if (!IsExclusive && SupportFingerClustering && (bool)ClusterManager)
		{
			UpdateUsingClusters();
		}
		else if (IsExclusive || RequiredFingerCount > 1)
		{
			UpdateExclusive();
		}
		else
		{
			UpdatePerFinger();
		}
	}

	private void UpdateUsingClusters()
	{
		ClusterManager.Update();
		foreach (FingerClusterManager.Cluster cluster2 in ClusterManager.Clusters)
		{
			ProcessCluster(cluster2);
		}
		foreach (T gesture in gestures)
		{
			FingerClusterManager.Cluster cluster = ClusterManager.FindClusterById(gesture.ClusterId);
			FingerGestures.IFingerList fingerList;
			if (cluster != null)
			{
				FingerGestures.IFingerList fingers = cluster.Fingers;
				fingerList = fingers;
			}
			else
			{
				fingerList = GestureRecognizer.EmptyFingerList;
			}
			FingerGestures.IFingerList touches = fingerList;
			UpdateGesture(gesture, touches);
		}
	}

	private void UpdateExclusive()
	{
		T gesture = gestures[0];
		FingerGestures.IFingerList touches = FingerGestures.Touches;
		if (gesture.State == GestureRecognitionState.Ready && CanBegin(gesture, touches))
		{
			Begin(gesture, 0, touches);
		}
		UpdateGesture(gesture, touches);
	}

	private void UpdatePerFinger()
	{
		for (int i = 0; i < FingerGestures.Instance.MaxFingers && i < MaxSimultaneousGestures; i++)
		{
			FingerGestures.Finger finger = FingerGestures.GetFinger(i);
			T gesture = Gestures[i];
			FingerGestures.FingerList fingerList = tempTouchList;
			fingerList.Clear();
			if (finger.IsDown)
			{
				fingerList.Add(finger);
			}
			if (gesture.State == GestureRecognitionState.Ready && CanBegin(gesture, fingerList))
			{
				Begin(gesture, 0, fingerList);
			}
			UpdateGesture(gesture, fingerList);
		}
	}

	protected virtual void ProcessCluster(FingerClusterManager.Cluster cluster)
	{
		if (FindGestureByCluster(cluster) != null || cluster.Fingers.Count != RequiredFingerCount)
		{
			return;
		}
		T val = MatchActiveGestureToCluster(cluster);
		if (val != null)
		{
			val.ClusterId = cluster.Id;
			return;
		}
		val = FindFreeGesture();
		if (val != null && CanBegin(val, cluster.Fingers))
		{
			Begin(val, cluster.Id, cluster.Fingers);
		}
	}

	private void ReleaseFingers(T gesture)
	{
		foreach (FingerGestures.Finger finger in gesture.Fingers)
		{
			Release(finger);
		}
	}

	protected virtual void Begin(T gesture, int clusterId, FingerGestures.IFingerList touches)
	{
		gesture.ClusterId = clusterId;
		gesture.StartTime = Time.time;
		foreach (FingerGestures.Finger touch in touches)
		{
			gesture.Fingers.Add(touch);
			Acquire(touch);
		}
		OnBegin(gesture, touches);
		gesture.PickStartSelection(Raycaster);
		gesture.State = GestureRecognitionState.Started;
	}

	protected virtual FingerGestures.IFingerList GetTouches(T gesture)
	{
		if (SupportFingerClustering && (bool)ClusterManager)
		{
			FingerClusterManager.Cluster cluster = ClusterManager.FindClusterById(gesture.ClusterId);
			FingerGestures.IFingerList result;
			if (cluster != null)
			{
				FingerGestures.IFingerList fingers = cluster.Fingers;
				result = fingers;
			}
			else
			{
				result = GestureRecognizer.EmptyFingerList;
			}
			return result;
		}
		return FingerGestures.Touches;
	}

	protected virtual void UpdateGesture(T gesture, FingerGestures.IFingerList touches)
	{
		if (gesture.State == GestureRecognitionState.Ready)
		{
			return;
		}
		if (gesture.State == GestureRecognitionState.Started)
		{
			gesture.State = GestureRecognitionState.InProgress;
		}
		switch (gesture.State)
		{
		case GestureRecognitionState.InProgress:
		{
			GestureRecognitionState gestureRecognitionState = OnRecognize(gesture, touches);
			if (gestureRecognitionState == GestureRecognitionState.InProgress)
			{
				gesture.PickSelection(Raycaster);
			}
			gesture.State = gestureRecognitionState;
			break;
		}
		case GestureRecognitionState.Failed:
		case GestureRecognitionState.Ended:
			if (gesture.PreviousState != gesture.State)
			{
				ReleaseFingers(gesture);
			}
			if (ResetMode == GestureResetMode.NextFrame || (ResetMode == GestureResetMode.EndOfTouchSequence && touches.Count == 0))
			{
				Reset(gesture);
			}
			break;
		default:
			gesture.State = GestureRecognitionState.Failed;
			break;
		}
	}

	protected void RaiseEvent(T gesture)
	{
		if (this.OnGesture != null)
		{
			this.OnGesture(gesture);
		}
		FingerGestures.FireEvent(gesture);
		if (!UseSendMessage || string.IsNullOrEmpty(EventMessageName))
		{
			return;
		}
		if ((bool)EventMessageTarget)
		{
			EventMessageTarget.SendMessage(EventMessageName, gesture, SendMessageOptions.DontRequireReceiver);
		}
		if (SendMessageToSelection != SelectionType.None)
		{
			GameObject gameObject = null;
			switch (SendMessageToSelection)
			{
			case SelectionType.Default:
				gameObject = GetDefaultSelectionForSendMessage(gesture);
				break;
			case SelectionType.CurrentSelection:
				gameObject = gesture.Selection;
				break;
			case SelectionType.StartSelection:
				gameObject = gesture.StartSelection;
				break;
			}
			if ((bool)gameObject && gameObject != EventMessageTarget)
			{
				gameObject.SendMessage(EventMessageName, gesture, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
public abstract class GestureRecognizer : MonoBehaviour
{
	public enum SelectionType
	{
		Default,
		StartSelection,
		CurrentSelection,
		None
	}

	protected static readonly FingerGestures.IFingerList EmptyFingerList = new FingerGestures.FingerList();

	[SerializeField]
	private int requiredFingerCount = 1;

	public int MaxSimultaneousGestures = 1;

	public GestureResetMode ResetMode;

	public ScreenRaycaster Raycaster;

	public FingerClusterManager ClusterManager;

	public GestureRecognizerDelegate Delegate;

	public bool UseSendMessage = true;

	public string EventMessageName;

	public GameObject EventMessageTarget;

	public SelectionType SendMessageToSelection;

	public bool IsExclusive;

	public virtual int RequiredFingerCount
	{
		get
		{
			return requiredFingerCount;
		}
		set
		{
			requiredFingerCount = value;
		}
	}

	public virtual bool SupportFingerClustering
	{
		get
		{
			return true;
		}
	}

	public virtual GestureResetMode GetDefaultResetMode()
	{
		return GestureResetMode.EndOfTouchSequence;
	}

	public abstract string GetDefaultEventMessageName();

	public abstract Type GetGestureType();

	protected virtual void Awake()
	{
		if (string.IsNullOrEmpty(EventMessageName))
		{
			EventMessageName = GetDefaultEventMessageName();
		}
		if (ResetMode == GestureResetMode.Default)
		{
			ResetMode = GetDefaultResetMode();
		}
		if (!EventMessageTarget)
		{
			EventMessageTarget = base.gameObject;
		}
		if (!Raycaster)
		{
			Raycaster = GetComponent<ScreenRaycaster>();
		}
	}

	protected virtual void OnEnable()
	{
		FingerGestures.Register(this);
	}

	protected virtual void OnDisable()
	{
		FingerGestures.Unregister(this);
	}

	protected void Acquire(FingerGestures.Finger finger)
	{
		if (!finger.GestureRecognizers.Contains(this))
		{
			finger.GestureRecognizers.Add(this);
		}
	}

	protected bool Release(FingerGestures.Finger finger)
	{
		return finger.GestureRecognizers.Remove(this);
	}

	protected virtual void Start()
	{
		if (!ClusterManager && SupportFingerClustering)
		{
			ClusterManager = GetComponent<FingerClusterManager>();
			if (!ClusterManager)
			{
				ClusterManager = FingerGestures.DefaultClusterManager;
			}
		}
	}

	protected bool Young(FingerGestures.IFingerList touches)
	{
		FingerGestures.Finger oldest = touches.GetOldest();
		if (oldest == null)
		{
			return false;
		}
		float num = Time.time - oldest.StarTime;
		return num < 0.25f;
	}
}
