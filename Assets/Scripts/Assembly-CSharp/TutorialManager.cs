using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : ILoadable
{
	public Dictionary<string, TutorialInfo> tutorials = new Dictionary<string, TutorialInfo>();

	public Dictionary<TutorialTrigger, TutorialInfo> triggers = new Dictionary<TutorialTrigger, TutorialInfo>();

	public Dictionary<string, string> tweenTriggers = new Dictionary<string, string>();

	private static TutorialManager instance;

	public static TutorialManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new TutorialManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Tutorials.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			TutorialInfo info = new TutorialInfo();
			info.Pointer1 = new TutorialInfo.PointerInfo();
			info.Pointer2 = new TutorialInfo.PointerInfo();
			info.Button1 = new TutorialInfo.ButtonInfo();
			info.Button2 = new TutorialInfo.ButtonInfo();
			info.TutorialID = TFUtils.LoadString(dict, "Tutorial_ID", string.Empty);
			info.TweenTrigger = TFUtils.LoadString(dict, "Tween Trigger", string.Empty);
			info.CanOverride = TFUtils.LoadBool(dict, "CanOverride", false);
			info.UseInputEnabler = TFUtils.LoadBool(dict, "UseInputEnabler", false);
			info.UseInputEnablerWhenDone = TFUtils.LoadBool(dict, "UseInputEnablerWhenDone", false);
			info.Pointer1.Pointer = TFUtils.LoadString(dict, "Pointer", string.Empty);
			info.Pointer1.PointerTarget = TFUtils.LoadString(dict, "PointerTarget", string.Empty);
			info.Pointer1.Corner = TFUtils.LoadString(dict, "Corner", string.Empty);
			info.Pointer1.Animate = TFUtils.LoadString(dict, "Animate", string.Empty);
			info.Pointer1.Hide = TFUtils.LoadBool(dict, "Hide", false);
			info.Pointer1.UpdatePosition = TFUtils.LoadBool(dict, "UpdatePosition", false);
			info.Pointer1.ScaleTarget = TFUtils.LoadBool(dict, "ScaleTarget", false);
			info.Pointer1.OffsetX = TFUtils.LoadFloat(dict, "OffsetX", 0f);
			info.Pointer1.OffsetY = TFUtils.LoadFloat(dict, "OffsetY", 0f);
			info.Pointer1.OffsetZ = TFUtils.LoadFloat(dict, "OffsetZ", 0f);
			info.Pointer1.PointerRotation = TFUtils.LoadString(dict, "PointerRotation1", string.Empty);
			info.Pointer1.ClickOnPointerTarget = TFUtils.LoadBool(dict, "ClickOnPointerTarget1", false);
			info.Pointer1.Layer = TFUtils.LoadString(dict, "PointerLayer1", string.Empty);
			info.Pointer2.Pointer = TFUtils.LoadString(dict, "Pointer 2", string.Empty);
			info.Pointer2.PointerTarget = TFUtils.LoadString(dict, "PointerTarget 2", string.Empty);
			info.Pointer2.Corner = TFUtils.LoadString(dict, "Corner 2", string.Empty);
			info.Pointer2.Animate = TFUtils.LoadString(dict, "Animate 2", string.Empty);
			info.Pointer2.Hide = TFUtils.LoadBool(dict, "Hide 2", false);
			info.Pointer2.UpdatePosition = TFUtils.LoadBool(dict, "UpdatePosition2", false);
			info.Pointer2.ScaleTarget = TFUtils.LoadBool(dict, "ScaleTarget2", false);
			info.Pointer2.OffsetX = TFUtils.LoadFloat(dict, "OffsetX 2", 0f);
			info.Pointer2.OffsetY = TFUtils.LoadFloat(dict, "OffsetY 2", 0f);
			info.Pointer2.OffsetZ = TFUtils.LoadFloat(dict, "OffsetZ 2", 0f);
			info.Pointer2.PointerRotation = TFUtils.LoadString(dict, "PointerRotation2", string.Empty);
			info.Pointer2.ClickOnPointerTarget = TFUtils.LoadBool(dict, "ClickOnPointerTarget2", false);
			info.Pointer2.Layer = TFUtils.LoadString(dict, "PointerLayer2", string.Empty);
			info.PointerTargetMax = TFUtils.LoadString(dict, "PointerTargetMax", string.Empty);
			info.Title = TFUtils.LoadString(dict, "Title", string.Empty);
			info.Text = TFUtils.LoadLocalizedString(dict, "Text", string.Empty);
			info.AddKeys = TFUtils.LoadInt(dict, "AddKeys", 0);
			info.TextWidth = TFUtils.LoadInt(dict, "Text Width", 0);
			info.Button1.ButtonText = TFUtils.LoadString(dict, "Button Text", string.Empty);
			info.Button1.ButtonAction = TFUtils.LoadString(dict, "Button Action", string.Empty);
			info.Button1.ButtonActionDelay = TFUtils.LoadFloat(dict, "Button Action Delay", 0f);
			info.Button1.ButtonSprite = TFUtils.LoadString(dict, "Button Sprite", string.Empty);
			info.Button2.ButtonText = TFUtils.LoadString(dict, "Button2 Text", string.Empty);
			info.Button2.ButtonAction = TFUtils.LoadString(dict, "Button2 Action", string.Empty);
			info.Button2.ButtonActionDelay = TFUtils.LoadFloat(dict, "Button2 Action Delay", 0f);
			info.Button2.ButtonSprite = TFUtils.LoadString(dict, "Button2 Sprite", string.Empty);
			info.Sprite = TFUtils.LoadString(dict, "Sprite", string.Empty);
			info.Layer = TFUtils.LoadString(dict, "Layer", "GUI");
			info.IsFinal = false;
			bool.TryParse(TFUtils.LoadString(dict, "IsFinal", "false"), out info.IsFinal);
			info.IsLastInFlow = false;
			bool.TryParse(TFUtils.LoadString(dict, "IsLastInFlow", "false"), out info.IsLastInFlow);
			info.Pos = new Vector3(TFUtils.LoadFloat(dict, "Pos X", 0f), TFUtils.LoadFloat(dict, "Pos Y", 0f), TFUtils.LoadFloat(dict, "Pos Z", 0f));
			info.Rot = new Vector3(TFUtils.LoadFloat(dict, "Rot X", 0f), TFUtils.LoadFloat(dict, "Rot Y", 0f), TFUtils.LoadFloat(dict, "Rot Z", 0f));
			info.Scale = new Vector3(TFUtils.LoadFloat(dict, "Scale X", 1f), TFUtils.LoadFloat(dict, "Scale Y", 1f), TFUtils.LoadFloat(dict, "Scale Z", 1f));
			info.TextPos = new Vector2(TFUtils.LoadFloat(dict, "Text Pos X", 0f), TFUtils.LoadFloat(dict, "Text Pos Y", 0f));
			info.SpritePos = new Vector2(TFUtils.LoadFloat(dict, "Sprite Pos X", 0f), TFUtils.LoadFloat(dict, "Sprite Pos Y", 0f));
			info.SpriteSize = new Vector2(TFUtils.LoadFloat(dict, "Sprite Size X", 1f), TFUtils.LoadFloat(dict, "Sprite Size Y", 1f));
			info.SpriteRot = new Vector3(0f, 0f, TFUtils.LoadFloat(dict, "Sprite Rot Z", 0f));
			info.Button1.ButtonPos = new Vector2(TFUtils.LoadFloat(dict, "Button Pos X", 0f), TFUtils.LoadFloat(dict, "Button Pos Y", 0f));
			info.Button1.ButtonSize = new Vector2(TFUtils.LoadFloat(dict, "Button Size X", 1f), TFUtils.LoadFloat(dict, "Button Size Y", 1f));
			info.Button2.ButtonPos = new Vector2(TFUtils.LoadFloat(dict, "Button2 Pos X", 0f), TFUtils.LoadFloat(dict, "Button2 Pos Y", 0f));
			info.Button2.ButtonSize = new Vector2(TFUtils.LoadFloat(dict, "Button2 Size X", 1f), TFUtils.LoadFloat(dict, "Button2 Size Y", 1f));
			try
			{
				info.Trigger = (TutorialTrigger)(int)Enum.Parse(typeof(TutorialTrigger), TFUtils.LoadString(dict, "Trigger"), true);
			}
			catch
			{
				info.Trigger = TutorialTrigger.None;
			}
			try
			{
				info.DependencyTrigger = (TutorialTrigger)(int)Enum.Parse(typeof(TutorialTrigger), TFUtils.LoadString(dict, "DependencyTrigger", string.Empty), true);
			}
			catch
			{
				info.DependencyTrigger = TutorialTrigger.None;
			}
			if (info.Trigger != 0)
			{
				triggers.Add(info.Trigger, info);
			}
			info.Reusable = TFUtils.LoadBool(dict, "Reusable", false);
			info.CurrentQuest = TFUtils.TryLoadNullableInt(dict, "CurrentQuest");
			info.LastClearedQuest = TFUtils.TryLoadNullableInt(dict, "LastClearedQuest");
			info.PauseGame = TFUtils.LoadBool(dict, "PauseGame", true);
			info.VOClip = TFUtils.LoadString(dict, "VOClip", string.Empty);
			info.UseVOScale = TFUtils.LoadBool(dict, "UseVOScale", true);
			info.Skippable = TFUtils.LoadBool(dict, "Skippable", true);
			info.OnPress = TFUtils.LoadBool(dict, "OnPress", false);
			info.DimBackground = TFUtils.LoadBool(dict, "DimBackground", false);
			info.DimFadeIn = TFUtils.LoadBool(dict, "DimFadeIn", true);
			info.DimFadeOut = TFUtils.LoadBool(dict, "DimFadeOut", true);
			info.DimDestroyOnClose = TFUtils.LoadBool(dict, "DimDestroyOnClose", false);
			info.dummy = TFUtils.LoadString(dict, "Type", string.Empty) == "dummy";
			info.Flow = TFUtils.LoadString(dict, "Flow", string.Empty);
			tutorials.Add(info.TutorialID, info);
			if (!string.IsNullOrEmpty(info.TweenTrigger))
			{
				tweenTriggers.Add(info.TweenTrigger, info.TutorialID);
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public void Destroy()
	{
		instance = null;
	}

	public TutorialInfo Find(string id)
	{
		if (!tutorials.ContainsKey(id))
		{
			return null;
		}
		return tutorials[id];
	}

	public TutorialInfo Find(TutorialTrigger trigger)
	{
		if (!triggers.ContainsKey(trigger))
		{
			return null;
		}
		return triggers[trigger];
	}

	public void markTutorialCompleted(string qname)
	{
		PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
		if (!playerInfoScript.tutorialsCompleted.Contains(qname))
		{
			playerInfoScript.tutorialsCompleted.Add(qname);
		}
	}

	public bool isTutorialCompleted(string qname)
	{
		PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
		TutorialInfo tutorialInfo = Find(qname);
		if (tutorialInfo != null && !string.IsNullOrEmpty(tutorialInfo.Flow) && playerInfoScript.tutorialsCompleted.Contains(tutorialInfo.Flow))
		{
			return true;
		}
		return playerInfoScript.tutorialsCompleted.Contains(qname);
	}

	public bool isTutorialCompleted(TutorialTrigger trigger)
	{
		TutorialInfo tutorialInfo = Find(trigger);
		if (tutorialInfo != null)
		{
			if (isTutorialCompleted(tutorialInfo.TutorialID))
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public void StartAfterDelay(string id, float waitTime, bool onlyOnce = false, GameObject Pointer = null, GameObject Pointer2 = null)
	{
	}

	public void FixupFlow()
	{
		foreach (string key in tutorials.Keys)
		{
			TutorialInfo tutorialInfo = tutorials[key];
			if (tutorialInfo != null && !string.IsNullOrEmpty(tutorialInfo.Flow))
			{
				PlayerInfoScript playerInfoScript = PlayerInfoScript.GetInstance();
				if (playerInfoScript != null && playerInfoScript.tutorialsCompleted != null && !playerInfoScript.tutorialsCompleted.Contains(tutorialInfo.Flow) && playerInfoScript.tutorialsCompleted.Contains(tutorialInfo.TutorialID))
				{
					playerInfoScript.tutorialsCompleted.Remove(tutorialInfo.TutorialID);
				}
			}
		}
	}
}
