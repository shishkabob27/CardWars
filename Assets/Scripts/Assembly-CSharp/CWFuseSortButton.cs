using System;
using UnityEngine;

public class CWFuseSortButton : MonoBehaviour
{
	public FusionSortType currentSort;

	public UILabel SortLabel;

	public CWFuseFuseCards FuseCardsScript;

	public void OnEnable()
	{
		currentSort = FusionSortType.NEW;
		SortLabel.text = currentSort.ToString();
	}

	public void OnClick()
	{
		currentSort = (FusionSortType)((int)(currentSort + 1) % Enum.GetValues(typeof(FusionSortType)).Length);
		FusionManager.SetSort(currentSort);
		SortLabel.text = currentSort.ToString();
		FuseCardsScript.Populate(null);
	}
}
