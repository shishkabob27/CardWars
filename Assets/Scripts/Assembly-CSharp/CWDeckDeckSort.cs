using UnityEngine;

public class CWDeckDeckSort : MonoBehaviour
{
	public enum ButtonEnum
	{
		DEFAULT,
		ATK,
		DEF,
		TYPE,
		NAME,
		FACT,
		RARE,
		MP
	}

	public ButtonEnum currentSort;

	public CardType currentFilter = CardType.None;

	public CWDeckDeckCards GridScript;

	public void OnClick()
	{
		switch (currentSort)
		{
		case ButtonEnum.DEFAULT:
			PlayerDeckManager.SetSort(SortType.TYPE, SortType.FACT);
			break;
		case ButtonEnum.NAME:
			PlayerDeckManager.SetSort(SortType.NAME, SortType.NONE);
			break;
		case ButtonEnum.ATK:
			PlayerDeckManager.SetSort(SortType.ATK, SortType.NAME);
			break;
		case ButtonEnum.DEF:
			PlayerDeckManager.SetSort(SortType.DEF, SortType.NAME);
			break;
		case ButtonEnum.TYPE:
			PlayerDeckManager.SetSort(SortType.TYPE, SortType.NAME);
			break;
		case ButtonEnum.FACT:
			PlayerDeckManager.SetSort(SortType.FACT, SortType.NAME);
			break;
		case ButtonEnum.RARE:
			PlayerDeckManager.SetSort(SortType.RARE, SortType.NAME);
			break;
		case ButtonEnum.MP:
			PlayerDeckManager.SetSort(SortType.MP, SortType.NAME);
			break;
		}
		if (GridScript != null)
		{
			GridScript.Sort(currentFilter);
		}
	}
}
