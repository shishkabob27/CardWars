//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISlider))]
public class UISliderInspector : Editor
{
	void ValidatePivot (Transform fg, string name, UISlider.Direction dir)
	{
		if (fg != null)
		{
			UISprite sprite = fg.GetComponent<UISprite>();

			if (sprite != null && sprite.type != UISprite.Type.Filled)
			{
				if (dir == UISlider.Direction.Horizontal)
				{
					if (sprite.pivot != UIWidget.Pivot.Left &&
						sprite.pivot != UIWidget.Pivot.TopLeft &&
						sprite.pivot != UIWidget.Pivot.BottomLeft)
					{
						GUI.color = new Color(1f, 0.7f, 0f);
						GUILayout.Label(name + " should use a Left pivot");
						GUI.color = Color.white;
					}
				}
				else if (sprite.pivot != UIWidget.Pivot.BottomLeft &&
						 sprite.pivot != UIWidget.Pivot.Bottom &&
						 sprite.pivot != UIWidget.Pivot.BottomRight)
				{
					GUI.color = new Color(1f, 0.7f, 0f);
					GUILayout.Label(name + " should use a Bottom pivot");
					GUI.color = Color.white;
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		RebasedEditorGUIUtility.LookLikeControls(80f);
		UISlider slider = target as UISlider;

		NGUIEditorTools.DrawSeparator();

		float sliderValue = EditorGUILayout.Slider("Value", slider.sliderValue, 0f, 1f);

		if (slider.sliderValue != sliderValue)
		{
			NGUIEditorTools.RegisterUndo("Slider Change", slider);
			slider.sliderValue = sliderValue;
			UnityEditor.EditorUtility.SetDirty(slider);
		}

		int steps = EditorGUILayout.IntSlider("Steps", slider.numberOfSteps, 0, 11);

		if (slider.numberOfSteps != steps)
		{
			NGUIEditorTools.RegisterUndo("Slider Change", slider);
			slider.numberOfSteps = steps;
			slider.ForceUpdate();
			UnityEditor.EditorUtility.SetDirty(slider);
		}

		NGUIEditorTools.DrawSeparator();

		Transform fg = EditorGUILayout.ObjectField("Foreground", slider.foreground, typeof(Transform), true) as Transform;
		Transform tb = EditorGUILayout.ObjectField("Thumb", slider.thumb, typeof(Transform), true) as Transform;
		UISlider.Direction dir = (UISlider.Direction)EditorGUILayout.EnumPopup("Direction", slider.direction);

		// If we're using a sprite for the foreground, ensure it's using a proper pivot.
		ValidatePivot(fg, "Foreground sprite", dir);

		NGUIEditorTools.DrawSeparator();

		GameObject er = EditorGUILayout.ObjectField("Event Recv.", slider.eventReceiver, typeof(GameObject), true) as GameObject;

		GUILayout.BeginHorizontal();
		string fn = EditorGUILayout.TextField("Function", slider.functionName);
		GUILayout.Space(18f);
		GUILayout.EndHorizontal();

		if (slider.foreground != fg ||
			slider.thumb != tb ||
			slider.direction != dir ||
			slider.eventReceiver != er ||
			slider.functionName != fn)
		{
			NGUIEditorTools.RegisterUndo("Slider Change", slider);
			slider.foreground = fg;
			slider.thumb = tb;
			slider.direction = dir;
			slider.eventReceiver = er;
			slider.functionName = fn;

			if (slider.thumb != null)
			{
				slider.thumb.localPosition = Vector3.zero;
				slider.sliderValue = -1f;
				slider.sliderValue = sliderValue;
			}
			else slider.ForceUpdate();

			UnityEditor.EditorUtility.SetDirty(slider);
		}
	}
}
