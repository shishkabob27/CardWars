using UnityEngine;

public class CWTutorialSkip : MonoBehaviour
{
	public bool leaveCollider;

	public bool useOnClick = true;

	public string tutorial_ID;

	public bool destroyWhenDone;

	public bool removeInputEnablerWhenDone;

	public Component[] componentsToEnable;

	public bool useInputEnablerWhenDone;

	private bool done;

	private void OnPress(bool isDown)
	{
		if (isDown && !useOnClick)
		{
			TriggerSkip();
		}
	}

	private void OnClick()
	{
		if (useOnClick)
		{
			TriggerSkip();
		}
	}

	private void TriggerSkip()
	{
		if (done)
		{
			return;
		}
		done = true;
		TutorialMonitor.Instance.StopTutorialAudio();
		CloseTutorialPopup(base.gameObject, removeInputEnablerWhenDone, this, useInputEnablerWhenDone);
		if (componentsToEnable == null)
		{
			return;
		}
		Component[] array = componentsToEnable;
		foreach (Component component in array)
		{
			if (component != null)
			{
				Behaviour behaviour = component as Behaviour;
				if (behaviour != null)
				{
					behaviour.enabled = true;
				}
				if (useOnClick)
				{
					component.SendMessage("OnClick", null, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					component.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static void CloseTutorialPopup(GameObject obj, bool removeInputEnabler, Component componentToDestroy, bool useinputenablerwhendone)
	{
		if (removeInputEnabler)
		{
			SLOTUI.RemoveInputEnabler(obj);
		}
		CWTutorialsPopup[] array = Object.FindObjectsOfType(typeof(CWTutorialsPopup)) as CWTutorialsPopup[];
		CWTutorialsPopup[] array2 = array;
		foreach (CWTutorialsPopup cWTutorialsPopup in array2)
		{
			if (cWTutorialsPopup.transform.parent != null && !cWTutorialsPopup.transform.parent.name.Contains("Debug_Tutorial"))
			{
				Object.Destroy(cWTutorialsPopup);
			}
		}
		CWTutorialClosePanelScript[] array3 = Object.FindObjectsOfType(typeof(CWTutorialClosePanelScript)) as CWTutorialClosePanelScript[];
		CWTutorialClosePanelScript[] array4 = array3;
		foreach (CWTutorialClosePanelScript obj2 in array4)
		{
			Object.Destroy(obj2);
		}
		Transform[] array5 = Object.FindObjectsOfType(typeof(Transform)) as Transform[];
		Transform[] array6 = array5;
		foreach (Transform transform in array6)
		{
			if (transform.name.IndexOf("Tutorial_Arrow(Clone)") != -1 || transform.name.IndexOf("Tutorial_Popup(Clone)") != -1)
			{
				TweenScale[] components = transform.GetComponents<TweenScale>();
				float t = 0f;
				TweenScale[] array7 = components;
				foreach (TweenScale tweenScale in array7)
				{
					tweenScale.Play(false);
					t = tweenScale.duration;
				}
				Object.Destroy(transform.gameObject, t);
			}
		}
		CWTutorialNext[] array8 = Object.FindObjectsOfType(typeof(CWTutorialNext)) as CWTutorialNext[];
		CWTutorialNext[] array9 = array8;
		foreach (CWTutorialNext obj3 in array9)
		{
			Object.Destroy(obj3);
		}
		TutorialMonitor.Instance.PopupActive = false;
		if (obj.name != "_Skip")
		{
			SLOTUI.RemoveInputEnablers();
		}
		UICamera.useInputEnabler = useinputenablerwhendone;
		Time.timeScale = 1f;
		if (componentToDestroy != null)
		{
			Object.Destroy(componentToDestroy);
		}
	}
}
