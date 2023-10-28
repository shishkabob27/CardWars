using UnityEngine;

public class CWDeckControlPanel : MonoBehaviour
{
	private CWDeckController deckCtlr;

	private void Awake()
	{
		deckCtlr = CWDeckController.GetInstance();
	}

	public void OpenPanel()
	{
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "table")
			{
				uIButtonTween.Play(true);
			}
		}
	}

	public void ClosePanel()
	{
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "table")
			{
				uIButtonTween.Play(false);
			}
		}
	}

	public void RaisePanel()
	{
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "panelMove")
			{
				uIButtonTween.Play(true);
			}
		}
	}

	public void LowerPanel()
	{
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "panelMove")
			{
				uIButtonTween.Play(false);
			}
		}
	}

	public void PlacePanelLow()
	{
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "panelMove")
			{
				uIButtonTween.tweenTarget.transform.localPosition = uIButtonTween.tweenTarget.GetComponent<TweenPosition>().from;
			}
		}
	}

	public void PlacePanelHigh()
	{
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "panelMove")
			{
				uIButtonTween.tweenTarget.transform.localPosition = uIButtonTween.tweenTarget.GetComponent<TweenPosition>().to;
			}
		}
	}

	public void RaiseBar()
	{
		if (deckCtlr == null)
		{
			deckCtlr = CWDeckController.GetInstance();
		}
		if (base.gameObject.name == "BuildingControl")
		{
			deckCtlr.buildingBarPos = BuildingPos.Up;
		}
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "bar")
			{
				uIButtonTween.Play(true);
			}
		}
	}

	public void LowerBar()
	{
		if (deckCtlr == null)
		{
			deckCtlr = CWDeckController.GetInstance();
		}
		if (base.gameObject.name == "BuildingControl")
		{
			deckCtlr.buildingBarPos = BuildingPos.Down;
		}
		UIButtonTween[] components = base.gameObject.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			if (uIButtonTween.label == "bar")
			{
				uIButtonTween.Play(false);
			}
		}
	}
}
