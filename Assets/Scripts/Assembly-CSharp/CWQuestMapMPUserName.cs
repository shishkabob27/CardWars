using Multiplayer;
using UnityEngine;

public class CWQuestMapMPUserName : AsyncData<MultiplayerData>
{
	public UILabel Label;

	public UIButtonTween ShowBottomInfo;

	public GameObject enterMapEvents;

	public GameObject BadNameWarning;

	public UIButtonTween CloseTween;

	private void Awake()
	{
		CloseTween = base.gameObject.GetComponent<UIButtonTween>();
	}

	private Deck GetCurrentMPDeck()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck result = null;
		if (CWDeckController.GetInstance() != null)
		{
			int currentMPDeck = CWDeckController.GetInstance().currentMPDeck;
			result = instance.DeckManager.Decks[currentMPDeck];
		}
		return result;
	}

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck currentMPDeck = GetCurrentMPDeck();
		string deck = currentMPDeck.MPDeckSerialize();
		string landscapes = currentMPDeck.MPLandscapeSerialize();
		if (Asyncdata.processed && Label.text != string.Empty)
		{
			global::Multiplayer.Multiplayer.CreateMultiplayerUser(SessionManager.GetInstance().theSession, Label.text, currentMPDeck.Leader.Form.IconAtlas, deck, currentMPDeck.SumOfAllLevels(), landscapes, currentMPDeck.Leader.Form.ID, currentMPDeck.Leader.Rank, instance.DeckManager.GetHighestLeaderRank(), MultiplayerDataCallback);
		}
		else if (Label.text == string.Empty)
		{
			if ((bool)CloseTween)
			{
				CloseTween.enabled = false;
			}
			if ((bool)BadNameWarning)
			{
				BadNameWarning.SetActive(true);
			}
		}
	}

	private void MultiplayerDataCallback(MultiplayerData data, ResponseFlag flag)
	{
		Asyncdata.Set(flag, data);
	}

	private void Update()
	{
		if (Asyncdata.processed)
		{
			return;
		}
		Asyncdata.processed = true;
		if (Asyncdata.MP_Data != null && Label != null && Label.text != string.Empty)
		{
			GlobalFlags instance = GlobalFlags.Instance;
			instance.InMPMode = true;
			instance.BattleResult = null;
			CWMapController.Activate(true);
			if (enterMapEvents != null)
			{
				if ((bool)CloseTween)
				{
					CloseTween.enabled = true;
				}
				CloseTween.Play(true);
				if ((bool)ShowBottomInfo)
				{
					ShowBottomInfo.Play(true);
				}
				PlayerInfoScript instance2 = PlayerInfoScript.GetInstance();
				instance2.MPPlayerName = Label.text;
				instance2.Save();
				TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.MultiplayerUnlocked);
				enterMapEvents.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				StartCoroutine(CWQuestMapMPButton.CompleteMultiplayerTutorial());
			}
		}
		else
		{
			if ((bool)CloseTween)
			{
				CloseTween.enabled = false;
			}
			if ((bool)BadNameWarning)
			{
				BadNameWarning.SetActive(true);
			}
			if (Label != null)
			{
				Label.text = string.Empty;
			}
		}
	}
}
