//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// EditorGUILayout.ObjectField doesn't support custom components, so a custom wizard saves the day.
/// Unfortunately this tool only shows components that are being used by the scene, so it's a "recently used" selection tool.
/// </summary>

public class ComponentSelector : ScriptableWizard
{
	public delegate void OnSelectionCallback (MonoBehaviour obj);

	System.Type mType;
	OnSelectionCallback mCallback;
	MonoBehaviour[] mObjects;

	/// <summary>
	/// Draw a button + object selection combo filtering specified types.
	/// </summary>

	static public void Draw<T> (string buttonName, T obj, OnSelectionCallback cb, params GUILayoutOption[] options) where T : MonoBehaviour
	{
		GUILayout.BeginHorizontal();
		bool show = GUILayout.Button(buttonName, "DropDownButton", GUILayout.Width(76f));
		GUILayout.BeginVertical();
		GUILayout.Space(5f);

		T o = EditorGUILayout.ObjectField(obj, typeof(T), false, options) as T;
		GUILayout.EndVertical();

		if (o != null && Selection.activeObject != o.gameObject && GUILayout.Button("Edit", GUILayout.Width(40f)))
		{
			Selection.activeObject = o.gameObject;
		}
		GUILayout.EndHorizontal();
		if (show) Show<T>(cb);
		else if (o != obj) cb(o);
	}

	/// <summary>
	/// Draw a button + object selection combo filtering specified types.
	/// </summary>

	static public void Draw<T> (T obj, OnSelectionCallback cb, params GUILayoutOption[] options) where T : MonoBehaviour
	{
		Draw<T>(NGUITools.GetName<T>(), obj, cb, options);
	}

	/// <summary>
	/// Show the selection wizard.
	/// </summary>

	static void Show<T> (OnSelectionCallback cb) where T : MonoBehaviour
	{
		System.Type type = typeof(T);
		ComponentSelector comp = ScriptableWizard.DisplayWizard<ComponentSelector>("Select " + type.ToString());
		comp.mType = type;
		comp.mCallback = cb;
		comp.mObjects = Resources.FindObjectsOfTypeAll(type) as MonoBehaviour[];
	}

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		RebasedEditorGUIUtility.LookLikeControls(80f);
		GUILayout.Label("Recently used components", "LODLevelNotifyText");
		NGUIEditorTools.DrawSeparator();

		if (mObjects.Length == 0)
		{
			EditorGUILayout.HelpBox("No recently used " + mType.ToString() + " components found.\nTry drag & dropping one instead, or creating a new one.", MessageType.Info);

			bool isDone = false;

			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (mType == typeof(UIFont))
			{
				if (GUILayout.Button("Open the Font Maker", GUILayout.Width(150f)))
				{
					EditorWindow.GetWindow<UIFontMaker>(false, "Font Maker", true);
					isDone = true;
				}
			}
			else if (mType == typeof(UIAtlas))
			{
				if (GUILayout.Button("Open the Atlas Maker", GUILayout.Width(150f)))
				{
					EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true);
					isDone = true;
				}
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if (isDone) Close();
		}
		else
		{
			MonoBehaviour sel = null;

			foreach (MonoBehaviour o in mObjects)
			{
				if (DrawObject(o))
				{
					sel = o;
				}
			}

			if (sel != null)
			{
				mCallback(sel);
				Close();
			}
		}
	}

	/// <summary>
	/// Draw details about the specified monobehavior in column format.
	/// </summary>

	bool DrawObject (MonoBehaviour mb)
	{
		bool retVal = false;

		GUILayout.BeginHorizontal();
		{
			if (EditorUtility.IsPersistent(mb.gameObject))
			{
				GUILayout.Label("Prefab", "AS TextArea", GUILayout.Width(80f), GUILayout.Height(20f));
			}
			else
			{
				GUI.color = Color.grey;
				GUILayout.Label("Object", "AS TextArea", GUILayout.Width(80f), GUILayout.Height(20f));
			}

			GUILayout.Label(NGUITools.GetHierarchy(mb.gameObject), "AS TextArea", GUILayout.Height(20f));
			GUI.color = Color.white;

			retVal = GUILayout.Button("Select", "ButtonLeft", GUILayout.Width(60f), GUILayout.Height(16f));
		}
		GUILayout.EndHorizontal();
		return retVal;
	}
}
