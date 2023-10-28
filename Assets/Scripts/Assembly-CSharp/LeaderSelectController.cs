using UnityEngine;

public class LeaderSelectController : MonoBehaviour
{
	private LeaderItem mSelectedLeader;

	public UILabel HeroName;

	public UISprite HeroIcon;

	public GameObject Labels;

	public LeaderItem SelectedLeader
	{
		get
		{
			return mSelectedLeader;
		}
		private set
		{
			mSelectedLeader = value;
			SelectedLeaderIndex = LeaderManager.Instance.leaders.IndexOf(mSelectedLeader);
		}
	}

	public int SelectedLeaderIndex { get; private set; }

	private void Start()
	{
	}

	private void OnEnable()
	{
		SetSelectedLeader();
	}

	public void SetSelectedLeader(LeaderItem leaderItem = null)
	{
		if (leaderItem == null)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			Deck selectedDeck = instance.GetSelectedDeck();
			leaderItem = selectedDeck.Leader;
		}
		SelectedLeader = leaderItem;
		UpdateUI();
	}

	public void SaveSelectedLeaderToPlayerDeck()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck selectedDeck = instance.GetSelectedDeck();
		selectedDeck.SetLeaderForPlayer(SelectedLeader);
	}

	private void UpdateUI()
	{
		if (SelectedLeader == null)
		{
			return;
		}
		if (HeroName != null && !GlobalFlags.Instance.InMPMode)
		{
			HeroName.text = SelectedLeader.Form.Name;
		}
		if (HeroIcon != null)
		{
			HeroIcon.atlas = LeaderManager.Instance.GetUiAtlas(SelectedLeader.Form.IconAtlas);
			HeroIcon.spriteName = SelectedLeader.Form.SpriteNameHero;
		}
		if (!(Labels != null))
		{
			return;
		}
		UILabel[] componentsInChildren = Labels.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "Lvl_Label")
			{
				uILabel.text = "Lvl " + SelectedLeader.Rank;
			}
			else if (uILabel.name == "HP_Label")
			{
				uILabel.text = SelectedLeader.HP.ToString();
			}
			else if (uILabel.name == "Ability_Label")
			{
				uILabel.text = SelectedLeader.Description;
			}
		}
	}
}
