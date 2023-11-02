//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UIWidgets.
/// </summary>

[CustomEditor(typeof(UIWidget))]
public class UIWidgetInspector : Editor
{
	enum Action
	{
		None,
		Move,
		Scale,
		Rotate,
	}

	Action mAction = Action.None;
	Action mActionUnderMouse = Action.None;
	bool mAllowSelection = true;

	protected UIWidget mWidget;

	static protected bool mUseShader = false;
	static Color mOutlineColor = Color.green;
	static GUIStyle mSelectedDot = null;
	static GUIStyle mNormalDot = null;
	static MouseCursor mCursor = MouseCursor.Arrow;

	static UIWidget.Pivot[] mPivots =
	{
		UIWidget.Pivot.TopLeft,
		UIWidget.Pivot.BottomLeft,
		UIWidget.Pivot.BottomRight,
		UIWidget.Pivot.TopRight,
		UIWidget.Pivot.Left,
		UIWidget.Pivot.Bottom,
		UIWidget.Pivot.Right,
		UIWidget.Pivot.Top,
	};

	static int s_Hash = "WidgetHash".GetHashCode();
	Vector3 mStartPos = Vector3.zero;
	Vector3 mStartScale = Vector3.zero;
	Vector3 mStartDrag = Vector3.zero;
	Vector2 mStartMouse = Vector2.zero;
	Vector3 mStartRot = Vector3.zero;
	Vector3 mStartDir = Vector3.right;
	UIWidget.Pivot mDragPivot = UIWidget.Pivot.Center;

	/// <summary>
	/// Register an Undo command with the Unity editor.
	/// </summary>

	void RegisterUndo ()
	{
		NGUIEditorTools.RegisterUndo("Widget Change", mWidget);
	}

	/// <summary>
	/// Raycast into the screen.
	/// </summary>

	static bool Raycast (Vector3[] corners, out Vector3 hit)
	{
		Plane plane = new Plane(corners[0], corners[1], corners[2]);
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		float dist = 0f;
		bool isHit = plane.Raycast(ray, out dist);
		hit = isHit ? ray.GetPoint(dist) : Vector3.zero;
		return isHit;
	}

	/// <summary>
	/// Draw a control dot at the specified world position.
	/// </summary>

	static void DrawKnob (Vector3 point, bool selected, int id)
	{
		if (mSelectedDot == null) mSelectedDot = "sv_label_5";
		if (mNormalDot == null) mNormalDot = "sv_label_3";

		Vector2 screenPoint = HandleUtility.WorldToGUIPoint(point);

		Rect rect = new Rect(screenPoint.x - 7f, screenPoint.y - 7f, 14f, 14f);
		if (selected) mSelectedDot.Draw(rect, GUIContent.none, id);
		else mNormalDot.Draw(rect, GUIContent.none, id);
	}

	/// <summary>
	/// Whether the mouse position is within one of the specified rectangles.
	/// </summary>

	static bool IsMouseOverRect (Vector2 mouse, List<Rect> rects)
	{
		for (int i = 0; i < rects.Count; ++i)
		{
			Rect r = rects[i];
			if (r.Contains(mouse)) return true;
		}
		return false;
	}

	/// <summary>
	/// Screen-space distance from the mouse position to the specified world position.
	/// </summary>

	static float GetScreenDistance (Vector3 worldPos, Vector2 mousePos)
	{
		Vector2 screenPos = HandleUtility.WorldToGUIPoint(worldPos);
		return Vector2.Distance(mousePos, screenPos);
	}

	/// <summary>
	/// Closest screen-space distance from the mouse position to one of the specified world points.
	/// </summary>

	static float GetScreenDistance (Vector3[] worldPoints, Vector2 mousePos, out int index)
	{
		float min = float.MaxValue;
		index = 0;

		for (int i = 0; i < worldPoints.Length; ++i)
		{
			float distance = GetScreenDistance(worldPoints[i], mousePos);
			
			if (distance < min)
			{
				index = i;
				min = distance;
			}
		}
		return min;
	}

	/// <summary>
	/// Set the mouse cursor rectangle, refreshing the screen when it gets changed.
	/// </summary>

	static void SetCursorRect (Rect rect, MouseCursor cursor)
	{
		EditorGUIUtility.AddCursorRect(rect, cursor);

		if (Event.current.type == EventType.MouseMove)
		{
			if (mCursor != cursor)
			{
				mCursor = cursor;
				Event.current.Use();
			}
		}
	}

	/// <summary>
	/// Draw the on-screen selection, knobs, and handle all interaction logic.
	/// </summary>

	public void OnSceneGUI ()
	{
		return;
		//if (!UIWidget.showHandles) return;

		mWidget = target as UIWidget;

		Handles.color = mOutlineColor;
		Transform t = mWidget.cachedTransform;

		Event e = Event.current;
		int id = GUIUtility.GetControlID(s_Hash, FocusType.Passive);
		EventType type = e.GetTypeForControl(id);

		Vector3[] corners = NGUIMath.CalculateWidgetCorners(mWidget);
		Handles.DrawLine(corners[0], corners[1]);
		Handles.DrawLine(corners[1], corners[2]);
		Handles.DrawLine(corners[2], corners[3]);
		Handles.DrawLine(corners[0], corners[3]);

		Vector3[] worldPos = new Vector3[8];
		
		worldPos[0] = corners[0];
		worldPos[1] = corners[1];
		worldPos[2] = corners[2];
		worldPos[3] = corners[3];

		worldPos[4] = (corners[0] + corners[1]) * 0.5f;
		worldPos[5] = (corners[1] + corners[2]) * 0.5f;
		worldPos[6] = (corners[2] + corners[3]) * 0.5f;
		worldPos[7] = (corners[0] + corners[3]) * 0.5f;

		Vector2[] screenPos = new Vector2[8];
		for (int i = 0; i < 8; ++i) screenPos[i] = HandleUtility.WorldToGUIPoint(worldPos[i]);

		Bounds b = new Bounds(screenPos[0], Vector3.zero);
		for (int i = 1; i < 8; ++i) b.Encapsulate(screenPos[i]);

		// Time to figure out what kind of action is underneath the mouse
		Action actionUnderMouse = mAction;
		UIWidget.Pivot pivotUnderMouse = UIWidget.Pivot.Center;

		if (actionUnderMouse == Action.None)
		{
			int index = 0;
			float dist = GetScreenDistance(worldPos, e.mousePosition, out index);

			//if (mWidget.showResizeHandles && dist < 10f)
			//{
			//	pivotUnderMouse = mPivots[index];
			//	actionUnderMouse = Action.Scale;
			//}
			if (e.modifiers == 0 && SceneViewDistanceToRectangle(corners, e.mousePosition) == 0f)
			{
				actionUnderMouse = Action.Move;
			}
			else if (dist < 30f)
			{
				actionUnderMouse = Action.Rotate;
			}
		}

		// Change the mouse cursor to a more appropriate one
#if !UNITY_3_5
		{
			Vector2 min = b.min;
			Vector2 max = b.max;

			min.x -= 30f;
			max.x += 30f;
			min.y -= 30f;
			max.y += 30f;

			Rect rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

			if (actionUnderMouse == Action.Rotate)
			{
				SetCursorRect(rect, MouseCursor.RotateArrow);
			}
			else if (actionUnderMouse == Action.Move)
			{
				SetCursorRect(rect, MouseCursor.MoveArrow);
			}
			//else if (mWidget.showResizeHandles && actionUnderMouse == Action.Scale)
			//{
			//	SetCursorRect(rect, MouseCursor.ScaleArrow);
			//}
			else SetCursorRect(rect, MouseCursor.Arrow);
		}
#endif

		switch (type)
		{
			case EventType.Repaint:
			{
				/*if (mWidget.showResizeHandles)
				{
					Handles.BeginGUI();
					{
						for (int i = 0; i < 8; ++i)
						{
							DrawKnob(worldPos[i], mWidget.pivot == mPivots[i], id);
						}
					}
					Handles.EndGUI();
				}*/
			}
			break;

			case EventType.MouseDown:
			{
				mStartMouse = e.mousePosition;
				mAllowSelection = true;

				if (e.button == 1)
				{
					if (e.modifiers == 0)
					{
						GUIUtility.hotControl = GUIUtility.keyboardControl = id;
						e.Use();
					}
				}
				else if (e.button == 0 && actionUnderMouse != Action.None && Raycast(corners, out mStartDrag))
				{
					mStartPos = t.position;
					mStartRot = t.localRotation.eulerAngles;
					mStartDir = mStartDrag - t.position;
					mStartScale = t.localScale;
					mDragPivot = pivotUnderMouse;
					mActionUnderMouse = actionUnderMouse;
					GUIUtility.hotControl = GUIUtility.keyboardControl = id;
					e.Use();
				}
			}
			break;

			case EventType.MouseDrag:
			{
				// Prevent selection once the drag operation begins
				bool dragStarted = (e.mousePosition - mStartMouse).magnitude > 3f;
				if (dragStarted) mAllowSelection = false;

				if (GUIUtility.hotControl == id)
				{
					e.Use();

					if (mAction != Action.None || mActionUnderMouse != Action.None)
					{
						Vector3 pos;

						if (Raycast(corners, out pos))
						{
							if (mAction == Action.None && mActionUnderMouse != Action.None)
							{
								// Wait until the mouse moves by more than a few pixels
								if (dragStarted)
								{
									if (mActionUnderMouse == Action.Move)
									{
										mStartPos = t.position;
										NGUIEditorTools.RegisterUndo("Move widget", t);
									}
									else if (mActionUnderMouse == Action.Rotate)
									{
										mStartRot = t.localRotation.eulerAngles;
										mStartDir = mStartDrag - t.position;
										NGUIEditorTools.RegisterUndo("Rotate widget", t);
									}
									else if (mActionUnderMouse == Action.Scale)
									{
										mStartPos = t.localPosition;
										mStartScale = t.localScale;
										mDragPivot = pivotUnderMouse;
										NGUIEditorTools.RegisterUndo("Scale widget", t);
									}
									mAction = actionUnderMouse;
								}
							}

							if (mAction != Action.None)
							{
								if (mAction == Action.Move)
								{
									t.position = mStartPos + (pos - mStartDrag);
									pos = t.localPosition;
									pos.x = Mathf.RoundToInt(pos.x);
									pos.y = Mathf.RoundToInt(pos.y);
									t.localPosition = pos;
								}
								else if (mAction == Action.Rotate)
								{
									Vector3 dir = pos - t.position;
									float angle = Vector3.Angle(mStartDir, dir);

									if (angle > 0f)
									{
										float dot = Vector3.Dot(Vector3.Cross(mStartDir, dir), t.forward);
										if (dot < 0f) angle = -angle;
										angle = mStartRot.z + angle;
										if (e.modifiers != EventModifiers.Shift) angle = Mathf.Round(angle / 15f) * 15f;
										else angle = Mathf.Round(angle);
										t.localRotation = Quaternion.Euler(mStartRot.x, mStartRot.y, angle);
									}
								}
								else if (mAction == Action.Scale)
								{
									// World-space delta since the drag started
									Vector3 delta = pos - mStartDrag;

									// Adjust the widget's position and scale based on the delta, restricted by the pivot
									AdjustPosAndScale(mWidget, mStartPos, mStartScale, delta, mDragPivot);
								}
							}
						}
					}
				}
			}
			break;

			case EventType.MouseUp:
			{
				if (GUIUtility.hotControl == id)
				{
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;

					if (e.button < 2)
					{
						bool handled = false;

						if (e.button == 1)
						{
							// Right-click: Select the widget below
							SelectWidget(mWidget, e.mousePosition, false);
							handled = true;
						}
						else if (mAction == Action.None)
						{
							if (mAllowSelection)
							{
								// Left-click: Select the widget above
								SelectWidget(mWidget, e.mousePosition, true);
								handled = true;
							}
						}
						else
						{
							// Finished dragging something
							mAction = Action.None;
							mActionUnderMouse = Action.None;
							Vector3 pos = t.localPosition;
							Vector3 scale = t.localScale;

							if (mWidget.pixelPerfectAfterResize)
							{
								t.localPosition = pos;
								t.localScale = scale;

								mWidget.MakePixelPerfect();
							}
							else
							{
								pos.x = Mathf.Round(pos.x);
								pos.y = Mathf.Round(pos.y);
								scale.x = Mathf.Round(scale.x);
								scale.y = Mathf.Round(scale.y);

								t.localPosition = pos;
								t.localScale = scale;
							}
							handled = true;
						}

						if (handled)
						{
							mActionUnderMouse = Action.None;
							mAction = Action.None;
							e.Use();
						}
					}
				}
				else if (mAllowSelection)
				{
					BetterList<UIWidget> widgets = SceneViewRaycast(mWidget.panel, e.mousePosition);
					if (widgets.size > 0) Selection.activeGameObject = widgets[0].gameObject;
				}
				mAllowSelection = true;
			}
			break;

			case EventType.KeyDown:
			{
				if (e.keyCode == KeyCode.UpArrow)
				{
					Vector3 pos = t.localPosition;
					pos.y += 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.DownArrow)
				{
					Vector3 pos = t.localPosition;
					pos.y -= 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.LeftArrow)
				{
					Vector3 pos = t.localPosition;
					pos.x -= 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.RightArrow)
				{
					Vector3 pos = t.localPosition;
					pos.x += 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.Escape)
				{
					if (GUIUtility.hotControl == id)
					{
						if (mAction != Action.None)
						{
							if (mAction == Action.Move)
							{
								t.position = mStartPos;
							}
							else if (mAction == Action.Rotate)
							{
								t.localRotation = Quaternion.Euler(mStartRot);
							}
							else if (mAction == Action.Scale)
							{
								t.position = mStartPos;
								t.localScale = mStartScale;
							}
						}

						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;

						mActionUnderMouse = Action.None;
						mAction = Action.None;
						e.Use();
					}
					else Selection.activeGameObject = null;
				}
			}
			break;
		}
	}

	/// <summary>
	/// Select the next widget in line.
	/// </summary>

	static public bool SelectWidget (UIWidget start, Vector2 pos, bool inFront)
	{
		GameObject go = null;
		UIPanel p = start.panel;
		if (p == null) p = NGUITools.FindInParents<UIPanel>(start.gameObject);
		BetterList<UIWidget> widgets = SceneViewRaycast(p, pos);

		if (inFront)
		{
			if (widgets.size > 0)
			{
				for (int i = 0; i < widgets.size; ++i)
				{
					UIWidget w = widgets[i];
					if (w == start) break;
					go = w.gameObject;
				}
			}
		}
		else
		{
			for (int i = widgets.size; i > 0; )
			{
				UIWidget w = widgets[--i];
				if (w == start) break;
				go = w.gameObject;
			}
		}

		if (go != null)
		{
			Selection.activeGameObject = go;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Adjust the transform's position and scale.
	/// </summary>

	static void AdjustPosAndScale (UIWidget w, Vector3 startLocalPos, Vector3 startLocalScale, Vector3 worldDelta, UIWidget.Pivot dragPivot)
	{
		Transform t = w.cachedTransform;
		Transform parent = t.parent;
		Matrix4x4 parentToLocal = (parent != null) ? t.parent.worldToLocalMatrix : Matrix4x4.identity;
		Matrix4x4 worldToLocal = parentToLocal;
		Quaternion invRot = Quaternion.Inverse(t.localRotation);
		worldToLocal = worldToLocal * Matrix4x4.TRS(Vector3.zero, invRot, Vector3.one);
		Vector3 localDelta = worldToLocal.MultiplyVector(worldDelta);

		bool canBeSquare = false;
		float left = 0f;
		float right = 0f;
		float top = 0f;
		float bottom = 0f;

		switch (dragPivot)
		{
			case UIWidget.Pivot.TopLeft:
			canBeSquare = (w.pivot == UIWidget.Pivot.BottomRight);
			left = localDelta.x;
			top = localDelta.y;
			break;

			case UIWidget.Pivot.Left:
			left = localDelta.x;
			break;

			case UIWidget.Pivot.BottomLeft:
			canBeSquare = (w.pivot == UIWidget.Pivot.TopRight);
			left = localDelta.x;
			bottom = localDelta.y;
			break;

			case UIWidget.Pivot.Top:
			top = localDelta.y;
			break;

			case UIWidget.Pivot.Bottom:
			bottom = localDelta.y;
			break;

			case UIWidget.Pivot.TopRight:
			canBeSquare = (w.pivot == UIWidget.Pivot.BottomLeft);
			right = localDelta.x;
			top = localDelta.y;
			break;

			case UIWidget.Pivot.Right:
			right = localDelta.x;
			break;

			case UIWidget.Pivot.BottomRight:
			canBeSquare = (w.pivot == UIWidget.Pivot.TopLeft);
			right = localDelta.x;
			bottom = localDelta.y;
			break;
		}

		AdjustWidget(w, startLocalPos, startLocalScale, left, top, right, bottom, canBeSquare && Event.current.modifiers == EventModifiers.Shift);
	}
	
	/// <summary>
	/// Adjust the widget's rectangle based on the specified modifier values.
	/// </summary>

	static void AdjustWidget (UIWidget w, Vector3 pos, Vector3 scale, float left, float top, float right, float bottom, bool makeSquare)
	{
		Vector2 offset = w.pivotOffset;
		Vector4 padding = w.relativePadding;
		Vector2 size = w.relativeSize;

		offset.x -= padding.x;
		offset.y -= padding.y;
		size.x += padding.x + padding.z;
		size.y += padding.y + padding.w;
		
		scale.Scale(size);

		offset.y = -offset.y;

		Transform t = w.cachedTransform;
		Quaternion rot = t.localRotation;
		UIWidget.Pivot pivot = w.pivot;

		Vector2 rotatedTL = new Vector2(left, top);
		Vector2 rotatedTR = new Vector2(right, top);
		Vector2 rotatedBL = new Vector2(left, bottom);
		Vector2 rotatedBR = new Vector2(right, bottom);
		Vector2 rotatedL  = new Vector2(left, 0f);
		Vector2 rotatedR  = new Vector2(right, 0f);
		Vector2 rotatedT  = new Vector2(0f, top);
		Vector2 rotatedB  = new Vector2(0f, bottom);
		
		rotatedTL = rot * rotatedTL;
		rotatedTR = rot * rotatedTR;
		rotatedBL = rot * rotatedBL;
		rotatedBR = rot * rotatedBR;
		rotatedL  = rot * rotatedL;
		rotatedR  = rot * rotatedR;
		rotatedT  = rot * rotatedT;
		rotatedB  = rot * rotatedB;

		switch (pivot)
		{
			case UIWidget.Pivot.TopLeft:
			pos.x += rotatedTL.x;
			pos.y += rotatedTL.y;
			break;

			case UIWidget.Pivot.BottomRight:
			pos.x += rotatedBR.x;
			pos.y += rotatedBR.y;
			break;

			case UIWidget.Pivot.BottomLeft:
			pos.x += rotatedBL.x;
			pos.y += rotatedBL.y;
			break;

			case UIWidget.Pivot.TopRight:
			pos.x += rotatedTR.x;
			pos.y += rotatedTR.y;
			break;

			case UIWidget.Pivot.Left:
			pos.x += rotatedL.x + (rotatedT.x + rotatedB.x) * 0.5f;
			pos.y += rotatedL.y + (rotatedT.y + rotatedB.y) * 0.5f;
			break;

			case UIWidget.Pivot.Right:
			pos.x += rotatedR.x + (rotatedT.x + rotatedB.x) * 0.5f;
			pos.y += rotatedR.y + (rotatedT.y + rotatedB.y) * 0.5f;
			break;

			case UIWidget.Pivot.Top:
			pos.x += rotatedT.x + (rotatedL.x + rotatedR.x) * 0.5f;
			pos.y += rotatedT.y + (rotatedL.y + rotatedR.y) * 0.5f;
			break;

			case UIWidget.Pivot.Bottom:
			pos.x += rotatedB.x + (rotatedL.x + rotatedR.x) * 0.5f;
			pos.y += rotatedB.y + (rotatedL.y + rotatedR.y) * 0.5f;
			break;

			case UIWidget.Pivot.Center:
			pos.x += (rotatedL.x + rotatedR.x + rotatedT.x + rotatedB.x) * 0.5f;
			pos.y += (rotatedT.y + rotatedB.y + rotatedL.y + rotatedR.y) * 0.5f;
			break;
		}

		scale.x -= left - right;
		scale.y += top - bottom;

		scale.x /= size.x;
		scale.y /= size.y;

		Vector4 border = w.border;
		float minx = Mathf.Max(2f, padding.x + padding.z + border.x + border.z);
		float miny = Mathf.Max(2f, padding.y + padding.w + border.y + border.w);

		if (scale.x < minx) scale.x = minx;
		if (scale.y < miny) scale.y = miny;

		// NOTE: This will only work correctly when dragging the corner opposite of the pivot point
		if (makeSquare)
		{
			scale.x = Mathf.Min(scale.x, scale.y);
			scale.y = scale.x;
		}

		t.localPosition = pos;
		t.localScale = scale;
	}

	/// <summary>
	/// Cache the reference.
	/// </summary>

	protected virtual void OnEnable ()
	{
		mWidget = target as UIWidget;
	}

	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		RebasedEditorGUIUtility.LookLikeControls(80f);
		EditorGUILayout.Space();

		// Check to see if we can draw the widget's default properties to begin with
		if (DrawProperties())
		{
			// Draw all common properties next
			DrawCommonProperties();
			DrawExtraProperties();
		}
	}

	/// <summary>
	/// All widgets have depth, color and make pixel-perfect options
	/// </summary>

	protected void DrawCommonProperties ()
	{
		PrefabType type = PrefabUtility.GetPrefabType(mWidget.gameObject);
		NGUIEditorTools.DrawSeparator();

#if UNITY_3_5
		// Pivot point -- old school drop-down style
		UIWidget.Pivot pivot = (UIWidget.Pivot)EditorGUILayout.EnumPopup("Pivot", mWidget.pivot);

		if (mWidget.pivot != pivot)
		{
		    NGUIEditorTools.RegisterUndo("Pivot Change", mWidget);
		    mWidget.pivot = pivot;
		}
#else
		// Pivot point -- the new, more visual style
		GUILayout.BeginHorizontal();
		GUILayout.Label("Pivot", GUILayout.Width(76f));
		Toggle("\u25C4", "ButtonLeft", UIWidget.Pivot.Left, true);
		Toggle("\u25AC", "ButtonMid", UIWidget.Pivot.Center, true);
		Toggle("\u25BA", "ButtonRight", UIWidget.Pivot.Right, true);
		Toggle("\u25B2", "ButtonLeft", UIWidget.Pivot.Top, false);
		Toggle("\u258C", "ButtonMid", UIWidget.Pivot.Center, false);
		Toggle("\u25BC", "ButtonRight", UIWidget.Pivot.Bottom, false);
		GUILayout.EndHorizontal();
#endif

		// Depth navigation
		if (type != PrefabType.Prefab)
		{
			GUILayout.Space(2f);
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Depth");

				int depth = mWidget.depth;
				if (GUILayout.Button("Back", GUILayout.Width(60f))) --depth;
				depth = EditorGUILayout.IntField(depth);
				if (GUILayout.Button("Forward", GUILayout.Width(60f))) ++depth;

				if (mWidget.depth != depth)
				{
					NGUIEditorTools.RegisterUndo("Depth Change", mWidget);
					mWidget.depth = depth;
				}
			}
			GUILayout.EndHorizontal();

			int matchingDepths = 0;

			for (int i = 0; i < UIWidget.list.size; ++i)
			{
				UIWidget w = UIWidget.list[i];
				if (w != null && w.depth == mWidget.depth)
					++matchingDepths;
			}

			if (matchingDepths > 1)
			{
				EditorGUILayout.HelpBox(matchingDepths + " widgets are using the depth value of " + mWidget.depth +
					". It may not be clear what should be in front of what.", MessageType.Warning);
			}
		}

		// Pixel-correctness
		if (type != PrefabType.Prefab)
		{
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Correction");

				if (GUILayout.Button("Make Pixel-Perfect"))
				{
					NGUIEditorTools.RegisterUndo("Make Pixel-Perfect", mWidget.transform);
					mWidget.MakePixelPerfect();
				}
			}
			GUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();

		// Color tint
		GUILayout.BeginHorizontal();
		Color color = EditorGUILayout.ColorField("Color Tint", mWidget.color);
		if (GUILayout.Button("Copy", GUILayout.Width(50f)))
			NGUISettings.color = color;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		NGUISettings.color = EditorGUILayout.ColorField("Clipboard", NGUISettings.color);
		if (GUILayout.Button("Paste", GUILayout.Width(50f)))
			color = NGUISettings.color;
		GUILayout.EndHorizontal();

		if (mWidget.color != color)
		{
			NGUIEditorTools.RegisterUndo("Color Change", mWidget);
			mWidget.color = color;
		}
	}

	/// <summary>
	/// Draw a toggle button for the pivot point.
	/// </summary>

	void Toggle (string text, string style, UIWidget.Pivot pivot, bool isHorizontal)
	{
		bool isActive = false;

		switch (pivot)
		{
			case UIWidget.Pivot.Left:
			isActive = IsLeft(mWidget.pivot);
			break;

			case UIWidget.Pivot.Right:
			isActive = IsRight(mWidget.pivot);
			break;

			case UIWidget.Pivot.Top:
			isActive = IsTop(mWidget.pivot);
			break;

			case UIWidget.Pivot.Bottom:
			isActive = IsBottom(mWidget.pivot);
			break;

			case UIWidget.Pivot.Center:
			isActive = isHorizontal ? pivot == GetHorizontal(mWidget.pivot) : pivot == GetVertical(mWidget.pivot);
			break;
		}

		if (GUILayout.Toggle(isActive, text, style) != isActive)
			SetPivot(pivot, isHorizontal);
	}

	static bool IsLeft (UIWidget.Pivot pivot)
	{
		return pivot == UIWidget.Pivot.Left ||
			pivot == UIWidget.Pivot.TopLeft ||
			pivot == UIWidget.Pivot.BottomLeft;
	}

	static bool IsRight (UIWidget.Pivot pivot)
	{
		return pivot == UIWidget.Pivot.Right ||
			pivot == UIWidget.Pivot.TopRight ||
			pivot == UIWidget.Pivot.BottomRight;
	}

	static bool IsTop (UIWidget.Pivot pivot)
	{
		return pivot == UIWidget.Pivot.Top ||
			pivot == UIWidget.Pivot.TopLeft ||
			pivot == UIWidget.Pivot.TopRight;
	}

	static bool IsBottom (UIWidget.Pivot pivot)
	{
		return pivot == UIWidget.Pivot.Bottom ||
			pivot == UIWidget.Pivot.BottomLeft ||
			pivot == UIWidget.Pivot.BottomRight;
	}

	static UIWidget.Pivot GetHorizontal (UIWidget.Pivot pivot)
	{
		if (IsLeft(pivot)) return UIWidget.Pivot.Left;
		if (IsRight(pivot)) return UIWidget.Pivot.Right;
		return UIWidget.Pivot.Center;
	}

	static UIWidget.Pivot GetVertical (UIWidget.Pivot pivot)
	{
		if (IsTop(pivot)) return UIWidget.Pivot.Top;
		if (IsBottom(pivot)) return UIWidget.Pivot.Bottom;
		return UIWidget.Pivot.Center;
	}

	static UIWidget.Pivot Combine (UIWidget.Pivot horizontal, UIWidget.Pivot vertical)
	{
		if (horizontal == UIWidget.Pivot.Left)
		{
			if (vertical == UIWidget.Pivot.Top) return UIWidget.Pivot.TopLeft;
			if (vertical == UIWidget.Pivot.Bottom) return UIWidget.Pivot.BottomLeft;
			return UIWidget.Pivot.Left;
		}

		if (horizontal == UIWidget.Pivot.Right)
		{
			if (vertical == UIWidget.Pivot.Top) return UIWidget.Pivot.TopRight;
			if (vertical == UIWidget.Pivot.Bottom) return UIWidget.Pivot.BottomRight;
			return UIWidget.Pivot.Right;
		}
		return vertical;
	}

	/// <summary>
	/// Determine the distance from the mouse position to the world rectangle specified by the 4 points.
	/// </summary>

	static public float SceneViewDistanceToRectangle (Vector3[] worldPoints, Vector2 mousePos)
	{
		Vector2[] screenPoints = new Vector2[4];
		for (int i = 0; i < 4; ++i)
			screenPoints[i] = HandleUtility.WorldToGUIPoint(worldPoints[i]);
		return NGUIMath.DistanceToRectangle(screenPoints, mousePos);
	}

	/// <summary>
	/// Raycast into the specified panel, returning a list of widgets.
	/// Just like NGUIMath.Raycast, but doesn't rely on having a camera.
	/// </summary>

	static public BetterList<UIWidget> SceneViewRaycast (UIPanel panel, Vector2 mousePos)
	{
		BetterList<UIWidget> list = new BetterList<UIWidget>();

		for (int i = 0; i < UIWidget.list.size; ++i)
		{
			UIWidget w = UIWidget.list[i];
			Vector3[] corners = NGUIMath.CalculateWidgetCorners(w);
			if (SceneViewDistanceToRectangle(corners, mousePos) == 0f)
				list.Add(w);
		}

		list.Sort(delegate(UIWidget w1, UIWidget w2) { return w2.depth.CompareTo(w1.depth); });
		return list;
	}

	void SetPivot (UIWidget.Pivot pivot, bool isHorizontal)
	{
		UIWidget.Pivot horizontal = GetHorizontal(mWidget.pivot);
		UIWidget.Pivot vertical = GetVertical(mWidget.pivot);

		pivot = isHorizontal ? Combine(pivot, vertical) : Combine(horizontal, pivot);

		if (mWidget.pivot != pivot)
		{
			NGUIEditorTools.RegisterUndo("Pivot change", mWidget);
			mWidget.pivot = pivot;
		}
	}

	/// <summary>
	/// Any and all derived functionality.
	/// </summary>

	protected virtual void OnInit() { }
	protected virtual bool DrawProperties () { return true; }
	protected virtual void DrawExtraProperties () { }
}
