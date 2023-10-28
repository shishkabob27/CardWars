using UnityEngine;

public class CardBoxIncreaseDialog : MonoBehaviour
{
	public GameObject PurchasePanel;

	public UILabel PurchasePromptLabel;

	public string PurchasePromptStringID = "!!D_3_CARDBOX_PURCHASE_PROMPT";

	public UILabel CurrentBoxSizeLabel;

	public UILabel CoinPriceLabel;

	public UILabel GemPriceLabel;

	public UILabel PopupMessageLabel;

	public string InventoryUpdateStringID = "!!D_3_CARDBOX_INVENTORY_UPDATED";

	public string NotEnoughGemsStringID = "!!D_3_CARDBOX_NOT_ENOUGH_GEMS";

	public string NotEnoughCoinsStringID = "!!D_3_CARDBOX_NOT_ENOUGH_COINS";

	public UIButtonTween PoupShowButtonTween;

	public GameObject MaxReachedPanel;

	public UILabel MaxReachedLabel;

	public string MaxReachedStringID = "!!D_3_CARDBOX_MAX_REACHED";

	private CardBoxTier CurrentTier;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		UpdateDialogInfo();
	}

	private void UpdateDialogInfo()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		CurrentTier = CardBoxManager.Instance.GetTier(instance.MaxInventory);
		if (CurrentTier != null && instance.MaxInventory + CurrentTier.Gain <= CardBoxManager.Instance.MaxBoxCapacity)
		{
			if (PurchasePromptLabel != null)
			{
				PurchasePromptLabel.text = string.Format(KFFLocalization.Get(PurchasePromptStringID), CurrentTier.Gain.ToString());
			}
			if (CurrentBoxSizeLabel != null)
			{
				CurrentBoxSizeLabel.text = instance.MaxInventory.ToString();
			}
			if (CoinPriceLabel != null)
			{
				CoinPriceLabel.text = CurrentTier.CoinPrice.ToString();
			}
			if (GemPriceLabel != null)
			{
				GemPriceLabel.text = CurrentTier.GemPrice.ToString();
			}
			if (PurchasePanel != null)
			{
				PurchasePanel.SetActive(true);
			}
			if (MaxReachedPanel != null)
			{
				MaxReachedPanel.SetActive(false);
			}
		}
		else
		{
			if (MaxReachedLabel != null)
			{
				MaxReachedLabel.text = KFFLocalization.Get(MaxReachedStringID);
			}
			if (MaxReachedPanel != null)
			{
				MaxReachedPanel.SetActive(true);
			}
			if (PurchasePanel != null)
			{
				PurchasePanel.SetActive(false);
			}
		}
	}

	private void DisplayMessage(bool success, GameCurrency currency)
	{
		if (!(PopupMessageLabel != null))
		{
			return;
		}
		if (success)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			PopupMessageLabel.text = string.Format(KFFLocalization.Get(InventoryUpdateStringID), instance.MaxInventory);
			if ((bool)PoupShowButtonTween)
			{
				PoupShowButtonTween.SendMessage("OnClick");
			}
			return;
		}
		if (currency == GameCurrency.Gems)
		{
			PopupMessageLabel.text = KFFLocalization.Get(NotEnoughGemsStringID);
		}
		else
		{
			PopupMessageLabel.text = KFFLocalization.Get(NotEnoughCoinsStringID);
		}
		if ((bool)PoupShowButtonTween)
		{
			PoupShowButtonTween.SendMessage("OnClick");
		}
	}

	private void OnClickGems()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.Gems >= CurrentTier.GemPrice)
		{
			CWUpdatePlayerStats instance2 = CWUpdatePlayerStats.GetInstance();
			if (instance2 != null)
			{
				instance2.holdUpdateFlag = false;
			}
			instance.Gems -= CurrentTier.GemPrice;
			instance.MaxInventory += CurrentTier.Gain;
			instance.Save();
			Singleton<AnalyticsManager>.Instance.LogBoxSpacePurchase();
			DisplayMessage(true, GameCurrency.Gems);
		}
		else
		{
			DisplayMessage(false, GameCurrency.Gems);
		}
	}

	private void OnClickCoins()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.Coins >= CurrentTier.CoinPrice)
		{
			CWUpdatePlayerStats instance2 = CWUpdatePlayerStats.GetInstance();
			if (instance2 != null)
			{
				instance2.holdUpdateFlag = false;
			}
			instance.Coins -= CurrentTier.CoinPrice;
			instance.MaxInventory += CurrentTier.Gain;
			instance.Save();
			Singleton<AnalyticsManager>.Instance.LogBoxSpacePurchase();
			DisplayMessage(true, GameCurrency.Coins);
		}
		else
		{
			DisplayMessage(false, GameCurrency.Coins);
		}
	}

	private void OnTweenFinished()
	{
		UpdateDialogInfo();
	}
}
