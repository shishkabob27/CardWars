using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Camera")]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float clickTime;

		public ClickNotification clickNotification = ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;
	}

	private class Highlighted
	{
		public GameObject go;

		public int counter;
	}

	public delegate void OnCustomInput();

	public bool debug;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyPress = true;

	public LayerMask eventReceiverMask = -1;

	public bool clipRaycasts = true;

	public float tooltipDelay = 1f;

	public bool stickyTooltip = true;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public static bool uguiConsume = false;

	public static bool useInputEnabler = false;

	public static OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static int currentTouchID = -1;

	public static MouseOrTouch currentTouch = null;

	public static bool inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static List<UICamera> mList = new List<UICamera>();

	private static List<Highlighted> mHighlighted = new List<Highlighted>();

	private static GameObject mSel = null;

	private static MouseOrTouch[] mMouse = new MouseOrTouch[3]
	{
		new MouseOrTouch(),
		new MouseOrTouch(),
		new MouseOrTouch()
	};

	private static GameObject mHover;

	private static MouseOrTouch mController = new MouseOrTouch();

	private static float mNextEvent = 0f;

	private static Dictionary<int, MouseOrTouch> mTouches = new Dictionary<int, MouseOrTouch>();

	private GameObject mTooltip;

	private Camera mCam;

	private LayerMask mLayerMask;

	private float mTooltipTime;

	private bool mIsEditor;

	private static int mLockInputCount = 0;

	public static bool isDragging = false;

	public static GameObject hoveredObject;

	private static RaycastHit mEmpty = default(RaycastHit);

	private bool handlesEvents
	{
		get
		{
			return eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = GetComponent<Camera>();
			}
			return mCam;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return mSel;
		}
		set
		{
			if (!(mSel != value))
			{
				return;
			}
			if (mSel != null)
			{
				UICamera uICamera = FindCameraForLayer(mSel.layer);
				if (uICamera != null)
				{
					current = uICamera;
					currentCamera = uICamera.mCam;
					Notify(mSel, "OnSelect", false);
					if (uICamera.useController || uICamera.useKeyboard)
					{
						Highlight(mSel, false);
					}
					current = null;
				}
			}
			mSel = value;
			if (!(mSel != null))
			{
				return;
			}
			UICamera uICamera2 = FindCameraForLayer(mSel.layer);
			if (uICamera2 != null)
			{
				current = uICamera2;
				currentCamera = uICamera2.mCam;
				if (uICamera2.useController || uICamera2.useKeyboard)
				{
					Highlight(mSel, true);
				}
				Notify(mSel, "OnSelect", true);
				current = null;
			}
		}
	}

	public static int touchCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value.pressed != null)
				{
					num++;
				}
			}
			for (int i = 0; i < mMouse.Length; i++)
			{
				if (mMouse[i].pressed != null)
				{
					num++;
				}
			}
			if (mController.pressed != null)
			{
				num++;
			}
			return num;
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value.dragged != null)
				{
					num++;
				}
			}
			for (int i = 0; i < mMouse.Length; i++)
			{
				if (mMouse[i].dragged != null)
				{
					num++;
				}
			}
			if (mController.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera uICamera = eventHandler;
			return (!(uICamera != null)) ? null : uICamera.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < mList.Count; i++)
			{
				UICamera uICamera = mList[i];
				if (!(uICamera == null) && uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
				{
					return uICamera;
				}
			}
			return null;
		}
	}

	public static void LockInput()
	{
		mLockInputCount++;
	}

	public static void UnlockInput()
	{
		if (mLockInputCount > 0)
		{
			mLockInputCount--;
		}
	}

	private void OnApplicationQuit()
	{
		mHighlighted.Clear();
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos, out RaycastHit hit)
	{
		for (int i = 0; i < mList.Count; i++)
		{
			UICamera uICamera = mList[i];
			if (!uICamera.enabled || !NGUITools.GetActive(uICamera.gameObject))
			{
				continue;
			}
			currentCamera = uICamera.cachedCamera;
			Vector3 vector = currentCamera.ScreenToViewportPoint(inPos);
			if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
			{
				continue;
			}
			Ray ray = currentCamera.ScreenPointToRay(inPos);
			int layerMask = currentCamera.cullingMask & (int)uICamera.eventReceiverMask;
			float maxDistance = ((!(uICamera.rangeDistance > 0f)) ? (currentCamera.farClipPlane - currentCamera.nearClipPlane) : uICamera.rangeDistance);
			if (uICamera.clipRaycasts)
			{
				RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, layerMask);
				if (array.Length > 1)
				{
					Array.Sort(array, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
					int j = 0;
					for (int num = array.Length; j < num; j++)
					{
						if (IsVisible(ref array[j]))
						{
							hit = array[j];
							return true;
						}
					}
				}
				else if (array.Length == 1 && IsVisible(ref array[0]))
				{
					hit = array[0];
					return true;
				}
			}
			else if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
			{
				return true;
			}
		}
		hit = mEmpty;
		return false;
	}

	private static bool IsVisible(ref RaycastHit hit)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(hit.collider.gameObject);
		if (uIPanel == null || uIPanel.IsVisible(hit.point))
		{
			return true;
		}
		return false;
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < mList.Count; i++)
		{
			UICamera uICamera = mList[i];
			Camera camera = uICamera.cachedCamera;
			if (camera != null && (camera.cullingMask & num) != 0)
			{
				return uICamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (mNextEvent < realtimeSinceStartup)
		{
			float axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static bool IsHighlighted(GameObject go)
	{
		int num = mHighlighted.Count;
		while (num > 0)
		{
			Highlighted highlighted = mHighlighted[--num];
			if (highlighted.go == go)
			{
				return true;
			}
		}
		return false;
	}

	private static void Highlight(GameObject go, bool highlighted)
	{
		if (!(go != null))
		{
			return;
		}
		int num = mHighlighted.Count;
		while (num > 0)
		{
			Highlighted highlighted2 = mHighlighted[--num];
			if (highlighted2 == null || highlighted2.go == null)
			{
				mHighlighted.RemoveAt(num);
			}
			else if (highlighted2.go == go)
			{
				if (highlighted)
				{
					highlighted2.counter++;
				}
				else if (--highlighted2.counter < 1)
				{
					mHighlighted.Remove(highlighted2);
					Notify(go, "OnHover", false);
				}
				return;
			}
		}
		if (highlighted)
		{
			Highlighted highlighted3 = new Highlighted();
			highlighted3.go = go;
			highlighted3.counter = 1;
			mHighlighted.Add(highlighted3);
			Notify(go, "OnHover", true);
		}
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (!(go != null))
		{
			return;
		}
		bool flag = true;
		if (useInputEnabler)
		{
			switch (funcName)
			{
			case "OnPress":
				if (obj != null && (bool)obj)
				{
					flag = false;
				}
				break;
			case "OnClick":
			case "OnDoubleClick":
			case "OnSelect":
			case "OnDrop":
				flag = false;
				break;
			}
			if (!flag)
			{
				UIInputEnabler component = go.GetComponent<UIInputEnabler>();
				if (component != null && component.inputEnabled)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (genericEventHandler != null && genericEventHandler != go)
			{
				genericEventHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static MouseOrTouch GetTouch(int id)
	{
		MouseOrTouch value = null;
		if (!mTouches.TryGetValue(id, out value))
		{
			value = new MouseOrTouch();
			value.touchBegan = true;
			mTouches.Add(id, value);
		}
		return value;
	}

	public static void RemoveTouch(int id)
	{
		mTouches.Remove(id);
	}

	private void Awake()
	{
		mList.Add(this);
		cachedCamera.eventMask = 0;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.BlackBerryPlayer)
		{
			useMouse = false;
			useTouch = true;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				useKeyboard = false;
				useController = false;
			}
		}
		else if (Application.platform == RuntimePlatform.PS3 || Application.platform == RuntimePlatform.XBOX360)
		{
			useMouse = false;
			useTouch = false;
			useKeyboard = false;
			useController = true;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			mIsEditor = true;
		}
		mMouse[0].pos.x = Input.mousePosition.x;
		mMouse[0].pos.y = Input.mousePosition.y;
		lastTouchPosition = mMouse[0].pos;
		if ((int)eventReceiverMask == -1)
		{
			eventReceiverMask = cachedCamera.cullingMask;
		}
	}

	private void OnDestroy()
	{
		mList.Remove(this);
	}

	private void OnEnable()
	{
		mList.Sort(CompareFunc);
	}

	private void FixedUpdate()
	{
		if (useMouse && Application.isPlaying && handlesEvents)
		{
			hoveredObject = ((!Raycast(Input.mousePosition, out lastHit)) ? fallThrough : lastHit.collider.gameObject);
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			for (int i = 0; i < 3; i++)
			{
				mMouse[i].current = hoveredObject;
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying || !handlesEvents)
		{
			return;
		}
		if (uguiConsume)
		{
			uguiConsume = false;
			return;
		}
		current = this;
		if (useMouse || (useTouch && mIsEditor))
		{
			ProcessMouse();
		}
		if (useTouch)
		{
			ProcessTouches();
		}
		if (onCustomInput != null)
		{
			onCustomInput();
		}
		if (useMouse && mSel != null && ((cancelKey0 != 0 && Input.GetKeyDown(cancelKey0)) || (cancelKey1 != 0 && Input.GetKeyDown(cancelKey1))))
		{
			selectedObject = null;
		}
		if (mSel != null)
		{
			string text = Input.inputString;
			if (useKeyboard && Input.GetKeyDown(KeyCode.Delete))
			{
				text += "\b";
			}
			if (text.Length > 0)
			{
				if (!stickyTooltip && mTooltip != null)
				{
					ShowTooltip(false);
				}
				Notify(mSel, "OnInput", text);
			}
		}
		else
		{
			inputHasFocus = false;
		}
		if (mSel != null)
		{
			ProcessOthers();
		}
		if (useMouse && mHover != null)
		{
			float axis = Input.GetAxis(scrollAxisName);
			if (axis != 0f)
			{
				Notify(mHover, "OnScroll", axis);
			}
			if (showTooltips && mTooltipTime != 0f && (mTooltipTime < Time.realtimeSinceStartup || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				mTooltip = mHover;
				ShowTooltip(true);
			}
		}
		current = null;
	}

	public void ProcessMouse()
	{
		bool flag = useMouse && Time.timeScale < 0.9f;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					flag = true;
					break;
				}
			}
		}
		mMouse[0].pos = Input.mousePosition;
		mMouse[0].delta = mMouse[0].pos - lastTouchPosition;
		bool flag2 = mMouse[0].pos != lastTouchPosition;
		lastTouchPosition = mMouse[0].pos;
		if (flag)
		{
			hoveredObject = ((!Raycast(Input.mousePosition, out lastHit)) ? fallThrough : lastHit.collider.gameObject);
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			mMouse[0].current = hoveredObject;
		}
		for (int j = 1; j < 3; j++)
		{
			mMouse[j].pos = mMouse[0].pos;
			mMouse[j].delta = mMouse[0].delta;
			mMouse[j].current = mMouse[0].current;
		}
		bool flag3 = false;
		for (int k = 0; k < 3; k++)
		{
			if (Input.GetMouseButton(k))
			{
				flag3 = true;
				break;
			}
		}
		if (flag3)
		{
			mTooltipTime = 0f;
		}
		else if (useMouse && flag2 && (!stickyTooltip || mHover != mMouse[0].current))
		{
			if (mTooltipTime != 0f)
			{
				mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			}
			else if (mTooltip != null)
			{
				ShowTooltip(false);
			}
		}
		if (useMouse && !flag3 && mHover != null && mHover != mMouse[0].current)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			Highlight(mHover, false);
			mHover = null;
		}
		if (useMouse)
		{
			for (int l = 0; l < 3; l++)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(l);
				bool mouseButtonUp = Input.GetMouseButtonUp(l);
				currentTouch = mMouse[l];
				currentTouchID = -1 - l;
				if (mouseButtonDown)
				{
					currentTouch.pressedCam = currentCamera;
				}
				else if (currentTouch.pressed != null)
				{
					currentCamera = currentTouch.pressedCam;
				}
				ProcessTouch(mouseButtonDown, mouseButtonUp);
			}
			currentTouch = null;
		}
		if (useMouse && !flag3 && mHover != mMouse[0].current)
		{
			mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			mHover = mMouse[0].current;
			Highlight(mHover, true);
		}
	}

	public void ProcessTouches()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			currentTouchID = ((!allowMultiTouch) ? 1 : touch.fingerId);
			currentTouch = GetTouch(currentTouchID);
			bool flag = touch.phase == TouchPhase.Began || currentTouch.touchBegan;
			bool flag2 = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
			currentTouch.touchBegan = false;
			if (flag)
			{
				currentTouch.delta = Vector2.zero;
			}
			else
			{
				currentTouch.delta = touch.position - currentTouch.pos;
			}
			currentTouch.pos = touch.position;
			hoveredObject = ((!Raycast(currentTouch.pos, out lastHit)) ? fallThrough : lastHit.collider.gameObject);
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;
			if (flag)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if (currentTouch.pressed != null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			if (touch.tapCount > 1)
			{
				currentTouch.clickTime = Time.realtimeSinceStartup;
			}
			ProcessTouch(flag, flag2);
			if (flag2)
			{
				RemoveTouch(currentTouchID);
			}
			currentTouch = null;
			if (!allowMultiTouch)
			{
				break;
			}
		}
	}

	public void ProcessOthers()
	{
		currentTouchID = -100;
		currentTouch = mController;
		inputHasFocus = mSel != null && mSel.GetComponent<UIInput>() != null;
		bool flag = (submitKey0 != 0 && Input.GetKeyDown(submitKey0)) || (submitKey1 != 0 && Input.GetKeyDown(submitKey1));
		bool flag2 = (submitKey0 != 0 && Input.GetKeyUp(submitKey0)) || (submitKey1 != 0 && Input.GetKeyUp(submitKey1));
		if (flag || flag2)
		{
			currentTouch.current = mSel;
			ProcessTouch(flag, flag2);
			currentTouch.current = null;
		}
		int num = 0;
		int num2 = 0;
		if (useKeyboard)
		{
			if (inputHasFocus)
			{
				num += GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				num += GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}
		if (useController)
		{
			if (!string.IsNullOrEmpty(verticalAxisName))
			{
				num += GetDirection(verticalAxisName);
			}
			if (!string.IsNullOrEmpty(horizontalAxisName))
			{
				num2 += GetDirection(horizontalAxisName);
			}
		}
		if (num != 0)
		{
			Notify(mSel, "OnKey", (num <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
		}
		if (num2 != 0)
		{
			Notify(mSel, "OnKey", (num2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
		}
		if (useKeyboard && Input.GetKeyDown(KeyCode.Tab))
		{
			Notify(mSel, "OnKey", KeyCode.Tab);
		}
		if (cancelKey0 != 0 && Input.GetKeyDown(cancelKey0))
		{
			Notify(mSel, "OnKey", KeyCode.Escape);
		}
		if (cancelKey1 != 0 && Input.GetKeyDown(cancelKey1))
		{
			Notify(mSel, "OnKey", KeyCode.Escape);
		}
		currentTouch = null;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		if (mLockInputCount > 0)
		{
			return;
		}
		bool flag = currentTouch == mMouse[0] || currentTouch == mMouse[1] || currentTouch == mMouse[2];
		float num = ((!flag) ? touchDragThreshold : mouseDragThreshold);
		float num2 = ((!flag) ? touchClickThreshold : mouseClickThreshold);
		if (pressed)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			currentTouch.pressStarted = true;
			Notify(currentTouch.pressed, "OnPress", false);
			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = ((!flag) ? ClickNotification.Always : ClickNotification.BasedOnDelta);
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;
			Notify(currentTouch.pressed, "OnPress", true);
			if (currentTouch.pressed != mSel)
			{
				if (mTooltip != null)
				{
					ShowTooltip(false);
				}
				selectedObject = null;
			}
		}
		else
		{
			if (!stickyPress && !unpressed && currentTouch.pressStarted && currentTouch.pressed != hoveredObject)
			{
				isDragging = true;
				Notify(currentTouch.pressed, "OnPress", false);
				currentTouch.pressed = hoveredObject;
				Notify(currentTouch.pressed, "OnPress", true);
				isDragging = false;
			}
			if (currentTouch.pressed != null)
			{
				float magnitude = currentTouch.delta.magnitude;
				if (magnitude != 0f)
				{
					currentTouch.totalDelta += currentTouch.delta;
					magnitude = currentTouch.totalDelta.magnitude;
					if (!currentTouch.dragStarted && num < magnitude)
					{
						currentTouch.dragStarted = true;
						currentTouch.delta = currentTouch.totalDelta;
					}
					if (currentTouch.dragStarted)
					{
						if (mTooltip != null)
						{
							ShowTooltip(false);
						}
						isDragging = true;
						bool flag2 = currentTouch.clickNotification == ClickNotification.None;
						Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);
						isDragging = false;
						if (flag2)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
						else if (currentTouch.clickNotification == ClickNotification.BasedOnDelta && num2 < magnitude)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
					}
				}
			}
		}
		if (!unpressed)
		{
			return;
		}
		currentTouch.pressStarted = false;
		if (mTooltip != null)
		{
			ShowTooltip(false);
		}
		if (currentTouch.pressed != null)
		{
			Notify(currentTouch.pressed, "OnPress", false);
			if (useMouse && currentTouch.pressed == mHover)
			{
				Notify(currentTouch.pressed, "OnHover", true);
			}
			if (currentTouch.dragged == currentTouch.current || (currentTouch.clickNotification != 0 && currentTouch.totalDelta.magnitude < num))
			{
				if (currentTouch.pressed != mSel)
				{
					mSel = currentTouch.pressed;
					Notify(currentTouch.pressed, "OnSelect", true);
				}
				else
				{
					mSel = currentTouch.pressed;
				}
				if (currentTouch.clickNotification != 0 && currentTouch.pressed.GetComponent<Collider>() != null && currentTouch.pressed.GetComponent<Collider>().enabled && lastHit.collider == currentTouch.pressed.GetComponent<Collider>())
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					Notify(currentTouch.pressed, "OnClick", null);
					if (currentTouch.clickTime + 0.35f > realtimeSinceStartup)
					{
						Notify(currentTouch.pressed, "OnDoubleClick", null);
					}
					currentTouch.clickTime = realtimeSinceStartup;
				}
			}
			else
			{
				Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
			}
		}
		currentTouch.dragStarted = false;
		currentTouch.pressed = null;
		currentTouch.dragged = null;
	}

	public void ShowTooltip(bool val)
	{
		mTooltipTime = 0f;
		Notify(mTooltip, "OnTooltip", val);
		if (!val)
		{
			mTooltip = null;
		}
	}
}
