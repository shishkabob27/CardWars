using UnityEngine;

public class QuestChangeDeckScript : MonoBehaviour
{
	public bool Increment;

	public LandscapePreviewScript Lane1;

	public LandscapePreviewScript Lane2;

	public LandscapePreviewScript Lane3;

	public LandscapePreviewScript Lane4;

	public CWQuestLandscapes LandscapePreviews;

	public GameObject Labels;

	public LeaderSelectController LeaderSelect;

	private void OnEnable()
	{
		UpdateUI();
		LeaderSelect.SetSelectedLeader();
	}

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance != null)
		{
			int selectedDeck = instance.SelectedDeck;
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				if (Increment)
				{
					instance.SelectedDeck = (instance.SelectedDeck + 1) % 5;
				}
				else if (instance.SelectedDeck == 0)
				{
					instance.SelectedDeck = 4;
				}
				else
				{
					instance.SelectedDeck--;
				}
				Deck selectedDeck2 = instance.GetSelectedDeck();
				if (selectedDeck2.CardCount() >= ParametersManager.Instance.Min_Cards_In_Deck)
				{
					break;
				}
				if (selectedDeck2.CardCount() > 0)
				{
					num++;
				}
			}
			if (selectedDeck == instance.SelectedDeck && num == 0)
			{
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.OnlyOneDeck);
			}
		}
		LeaderSelect.SetSelectedLeader();
		UpdateUI();
	}

	private void UpdateUI()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (Lane1 != null)
		{
			Lane1.UpdatePreview();
		}
		if (Lane2 != null)
		{
			Lane2.UpdatePreview();
		}
		if (Lane3 != null)
		{
			Lane3.UpdatePreview();
		}
		if (Lane4 != null)
		{
			Lane4.UpdatePreview();
		}
		if ((bool)LandscapePreviews)
		{
			LandscapePreviews.UpdatePreview();
		}
		if (!(Labels != null))
		{
			return;
		}
		Deck selectedDeck = instance.GetSelectedDeck();
		UILabel[] componentsInChildren = Labels.GetComponentsInChildren<UILabel>();
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			if (uILabel.name == "Lvl_Label")
			{
				uILabel.text = "Lvl " + selectedDeck.Leader.Rank;
			}
			if (uILabel.name == "HP_Label")
			{
				uILabel.text = selectedDeck.Leader.HP.ToString();
			}
			if (uILabel.name == "Ability_Label")
			{
				uILabel.text = selectedDeck.Leader.Description;
			}
		}
	}
}
