//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Editor component used to display a list of sprites.
/// </summary>

public class SpriteSelector : ScriptableWizard
{
	public delegate void Callback (string sprite);

	UIAtlas mAtlas;
	UISprite mSprite;
	string mName;
	Vector2 mPos = Vector2.zero;
	Callback mCallback;
	float mClickTime = 0f;

	/// <summary>
	/// Name of the selected sprite.
	/// </summary>

	public string spriteName { get { return (mSprite != null) ? mSprite.spriteName : mName; } }

	/// <summary>
	/// Show the selection wizard.
	/// </summary>

	public static void Show (UIAtlas atlas, string selectedSprite, Callback callback)
	{
		SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
		comp.mAtlas = atlas;
		comp.mSprite = null;
		comp.mName = selectedSprite;
		comp.mCallback = callback;
	}

	/// <summary>
	/// Show the selection wizard.
	/// </summary>

	public static void Show (UIAtlas atlas, UISprite selectedSprite)
	{
		SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
		comp.mAtlas = atlas;
		comp.mSprite = selectedSprite;
		comp.mCallback = null;
	}

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		RebasedEditorGUIUtility.LookLikeControls(80f);

		if (mAtlas == null)
		{
			GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
		}
		else
		{
			bool close = false;
			GUILayout.Label(mAtlas.name + " Sprites", "LODLevelNotifyText");
			NGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			GUILayout.Space(84f);

			string before = NGUISettings.partialSprite;
			string after = EditorGUILayout.TextField("", before, "SearchTextField");
			NGUISettings.partialSprite = after;

			if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
			{
				NGUISettings.partialSprite = "";
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.Space(84f);
			GUILayout.EndHorizontal();

			Texture2D tex = mAtlas.texture as Texture2D;

			if (tex == null)
			{
				GUILayout.Label("The atlas doesn't have a texture to work with");
				return;
			}

			BetterList<string> sprites = mAtlas.GetListOfSprites(NGUISettings.partialSprite);
			
			float size = 80f;
			float padded = size + 10f;
			int columns = Mathf.FloorToInt(Screen.width / padded);
			if (columns < 1) columns = 1;

			int offset = 0;
			Rect rect = new Rect(10f, 0, size, size);

			GUILayout.Space(10f);
			mPos = GUILayout.BeginScrollView(mPos);

			while (offset < sprites.size)
			{
				GUILayout.BeginHorizontal();
				{
					int col = 0;
					rect.x = 10f;

					for (; offset < sprites.size; ++offset)
					{
						UIAtlas.Sprite sprite = mAtlas.GetSprite(sprites[offset]);
						if (sprite == null) continue;

						// Button comes first
						if (GUI.Button(rect, ""))
						{
							float delta = Time.realtimeSinceStartup - mClickTime;
							mClickTime = Time.realtimeSinceStartup;

							if (spriteName != sprite.name)
							{
								if (mSprite != null)
								{
									NGUIEditorTools.RegisterUndo("Atlas Selection", mSprite);
									mSprite.spriteName = sprite.name;
									mSprite.MakePixelPerfect();
									EditorUtility.SetDirty(mSprite.gameObject);
								}
								
								if (mCallback != null)
								{
									mName = sprite.name;
									mCallback(sprite.name);
								}
							}
							else if (delta < 0.5f) close = true;
						}
						
						if (Event.current.type == EventType.Repaint)
						{
							// On top of the button we have a checkboard grid
							NGUIEditorTools.DrawTiledTexture(rect, NGUIEditorTools.backdropTexture);
	
							Rect uv = sprite.outer;
							if (mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
								uv = NGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);
	
							// Calculate the texture's scale that's needed to display the sprite in the clipped area
							float scaleX = rect.width / uv.width;
							float scaleY = rect.height / uv.height;
	
							// Stretch the sprite so that it will appear proper
							float aspect = (scaleY / scaleX) / ((float)tex.height / tex.width);
							Rect clipRect = rect;
	
							if (aspect != 1f)
							{
								if (aspect < 1f)
								{
									// The sprite is taller than it is wider
									float padding = size * (1f - aspect) * 0.5f;
									clipRect.xMin += padding;
									clipRect.xMax -= padding;
								}
								else
								{
									// The sprite is wider than it is taller
									float padding = size * (1f - 1f / aspect) * 0.5f;
									clipRect.yMin += padding;
									clipRect.yMax -= padding;
								}
							}
	
							GUI.DrawTextureWithTexCoords(clipRect, tex, uv);
	
							// Draw the selection
							if (spriteName == sprite.name)
							{
								NGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
							}
						}

						if (++col >= columns)
						{
							++offset;
							break;
						}
						rect.x += padded;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(padded);
				rect.y += padded;
			}
			GUILayout.EndScrollView();
			if (close) Close();
		}
	}
}
