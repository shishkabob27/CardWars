using UnityEngine;

public class DebugDisplayDButton : MonoBehaviour
{
	private const string GO_NAME_DEBUG_BUTTON = "DebugToggle";

	public Canvas debugCanvasPrefab;

	private DebugFlagsScript debugFlag;

	private Canvas canvas;

	private GameObject debugButton;

	private bool pressedFlag;

	private bool displayFlag;

	private void Start()
	{
		debugFlag = DebugFlagsScript.GetInstance();
	}

	private void Update()
	{
		if (debugCanvasPrefab == null || debugFlag.DebugInBuild == displayFlag)
		{
			return;
		}
		if (debugFlag.DebugInBuild)
		{
			if (canvas == null)
			{
				canvas = Object.Instantiate(debugCanvasPrefab);
				canvas.gameObject.SetActive(true);
				debugButton = null;
				Transform[] componentsInChildren = canvas.GetComponentsInChildren<Transform>(true);
				foreach (Transform transform in componentsInChildren)
				{
					if (transform.name.Equals("DebugToggle"))
					{
						debugButton = transform.gameObject;
						break;
					}
				}
				if (debugButton != null)
				{
					debugButton.SetActive(true);
				}
			}
			else if (debugButton != null)
			{
				debugButton.SetActive(true);
			}
			displayFlag = true;
		}
		else
		{
			if (debugButton != null)
			{
				debugButton.SetActive(false);
			}
			displayFlag = false;
		}
		if (debugButton != null)
		{
			Rect rect = (canvas.transform as RectTransform).rect;
			RectTransform rectTransform = debugButton.transform as RectTransform;
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMin = rectTransform.anchorMax;
			if (PanelManagerBattle.GetInstance() == null)
			{
				rectTransform.anchoredPosition = new Vector2(rect.width * 0.5f - 200f, -208f);
			}
			else
			{
				rectTransform.anchoredPosition = new Vector2(-572f, -348f);
			}
		}
	}
}
