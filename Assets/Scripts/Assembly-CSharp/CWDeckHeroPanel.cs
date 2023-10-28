using UnityEngine;

public class CWDeckHeroPanel : MonoBehaviour
{
	public UILabel AbilityLabel;

	public UISprite HeroSprite;

	public UILabel HeroNameLabel;

	public UILabel HPLabel;

	public UILabel LevelLabel;

	public UILabel XPLabel;

	public UILabel DeckSizeLabel;

	public UILabel HeroCardsLabel;

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		int currentDeck = CWDeckController.GetInstance().currentDeck;
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		Deck deck = deckManager.Decks[currentDeck];
		LeaderItem leader = deck.Leader;
		LeaderManager instance = LeaderManager.Instance;
		AbilityLabel.text = leader.Description;
		SQUtils.SetIcon(HeroSprite, leader.Form.IconAtlas, leader.Form.SpriteNameHero);
		HeroNameLabel.text = leader.Form.Name;
		HPLabel.text = leader.HP.ToString();
		LevelLabel.text = leader.Rank.ToString();
		XPLabel.text = leader.ToNextRank.ToString();
		HeroCardsLabel.text = string.Format("{0}/{1}", instance.leaders.Count, instance.leaderForms.Count);
	}

	public void Update()
	{
		int currentDeck = CWDeckController.GetInstance().currentDeck;
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		Deck deck = deckManager.Decks[currentDeck];
		LeaderItem leader = deck.Leader;
		DeckSizeLabel.text = string.Format("{0}/{1}", deck.CardCount(), leader.RankValues.DeckMaxSize);
	}
}
