using UnityEngine;

public class MenuController : ReloadHandler
{
	public enum MenuStates
	{
		None = 0,
		Wait = 1,
		Start = 2,
		MainMenu = 3,
		Options = 4,
		Market = 5,
		Gacha = 6,
		Battle = 7,
		Dungeon = 8,
		Deck = 9,
		Update = 10,
		Messages = 11,
		Reload = 12,
		AcceptPolicies = 13,
	}

	public MenuStates MenuState;
	public GameObject AsyncLoaders;
	public GameObject MainLogo;
	public GameObject PlayerStats;
	public LogoPanelScript LogoPanel;
	public Transform BattleSelectCameraSnap;
	public Transform BattleSelectCameraTargetSnap;
	public Transform DungeonSelectCameraSnap;
	public Transform DungeonSelectCameraTargetSnap;
	public GameObject MainMenuCamera;
	public GameObject WaitHide;
	public GameObject WaitShow;
	public GameObject StartHide;
	public GameObject StartShow;
	public GameObject AcceptPoliciesShow;
	public GameObject AcceptPoliciesHide;
	public GameObject MainMenuHide;
	public GameObject MainMenuShow;
	public GameObject OptionsHide;
	public GameObject OptionsShow;
	public GameObject MarketHide;
	public GameObject MarketShow;
	public GameObject GachaHide;
	public GameObject GachaShow;
	public GameObject BattleHide;
	public GameObject BattleShow;
	public GameObject DungeonHide;
	public GameObject DungeonShow;
	public GameObject DeckHide;
	public GameObject DeckShow;
	public GameObject UpdateHide;
	public GameObject UpdateShow;
	public GameObject MessagesHide;
	public GameObject MessagesShow;
	public GameObject ReloadHide;
	public GameObject ReloadShow;
	public UIButtonTween CalendarShow;
	public UIButtonTween TooManyCardsShow;
	public UIButtonTween DungeonExtrasShow;
	public GameObject YouGotThis;
	public UIButtonTween ElFistoVictoryShow;
	public UIButtonTween ElFistoCompleteShow;
	public GameObject FCMapButton;
	public bool hasAwardStuff;
}
