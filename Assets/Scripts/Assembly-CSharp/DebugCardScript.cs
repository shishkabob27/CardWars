public class DebugCardScript : CardScript
{
	public override bool CardFilter(CardItem item)
	{
		for (int i = 0; i < 4; i++)
		{
			if (item.Form.CanPlay(PlayerType.User, i))
			{
				return true;
			}
		}
		return false;
	}

	public override void CardSelection(CardItem item)
	{
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		instance.cardSelection = false;
		GameState.Instance.PlaceCardInHand(PlayerType.User, item);
		CloseDiscardPile();
	}
}
