using UnityEngine;

public class CWClosePanelScript : MonoBehaviour
{
	public GameObject ObjectToClose;

	public GameObject col;

	private bool destoryFlag;

	private float timer;

	private float timeTarget;

	private void OnEnable()
	{
	}

	private void Deactivate()
	{
	}

	private void OnClick()
	{
		UITweener component = ObjectToClose.GetComponent<UITweener>();
		if (component != null)
		{
			timeTarget = component.duration;
		}
		Object.Destroy(col, timeTarget);
		Object.Destroy(ObjectToClose, timeTarget);
		destoryFlag = true;
	}

	private void Update()
	{
		if (destoryFlag)
		{
			timer += Time.deltaTime;
			if (!(timer >= timeTarget))
			{
			}
		}
	}
}
