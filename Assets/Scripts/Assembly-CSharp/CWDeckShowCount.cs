using UnityEngine;

public class CWDeckShowCount : MonoBehaviour
{
	public UILabel LabelCount;

	private void Update()
	{
		if (!(LabelCount == null))
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			int num = instance.DeckManager.CardCount();
			int maxInventory = instance.MaxInventory;
			LabelCount.text = string.Format("{0}/{1} Cards", num, maxInventory);
			LabelCount.color = ((num <= maxInventory) ? Color.white : Color.red);
		}
	}
}
