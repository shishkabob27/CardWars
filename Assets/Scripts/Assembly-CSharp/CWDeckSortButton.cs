using System;
using UnityEngine;

public class CWDeckSortButton : MonoBehaviour
{
	public SortType currentSort;

	public GameObject GridObject;

	public bool StaticSort;

	public void OnEnable()
	{
		if (!StaticSort)
		{
			currentSort = SortType.NONE;
			SetSort();
		}
		UpdateLabel();
	}

	private void SetSort()
	{
		switch (currentSort)
		{
		case SortType.NONE:
			PlayerDeckManager.ResetSort();
			break;
		case SortType.NAME:
			PlayerDeckManager.SetSort(currentSort, SortType.NONE);
			break;
		case SortType.ATK:
			PlayerDeckManager.SetSort(currentSort, SortType.NAME);
			break;
		case SortType.DEF:
			PlayerDeckManager.SetSort(currentSort, SortType.NAME);
			break;
		case SortType.TYPE:
			PlayerDeckManager.SetSort(currentSort, SortType.NAME);
			break;
		case SortType.FACT:
			PlayerDeckManager.SetSort(currentSort, SortType.NAME);
			break;
		case SortType.RARE:
			PlayerDeckManager.SetSort(currentSort, SortType.NAME);
			break;
		case SortType.MP:
			PlayerDeckManager.SetSort(currentSort, SortType.NAME);
			break;
		}
	}

	private void UpdateLabel()
	{
	}

	public void OnClick()
	{
		if (!StaticSort)
		{
			currentSort = (SortType)((int)(currentSort + 1) % Enum.GetValues(typeof(SortType)).Length);
		}
		SetSort();
		UpdateLabel();
		if (GridObject != null)
		{
			GridObject.BroadcastMessage("Sort", null, SendMessageOptions.DontRequireReceiver);
		}
	}
}
