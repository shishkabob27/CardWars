using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SlotQuest/CWTutorialsPopup")]
public class CWTutorialsPopup : MonoBehaviour
{
	private class GameObjectNameComparer : IComparer<UnityEngine.Object>
	{
		public int Compare(UnityEngine.Object o1, UnityEngine.Object o2)
		{
			GameObject gameObject = o1 as GameObject;
			GameObject gameObject2 = o2 as GameObject;
			string name = gameObject.name;
			string name2 = gameObject2.name;
			return string.Compare(name, name2);
		}
	}

	public static float FF_DELAY;

	public static float SKIP_DELAY = 1f;

	private static float TYPEWRITER_SPEED_SCALE = 3f;

	public string tutorial_ID;

	public AudioClip VOClip;

	public float VOLength;

	public bool onlyOnce;

	private GameObject pointer;

	private GameObject pointer2;

	private GameObject target;

	private bool findTargetFailed;

	private bool flag;

	private float timer;

	private TutorialInfo tutorialInfo;

	private Transform spriteGroup;

	private Transform spriteGroup2;

	private bool done;

	private Transform titleT;

	private static GameObject dummy;

	private GameObject button1;

	public GameObject Pointer
	{
		set
		{
			pointer = value;
			spriteGroup = FindSpriteGroup(pointer);
		}
	}

	public GameObject Pointer2
	{
		set
		{
			pointer2 = value;
			spriteGroup2 = FindSpriteGroup(pointer2);
		}
	}

	public GameObject Button1
	{
		get
		{
			return button1;
		}
	}

	public bool Done
	{
		set
		{
			done = value;
		}
	}

	private void Awake()
	{
		if (dummy == null)
		{
			dummy = new GameObject();
			dummy.transform.parent = base.transform;
			dummy.name = "dummy";
		}
	}

	public void OnClick()
	{
		OnClick(false);
	}

	public void OnClick(bool force)
	{
		if ((!base.enabled && !force) || (done && onlyOnce && !force))
		{
			return;
		}
		this.tutorialInfo = TutorialManager.Instance.Find(tutorial_ID);
		TutorialMonitor.Instance.StopTutorialAudio();
		TutorialInfo tutorialInfo = this.tutorialInfo;
		if (tutorialInfo == null)
		{
			return;
		}
		CWTutorialSkip.CloseTutorialPopup(base.gameObject, true, null, tutorialInfo.UseInputEnablerWhenDone);
		TutorialManager.Instance.markTutorialCompleted(tutorialInfo.TutorialID);
		TutorialMonitor.Instance.OnStartTutorial(tutorialInfo);
		GameObject popup = null;
		if (!tutorialInfo.dummy)
		{
			PlayTutorialVoiceOver(tutorialInfo, 0.3f);
			List<GameObject> list = new List<GameObject>();
			setCollider(null, null);
			if (tutorialInfo.Pointer1.Pointer != string.Empty)
			{
				Pointer = CreatePointer(tutorialInfo.Pointer1);
				setPointerPos(tutorialInfo.Pointer1, pointer, spriteGroup);
				GameObject pointerTarget = getPointerTarget(tutorialInfo.Pointer1);
				if (pointerTarget != null)
				{
					SetupInputEnabler(tutorialInfo.Pointer1, pointerTarget, false, false, SKIP_DELAY);
					if (tutorialInfo.UseInputEnabler && tutorialInfo.Pointer1.ClickOnPointerTarget)
					{
						list.Add(pointerTarget);
					}
				}
			}
			if (tutorialInfo.Pointer2.Pointer != string.Empty)
			{
				pointer2 = CreatePointer(tutorialInfo.Pointer2);
				spriteGroup2 = FindSpriteGroup(pointer2);
				setPointerPos(tutorialInfo.Pointer2, pointer2, spriteGroup2);
				GameObject pointerTarget2 = getPointerTarget(tutorialInfo.Pointer2);
				if (pointerTarget2 != null)
				{
					SetupInputEnabler(tutorialInfo.Pointer2, pointerTarget2, false, false, (!string.IsNullOrEmpty(tutorialInfo.Text)) ? SKIP_DELAY : 0f);
					if (tutorialInfo.UseInputEnabler && tutorialInfo.Pointer2.ClickOnPointerTarget)
					{
						list.Add(pointerTarget2);
					}
				}
			}
			if (findTargetFailed)
			{
				return;
			}
			target = null;
			popup = CreatePopup(tutorialInfo);
			foreach (GameObject item in list)
			{
				if (item != null && item.GetComponent<Collider>() != null && item.GetComponent<Collider>().enabled)
				{
					CWTutorialNext cWTutorialNext = item.GetComponent(typeof(CWTutorialNext)) as CWTutorialNext;
					if (cWTutorialNext == null)
					{
						cWTutorialNext = item.AddComponent(typeof(CWTutorialNext)) as CWTutorialNext;
					}
					if (cWTutorialNext != null)
					{
						cWTutorialNext.target = button1;
						cWTutorialNext.functionName = "OnClick";
						cWTutorialNext.useOnPress = tutorialInfo.OnPress;
					}
				}
			}
		}
		if (tutorialInfo.UseInputEnabler)
		{
			UICamera.useInputEnabler = true;
		}
		TutorialMonitor.Instance.OnTutorialStarted(tutorialInfo, popup);
		done = true;
	}

	private void RetryOnClick()
	{
		if (pointer != null)
		{
			UnityEngine.Object.Destroy(pointer);
			pointer = null;
		}
		if (pointer2 != null)
		{
			UnityEngine.Object.Destroy(pointer2);
			pointer2 = null;
		}
		OnClick(true);
	}

	private void PlayTutorialVoiceOver(TutorialInfo info, float delay)
	{
		AudioClip audioClip = null;
		if (info.VOClip.Length > 0)
		{
			//audioClip = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("VO/Tutorial/" + info.VOClip) as AudioClip;
			audioClip = Resources.Load("VO/Tutorial/" + info.VOClip, typeof(AudioClip)) as AudioClip;
            TutorialMonitor.Instance.PlayAudioForTutorial(audioClip, delay);
			if (audioClip != null)
			{
				VOClip = audioClip;
			}
			if (VOClip != null && info.UseVOScale)
			{
				VOLength = VOClip.length;
			}
			else
			{
				VOLength = 1f;
			}
		}
	}

	private GameObject getPointerTarget(TutorialInfo.PointerInfo pinfo)
	{
		GameObject gameObject = null;
		if (pinfo != null)
		{
			if (pinfo.PointerTarget == "~!@#DeckCard")
			{
				CWDeckAddCards cWDeckAddCards = UnityEngine.Object.FindObjectOfType(typeof(CWDeckAddCards)) as CWDeckAddCards;
				if (cWDeckAddCards != null)
				{
					gameObject = cWDeckAddCards.GetAnyCard();
				}
			}
			else
			{
				gameObject = FindGameObject(pinfo.PointerTarget);
			}
			findTargetFailed = gameObject == null;
		}
		return gameObject;
	}

	private GameObject getPointerTargetMax(TutorialInfo info)
	{
		GameObject result = null;
		if (info != null)
		{
			result = FindGameObject(info.PointerTargetMax);
		}
		return result;
	}

	private void SetLayer(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform item in obj.transform)
		{
			SetLayer(item.gameObject, layer);
		}
	}

	private GameObject CreatePopup(TutorialInfo info)
	{
        //UnityEngine.Object original = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("MenuItems/Tutorial_Popup");
        GameObject original = Instantiate(Resources.Load("menuitems/Tutorial_Popup", typeof(GameObject))) as GameObject;
		GameObject gameObject = original;
		gameObject.name = gameObject.name + "_" + info.TutorialID;
		gameObject.SendMessage("Reset");
		gameObject.transform.localPosition = info.Pos;
		gameObject.transform.localEulerAngles = info.Rot;
		gameObject.transform.localScale = info.Scale;
		TweenScale tweenScale = gameObject.GetComponent(typeof(TweenScale)) as TweenScale;
		if (tweenScale != null)
		{
			tweenScale.to = info.Scale;
		}
		SetLayer(gameObject, LayerMask.NameToLayer(info.Layer));
		Transform transform = FindSpriteGroupInPopup(gameObject);
		if (transform != null)
		{
			UISprite[] componentsInChildren = transform.gameObject.GetComponentsInChildren<UISprite>();
			UISprite[] array = componentsInChildren;
			foreach (UISprite uISprite in array)
			{
				if (uISprite.name != "Sprite_Frame")
				{
					if (info.Sprite == string.Empty)
					{
						uISprite.spriteName = "Blank";
					}
					else
					{
						uISprite.spriteName = info.Sprite;
					}
					uISprite.transform.localScale = MakeVector3(info.SpriteSize, transform.localScale.z);
					continue;
				}
				float num = info.SpriteSize.x;
				float num2 = info.SpriteSize.y;
				UIAtlas.Sprite sprite = uISprite.atlas.GetSprite(info.Sprite);
				UIAtlas.Sprite atlasSprite = uISprite.GetAtlasSprite();
				if (sprite != null && atlasSprite != null)
				{
					num = atlasSprite.outer.width * (num / sprite.outer.width);
					num2 = atlasSprite.outer.height * (num2 / sprite.outer.height);
				}
				uISprite.transform.localScale = new Vector3(num, num2, transform.localScale.z);
			}
			transform.localPosition = MakeVector3(info.SpritePos, -100f);
			transform.rotation = Quaternion.Euler(info.SpriteRot);
		}
		Transform transform2 = gameObject.transform.Find("Black");
		if (transform2 != null)
		{
			if (!info.DimBackground)
			{
				UnityEngine.Object.Destroy(transform2.gameObject);
			}
			else
			{
				ScreenDimmer screenDimmer = transform2.gameObject.GetComponent(typeof(ScreenDimmer)) as ScreenDimmer;
				if (screenDimmer != null)
				{
					screenDimmer.startupType = ((!info.DimFadeIn) ? ScreenDimmer.StartupType.FadedIn : ScreenDimmer.StartupType.FadeIn);
					if (info.DimFadeOut)
					{
						GameObject gameObject2 = null;
						CWTutorialTapDelegate cWTutorialTapDelegate = gameObject.GetComponent(typeof(CWTutorialTapDelegate)) as CWTutorialTapDelegate;
						if (cWTutorialTapDelegate != null)
						{
							gameObject2 = cWTutorialTapDelegate.buttonObj;
						}
						if (gameObject2 != null)
						{
							UIButtonMessage uIButtonMessage = gameObject2.AddComponent(typeof(UIButtonMessage)) as UIButtonMessage;
							if (uIButtonMessage != null)
							{
								uIButtonMessage.target = screenDimmer.gameObject;
								uIButtonMessage.functionName = "FadeOut";
							}
						}
					}
					if (info.DimDestroyOnClose)
					{
						GameObject gameObject3 = null;
						CWTutorialTapDelegate cWTutorialTapDelegate2 = gameObject.GetComponent(typeof(CWTutorialTapDelegate)) as CWTutorialTapDelegate;
						if (cWTutorialTapDelegate2 != null)
						{
							gameObject3 = cWTutorialTapDelegate2.buttonObj;
						}
						if (gameObject3 != null)
						{
							UIButtonMessage uIButtonMessage2 = gameObject3.AddComponent(typeof(UIButtonMessage)) as UIButtonMessage;
							if (uIButtonMessage2 != null)
							{
								uIButtonMessage2.target = screenDimmer.gameObject;
								uIButtonMessage2.functionName = "DestroyOnClose";
							}
						}
					}
				}
			}
		}
		titleT = gameObject.transform.Find("Title");
		if (titleT != null)
		{
			UILabel componentInChildren = titleT.gameObject.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = info.Title;
			}
		}
		transform = gameObject.transform.Find("Text");
		float num3 = 150f;
		if (transform != null)
		{
			if (string.IsNullOrEmpty(info.Text))
			{
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			else
			{
				UILabel componentInChildren2 = transform.gameObject.GetComponentInChildren<UILabel>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.text = info.Text.Replace("<br>", "\n");
					TypewriterEffectIgnoreTime component = transform.GetComponent<TypewriterEffectIgnoreTime>();
					if (component != null)
					{
						component.duration = VOLength;
						component.CWtutorialText = componentInChildren2.text;
						int num4 = componentInChildren2.text.Split('.').Length - 1;
						num4 += componentInChildren2.text.Split(',').Length - 1;
						num4 += componentInChildren2.text.Split('\n').Length - 1;
						num4 += componentInChildren2.text.Split('?').Length - 1;
						num4 += componentInChildren2.text.Split('!').Length - 1;
						int num5 = componentInChildren2.text.Length + 5 * num4;
						if (info.UseVOScale && VOLength != 0f)
						{
							component.charsPerSecond = (int)((float)num5 / VOLength * TYPEWRITER_SPEED_SCALE);
						}
						else
						{
							component.charsPerSecond = 40;
						}
					}
					transform.localPosition = new Vector3(transform.localPosition.x, info.SpritePos.y - transform.localScale.y, transform.localPosition.z);
					Transform transform3 = gameObject.transform.Find("Background/Frame");
					num3 = componentInChildren2.relativeSize.y * transform.localScale.y + 100f;
					transform3.localScale = new Vector3(transform3.localScale.x, num3, 1f);
				}
			}
		}
		transform = gameObject.transform.Find("Background");
		Transform transform4 = transform;
		if (string.IsNullOrEmpty(info.Text))
		{
			if (transform4 != null)
			{
				UnityEngine.Object.Destroy(transform4.gameObject);
			}
		}
		else
		{
			transform.position = new Vector3(transform.position.x, info.SpritePos.y, info.SpritePos.z);
			transform4.localPosition = new Vector3(transform4.localPosition.x, info.SpritePos.y, info.SpritePos.z);
		}
		CWTutorialTapDelegate component2 = gameObject.GetComponent<CWTutorialTapDelegate>();
		if ((bool)component2)
		{
			component2.info = info;
		}
		Transform transform5 = setButton(gameObject, info, info.Button1, info.Pointer1, "Button", num3);
		button1 = ((!(transform5 != null)) ? null : transform5.gameObject);
		transform5 = setButton(gameObject, info, info.Button2, info.Pointer2, "Button2", num3);
		TutorialMonitor.Instance.PopupActive = true;
		bool clickOnPointerTarget = info.Pointer1.ClickOnPointerTarget;
		bool clickOnPointerTarget2 = info.Pointer2.ClickOnPointerTarget;
		if (info.UseInputEnabler && !clickOnPointerTarget && !clickOnPointerTarget2)
		{
			SLOTGameSingleton<SLOTUI>.GetInstance().AddInputEnabler(gameObject, false, true, FF_DELAY);
		}
		return gameObject;
	}

	public void setCollider(GameObject t, GameObject tt)
	{
		setCollider(t, tt, false, false, false);
	}

	private void setCollider(GameObject t, GameObject tt, bool addinputenabler, bool useOnPress, bool useOnClick)
	{
		target = t;
		if (addinputenabler)
		{
			SLOTGameSingleton<SLOTUI>.GetInstance().AddInputEnabler(t, useOnPress, useOnClick, FF_DELAY);
		}
	}

	private GameObject CreatePointer(TutorialInfo.PointerInfo pinfo)
	{
		//UnityEngine.Object original = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("menuitems/Tutorial_Arrow");
		GameObject original = Instantiate(Resources.Load("menuitems/Tutorial_Arrow", typeof(GameObject))) as GameObject; 
		GameObject gameObject = null;
		if (!pinfo.Hide || pinfo.ScaleTarget)
		{
			gameObject = original as GameObject;
			
			gameObject.name += pinfo.PointerTarget;
			Transform transform = FindSpriteGroup(gameObject);
			if (!pinfo.Hide)
			{
				Vector3 rot = getRot(pinfo.Corner, pinfo.PointerRotation);
				transform.rotation = Quaternion.Euler(rot);
				TweenScale component = transform.GetComponent<TweenScale>();
				if (pinfo.Animate == "True")
				{
					component.enabled = true;
				}
				else
				{
					component.enabled = false;
				}
				if (!string.IsNullOrEmpty(pinfo.Layer))
				{
					SetLayer(gameObject, LayerMask.NameToLayer(pinfo.Layer));
				}
			}
			UIPanel uIPanel = gameObject.GetComponent(typeof(UIPanel)) as UIPanel;
			if (uIPanel != null)
			{
				uIPanel.enabled = !pinfo.Hide;
			}
			CWTutorialArrow cWTutorialArrow = gameObject.GetComponent(typeof(CWTutorialArrow)) as CWTutorialArrow;
			if (cWTutorialArrow != null)
			{
				cWTutorialArrow.UpdatePosition = pinfo.UpdatePosition;
				cWTutorialArrow.SpriteGroup = transform;
				cWTutorialArrow.Target = getPointerTarget(pinfo);
				cWTutorialArrow.Corner = pinfo.Corner;
				cWTutorialArrow.OffsetX = pinfo.OffsetX;
				cWTutorialArrow.OffsetY = pinfo.OffsetY;
				cWTutorialArrow.OffsetZ = pinfo.OffsetZ;
				cWTutorialArrow.Layer = gameObject.layer;
				cWTutorialArrow.ScaleTarget = pinfo.ScaleTarget;
			}
		}
		return gameObject;
	}

	private void SetupInputEnabler(TutorialInfo.PointerInfo pinfo, GameObject obj, bool removeOnPress, bool removeOnClick, float skipDelay)
	{
		if (pinfo.ClickOnPointerTarget)
		{
			SLOTGameSingleton<SLOTUI>.GetInstance().AddInputEnabler(obj, removeOnPress, removeOnClick, skipDelay);
		}
	}

	private void setPointerPos(TutorialInfo.PointerInfo pinfo, GameObject pointer, Transform spritegroup)
	{
		string corner = pinfo.Corner;
		target = getPointerTarget(pinfo);
		Vector3 position = Vector3.zero;
		if (target != null)
		{
			position = CWTutorialArrow.getPos(target, corner, pinfo.OffsetX, pinfo.OffsetY, pinfo.OffsetZ, (!(pointer != null)) ? (-1) : pointer.layer);
		}
		if (!(spritegroup != null))
		{
			return;
		}
		spritegroup.position = position;
		Vector3 rot = getRot(pinfo.Corner, pinfo.PointerRotation);
		if (pointer != null)
		{
			Camera camera = NGUITools.FindCameraForLayer(pointer.layer);
			if (camera != null)
			{
				rot.x += camera.transform.eulerAngles.x;
				rot.y += camera.transform.eulerAngles.y;
				rot.z += camera.transform.eulerAngles.z;
			}
		}
		spritegroup.rotation = Quaternion.Euler(rot);
	}

	private void AddTutorialsPopup(GameObject obj, string action)
	{
		CWTutorialsPopup cWTutorialsPopup = obj.AddComponent<CWTutorialsPopup>();
		cWTutorialsPopup.onlyOnce = true;
		cWTutorialsPopup.tutorial_ID = action;
	}

	private Vector3 getRot(string position, string rotationString)
	{
		Vector3 result = Vector3.zero;
		float num = 0f;
		bool flag = false;
		if (rotationString != null && !string.IsNullOrEmpty(rotationString))
		{
			try
			{
				num = float.Parse(rotationString);
				flag = true;
			}
			catch (Exception)
			{
			}
		}
		switch (position)
		{
		case "RightTop":
			result = new Vector3(0f, 0f, (!flag) ? 30f : num);
			break;
		case "LeftTop":
			result = new Vector3(0f, 0f, (!flag) ? 150f : num);
			break;
		case "RightBottom":
			result = new Vector3(0f, 0f, (!flag) ? (-30f) : num);
			break;
		case "LeftBottom":
			result = new Vector3(0f, 0f, (!flag) ? (-150f) : num);
			break;
		case "Top":
			result = new Vector3(0f, 0f, (!flag) ? 90f : num);
			break;
		case "Bottom":
			result = new Vector3(0f, 0f, (!flag) ? 270f : num);
			break;
		case "Left":
			result = new Vector3(0f, 0f, (!flag) ? 180f : num);
			break;
		case "Right":
			result = new Vector3(0f, 0f, (!flag) ? 0f : num);
			break;
		default:
			result = new Vector3(0f, 0f, (!flag) ? 0f : num);
			break;
		}
		return result;
	}

	private Transform setButton(GameObject popup, TutorialInfo info, TutorialInfo.ButtonInfo binfo, TutorialInfo.PointerInfo pinfo, string buttonName, float frameHeight)
	{
		Transform transform = popup.transform.Find(buttonName);
		if (transform != null)
		{
			string buttonAction = binfo.ButtonAction;
			if (!string.IsNullOrEmpty(buttonAction))
			{
				if (this.tutorialInfo.IsFinal)
				{
					transform.gameObject.AddComponent<CWTutorialEnd>();
				}
				if (binfo.ButtonText == string.Empty)
				{
					transform.GetComponent<Collider>().enabled = false;
				}
				TutorialInfo tutorialInfo = TutorialManager.Instance.Find(buttonAction);
				if (tutorialInfo != null)
				{
					CWTutorialClosePanelScript cWTutorialClosePanelScript = transform.gameObject.AddComponent<CWTutorialClosePanelScript>();
					cWTutorialClosePanelScript.ObjectToClose = popup;
					cWTutorialClosePanelScript.tutorialID = tutorial_ID;
					if (pointer != null)
					{
						cWTutorialClosePanelScript.pointer = pointer;
					}
					if (pointer2 != null)
					{
						cWTutorialClosePanelScript.pointer2 = pointer2;
					}
					CWTutorialsPopup cWTutorialsPopup = transform.gameObject.AddComponent<CWTutorialsPopup>();
					cWTutorialsPopup.onlyOnce = true;
					cWTutorialsPopup.tutorial_ID = buttonAction;
					cWTutorialsPopup.Pointer = pointer;
					cWTutorialsPopup.Pointer2 = pointer2;
				}
				else
				{
					try
					{
						Type type = Type.GetType(buttonAction);
						Component component = transform.gameObject.AddComponent(type);
						if (buttonAction == "CWTutorialClosePanelScript")
						{
							((CWTutorialClosePanelScript)component).ObjectToClose = popup;
							((CWTutorialClosePanelScript)component).tutorialID = tutorial_ID;
						}
						if (buttonAction == "CWTutorialSkip")
						{
							CWTutorialSkip cWTutorialSkip = transform.gameObject.GetComponent(typeof(CWTutorialSkip)) as CWTutorialSkip;
							if (cWTutorialSkip != null)
							{
								cWTutorialSkip.useInputEnablerWhenDone = info.UseInputEnablerWhenDone;
							}
						}
					}
					catch
					{
					}
				}
				transform.localPosition = MakeVector3(binfo.ButtonPos, transform.localPosition.z);
				if (titleT != null)
				{
					titleT.localPosition = new Vector3(titleT.localPosition.x, frameHeight, titleT.localPosition.z);
				}
				Transform transform2 = transform.Find("Background");
				if (transform2 != null)
				{
					transform2.localScale = MakeVector3(binfo.ButtonSize, transform.localScale.z);
				}
				UISprite componentInChildren = transform.gameObject.GetComponentInChildren<UISprite>();
				if (componentInChildren != null)
				{
					try
					{
						if (binfo.ButtonSprite != string.Empty)
						{
							componentInChildren.spriteName = binfo.ButtonSprite;
						}
					}
					catch (KeyNotFoundException)
					{
					}
				}
				UILabel componentInChildren2 = transform.gameObject.GetComponentInChildren<UILabel>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.text = binfo.ButtonText;
				}
				if (buttonAction == "AreYouSureSkipTutorial")
				{
					SLOTGameSingleton<SLOTUI>.GetInstance().AddInputEnabler(transform.gameObject, false, false, FF_DELAY);
				}
				else if (transform.gameObject.GetComponent<Collider>() != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject.GetComponent<Collider>());
				}
			}
			else
			{
				NGUITools.SetActive(transform.gameObject, false);
			}
		}
		return transform;
	}

	private Transform FindSpriteGroup(GameObject obj)
	{
		if (obj != null)
		{
			return obj.transform.Find("Sprite_Group");
		}
		return null;
	}

	private Transform FindSpriteGroupInPopup(GameObject obj)
	{
		if (obj != null)
		{
			return obj.transform.Find("Sprite_Popup");
		}
		return null;
	}

	private Vector3 MakeVector3(Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	private GameObject FindGameObject(string name)
	{
		return GameObject.Find(name);
	}
}
