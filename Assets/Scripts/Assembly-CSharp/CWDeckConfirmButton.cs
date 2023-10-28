using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

public class CWDeckConfirmButton : AsyncData<string>
{
	public CWDeckAddCards DeckAddCards;

	public UILabel SizeLabel;

	public UIButtonTween closeTween;

	public AudioClip[] placeCardSounds;

	public CWDeckManagerShowHideBackButton showhide;

	private void OnEnable()
	{
		if (showhide != null)
		{
			showhide.enabled = false;
		}
	}

	public void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		PlayerDeckManager deckManager = instance.DeckManager;
		CWDeckController instance2 = CWDeckController.GetInstance();
		Deck deck = deckManager.GetDeck(instance2.currentDeck);
		LeaderItem leader = deck.Leader;
		int count = DeckAddCards.chosenList.Count;
		if (count > leader.RankValues.DeckMaxSize)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.DM_TooManyCards);
			return;
		}
		if (count < ParametersManager.Instance.Min_Cards_In_Deck)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.DM_TooFewCards);
			return;
		}
		string text = CheckDuplicates(ParametersManager.Instance.Max_Duplicates_In_Deck);
		if (text != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.DM_TooManyDuplicates);
			return;
		}
		deck.Empty();
		foreach (CardItem chosen in DeckAddCards.chosenList)
		{
			deck.AddCard(chosen);
		}
		instance2.SetLandscapes();
		instance2.ChangeDeck();
		CWDeckBuildDeckController instance3 = CWDeckBuildDeckController.GetInstance();
		if (instance3 != null)
		{
			instance3.UpdateUI();
		}
		if (placeCardSounds != null && placeCardSounds.Length > 0)
		{
			int randomIndex = KFFRandom.GetRandomIndex(placeCardSounds.Length);
			if (placeCardSounds[randomIndex] != null)
			{
				SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayGUISound(placeCardSounds[randomIndex]);
			}
		}
		instance.Save();
		closeTween.Play(true);
		if (showhide != null)
		{
			showhide.enabled = true;
			showhide.OnClick();
			showhide.enabled = false;
		}
		PlayerInfoScript instance4 = PlayerInfoScript.GetInstance();
		if (Asyncdata.processed && instance4.MPPlayerName != null && instance4.MPPlayerName != string.Empty && CWDeckBuildDeckController.GetInstance().currentDeck == instance4.SelectedMPDeck)
		{
			instance4.DeckManager.UpdateMPDeck(SuccessCallback);
		}
	}

	public void SuccessCallback(ResponseFlag flag)
	{
		Asyncdata.Set(flag);
	}

	public string CheckDuplicates(int maxDups)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (CardItem chosen in DeckAddCards.chosenList)
		{
			try
			{
				int num = dictionary[chosen.Form.ID];
				if (num >= maxDups)
				{
					return chosen.Form.Name;
				}
				dictionary[chosen.Form.ID] = num + 1;
			}
			catch (KeyNotFoundException)
			{
				dictionary.Add(chosen.Form.ID, 1);
			}
		}
		return null;
	}

	public void Update()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		PlayerDeckManager deckManager = instance.DeckManager;
		CWDeckController instance2 = CWDeckController.GetInstance();
		Deck deck = deckManager.GetDeck(instance2.currentDeck);
		LeaderItem leader = deck.Leader;
		int count = DeckAddCards.chosenList.Count;
		SizeLabel.text = string.Format("{0}/{1}", count, leader.RankValues.DeckMaxSize);
		if (count > leader.RankValues.DeckMaxSize || count < ParametersManager.Instance.Min_Cards_In_Deck)
		{
			SizeLabel.color = Color.red;
		}
		else
		{
			SizeLabel.color = Color.black;
		}
		if (!Asyncdata.processed)
		{
			Asyncdata.processed = true;
			if (Asyncdata.flag == ResponseFlag.Success)
			{
			}
		}
	}
}
