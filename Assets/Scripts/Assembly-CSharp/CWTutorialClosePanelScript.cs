using System.Collections;
using UnityEngine;

[AddComponentMenu("SlotQuest/SLOTTutorialClosePanelScript")]
public class CWTutorialClosePanelScript : MonoBehaviour
{
	public string tutorialID;

	public GameObject ObjectToClose;

	public GameObject pointer;

	public GameObject pointer2;

	public bool destroyPopupScript = true;

	private CWTutorialsPopup popupScript;

	private PanelManager pManager;

	private float timer;

	private float timeTarget;

	public bool useOnClick = true;

	private void OnEnable()
	{
		destroyPopupScript = true;
	}

	public void Trigger()
	{
		if (base.enabled)
		{
			if (pointer != null)
			{
				Object.Destroy(pointer, 0f);
			}
			if (pointer2 != null)
			{
				Object.Destroy(pointer2, 0f);
			}
			if (popupScript != null && destroyPopupScript)
			{
				Object.Destroy(popupScript);
			}
			StartCoroutine(DestroyAfterTime(0.3f));
		}
	}

	private IEnumerator DestroyAfterTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SLOTUI.RemoveInputEnabler(base.gameObject);
		Object.DestroyImmediate(ObjectToClose);
		Object.Destroy(this);
	}

	public void OnClick()
	{
		Trigger();
	}

	private void onPress(bool isDown)
	{
		if (isDown && !useOnClick)
		{
			Trigger();
		}
	}

	private void Update()
	{
	}
}
