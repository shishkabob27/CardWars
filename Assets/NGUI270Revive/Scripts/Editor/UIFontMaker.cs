//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Font maker lets you create font prefabs with a single click of a button.
/// </summary>

public class UIFontMaker : EditorWindow
{
	enum FontType
	{
		Bitmap,
		Dynamic,
	}

	FontType mType = FontType.Bitmap;

	/// <summary>
	/// Update all labels associated with this font.
	/// </summary>

	void MarkAsChanged ()
	{
		if (NGUISettings.font != null)
		{
			List<UILabel> labels = NGUIEditorTools.FindInScene<UILabel>();

			foreach (UILabel lbl in labels)
			{
				if (lbl.font == NGUISettings.font)
				{
					lbl.font = null;
					lbl.font = NGUISettings.font;
				}
			}
		}
	}

	/// <summary>
	/// Font selection callback.
	/// </summary>

	void OnSelectFont (MonoBehaviour obj)
	{
		NGUISettings.font = obj as UIFont;
		Repaint();
	}

	/// <summary>
	/// Atlas selection callback.
	/// </summary>

	void OnSelectAtlas (MonoBehaviour obj)
	{
		NGUISettings.atlas = obj as UIAtlas;
		Repaint();
	}

	/// <summary>
	/// Refresh the window on selection.
	/// </summary>

	void OnSelectionChange () { Repaint(); }

	/// <summary>
	/// Draw the UI for this tool.
	/// </summary>

	void OnGUI ()
	{
		string prefabPath = "";
		string matPath = "";

		if (NGUISettings.font != null && NGUISettings.font.name == NGUISettings.fontName)
		{
			prefabPath = AssetDatabase.GetAssetPath(NGUISettings.font.gameObject.GetInstanceID());
			if (NGUISettings.font.material != null) matPath = AssetDatabase.GetAssetPath(NGUISettings.font.material.GetInstanceID());
		}

		// Assume default values if needed
		if (string.IsNullOrEmpty(NGUISettings.fontName)) NGUISettings.fontName = "New Font";
		if (string.IsNullOrEmpty(prefabPath)) prefabPath = NGUIEditorTools.GetSelectionFolder() + NGUISettings.fontName + ".prefab";
		if (string.IsNullOrEmpty(matPath)) matPath = NGUIEditorTools.GetSelectionFolder() + NGUISettings.fontName + ".mat";

		RebasedEditorGUIUtility.LookLikeControls(80f);
		NGUIEditorTools.DrawHeader("Input");

		GUILayout.BeginHorizontal();
		mType = (FontType)EditorGUILayout.EnumPopup("Type", mType);
		GUILayout.Space(18f);
		GUILayout.EndHorizontal();
		int create = 0;

		if (mType == FontType.Dynamic)
		{
			NGUISettings.dynamicFont = EditorGUILayout.ObjectField("Font TTF", NGUISettings.dynamicFont, typeof(Font), false) as Font;

			GUILayout.BeginHorizontal();
			NGUISettings.dynamicFontSize = EditorGUILayout.IntField("Font Size", NGUISettings.dynamicFontSize, GUILayout.Width(120f));
			NGUISettings.dynamicFontStyle = (FontStyle)EditorGUILayout.EnumPopup(NGUISettings.dynamicFontStyle);
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();

			if (NGUISettings.dynamicFont != null)
			{
				NGUIEditorTools.DrawHeader("Output");

				GUILayout.BeginHorizontal();
				GUILayout.Label("Font Name", GUILayout.Width(76f));
				GUI.backgroundColor = Color.white;
				NGUISettings.fontName = GUILayout.TextField(NGUISettings.fontName);
				GUILayout.EndHorizontal();
			}
			NGUIEditorTools.DrawSeparator();

#if UNITY_3_5
			EditorGUILayout.HelpBox("Dynamic fonts require Unity 4.0 or higher.", MessageType.Error);
#else
			// Helpful info
			if (NGUISettings.dynamicFont == null)
			{
				EditorGUILayout.HelpBox("Dynamic font creation happens right in Unity. Simply specify the TrueType font to be used as source.", MessageType.Info);
			}
			EditorGUILayout.HelpBox("Please note that dynamic fonts can't be made a part of an atlas, and they will always be drawn in a separate draw call. You WILL need to adjust transform position's Z rather than depth!", MessageType.Warning);

			if (NGUISettings.dynamicFont != null)
			{
				NGUIEditorTools.DrawSeparator();

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUI.backgroundColor = Color.green;

				GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

				if (go != null)
				{
					if (go.GetComponent<UIFont>() != null)
					{
						GUI.backgroundColor = Color.red;
						if (GUILayout.Button("Replace the Font", GUILayout.Width(140f))) create = 1;
					}
					else
					{
						GUI.backgroundColor = Color.grey;
						GUILayout.Button("Rename Your Font", GUILayout.Width(140f));
					}
				}
				else
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Create the Font", GUILayout.Width(140f))) create = 1;
				}

				GUI.backgroundColor = Color.white;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
#endif
		}
		else
		{
			NGUISettings.fontData = EditorGUILayout.ObjectField("Font Data", NGUISettings.fontData, typeof(TextAsset), false) as TextAsset;
			NGUISettings.fontTexture = EditorGUILayout.ObjectField("Texture", NGUISettings.fontTexture, typeof(Texture2D), false) as Texture2D;

			// Draw the atlas selection only if we have the font data and texture specified, just to make it easier
			if (NGUISettings.fontData != null && NGUISettings.fontTexture != null)
			{
				NGUIEditorTools.DrawHeader("Output");

				GUILayout.BeginHorizontal();
				GUILayout.Label("Font Name", GUILayout.Width(76f));
				GUI.backgroundColor = Color.white;
				NGUISettings.fontName = GUILayout.TextField(NGUISettings.fontName);
				GUILayout.EndHorizontal();

				ComponentSelector.Draw<UIFont>("Select", NGUISettings.font, OnSelectFont);
				ComponentSelector.Draw<UIAtlas>(NGUISettings.atlas, OnSelectAtlas);
			}
			NGUIEditorTools.DrawSeparator();

			// Helpful info
			if (NGUISettings.fontData == null)
			{
				EditorGUILayout.HelpBox("The bitmap font creation mostly takes place outside of Unity. You can use BMFont on " +
					"Windows or your choice of Glyph Designer or the less expensive bmGlyph on the Mac.\n\n" +
					"Either of these tools will create a FNT file for you that you will drag & drop into the field above.", MessageType.Info);
			}
			else if (NGUISettings.fontTexture == null)
			{
				EditorGUILayout.HelpBox("When exporting your font, you should get two files: the TXT, and the texture. Only one texture can be used per font.", MessageType.Info);
			}
			else if (NGUISettings.atlas == null)
			{
				EditorGUILayout.HelpBox("You can create a font that doesn't use a texture atlas. This will mean that the text " +
					"labels using this font will generate an extra draw call, and will need to be sorted by " +
					"adjusting the Z instead of the Depth.\n\nIf you do specify an atlas, the font's texture will be added to it automatically.", MessageType.Info);

				NGUIEditorTools.DrawSeparator();

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Create a Font without an Atlas", GUILayout.Width(200f))) create = 2;
				GUI.backgroundColor = Color.white;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

				if (go != null)
				{
					if (go.GetComponent<UIFont>() != null)
					{
						GUI.backgroundColor = Color.red;
						if (GUILayout.Button("Replace the Font", GUILayout.Width(140f))) create = 3;
					}
					else
					{
						GUI.backgroundColor = Color.grey;
						GUILayout.Button("Rename Your Font", GUILayout.Width(140f));
					}
				}
				else
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Create the Font", GUILayout.Width(140f))) create = 3;
				}
				GUI.backgroundColor = Color.white;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		if (create != 0)
		{
			GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

			if (go == null || EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to replace the contents of the " +
				NGUISettings.fontName + " font with the currently selected values? This action can't be undone.", "Yes", "No"))
			{
				// Try to load the material
				Material mat = null;
				
				// Non-atlased font
				if (create == 2)
				{
					mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

					// If the material doesn't exist, create it
					if (mat == null)
					{
						Shader shader = Shader.Find("Unlit/Transparent Colored");
						mat = new Material(shader);

						// Save the material
						AssetDatabase.CreateAsset(mat, matPath);
						AssetDatabase.Refresh();

						// Load the material so it's usable
						mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
					}
					mat.mainTexture = NGUISettings.fontTexture;
				}
				else if (create != 1)
				{
					UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, NGUISettings.fontTexture);
				}

				// Font doesn't exist yet
				if (go == null || go.GetComponent<UIFont>() == null)
				{
					// Create a new prefab for the atlas
					Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);

					// Create a new game object for the font
					go = new GameObject(NGUISettings.fontName);
					NGUISettings.font = go.AddComponent<UIFont>();
					CreateFont(NGUISettings.font, create, mat);

					// Update the prefab
					PrefabUtility.ReplacePrefab(go, prefab);
					DestroyImmediate(go);
					AssetDatabase.Refresh();

					// Select the atlas
					go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
					NGUISettings.font = go.GetComponent<UIFont>();
				}
				else
				{
					NGUISettings.font = go.GetComponent<UIFont>();
					CreateFont(NGUISettings.font, create, mat);
				}
				MarkAsChanged();
			}
		}
	}

	static void CreateFont (UIFont font, int create, Material mat)
	{
		if (create == 1)
		{
			// New dynamic font
			font.atlas = null;
			font.dynamicFont = NGUISettings.dynamicFont;
			font.dynamicFontSize = NGUISettings.dynamicFontSize;
			font.dynamicFontStyle = NGUISettings.dynamicFontStyle;
		}
		else
		{
			// New bitmap font
			font.dynamicFont = null;
			BMFontReader.Load(font.bmFont, NGUITools.GetHierarchy(font.gameObject), NGUISettings.fontData.bytes);

			if (create == 2)
			{
				font.atlas = null;
				font.material = mat;
			}
			else if (create == 3)
			{
				font.spriteName = NGUISettings.fontTexture.name;
				font.atlas = NGUISettings.atlas;
			}
		}
	}
}
