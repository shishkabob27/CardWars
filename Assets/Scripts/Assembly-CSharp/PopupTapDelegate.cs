using System.Collections.Generic;
using UnityEngine;

public class PopupTapDelegate : MonoBehaviour
{
	public GameObject buttonObj;

	public UILabel BodyText;

	private TypewriterEffectIgnoreTime typeFX;

	private string info;

	private List<GameObject> CustomTargets;

	private List<UIButtonTween> CustomTargetTweens;

	private string CustomTargetMethod;

	private float startTime;

	private bool fastForward;

	private bool isInit;

	private void Awake()
	{
		typeFX = BodyText.GetComponent<TypewriterEffectIgnoreTime>();
		startTime = Time.realtimeSinceStartup;
		fastForward = false;
	}

	public void Init(string text, GameObject[] targets, UIButtonTween[] tweens, string targetMethod)
	{
		info = text;
		isInit = true;
		CustomTargetMethod = targetMethod;
		if (CustomTargets == null)
		{
			CustomTargets = new List<GameObject>();
		}
		if (CustomTargetTweens == null)
		{
			CustomTargetTweens = new List<UIButtonTween>();
		}
		if (targets != null)
		{
			foreach (GameObject gameObject in targets)
			{
				if (gameObject != null)
				{
					CustomTargets.Add(gameObject);
				}
			}
		}
		if (tweens == null)
		{
			return;
		}
		foreach (UIButtonTween uIButtonTween in tweens)
		{
			if (uIButtonTween != null)
			{
				CustomTargetTweens.Add(uIButtonTween);
			}
		}
	}

	private void Update()
	{
		if (!isInit || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		UIInputEnabler uIInputEnabler = base.gameObject.GetComponent(typeof(UIInputEnabler)) as UIInputEnabler;
		float num = Time.realtimeSinceStartup - startTime;
		if (!UICamera.useInputEnabler || (uIInputEnabler != null && uIInputEnabler.inputEnabled))
		{
			bool flag = true;
			if (typeFX != null)
			{
				flag = typeFX.IsDone();
			}
			if (num > CWTutorialsPopup.SKIP_DELAY && (fastForward || flag))
			{
				if (BodyText != null && info != null)
				{
					BodyText.text = info;
				}
				if (typeFX != null)
				{
					Object.Destroy(typeFX);
				}
				if (CustomTargets != null && CustomTargets.Count > 0 && CustomTargetMethod != null)
				{
					foreach (GameObject customTarget in CustomTargets)
					{
						customTarget.SendMessage(CustomTargetMethod);
					}
				}
				if (CustomTargetTweens != null && CustomTargetTweens.Count > 0)
				{
					foreach (UIButtonTween customTargetTween in CustomTargetTweens)
					{
						customTargetTween.Play(true);
					}
				}
				buttonObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				Object.Destroy(base.gameObject);
			}
		}
		if (num > CWTutorialsPopup.FF_DELAY && !fastForward)
		{
			fastForward = true;
			if (typeFX != null)
			{
				typeFX.FastForward();
			}
		}
	}
}
