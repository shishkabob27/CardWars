//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// This script adds the NGUI menu options to the Unity Editor.
/// </summary>

static public class NGUIMenu
{
	/// <summary>
	/// Same as SelectedRoot(), but with a log message if nothing was found.
	/// </summary>

	static public GameObject SelectedRoot ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot();

		if (go == null)
		{
			Debug.Log("No UI found. You can create a new one easily by using the UI creation wizard.\nOpening it for your convenience.");
			CreateUIWizard();
		}
		return go;
	}

	[MenuItem("NGUI/Create a Sprite #&s")]
	static public void AddSprite ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{	
			UISprite sprite = NGUITools.AddWidget<UISprite>(go);
			sprite.name = "Sprite";
			sprite.atlas = NGUISettings.atlas;
			
			if (sprite.atlas != null)
			{
				string sn = EditorPrefs.GetString("NGUI Sprite", "");
				UIAtlas.Sprite sp = sprite.atlas.GetSprite(sn);
				
				if (sp != null)
				{
					sprite.spriteName = sn;
					if (sp.inner != sp.outer) sprite.type = UISprite.Type.Sliced;
				}
			}
			sprite.pivot = NGUISettings.pivot;
			sprite.cachedTransform.localScale = new Vector3(100f, 100f, 1f);
			sprite.MakePixelPerfect();
			Selection.activeGameObject = sprite.gameObject;

			// Fixing deprecated Undo.RegisterSceneUndo("Add a Sprite");
			RebasedUndo.RegisterUndo(sprite.gameObject, "Add a Sprite");

		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Create a Label #&l")]
	static public void AddLabel ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.name = "Label";
			lbl.font = NGUISettings.font;
			lbl.text = "New Label";
			lbl.pivot = NGUISettings.pivot;
			lbl.cachedTransform.localScale = new Vector3(100f, 100f, 1f);
			lbl.MakePixelPerfect();
			Selection.activeGameObject = lbl.gameObject;

			RebasedUndo.RegisterUndo(lbl.gameObject, "Add a Label");
		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Create a Texture #&t")]
	static public void AddTexture ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
			UITexture tex = NGUITools.AddWidget<UITexture>(go);
			tex.name = "Texture";
			tex.pivot = NGUISettings.pivot;
			tex.cachedTransform.localScale = new Vector3(100f, 100f, 1f);
			Selection.activeGameObject = tex.gameObject;

			// Fixing deprecated Undo.RegisterSceneUndo("Add a Texture");
			RebasedUndo.RegisterUndo(tex.gameObject, "Add a Texture");

		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Create a Panel")]
	static public void AddPanel ()
	{
		GameObject go = SelectedRoot();

		if (NGUIEditorTools.WillLosePrefab(go))
		{
			NGUIEditorTools.RegisterUndo("Add a child UI Panel", go);

			GameObject child = new GameObject(NGUITools.GetName<UIPanel>());
			child.layer = go.layer;

			Transform ct = child.transform;
			ct.parent = go.transform;
			ct.localPosition = Vector3.zero;
			ct.localRotation = Quaternion.identity;
			ct.localScale = Vector3.one;

			child.AddComponent<UIPanel>().sortByDepth = true;
			Selection.activeGameObject = child;
		}
	}

	[MenuItem("NGUI/Attach a Collider #&c")]
	static public void AddCollider ()
	{
		GameObject go = Selection.activeGameObject;

		if (NGUIEditorTools.WillLosePrefab(go))
		{
			if (go != null)
			{
				NGUIEditorTools.RegisterUndo("Add Widget Collider", go);
				NGUITools.AddWidgetCollider(go);
			}
			else
			{
				Debug.Log("You must select a game object first, such as your button.");
			}
		}
	}

	[MenuItem("NGUI/Attach an Anchor #&h")]
	static public void AddAnchor ()
	{
		GameObject go = Selection.activeGameObject;

		if (go != null)
		{
			NGUIEditorTools.RegisterUndo("Add an Anchor", go);
			if (go.GetComponent<UIAnchor>() == null) go.AddComponent<UIAnchor>();
		}
		else
		{
			Debug.Log("You must select a game object first.");
		}
	}

	[MenuItem("NGUI/Make Pixel Perfect #&p")]
	static void PixelPerfectSelection ()
	{
		if (Selection.activeTransform == null)
		{
			Debug.Log("You must select an object in the scene hierarchy first");
			return;
		}
		foreach (Transform t in Selection.transforms) NGUITools.MakePixelPerfect(t);
	}

	[MenuItem("NGUI/Open the Widget Wizard")]
	static public void CreateWidgetWizard ()
	{
		EditorWindow.GetWindow<UICreateWidgetWizard>(false, "Widget Tool", true);
	}

	[MenuItem("NGUI/Open the UI Wizard")]
	static public void CreateUIWizard ()
	{
		EditorWindow.GetWindow<UICreateNewUIWizard>(false, "UI Tool", true);
	}

	[MenuItem("NGUI/Open the Panel Tool")]
	static public void OpenPanelWizard ()
	{
		EditorWindow.GetWindow<UIPanelTool>(false, "Panel Tool", true);
	}

	[MenuItem("NGUI/Open the Camera Tool")]
	static public void OpenCameraWizard ()
	{
		EditorWindow.GetWindow<UICameraTool>(false, "Camera Tool", true);
	}

	[MenuItem("NGUI/Open the Font Maker #&f")]
	static public void OpenFontMaker ()
	{
		EditorWindow.GetWindow<UIFontMaker>(false, "Font Maker", true);
	}

	[MenuItem("NGUI/Open the Atlas Maker #&m")]
	static public void OpenAtlasMaker ()
	{
		EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true);
	}

	[MenuItem("NGUI/Toggle Draggable Handles")]
	static public void ToggleNewGUI ()
	{
		//UIWidget.showHandlesWithMoveTool = !UIWidget.showHandlesWithMoveTool;

		//if (UIWidget.showHandlesWithMoveTool)
		//{
		//	Debug.Log("Simple Mode: Draggable Handles will show up with the Move Tool selected (W).");
		//}
		//else
		//{
			Debug.Log("Classic Mode: Draggable Handles will show up only with the View Tool selected (Q).");
		//}
	}
}
