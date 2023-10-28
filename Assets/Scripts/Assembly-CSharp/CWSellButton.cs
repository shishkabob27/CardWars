using System.Linq;
using UnityEngine;

public class CWSellButton : MonoBehaviour
{
	private enum ErrorCode
	{
		None,
		NoCardsSelected,
		OneCard,
		MultipleCards,
		TooManyCreatures
	}

	public UILabel errorText;

	public UIButtonTween showError;

	public UIButtonTween showConfirm;

	public GameObject tableObject;

	public UILabel CoinLabel;

	public UILabel ConfirmLabel;

	public CWDeckCardList SelectList;

	public UITweener canSellTween;

	private int lastCoins = -1;

	private int lastCardCount = -1;

	private int lastCanSell = -1;

	private void OnEnable()
	{
		lastCoins = -1;
		lastCardCount = -1;
		lastCanSell = -1;
	}

	public void OnClick()
	{
		ErrorCode errorCode;
		if (!CanSell(out errorCode))
		{
			switch (errorCode)
			{
			case ErrorCode.NoCardsSelected:
				errorText.text = KFFLocalization.Get("!!ERROR_NO_CARDS_SELECTED_TO_SELL");
				showError.Play(true);
				return;
			case ErrorCode.OneCard:
				errorText.text = string.Format(KFFLocalization.Get("!!ERROR_SELLING_ONE_CARD"), ParametersManager.Instance.Min_Cards_In_Inventory);
				showError.Play(true);
				return;
			case ErrorCode.MultipleCards:
				errorText.text = string.Format(KFFLocalization.Get("!!ERROR_SELLING_MULTIPLE_CARDS"), SelectList.chosenList.Count, ParametersManager.Instance.Min_Cards_In_Inventory);
				showError.Play(true);
				return;
			case ErrorCode.TooManyCreatures:
				errorText.text = string.Format(KFFLocalization.Get("!!ERROR_SELLING_TOO_MANY_CREATURES"), ParametersManager.Instance.Min_Creatures_In_Inventory);
				showError.Play(true);
				return;
			}
		}
		showConfirm.Play(true);
	}

	private bool CanSell(out ErrorCode errorCode)
	{
		ParametersManager instance = ParametersManager.Instance;
		int count = SelectList.chosenList.Count;
		if (count == 0)
		{
			errorCode = ErrorCode.NoCardsSelected;
			return false;
		}
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		if (deckManager.CardCount() - count < instance.Min_Cards_In_Inventory)
		{
			if (count == 1)
			{
				errorCode = ErrorCode.OneCard;
			}
			else
			{
				errorCode = ErrorCode.MultipleCards;
			}
			return false;
		}
		int num = deckManager.Inventory.Count((CardItem c) => c.Form.Type == CardType.Creature);
		int num2 = SelectList.chosenList.Count((CardItem c) => c.Form.Type == CardType.Creature);
		if (num - num2 < instance.Min_Creatures_In_Inventory)
		{
			errorCode = ErrorCode.TooManyCreatures;
			return false;
		}
		errorCode = ErrorCode.None;
		return true;
	}

	private void Update()
	{
		if (!(tableObject != null))
		{
			return;
		}
		int num = 0;
		foreach (CardItem chosen in SelectList.chosenList)
		{
			num += chosen.SalePrice;
		}
		if (CoinLabel != null)
		{
			CoinLabel.text = string.Format(KFFLocalization.Get("!!FORMAT_COINS"), num);
		}
		if (ConfirmLabel != null)
		{
			ConfirmLabel.text = string.Format(KFFLocalization.Get("!!FORMAT_SELL_CARDS_FOR_X_COINS"), num);
		}
		int count = SelectList.chosenList.Count;
		if (num != lastCoins || count != lastCardCount)
		{
			lastCoins = num;
			lastCardCount = count;
			ErrorCode errorCode;
			bool flag = CanSell(out errorCode);
			if ((flag ? 1 : 0) != lastCanSell && canSellTween != null)
			{
				lastCanSell = (flag ? 1 : 0);
				UITweener.Style style = canSellTween.style;
				canSellTween.style = UITweener.Style.Once;
				canSellTween.Play(true);
				canSellTween.Reset();
				canSellTween.style = style;
				canSellTween.enabled = flag;
			}
		}
	}
}
