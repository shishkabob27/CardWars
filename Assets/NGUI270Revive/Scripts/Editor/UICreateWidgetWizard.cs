//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI Widget Creation Wizard
/// </summary>

public class UICreateWidgetWizard : EditorWindow
{
	public enum WidgetType
	{
		Label,
		Sprite,
		Texture,
		Button,
		ImageButton,
		Checkbox,
		ProgressBar,
		Slider,
		Input,
		PopupList,
		PopupMenu,
		ScrollBar,
	}

	static WidgetType mType = WidgetType.Button;
	static string mSprite = "";
	static string mButton = "";
	static string mImage0 = "";
	static string mImage1 = "";
	static string mImage2 = "";
	static string mImage3 = "";
	static string mSliderBG = "";
	static string mSliderFG = "";
	static string mSliderTB = "";
	static string mCheckBG = "";
	static string mCheck = "";
	static string mInputBG = "";
	static string mListFG = "";
	static string mListBG = "";
	static string mListHL = "";
	static string mScrollBG = "";
	static string mScrollFG = "";
	static Color mColor = Color.white;
	static bool mLoaded = false;
	static bool mScrollCL = true;
	static UIScrollBar.Direction mScrollDir = UIScrollBar.Direction.Horizontal;

	/// <summary>
	/// Save the specified string into player prefs.
	/// </summary>

	static void SaveString (string field, string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			EditorPrefs.DeleteKey(field);
		}
		else
		{
			EditorPrefs.SetString(field, val);
		}
	}

	/// <summary>
	/// Load the specified string from player prefs.
	/// </summary>

	static string LoadString (string field) { string s = EditorPrefs.GetString(field); return (string.IsNullOrEmpty(s)) ? "" : s; }

	/// <summary>
	/// Save all serialized values in editor prefs.
	/// This is necessary because static values get wiped out as soon as scripts get recompiled.
	/// </summary>

	static void Save ()
	{
		EditorPrefs.SetInt("NGUI Widget Type", (int)mType);
		EditorPrefs.SetInt("NGUI Color", NGUIMath.ColorToInt(mColor));
		EditorPrefs.SetBool("NGUI ScrollCL", mScrollCL);
		EditorPrefs.SetInt("NGUI Scroll Dir", (int)mScrollDir);

		SaveString("NGUI Sprite", mSprite);
		SaveString("NGUI Button", mButton);
		SaveString("NGUI Image 0", mImage0);
		SaveString("NGUI Image 1", mImage1);
		SaveString("NGUI Image 2", mImage2);
		SaveString("NGUI Image 3", mImage3);
		SaveString("NGUI CheckBG", mCheckBG);
		SaveString("NGUI Check", mCheck);
		SaveString("NGUI SliderBG", mSliderBG);
		SaveString("NGUI SliderFG", mSliderFG);
		SaveString("NGUI SliderTB", mSliderTB);
		SaveString("NGUI InputBG", mInputBG);
		SaveString("NGUI ListFG", mListFG);
		SaveString("NGUI ListBG", mListBG);
		SaveString("NGUI ListHL", mListHL);
		SaveString("NGUI ScrollBG", mScrollBG);
		SaveString("NGUI ScrollFG", mScrollFG);
	}

	/// <summary>
	/// Load all serialized values from editor prefs.
	/// This is necessary because static values get wiped out as soon as scripts get recompiled.
	/// </summary>

	static void Load ()
	{
		mType = (WidgetType)EditorPrefs.GetInt("NGUI Widget Type", 0);
		mScrollDir = (UIScrollBar.Direction)EditorPrefs.GetInt("NGUI Scroll Dir", 0);

		int color = EditorPrefs.GetInt("NGUI Color", -1);
		if (color != -1) mColor = NGUIMath.IntToColor(color);

		mSprite		= LoadString("NGUI Sprite");
		mButton		= LoadString("NGUI Button");
		mImage0		= LoadString("NGUI Image 0");
		mImage1		= LoadString("NGUI Image 1");
		mImage2		= LoadString("NGUI Image 2");
		mImage3		= LoadString("NGUI Image 3");
		mCheckBG	= LoadString("NGUI CheckBG");
		mCheck		= LoadString("NGUI Check");
		mSliderBG	= LoadString("NGUI SliderBG");
		mSliderFG	= LoadString("NGUI SliderFG");
		mSliderTB	= LoadString("NGUI SliderTB");
		mInputBG	= LoadString("NGUI InputBG");
		mListFG		= LoadString("NGUI ListFG");
		mListBG		= LoadString("NGUI ListBG");
		mListHL		= LoadString("NGUI ListHL");
		mScrollBG	= LoadString("NGUI ScrollBG");
		mScrollFG	= LoadString("NGUI ScrollFG");
		mScrollCL	= EditorPrefs.GetBool("NGUI ScrollCL", true);
	}

	/// <summary>
	/// Atlas selection function.
	/// </summary>

	void OnSelectAtlas (MonoBehaviour obj)
	{
		NGUISettings.atlas = obj as UIAtlas;
		Repaint();
	}

	/// <summary>
	/// Font selection function.
	/// </summary>

	void OnSelectFont (MonoBehaviour obj)
	{
		NGUISettings.font = obj as UIFont;
		Repaint();
	}

	/// <summary>
	/// Convenience function -- creates the "Add To" button and the parent object field to the right of it.
	/// </summary>

	static public bool ShouldCreate (GameObject go, bool isValid)
	{
		GUI.color = isValid ? Color.green : Color.grey;

		GUILayout.BeginHorizontal();
		bool retVal = GUILayout.Button("Add To", GUILayout.Width(76f));
		GUI.color = Color.white;
		GameObject sel = EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(140f)) as GameObject;
		GUILayout.Label("Select the parent in the Hierarchy View", GUILayout.MinWidth(10000f));
		GUILayout.EndHorizontal();

		if (sel != go) Selection.activeGameObject = sel;

		if (retVal && isValid)
		{
			NGUIEditorTools.RegisterUndo("Add a Widget");
			return true;
		}
		return false;
	}

	/// <summary>
	/// Label creation function.
	/// </summary>

	void CreateLabel (GameObject go)
	{
		GUILayout.BeginHorizontal();
		Color c = EditorGUILayout.ColorField("Color", mColor, GUILayout.Width(220f));
		GUILayout.Label("Color tint the label will start with");
		GUILayout.EndHorizontal();

		if (mColor != c)
		{
			mColor = c;
			Save();
		}

		if (ShouldCreate(go, NGUISettings.font != null))
		{
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = NGUISettings.font;
			lbl.text = "New Label";
			lbl.color = mColor;
			lbl.MakePixelPerfect();
			Selection.activeGameObject = lbl.gameObject;
		}
	}

	/// <summary>
	/// Sprite creation function.
	/// </summary>

	void CreateSprite (GameObject go, string field)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Sprite", "Sprite that will be created", NGUISettings.atlas, field, OnSprite);

			if (!string.IsNullOrEmpty(field))
			{
				GUILayout.BeginHorizontal();
				NGUISettings.pivot = (UIWidget.Pivot)EditorGUILayout.EnumPopup("Pivot", NGUISettings.pivot, GUILayout.Width(200f));
				GUILayout.Space(20f);
				GUILayout.Label("Initial pivot point used by the sprite");
				GUILayout.EndHorizontal();
			}
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			UISprite sprite = NGUITools.AddWidget<UISprite>(go);
			sprite.name = sprite.name + " (" + field + ")";
			sprite.atlas = NGUISettings.atlas;
			sprite.spriteName = field;
			sprite.pivot = NGUISettings.pivot;
			sprite.MakePixelPerfect();
			Selection.activeGameObject = sprite.gameObject;
		}
	}

	void OnSprite (string val) { mSprite = val; Save(); Repaint(); }

	/// <summary>
	/// UI Texture doesn't do anything other than creating the widget.
	/// </summary>

	void CreateSimpleTexture (GameObject go)
	{
		if (ShouldCreate(go, true))
		{
			UITexture tex = NGUITools.AddWidget<UITexture>(go);
			Selection.activeGameObject = tex.gameObject;
		}
	}

	/// <summary>
	/// Button creation function.
	/// </summary>

	void CreateButton (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Background", "Sliced Sprite for the background", NGUISettings.atlas, mButton, OnButton);
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Button";

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mButton;
			bg.transform.localScale = new Vector3(150f, 40f, 1f);
			bg.MakePixelPerfect();

			if (NGUISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = NGUISettings.font;
				lbl.text = go.name;
				if (lbl.font.dynamicFont) lbl.transform.localPosition = new Vector3(0f, 0f, -1f);
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UIButton>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>();
			go.AddComponent<UIButtonOffset>();
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	void OnButton (string val) { mButton = val; Save(); Repaint(); }

	/// <summary>
	/// Button creation function.
	/// </summary>

	void CreateImageButton (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Normal", "Normal state sprite", NGUISettings.atlas, mImage0, OnImage0);
			NGUIEditorTools.SpriteField("Hover", "Hover state sprite", NGUISettings.atlas, mImage1, OnImage1);
			NGUIEditorTools.SpriteField("Pressed", "Pressed state sprite", NGUISettings.atlas, mImage2, OnImage2);
			NGUIEditorTools.SpriteField("Disabled", "Disabled state sprite", NGUISettings.atlas, mImage3, OnImage3);
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Image Button";

			UIAtlas.Sprite sp = NGUISettings.atlas.GetSprite(mImage0);
			UISprite sprite = NGUITools.AddWidget<UISprite>(go);
			sprite.type = (sp.inner == sp.outer) ? UISprite.Type.Simple : UISprite.Type.Sliced;
			sprite.name = "Background";
			sprite.depth = depth;
			sprite.atlas = NGUISettings.atlas;
			sprite.spriteName = mImage0;
			sprite.transform.localScale = new Vector3(150f, 40f, 1f);
			sprite.MakePixelPerfect();

			if (NGUISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = NGUISettings.font;
				lbl.text = go.name;
				if (lbl.font.dynamicFont) lbl.transform.localPosition = new Vector3(0f, 0f, -1f);
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			UIImageButton ib	= go.AddComponent<UIImageButton>();
			ib.target			= sprite;
			ib.normalSprite		= mImage0;
			ib.hoverSprite		= mImage1;
			ib.pressedSprite	= mImage2;
			ib.disabledSprite	= mImage3;
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	void OnImage0 (string val) { mImage0 = val; Save(); Repaint(); }
	void OnImage1 (string val) { mImage1 = val; Save(); Repaint(); }
	void OnImage2 (string val) { mImage2 = val; Save(); Repaint(); }
	void OnImage3 (string val) { mImage3 = val; Save(); Repaint(); }

	/// <summary>
	/// Checkbox creation function.
	/// </summary>

	void CreateCheckbox (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Background", "Sprite used for the background", NGUISettings.atlas, mCheckBG, OnCheckBG);
			NGUIEditorTools.SpriteField("Checkmark", "Sprite used for the checkmark", NGUISettings.atlas, mCheck, OnCheck);
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Checkbox";

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mCheckBG;
			bg.transform.localScale = new Vector3(26f, 26f, 1f);
			bg.MakePixelPerfect();

			UISprite fg = NGUITools.AddWidget<UISprite>(go);
			fg.name = "Checkmark";
			fg.atlas = NGUISettings.atlas;
			fg.spriteName = mCheck;
			fg.MakePixelPerfect();

			if (NGUISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = NGUISettings.font;
				lbl.text = go.name;
				lbl.pivot = UIWidget.Pivot.Left;
				lbl.transform.localPosition = new Vector3(16f, 0f, 0f);
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UICheckbox>().checkSprite = fg;
			go.AddComponent<UIButton>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>().tweenTarget = bg.transform;
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	void OnCheckBG (string val) { mCheckBG = val; Save(); Repaint(); }
	void OnCheck (string val) { mCheck = val; Save(); Repaint(); }

	/// <summary>
	/// Scroll bar template.
	/// </summary>

	void CreateScrollBar (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Background", "Sprite used for the background", NGUISettings.atlas, mScrollBG, OnScrollBG);
			NGUIEditorTools.SpriteField("Foreground", "Sprite used for the foreground (thumb)", NGUISettings.atlas, mScrollFG, OnScrollFG);

			GUILayout.BeginHorizontal();
			UIScrollBar.Direction dir = (UIScrollBar.Direction)EditorGUILayout.EnumPopup("Direction", mScrollDir, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Add colliders?", GUILayout.Width(90f));
			bool draggable = EditorGUILayout.Toggle(mScrollCL);
			GUILayout.EndHorizontal();

			if (mScrollCL != draggable || mScrollDir != dir)
			{
				mScrollCL = draggable;
				mScrollDir = dir;
				Save();
			}
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Scroll Bar";

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mScrollBG;
			bg.transform.localScale = new Vector3(400f + bg.border.x + bg.border.z, 14f + bg.border.y + bg.border.w, 1f);
			bg.MakePixelPerfect();

			UISprite fg = NGUITools.AddWidget<UISprite>(go);
			fg.type = UISprite.Type.Sliced;
			fg.name = "Foreground";
			fg.atlas = NGUISettings.atlas;
			fg.spriteName = mScrollFG;

			UIScrollBar sb = go.AddComponent<UIScrollBar>();
			sb.background = bg;
			sb.foreground = fg;
			sb.direction = mScrollDir;
			sb.barSize = 0.3f;
			sb.scrollValue = 0.3f;
			sb.ForceUpdate();

			if (mScrollCL)
			{
				NGUITools.AddWidgetCollider(bg.gameObject);
				NGUITools.AddWidgetCollider(fg.gameObject);
			}
			Selection.activeGameObject = go;
		}
	}

	void OnScrollBG (string val) { mScrollBG = val; Save(); Repaint(); }
	void OnScrollFG (string val) { mScrollFG = val; Save(); Repaint(); }

	/// <summary>
	/// Progress bar creation function.
	/// </summary>

	void CreateSlider (GameObject go, bool slider)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Empty", "Sprite for the background (empty bar)", NGUISettings.atlas, mSliderBG, OnSliderBG);
			NGUIEditorTools.SpriteField("Full", "Sprite for the foreground (full bar)", NGUISettings.atlas, mSliderFG, OnSliderFG);

			if (slider)
			{
				NGUIEditorTools.SpriteField("Thumb", "Sprite for the thumb indicator", NGUISettings.atlas, mSliderTB, OnSliderTB);
			}
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = slider ? "Slider" : "Progress Bar";

			// Background sprite
			UIAtlas.Sprite bgs = NGUISettings.atlas.GetSprite(mSliderBG);
			UISprite back = (UISprite)NGUITools.AddWidget<UISprite>(go);

			back.type = (bgs.inner == bgs.outer) ? UISprite.Type.Simple : UISprite.Type.Sliced;
			back.name = "Background";
			back.depth = depth;
			back.pivot = UIWidget.Pivot.Left;
			back.atlas = NGUISettings.atlas;
			back.spriteName = mSliderBG;
			back.transform.localScale = new Vector3(200f, 30f, 1f);
			back.transform.localPosition = Vector3.zero;
			back.MakePixelPerfect();

			// Foreground sprite
			UIAtlas.Sprite fgs = NGUISettings.atlas.GetSprite(mSliderFG);
			UISprite front = NGUITools.AddWidget<UISprite>(go);
			front.type = (fgs.inner == fgs.outer) ? UISprite.Type.Filled : UISprite.Type.Sliced;
			front.name = "Foreground";
			front.pivot = UIWidget.Pivot.Left;
			front.atlas = NGUISettings.atlas;
			front.spriteName = mSliderFG;
			front.transform.localScale = new Vector3(200f, 30f, 1f);
			front.transform.localPosition = Vector3.zero;
			front.MakePixelPerfect();

			// Add a collider
			if (slider) NGUITools.AddWidgetCollider(go);

			// Add the slider script
			UISlider uiSlider = go.AddComponent<UISlider>();
			uiSlider.foreground = front.transform;

			// Thumb sprite
			if (slider)
			{
				UIAtlas.Sprite tbs = NGUISettings.atlas.GetSprite(mSliderTB);
				UISprite thb = NGUITools.AddWidget<UISprite>(go);

				thb.type = (tbs.inner == tbs.outer) ? UISprite.Type.Simple : UISprite.Type.Sliced;
				thb.name = "Thumb";
				thb.atlas = NGUISettings.atlas;
				thb.spriteName = mSliderTB;
				thb.transform.localPosition = new Vector3(200f, 0f, 0f);
				thb.transform.localScale = new Vector3(20f, 40f, 1f);
				thb.MakePixelPerfect();

				NGUITools.AddWidgetCollider(thb.gameObject);
				thb.gameObject.AddComponent<UIButtonColor>();
				thb.gameObject.AddComponent<UIButtonScale>();

				uiSlider.thumb = thb.transform;
			}
			uiSlider.sliderValue = 1f;

			// Select the slider
			Selection.activeGameObject = go;
		}
	}

	void OnSliderBG (string val) { mSliderBG = val; Save(); Repaint(); }
	void OnSliderFG (string val) { mSliderFG = val; Save(); Repaint(); }
	void OnSliderTB (string val) { mSliderTB = val; Save(); Repaint(); }

	/// <summary>
	/// Input field creation function.
	/// </summary>

	void CreateInput (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Background", "Sliced Sprite for the background", NGUISettings.atlas, mInputBG, OnInputBG);
		}

		if (ShouldCreate(go, NGUISettings.atlas != null && NGUISettings.font != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Input";

			float padding = 3f;

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mInputBG;
			bg.pivot = UIWidget.Pivot.Left;
			bg.transform.localScale = new Vector3(400f, NGUISettings.font.size + padding * 2f, 1f);
			bg.transform.localPosition = Vector3.zero;
			bg.MakePixelPerfect();

			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = NGUISettings.font;
			lbl.pivot = UIWidget.Pivot.Left;
			lbl.transform.localPosition = new Vector3(padding, 0f, 0f);
			lbl.multiLine = false;
			lbl.supportEncoding = false;
			lbl.lineWidth = Mathf.RoundToInt(400f - padding * 2f);
			lbl.text = "You can type here";
			lbl.MakePixelPerfect();

			// Add a collider to the background
			NGUITools.AddWidgetCollider(go);

			// Add an input script to the background and have it point to the label
			UIInput input = go.AddComponent<UIInput>();
			input.label = lbl;

			// Update the selection
			Selection.activeGameObject = go;
		}
	}

	void OnInputBG (string val) { mInputBG = val; Save(); Repaint(); }

	/// <summary>
	/// Create a popup list or a menu.
	/// </summary>

	void CreatePopup (GameObject go, bool isDropDown)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.SpriteField("Foreground", "Foreground sprite (shown on the button)", NGUISettings.atlas, mListFG, OnListFG);
			NGUIEditorTools.SpriteField("Background", "Background sprite (envelops the options)", NGUISettings.atlas, mListBG, OnListBG);
			NGUIEditorTools.SpriteField("Highlight", "Sprite used to highlight the selected option", NGUISettings.atlas, mListHL, OnListHL);
		}

		if (ShouldCreate(go, NGUISettings.atlas != null && NGUISettings.font != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = isDropDown ? "Popup List" : "Popup Menu";

			UIAtlas.Sprite sphl = NGUISettings.atlas.GetSprite(mListHL);
			UIAtlas.Sprite spfg = NGUISettings.atlas.GetSprite(mListFG);

			Vector2 hlPadding = new Vector2(
				Mathf.Max(4f, sphl.inner.xMin - sphl.outer.xMin),
				Mathf.Max(4f, sphl.inner.yMin - sphl.outer.yMin));

			Vector2 fgPadding = new Vector2(
				Mathf.Max(4f, spfg.inner.xMin - spfg.outer.xMin),
				Mathf.Max(4f, spfg.inner.yMin - spfg.outer.yMin));

			// Background sprite
			UISprite sprite = NGUITools.AddSprite(go, NGUISettings.atlas, mListFG);
			sprite.depth = depth;
			sprite.atlas = NGUISettings.atlas;
			sprite.pivot = UIWidget.Pivot.Left;
			sprite.transform.localScale = new Vector3(150f + fgPadding.x * 2f, NGUISettings.font.size + fgPadding.y * 2f, 1f);
			sprite.transform.localPosition = Vector3.zero;
			sprite.MakePixelPerfect();

			// Text label
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = NGUISettings.font;
			lbl.text = go.name;
			lbl.pivot = UIWidget.Pivot.Left;
			lbl.cachedTransform.localPosition = new Vector3(fgPadding.x, 0f, 0f);
			lbl.MakePixelPerfect();

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the popup list
			UIPopupList list = go.AddComponent<UIPopupList>();
			list.atlas = NGUISettings.atlas;
			list.font = NGUISettings.font;
			list.backgroundSprite = mListBG;
			list.highlightSprite = mListHL;
			list.padding = hlPadding;
			if (isDropDown) list.textLabel = lbl;
			for (int i = 0; i < 5; ++i) list.items.Add(isDropDown ? ("List Option " + i) : ("Menu Option " + i));

			// Add the scripts
			go.AddComponent<UIButton>().tweenTarget = sprite.gameObject;
			go.AddComponent<UIButtonSound>();

			Selection.activeGameObject = go;
		}
	}

	void OnListFG (string val) { mListFG = val; Save(); Repaint(); }
	void OnListBG (string val) { mListBG = val; Save(); Repaint(); }
	void OnListHL (string val) { mListHL = val; Save(); Repaint(); }

	/// <summary>
	/// Repaint the window on selection.
	/// </summary>

	void OnSelectionChange () { Repaint(); }

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		// Load the saved preferences
		if (!mLoaded) { mLoaded = true; Load(); }

		RebasedEditorGUIUtility.LookLikeControls(80f);
		GameObject go = NGUIEditorTools.SelectedRoot();

		if (go == null)
		{
			GUILayout.Label("You must create a UI first.");
			
			if (GUILayout.Button("Open the New UI Wizard"))
			{
				EditorWindow.GetWindow<UICreateNewUIWizard>(false, "New UI", true);
			}
		}
		else
		{
			GUILayout.Space(4f);

			GUILayout.BeginHorizontal();
			ComponentSelector.Draw<UIAtlas>(NGUISettings.atlas, OnSelectAtlas, GUILayout.Width(140f));
			GUILayout.Label("Texture atlas used by widgets", GUILayout.MinWidth(10000f));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			ComponentSelector.Draw<UIFont>(NGUISettings.font, OnSelectFont, GUILayout.Width(140f));
			GUILayout.Label("Font used by labels", GUILayout.MinWidth(10000f));
			GUILayout.EndHorizontal();

			GUILayout.Space(-2f);
			NGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			WidgetType wt = (WidgetType)EditorGUILayout.EnumPopup("Template", mType, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Select a widget template to use");
			GUILayout.EndHorizontal();

			if (mType != wt) { mType = wt; Save(); }

			switch (mType)
			{
				case WidgetType.Label:			CreateLabel(go); break;
				case WidgetType.Sprite:			CreateSprite(go, mSprite); break;
				case WidgetType.Texture:		CreateSimpleTexture(go); break;
				case WidgetType.Button:			CreateButton(go); break;
				case WidgetType.ImageButton:	CreateImageButton(go); break;
				case WidgetType.Checkbox:		CreateCheckbox(go); break;
				case WidgetType.ProgressBar:	CreateSlider(go, false); break;
				case WidgetType.Slider:			CreateSlider(go, true); break;
				case WidgetType.Input:			CreateInput(go); break;
				case WidgetType.PopupList:		CreatePopup(go, true); break;
				case WidgetType.PopupMenu:		CreatePopup(go, false); break;
				case WidgetType.ScrollBar:		CreateScrollBar(go); break;
			}
		}
	}
}
