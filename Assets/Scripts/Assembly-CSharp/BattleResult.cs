public class BattleResult
{
	public enum Menu
	{
		MapMain,
		BattleModeSelect,
		DungeonSelect,
		BuildDeck
	}

	public Menu returnMenu { get; protected set; }

	public bool newlyCleared { get; private set; }

	public bool cleared { get; private set; }

	public BattleResult(Menu inReturnMenu = Menu.BattleModeSelect, bool inCleared = false, bool inNewlyCleared = false)
	{
		returnMenu = inReturnMenu;
		SetCleared(inCleared, inNewlyCleared);
	}

	public void SetCleared(bool inCleared, bool inNewlyCleared = false)
	{
		cleared = inCleared;
		newlyCleared = inCleared && inNewlyCleared;
		GlobalFlags.Instance.Cleared = cleared;
		GlobalFlags.Instance.NewlyCleared = newlyCleared;
	}
}
