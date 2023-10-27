using UnityEngine;

public class CWDeckDeckSort : MonoBehaviour
{
	public enum ButtonEnum
	{
		DEFAULT = 0,
		ATK = 1,
		DEF = 2,
		TYPE = 3,
		NAME = 4,
		FACT = 5,
		RARE = 6,
		MP = 7,
	}

	public ButtonEnum currentSort;
	public CardType currentFilter;
	public CWDeckDeckCards GridScript;
}
