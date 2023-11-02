//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI Creation Wizard
/// </summary>

public class UICreateNewUIWizard : EditorWindow
{
	public enum CameraType
	{
		None,
		Simple2D,
		Advanced3D,
	}

	static public CameraType camType = CameraType.Simple2D;

	/// <summary>
	/// Refresh the window on selection.
	/// </summary>

	void OnSelectionChange () { Repaint(); }

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		RebasedEditorGUIUtility.LookLikeControls(80f);

		GUILayout.Label("Create a new UI with the following parameters:");
		NGUIEditorTools.DrawSeparator();

		GUILayout.BeginHorizontal();
		NGUISettings.layer = EditorGUILayout.LayerField("Layer", NGUISettings.layer, GUILayout.Width(200f));
		GUILayout.Space(20f);
		GUILayout.Label("This is the layer your UI will reside on");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		camType = (CameraType)EditorGUILayout.EnumPopup("Camera", camType, GUILayout.Width(200f));
		GUILayout.Space(20f);
		GUILayout.Label("Should this UI have a camera?");
		GUILayout.EndHorizontal();

		NGUIEditorTools.DrawSeparator();
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("When ready,");
		bool create = GUILayout.Button("Create Your UI", GUILayout.Width(120f));
		GUILayout.EndHorizontal();

		if (create) CreateNewUI();
	}

	/// <summary>
	/// Create a brand-new UI hierarchy.
	/// </summary>

	static public GameObject CreateNewUI ()
	{
		NGUIEditorTools.RegisterUndo("Create New UI");

		// Root for the UI
		GameObject root = null;

		if (camType == CameraType.Simple2D)
		{
			root = new GameObject("UI Root (2D)");
			root.AddComponent<UIRoot>().scalingStyle = UIRoot.Scaling.PixelPerfect;
		}
		else
		{
			root = new GameObject((camType == CameraType.Advanced3D) ? "UI Root (3D)" : "UI Root");
			root.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
			root.AddComponent<UIRoot>().scalingStyle = UIRoot.Scaling.FixedSize;
		}

		// Assign the layer to be used by everything
		root.layer = NGUISettings.layer;

		// Figure out the depth of the highest camera
		if (camType == CameraType.None)
		{
			// No camera requested -- simply add a panel
			UIPanel panel = NGUITools.AddChild<UIPanel>(root.gameObject);
			panel.sortByDepth = true;
			Selection.activeGameObject = panel.gameObject;
		}
		else
		{
			int mask = 1 << NGUISettings.layer;
			float depth = -1f;
			bool clearColor = true;
			bool audioListener = true;

			List<Camera> cameras = NGUIEditorTools.FindInScene<Camera>();

			foreach (Camera c in cameras)
			{
				// Choose the maximum depth
				depth = Mathf.Max(depth, c.depth);

				// Automatically exclude the specified layer mask from the camera if it can see more than that layer
				if (NGUISettings.layer != 0 && c.cullingMask != mask) c.cullingMask = (c.cullingMask & (~mask));

				// Only consider this object if it's active
				if (c.enabled && NGUITools.GetActive(c.gameObject)) clearColor = false;

				// If this camera has an audio listener, we won't need to add one
				if (c.GetComponent<AudioListener>() != null) audioListener = false;
			}

			// Camera and UICamera for this UI
			Camera cam = NGUITools.AddChild<Camera>(root);
			cam.depth = depth + 1;
			cam.backgroundColor = Color.grey;
			cam.cullingMask = mask;

			if (camType == CameraType.Simple2D)
			{
				cam.orthographicSize = 1f;
				cam.orthographic = true;
				cam.nearClipPlane = -2f;
				cam.farClipPlane = 2f;
			}
			else
			{
				cam.nearClipPlane = 0.1f;
				cam.farClipPlane = 4f;
				cam.transform.localPosition = new Vector3(0f, 0f, -700f);
			}

			// We don't want to clear color if this is not the first camera
			if (cameras.Count > 0) cam.clearFlags = clearColor ? CameraClearFlags.Skybox : CameraClearFlags.Depth;

			// Add an audio listener if we need one
			if (audioListener) cam.gameObject.AddComponent<AudioListener>();

			// Add a UI Camera for event handling
			cam.gameObject.AddComponent<UICamera>();

			if (camType == CameraType.Simple2D)
			{
				// Anchor is useful to have
				UIAnchor anchor = NGUITools.AddChild<UIAnchor>(cam.gameObject);
				anchor.uiCamera = cam;

				// And finally -- the first UI panel
				UIPanel panel = NGUITools.AddChild<UIPanel>(anchor.gameObject);
				panel.sortByDepth = true;
				Selection.activeGameObject = panel.gameObject;
			}
			else
			{
				UIPanel panel = NGUITools.AddChild<UIPanel>(root);
				panel.sortByDepth = true;
				Selection.activeGameObject = panel.gameObject;
			}
		}
		return Selection.activeGameObject;
	}
}
