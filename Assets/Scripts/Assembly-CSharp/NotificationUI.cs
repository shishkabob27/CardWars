using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
	private enum NotificationType
	{
		DeckWars,
		NoUpdates
	}

	private const int MAX_NOTIFICATIONS = 100;

	public NotificationListItem[] notificationPrefabs;

	public UIGrid grid;

	public UIDraggablePanel dragpanel;

	private List<NotificationListItem> listItems = new List<NotificationListItem>();

	private float currDragAmountY = -1f;

	private float destDragAmountY = -1f;

	private NotificationListItem noUpdateListItem;

	private void Awake()
	{
		FindComponents();
	}

	private void FindComponents()
	{
		if (grid == null)
		{
			grid = SLOTGame.GetComponentInChildren<UIGrid>(base.gameObject, true);
		}
		if (dragpanel == null)
		{
			dragpanel = SLOTGame.GetComponentInChildren<UIDraggablePanel>(base.gameObject, true);
		}
	}

	private void OnEnable()
	{
		if (IsEmpty())
		{
			DisplayNoUpdateNotification(true);
		}
	}

	public void AddPlayerAttackedNotification(RecentNotification recent)
	{
		DisplayNoUpdateNotification(false, true);
		FindComponents();
		NotificationListItem notificationListItem = AddPrefab(NotificationType.DeckWars);
		if (notificationListItem != null)
		{
			notificationListItem.Setup(recent);
		}
		if (grid != null)
		{
			grid.Reposition();
		}
	}

	public void AddRankChangedNotification(string newRank, string prevRank)
	{
		DisplayNoUpdateNotification(false, true);
		FindComponents();
		NotificationListItem notificationListItem = AddPrefab(NotificationType.DeckWars);
		if (notificationListItem != null)
		{
			notificationListItem.Setup(newRank, prevRank);
		}
		if (grid != null)
		{
			grid.Reposition();
		}
	}

	private NotificationListItem GetPrefab(NotificationType notificationType)
	{
		if (notificationType >= NotificationType.DeckWars && (int)notificationType < notificationPrefabs.Length)
		{
			return notificationPrefabs[(int)notificationType];
		}
		return null;
	}

	private NotificationListItem AddPrefab(NotificationType notificationType)
	{
		NotificationListItem notificationListItem = null;
		if (grid != null)
		{
			NotificationListItem prefab = GetPrefab(notificationType);
			if (prefab != null)
			{
				GameObject gameObject = NGUITools.AddChild(grid.gameObject, prefab.gameObject);
				if (gameObject != null)
				{
					notificationListItem = gameObject.GetComponent<NotificationListItem>();
					if (notificationListItem != null)
					{
						notificationListItem.transform.parent = grid.transform;
						listItems.Add(notificationListItem);
						while (listItems.Count > 100)
						{
							NotificationListItem notificationListItem2 = listItems[0];
							if (notificationListItem2 != null)
							{
								NGUITools.Destroy(notificationListItem2.gameObject);
							}
							listItems.RemoveAt(0);
						}
						Transform parent = gameObject.transform.parent;
						while (parent != null)
						{
							if (parent.localScale == Vector3.zero)
							{
								parent.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
							}
							parent = parent.parent;
						}
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localPosition = Vector3.one;
						grid.Reposition();
					}
					else
					{
						NGUITools.Destroy(gameObject);
					}
				}
			}
		}
		return notificationListItem;
	}

	public void ClearNotifications()
	{
		if (dragpanel != null)
		{
			dragpanel.ResetPosition();
		}
		foreach (NotificationListItem listItem in listItems)
		{
			if (listItem != null)
			{
				NGUITools.Destroy(listItem.gameObject);
			}
		}
		listItems.Clear();
		if (dragpanel != null)
		{
			dragpanel.ResetPosition();
		}
	}

	public void ResetPosition()
	{
		if (grid != null)
		{
			grid.Reposition();
		}
		if (dragpanel != null)
		{
			dragpanel.ResetPosition();
		}
	}

	private void FixedUpdate()
	{
		if (!(dragpanel != null) || !(destDragAmountY >= 0f))
		{
			return;
		}
		if (dragpanel.Pressed)
		{
			destDragAmountY = -1f;
			return;
		}
		float num = destDragAmountY - currDragAmountY;
		if (Mathf.Abs(num) <= 0.001f)
		{
			currDragAmountY = destDragAmountY;
			destDragAmountY = -1f;
		}
		else
		{
			currDragAmountY += num * 0.1f;
		}
		dragpanel.SetDragAmount(0f, currDragAmountY, false);
		dragpanel.SetDragAmount(0f, currDragAmountY, true);
	}

	private bool IsAtBottom()
	{
		if (dragpanel != null && dragpanel.verticalScrollBar != null && dragpanel.panel != null)
		{
			if (destDragAmountY >= 0f)
			{
				return true;
			}
			float w = dragpanel.panel.clipRange.w;
			float y = dragpanel.bounds.size.y;
			if (y <= 0f || y <= w)
			{
				return true;
			}
			return dragpanel.verticalScrollBar.scrollValue >= (y - w) / y;
		}
		return false;
	}

	private bool CanScroll()
	{
		if (dragpanel != null && dragpanel.panel != null)
		{
			float w = dragpanel.panel.clipRange.w;
			float y = dragpanel.bounds.size.y;
			if (y <= 0f || y <= w)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void ScrollToBottom()
	{
		if (dragpanel != null && dragpanel.verticalScrollBar != null && CanScroll())
		{
			currDragAmountY = dragpanel.verticalScrollBar.scrollValue;
			destDragAmountY = 1f;
		}
	}

	public bool IsEmpty()
	{
		return listItems == null || listItems.Count <= 0;
	}

	private void DisplayNoUpdateNotification(bool show, bool suppressGridUpdate = false)
	{
		bool flag = false;
		if (show)
		{
			if (noUpdateListItem == null)
			{
				noUpdateListItem = AddPrefab(NotificationType.NoUpdates);
			}
			flag = noUpdateListItem != null;
		}
		else if (noUpdateListItem != null)
		{
			NGUITools.Destroy(noUpdateListItem);
			noUpdateListItem = null;
			flag = true;
		}
		if (!suppressGridUpdate && grid != null && flag)
		{
			grid.Reposition();
		}
	}
}
