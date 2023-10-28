using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FingerEventDetector<T> : FingerEventDetector where T : FingerEvent, new()
{
	public delegate void FingerEventHandler(T eventData);

	private List<T> fingerEventsList;

	protected virtual T CreateFingerEvent()
	{
		return new T();
	}

	public override Type GetEventType()
	{
		return typeof(T);
	}

	protected override void Start()
	{
		base.Start();
		InitFingerEventsList(FingerGestures.Instance.MaxFingers);
	}

	protected virtual void InitFingerEventsList(int fingersCount)
	{
		fingerEventsList = new List<T>(fingersCount);
		for (int i = 0; i < fingersCount; i++)
		{
			T item = CreateFingerEvent();
			item.Detector = this;
			item.Finger = FingerGestures.GetFinger(i);
			fingerEventsList.Add(item);
		}
	}

	protected T GetEvent(FingerGestures.Finger finger)
	{
		return GetEvent(finger.Index);
	}

	protected virtual T GetEvent(int fingerIndex)
	{
		return fingerEventsList[fingerIndex];
	}
}
public abstract class FingerEventDetector : MonoBehaviour
{
	public int FingerIndexFilter = -1;

	public ScreenRaycaster Raycaster;

	public bool UseSendMessage = true;

	public bool SendMessageToSelection = true;

	public GameObject MessageTarget;

	private FingerGestures.Finger activeFinger;

	private RaycastHit lastHit = default(RaycastHit);

	internal RaycastHit LastHit
	{
		get
		{
			return lastHit;
		}
	}

	protected abstract void ProcessFinger(FingerGestures.Finger finger);

	public abstract Type GetEventType();

	protected virtual void Awake()
	{
		if (!Raycaster)
		{
			Raycaster = GetComponent<ScreenRaycaster>();
		}
		if (!MessageTarget)
		{
			MessageTarget = base.gameObject;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		ProcessFingers();
	}

	protected virtual void ProcessFingers()
	{
		if (FingerIndexFilter >= 0 && FingerIndexFilter < FingerGestures.Instance.MaxFingers)
		{
			ProcessFinger(FingerGestures.GetFinger(FingerIndexFilter));
			return;
		}
		for (int i = 0; i < FingerGestures.Instance.MaxFingers; i++)
		{
			ProcessFinger(FingerGestures.GetFinger(i));
		}
	}

	protected void TrySendMessage(FingerEvent eventData)
	{
		FingerGestures.FireEvent(eventData);
		if (UseSendMessage)
		{
			MessageTarget.SendMessage(eventData.Name, eventData, SendMessageOptions.DontRequireReceiver);
			if (SendMessageToSelection && (bool)eventData.Selection && eventData.Selection != MessageTarget)
			{
				eventData.Selection.SendMessage(eventData.Name, eventData, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public GameObject PickObject(Vector2 screenPos)
	{
		if (!Raycaster || !Raycaster.enabled)
		{
			return null;
		}
		if (!Raycaster.Raycast(screenPos, out lastHit))
		{
			return null;
		}
		return lastHit.collider.gameObject;
	}

	protected void UpdateSelection(FingerEvent e)
	{
		e.Selection = PickObject(e.Position);
		e.Hit = LastHit;
	}
}
