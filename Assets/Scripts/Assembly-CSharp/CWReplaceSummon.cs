using UnityEngine;

public class CWReplaceSummon : MonoBehaviour
{
	public UILabel nameFrom;

	public UILabel nameTo;

	public int lane;

	public CardItem newCard;

	public string newCardName;

	private CWPlayerHandsController handCtlr;

	private void OnEnable()
	{
		handCtlr = CWPlayerHandsController.GetInstance();
		lane = handCtlr.lane;
		newCard = handCtlr.card;
		newCardName = handCtlr.cardName;
		UpdateText(lane);
	}

	private void OnClick()
	{
		handCtlr.Summon(lane, newCard);
	}

	private void UpdateText(int lane)
	{
		GameState instance = GameState.Instance;
		CardItem card = instance.GetCard(PlayerType.User, lane, newCard.Form.Type);
		nameFrom.text = card.Form.Name;
		nameTo.text = newCard.Form.Name;
	}
}
