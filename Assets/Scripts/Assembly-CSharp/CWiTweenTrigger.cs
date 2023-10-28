using UnityEngine;

public class CWiTweenTrigger : MonoBehaviour
{
	public GameObject[] tweenTargets;

	public string tweenName;

	public string tweenNameOnPressTrue;

	public string tweenNameOnPressFalse;

	private void Start()
	{
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			TriggerTweens(tweenNameOnPressTrue);
		}
		else
		{
			TriggerTweens(tweenNameOnPressFalse);
		}
	}

	public void OnClick()
	{
		TriggerTweens(tweenName);
	}

	public void TriggerTweens(string tweenName)
	{
		if (!(tweenName != string.Empty))
		{
			return;
		}
		GameObject[] array = tweenTargets;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.activeInHierarchy)
			{
				iTweenEvent @event = iTweenEvent.GetEvent(gameObject, tweenName);
				if (@event != null)
				{
					@event.Play();
				}
			}
		}
	}

	private void Update()
	{
	}
}
