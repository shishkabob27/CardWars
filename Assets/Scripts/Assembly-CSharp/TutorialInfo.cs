using UnityEngine;

public class TutorialInfo
{
	public class PointerInfo
	{
		public string Pointer;

		public string PointerTarget;

		public string Corner;

		public string PointerRotation;

		public string Animate;

		public bool Hide;

		public bool UpdatePosition;

		public bool ScaleTarget;

		public float OffsetX;

		public float OffsetY;

		public float OffsetZ;

		public bool ClickOnPointerTarget;

		public string Layer;
	}

	public class ButtonInfo
	{
		public string ButtonText;

		public string ButtonAction;

		public float ButtonActionDelay;

		public Vector3 ButtonPos;

		public Vector3 ButtonSize;

		public string ButtonSprite;
	}

	public string TutorialID;

	public string Flow;

	public string TweenTrigger;

	public bool CanOverride;

	public bool UseInputEnabler;

	public bool UseInputEnablerWhenDone;

	public PointerInfo Pointer1;

	public PointerInfo Pointer2;

	public string PointerTargetMax;

	public string Title;

	public Vector3 Pos;

	public Vector3 Rot;

	public Vector3 Scale;

	public string Layer;

	public string Text;

	public int AddKeys;

	public Vector3 TextPos;

	public int TextWidth;

	public ButtonInfo Button1;

	public ButtonInfo Button2;

	public string Sprite;

	public Vector3 SpritePos;

	public Vector3 SpriteSize;

	public Vector3 SpriteRot;

	public bool IsFinal;

	public bool IsLastInFlow;

	public TutorialTrigger Trigger;

	public TutorialTrigger DependencyTrigger;

	public bool Reusable;

	public int? CurrentQuest;

	public int? LastClearedQuest;

	public bool PauseGame;

	public string VOClip;

	public bool UseVOScale;

	public bool Skippable;

	public bool OnPress;

	public bool DimBackground;

	public bool DimFadeIn;

	public bool DimFadeOut;

	public bool DimDestroyOnClose;

	public bool dummy;
}
