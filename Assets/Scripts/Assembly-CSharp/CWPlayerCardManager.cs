using UnityEngine;

public class CWPlayerCardManager : MonoBehaviour
{
	public float verticalDragTolerance = 20f;

	private FingerGestures.Finger activeFinger;

	private CWTBTapToBringForward selectedCard;

	private CWTBTapToBringForward initialSelectedCard;

	private CWTBDragToMove dragToMove;

	private Vector2 dragBeginPos;

	private bool dragging;

	private Vector2 lastPos = Vector2.zero;

	public static bool inFirstTurnTutorial;

	private void Awake()
	{
		inFirstTurnTutorial = false;
	}

	private bool ShouldDisableInput(GameObject selection)
	{
		if (UICamera.useInputEnabler)
		{
			if (selection == null)
			{
				return false;
			}
			UIInputEnabler component = selection.GetComponent<UIInputEnabler>();
			if (component == null || !component.inputEnabled)
			{
				return true;
			}
		}
		return false;
	}

	private void OnFingerDown(FingerDownEvent e)
	{
		if (!ShouldDisableInput(e.Selection) && (!(selectedCard != null) || !selectedCard.zoomed) && !dragging && (activeFinger == null || e.Finger == activeFinger))
		{
			activeFinger = e.Finger;
			CWTBTapToBringForward cWTBTapToBringForward = selectedCard;
			SelectCard(e.Selection, e.Position);
			if (selectedCard != null && cWTBTapToBringForward == selectedCard)
			{
				initialSelectedCard = selectedCard;
			}
			else
			{
				initialSelectedCard = null;
			}
			lastPos = e.Position;
		}
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		if ((!(selectedCard != null) || !selectedCard.zoomed) && (!(selectedCard != null) || !ShouldDisableInput(selectedCard.gameObject)) && activeFinger == e.Finger)
		{
			if (!dragging && selectedCard != null && initialSelectedCard == selectedCard && e.Selection == selectedCard.gameObject)
			{
				selectedCard.PlayZoomTween(true);
			}
			if (dragging && dragToMove != null)
			{
				dragToMove.ReleaseCard(e.Position);
			}
			activeFinger = null;
			dragging = false;
		}
	}

	private void OnFingerHover(FingerHoverEvent e)
	{
		if (ShouldDisableInput(e.Selection) || (selectedCard != null && selectedCard.zoomed) || e.Finger != activeFinger || dragging)
		{
			return;
		}
		switch (e.Phase)
		{
		case FingerHoverPhase.Enter:
			SelectCard(e.Selection, e.Position);
			if (selectedCard != initialSelectedCard)
			{
				initialSelectedCard = null;
			}
			break;
		}
	}

	private void SelectCard(GameObject obj, Vector2 pos)
	{
		CWTBTapToBringForward cWTBTapToBringForward = (selectedCard = ((!(obj == null)) ? (obj.GetComponent(typeof(CWTBTapToBringForward)) as CWTBTapToBringForward) : null));
		if (cWTBTapToBringForward != null)
		{
			dragBeginPos = pos;
			dragging = false;
			if (!cWTBTapToBringForward.selected)
			{
				cWTBTapToBringForward.Select();
			}
			dragToMove = ((!(obj == null)) ? (obj.GetComponent(typeof(CWTBDragToMove)) as CWTBDragToMove) : null);
		}
		else
		{
			dragToMove = null;
		}
	}

	private void OnFingerMove(FingerMotionEvent e)
	{
		if ((selectedCard != null && selectedCard.zoomed) || !(selectedCard != null) || activeFinger == null || activeFinger != e.Finger)
		{
			return;
		}
		Vector2 vector = e.Position - lastPos;
		lastPos = e.Position;
		if (dragging)
		{
			UpdateDraggingCard(e.Position);
			return;
		}
		Vector2 vector2 = e.Position - dragBeginPos;
		if (Mathf.Abs(vector.y) > Mathf.Abs(vector.x) && vector2.y > verticalDragTolerance && e.Selection == selectedCard.gameObject)
		{
			dragging = true;
			if (dragToMove != null)
			{
				dragToMove.BeginDrag(e.Position);
			}
			UpdateDraggingCard(e.Position);
		}
	}

	private void UpdateDraggingCard(Vector2 pos)
	{
		if (selectedCard != null && !selectedCard.zoomed && dragToMove != null)
		{
			dragToMove.DragMode(pos, activeFinger);
		}
	}

	public void AddInputEnablersOnAllCards()
	{
		Object[] array = Object.FindObjectsOfType(typeof(CWTBTapToBringForward));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			CWTBTapToBringForward cWTBTapToBringForward = @object as CWTBTapToBringForward;
			if (cWTBTapToBringForward != null)
			{
				SLOTUI.AddInputEnabler(cWTBTapToBringForward.gameObject);
			}
		}
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		if (instance != null && instance.tweenZoom != null)
		{
			UIButtonTween uIButtonTween = instance.tweenZoom.GetComponent(typeof(UIButtonTween)) as UIButtonTween;
			if (uIButtonTween != null && uIButtonTween.tweenTarget != null)
			{
				SLOTUI.AddInputEnabler(uIButtonTween.tweenTarget);
			}
		}
	}

	public void RemoveInputEnablersFromAllCards()
	{
		Object[] array = Object.FindObjectsOfType(typeof(CWTBTapToBringForward));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			CWTBTapToBringForward cWTBTapToBringForward = @object as CWTBTapToBringForward;
			if (cWTBTapToBringForward != null)
			{
				SLOTUI.RemoveInputEnabler(cWTBTapToBringForward.gameObject);
			}
		}
		CWPlayerHandsController instance = CWPlayerHandsController.GetInstance();
		if (instance != null && instance.tweenZoom != null)
		{
			UIButtonTween uIButtonTween = instance.tweenZoom.GetComponent(typeof(UIButtonTween)) as UIButtonTween;
			if (uIButtonTween != null && uIButtonTween.tweenTarget != null)
			{
				SLOTUI.RemoveInputEnabler(uIButtonTween.tweenTarget);
			}
		}
	}

	public void StartFirstTurnTutorial()
	{
		AddInputEnablersOnAllCards();
		inFirstTurnTutorial = true;
	}

	public void EndFirstTurnTutorial()
	{
		RemoveInputEnablersFromAllCards();
		inFirstTurnTutorial = false;
	}

	public void CancelDrag()
	{
		if (dragging && dragToMove != null)
		{
			dragToMove.CancelDrag();
			dragging = false;
		}
	}
}
