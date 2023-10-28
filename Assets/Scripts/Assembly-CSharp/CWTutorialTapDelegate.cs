using UnityEngine;

public class CWTutorialTapDelegate : MonoBehaviour
{
	public GameObject buttonObj;

	public UILabel tutorialText;

	private TypewriterEffectIgnoreTime typeFX;

	public TutorialInfo info;

	public bool skipFlag;

	private float startTime;

	private bool fastForward;

	private void Awake()
	{
		typeFX = tutorialText.GetComponent<TypewriterEffectIgnoreTime>();
		startTime = Time.realtimeSinceStartup;
		fastForward = false;
	}

	private void Update()
	{
		if ((!skipFlag && (info == null || !info.Skippable)) || !Input.GetMouseButtonDown(0))
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
				if (tutorialText != null && info != null)
				{
					tutorialText.text = info.Text;
				}
				if (typeFX != null)
				{
					Object.Destroy(typeFX);
				}
				TutorialMonitor.Instance.StopTutorialAudio();
				buttonObj.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				Object.Destroy(this);
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
