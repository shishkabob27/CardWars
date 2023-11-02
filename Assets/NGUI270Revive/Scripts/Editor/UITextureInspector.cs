//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CustomEditor(typeof(UITexture))]
public class UITextureInspector : UIWidgetInspector
{
	UITexture mTex;

	protected override void OnEnable ()
	{
		base.OnEnable();
		mTex = target as UITexture;
	}

	protected override bool DrawProperties ()
	{
		if (mTex.material != null || mTex.mainTexture == null)
		{
			Material mat = EditorGUILayout.ObjectField("Material", mTex.material, typeof(Material), false) as Material;

			if (mTex.material != mat)
			{
				NGUIEditorTools.RegisterUndo("Material Selection", mTex);
				mTex.material = mat;
			}
		}

		if (mTex.material == null || mTex.hasDynamicMaterial)
		{
			Shader shader = EditorGUILayout.ObjectField("Shader", mTex.shader, typeof(Shader), false) as Shader;

			if (mTex.shader != shader)
			{
				NGUIEditorTools.RegisterUndo("Shader Selection", mTex);
				mTex.shader = shader;
			}

			Texture tex = EditorGUILayout.ObjectField("Texture", mTex.mainTexture, typeof(Texture), false) as Texture;

			if (mTex.mainTexture != tex)
			{
				NGUIEditorTools.RegisterUndo("Texture Selection", mTex);
				mTex.mainTexture = tex;
			}
		}

		if (mTex.mainTexture != null)
		{
			Rect rect = EditorGUILayout.RectField("UV Rectangle", mTex.uvRect);

			if (rect != mTex.uvRect)
			{
				NGUIEditorTools.RegisterUndo("UV Rectangle Change", mTex);
				mTex.uvRect = rect;
			}
		}
		return (mWidget.material != null);
	}

	/// <summary>
	/// Allow the texture to be previewed.
	/// </summary>

	public override bool HasPreviewGUI ()
	{
		return (mTex != null) && (mTex.mainTexture as Texture2D != null);
	}

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		Texture2D tex = mTex.mainTexture as Texture2D;

		if (tex != null)
		{
			Rect uv = mTex.uvRect;
			Rect outer = NGUIMath.ConvertToPixels(uv, tex.width, tex.height, true);
			NGUIEditorTools.DrawSprite(tex, rect, outer, outer, uv, mTex.color);
		}
	}
}
