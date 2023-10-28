using System.Collections.Generic;
using UnityEngine;

public class CWMapQuestInfo : MonoBehaviour
{
	public ScreenDimmer questMapDimmer;

	private List<string> chars = new List<string>();

	private GameObject backButton;

	private GameObject backButton2;

	public GameObject CloseButton;

	public GameObject RewardsUnknown;

	public GameObject[] DeckCards;

	private void OnEnable()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.GetCurrentQuest() != null)
		{
			FillQuestInfoPanel();
		}
	}

	private void debugAddChars()
	{
		chars.Add("Portrait_Jake_Frame");
		chars.Add("Portrait_Beemo_Frame");
		chars.Add("Portrait_Princess_Frame");
		chars.Add("Portrait_IceKing_Frame");
		chars.Add("Portrait_Lumpy_Frame");
		chars.Add("Portrait_Marceline_Frame");
		chars.Add("Portrait_Earl_Frame");
	}

	private void FillQuestInfoPanel()
	{
		GlobalFlags.Instance.enableMapDrag = false;
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData currentQuest = instance.GetCurrentQuest();
		SetupQuestRewardPanel();
		bool flag = false;
		if (currentQuest.QuestType != MapControllerBase.GetInstance().MapQuestType)
		{
			flag = true;
		}
		int questProgress = instance.GetQuestProgress(currentQuest);
		UISprite[] componentsInChildren = GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
			case "OpponentPortrait":
				uISprite.atlas = LeaderManager.Instance.GetUiAtlas(currentQuest.Opponent.PortraitAtlas);
				uISprite.spriteName = currentQuest.Opponent.PortraitSprite.Replace("Frame", "Hero");
				break;
			case "Star1":
				uISprite.spriteName = ((questProgress < 1) ? "UI_Star_Empty" : "UI_Star_Full");
				uISprite.enabled = !flag;
				break;
			case "Star2":
				uISprite.spriteName = ((questProgress < 2) ? "UI_Star_Empty" : "UI_Star_Full");
				uISprite.enabled = !flag;
				break;
			case "Star3":
				uISprite.spriteName = ((questProgress < 3) ? "UI_Star_Empty" : "UI_Star_Full");
				uISprite.enabled = !flag;
				break;
			case "Star3_BG":
				uISprite.enabled = !flag;
				break;
			}
		}
		string text = currentQuest.StaminaCost.ToString();
		UILabel[] componentsInChildren2 = GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			if (uILabel.name == "Level")
			{
				uILabel.text = currentQuest.QuestLabel;
			}
			if (uILabel.name == "Stamina")
			{
				uILabel.text = text;
			}
			if (uILabel.name == "QuestInfo")
			{
				if (questProgress >= 3)
				{
					uILabel.text = KFFLocalization.Get("!!GOT_ALL_THREE_STARS");
				}
				else
				{
					string conditionDescription = currentQuest.GetConditionDescription(questProgress);
					if (string.IsNullOrEmpty(conditionDescription))
					{
						uILabel.text = KFFLocalization.Get("!!MISSING_QUEST");
					}
					else
					{
						uILabel.text = conditionDescription;
					}
				}
			}
			if (uILabel.name == "OpponentName")
			{
				CharacterData characterData = CharacterDataManager.Instance.GetCharacterData(currentQuest.Opponent.ID.ToString());
				if (characterData != null)
				{
					uILabel.text = characterData.Name;
				}
				else
				{
					uILabel.text = currentQuest.Opponent.ID;
				}
			}
			if (uILabel.name == "Lvl_Label" && uILabel.transform.parent.parent.gameObject.name == "Opponent")
			{
				uILabel.text = "Lvl " + currentQuest.LeaderLevel;
			}
			if (uILabel.name == "HP_Label" && uILabel.transform.parent.parent.gameObject.name == "Opponent")
			{
				Deck deckCopy = AIDeckManager.Instance.GetDeckCopy(currentQuest.OpponentDeckID);
				deckCopy.Leader = LeaderManager.Instance.CreateLeader(currentQuest.LeaderID, currentQuest.LeaderLevel);
				uILabel.text = deckCopy.Leader.HP.ToString();
			}
			if (uILabel.name == "Ability_Label" && uILabel.transform.parent.parent.gameObject.name == "Opponent")
			{
				Deck deckCopy2 = AIDeckManager.Instance.GetDeckCopy(currentQuest.OpponentDeckID);
				deckCopy2.Leader = LeaderManager.Instance.CreateLeader(currentQuest.LeaderID, currentQuest.LeaderLevel);
				uILabel.text = deckCopy2.Leader.Description;
			}
		}
	}

	private void SetupQuestRewardPanel()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		QuestData currentQuest = instance.GetCurrentQuest();
		if (!currentQuest.UseNewCardDropSystem || currentQuest.IsQuestType("tcat"))
		{
			RewardsUnknown.SetActive(true);
			GameObject[] deckCards = DeckCards;
			foreach (GameObject gameObject in deckCards)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		RewardsUnknown.SetActive(false);
		List<CardItem> list = new List<CardItem>();
		DropProfile dropProfile = DropProfileDataManager.Instance.GetDropProfile(currentQuest.DropProfileID);
		foreach (string cardDrop in currentQuest.CardDrops)
		{
			CardForm card = CardDataManager.Instance.GetCard(cardDrop, false);
			if (card != null)
			{
				CardItem item = new CardItem(card);
				list.Add(item);
			}
		}
		list.Sort((CardItem a, CardItem b) => b.Form.Rarity.CompareTo(a.Form.Rarity));
		int count = Mathf.Min(list.Count, DeckCards.Length);
		list = list.GetRange(0, count);
		for (int j = 0; j < DeckCards.Length; j++)
		{
			GameObject gameObject2 = DeckCards[j];
			if (j >= list.Count)
			{
				gameObject2.SetActive(false);
				continue;
			}
			gameObject2.SetActive(true);
			CardItem card2 = list[j];
			PanelManagerBattle.FillCardInfo(DeckCards[j], card2, PlayerType.User);
			CWQuestPanelZoom component = DeckCards[j].GetComponent<CWQuestPanelZoom>();
			if (null != component)
			{
				component.card = card2;
			}
		}
	}

	public void FillDebugQuest()
	{
	}

	private void OnTweenEnableForward()
	{
		if (backButton == null)
		{
			backButton = GameObject.Find("F_1_QuestsMap_BottomInfo/BackButton");
		}
		if (backButton2 == null)
		{
			backButton2 = GameObject.Find("F_1_QuestsMap_BottomInfo/DeckButton");
		}
		if (questMapDimmer == null)
		{
			GameObject gameObject = GameObject.Find("QuestMapDimmer");
			if (gameObject != null)
			{
				questMapDimmer = gameObject.GetComponent(typeof(ScreenDimmer)) as ScreenDimmer;
			}
		}
		if (questMapDimmer != null)
		{
			questMapDimmer.FadeIn();
		}
		if (backButton != null)
		{
			backButton.SetActive(false);
		}
		if (backButton2 != null)
		{
			backButton2.SetActive(false);
		}
		if (TutorialMonitor.Instance != null)
		{
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.EnterQuestInfo);
		}
	}

	private void OnTweenEnableReverse()
	{
		if (backButton == null)
		{
			backButton = GameObject.Find("F_1_QuestsMap_BottomInfo/BackButton");
		}
		if (backButton != null)
		{
			backButton.SetActive(true);
		}
		if (backButton2 != null)
		{
			backButton2.SetActive(true);
		}
		if (questMapDimmer == null)
		{
			GameObject gameObject = GameObject.Find("QuestMapDimmer");
			if (gameObject != null)
			{
				questMapDimmer = gameObject.GetComponent(typeof(ScreenDimmer)) as ScreenDimmer;
			}
		}
		if (questMapDimmer != null)
		{
			questMapDimmer.FadeOut();
		}
		GlobalFlags.Instance.enableMapDrag = true;
	}
}
