using UnityEngine;

public class CWTBTapToBringForward : MonoBehaviour
{
	public Camera RaycastCamera;

	public GestureRecognizer gestureRecognizer;

	public bool selected;

	public bool zoomed;

	public CardItem card;

	public string cardName;

	private GameObject zoomCard;

	private bool selectedOnBeginTap;

	private void Start()
	{
		if (!RaycastCamera)
		{
			RaycastCamera = Camera.main;
		}
		zoomCard = PanelManagerBattle.GetInstance().zoomCard;
		selectedOnBeginTap = false;
	}

	public void OnTap(TapGesture gesture)
	{
		if (selectedOnBeginTap)
		{
			PlayZoomTween(true);
		}
	}

	private void OnBeginTap(TapRecognizer tapRecognizer)
	{
		selectedOnBeginTap = selected;
		if (!selected)
		{
			Select();
		}
	}

	public void BringForward()
	{
		if (!selected)
		{
			Select();
		}
		else if (selected)
		{
			PlayZoomTween(true);
		}
		else if (!zoomed)
		{
		}
	}

	public void Select()
	{
		ApplySorting();
		ApplyScaling();
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), instance.cardPickSound);
	}

	public void ApplyScaling()
	{
		int num = int.Parse(base.name.Substring(base.name.Length - 1, 1)) - 1;
		CWTBTapToBringForward[] componentsInChildren = base.transform.parent.GetComponentsInChildren<CWTBTapToBringForward>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			CWTBTapToBringForward cWTBTapToBringForward = componentsInChildren[i];
			cWTBTapToBringForward.PlaySelectTween(i == num);
		}
	}

	public void ApplySorting()
	{
		int num = int.Parse(base.name.Substring(base.name.Length - 1, 1)) - 1;
		CWTBTapToBringForward[] componentsInChildren = base.transform.parent.GetComponentsInChildren<CWTBTapToBringForward>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			CWTBTapToBringForward cWTBTapToBringForward = componentsInChildren[i];
			cWTBTapToBringForward.transform.localPosition = new Vector3(cWTBTapToBringForward.transform.localPosition.x, cWTBTapToBringForward.transform.localPosition.y, (float)Mathf.Abs(i - num) * 10f);
		}
	}

	private void OnLongPress(LongPressGesture gesture)
	{
	}

	public void PlaySelectTween(bool zoom)
	{
		if (selected == zoom)
		{
			return;
		}
		TweenScale[] components = GetComponents<TweenScale>();
		int num = ((!zoom) ? 1 : 0);
		TweenScale[] array = components;
		foreach (TweenScale tweenScale in array)
		{
			if (tweenScale.tweenGroup == num)
			{
				tweenScale.enabled = true;
				tweenScale.Reset();
				tweenScale.Play(true);
			}
			else
			{
				tweenScale.enabled = false;
			}
		}
		selected = zoom;
	}

	public CWHandCardZoom PlayZoomTween(bool zoom)
	{
		zoomed = zoom;
		GameObject tweenZoom = CWPlayerHandsController.GetInstance().tweenZoom;
		CWHandCardZoom component = zoomCard.GetComponent<CWHandCardZoom>();
		component.card = card;
		component.tapToBringForward = this;
		TweenTransform component2 = zoomCard.GetComponent<TweenTransform>();
		component2.from = base.transform;
		UIButtonTween[] components = tweenZoom.GetComponents<UIButtonTween>();
		UIButtonTween[] array = components;
		foreach (UIButtonTween uIButtonTween in array)
		{
			uIButtonTween.Play(zoom);
		}
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), (!zoom) ? instance.cardUnzoomSound : instance.cardZoomSound);
		return component;
	}
}
