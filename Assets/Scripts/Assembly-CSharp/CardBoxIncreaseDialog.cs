using UnityEngine;

public class CardBoxIncreaseDialog : MonoBehaviour
{
	public GameObject PurchasePanel;
	public UILabel PurchasePromptLabel;
	public string PurchasePromptStringID;
	public UILabel CurrentBoxSizeLabel;
	public UILabel CoinPriceLabel;
	public UILabel GemPriceLabel;
	public UILabel PopupMessageLabel;
	public string InventoryUpdateStringID;
	public string NotEnoughGemsStringID;
	public string NotEnoughCoinsStringID;
	public UIButtonTween PoupShowButtonTween;
	public GameObject MaxReachedPanel;
	public UILabel MaxReachedLabel;
	public string MaxReachedStringID;
}
