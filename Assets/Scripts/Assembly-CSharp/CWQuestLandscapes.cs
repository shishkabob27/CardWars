using UnityEngine;

public class CWQuestLandscapes : MonoBehaviour
{
	public bool IsOpponent;

	public UISprite Lane1;

	public UISprite Lane2;

	public UISprite Lane3;

	public UISprite Lane4;

	private string sprite_Corn = "UI_CornIcon";

	private string sprite_Cotton = "UI_CottonIcon";

	private string sprite_Plains = "UI_PlainsIcon";

	private string sprite_Swamp = "UI_SwampIcon";

	private string sprite_Sand = "UI_SandIcon";

	private PlayerInfoScript pInfo;

	private Deck deck;

	private void OnEnable()
	{
		pInfo = PlayerInfoScript.GetInstance();
		this.deck = pInfo.GetSelectedDeck();
		if (this.deck.CardCount() < ParametersManager.Instance.Min_Cards_In_Deck)
		{
			for (int i = 0; i < 5; i++)
			{
				Deck deck = pInfo.DeckManager.GetDeck(i);
				if (deck != null && deck.CardCount() >= ParametersManager.Instance.Min_Cards_In_Deck)
				{
					this.deck = deck;
					pInfo.SelectedDeck = i;
					break;
				}
			}
		}
		UpdatePreview();
	}

	public void UpdateMPPreview(string[] LandscapeNames)
	{
		if (Lane1 != null)
		{
			Lane1.spriteName = GetMPSprite(LandscapeNames[0]);
		}
		if (Lane2 != null)
		{
			Lane2.spriteName = GetMPSprite(LandscapeNames[1]);
		}
		if (Lane3 != null)
		{
			Lane3.spriteName = GetMPSprite(LandscapeNames[2]);
		}
		if (Lane4 != null)
		{
			Lane4.spriteName = GetMPSprite(LandscapeNames[3]);
		}
	}

	private string GetMPSprite(string LandscapeNames)
	{
		switch (LandscapeNames)
		{
		case "Corn":
			return sprite_Corn;
		case "Cotton":
			return sprite_Cotton;
		case "Plains":
			return sprite_Plains;
		case "Swamp":
			return sprite_Swamp;
		case "Sand":
			return sprite_Sand;
		default:
			return "CW_Grey";
		}
	}

	public void UpdatePreview()
	{
		if (pInfo == null)
		{
			pInfo = PlayerInfoScript.GetInstance();
		}
		if (IsOpponent)
		{
			QuestData currentQuest = pInfo.GetCurrentQuest();
			deck = AIDeckManager.Instance.GetDeckCopy(currentQuest.OpponentDeckID);
		}
		else
		{
			deck = pInfo.GetSelectedDeck();
		}
		if (deck != null)
		{
			if (Lane1 != null)
			{
				Lane1.spriteName = GetSprite(deck.GetLandscape(0));
			}
			if (Lane2 != null)
			{
				Lane2.spriteName = GetSprite(deck.GetLandscape(1));
			}
			if (Lane3 != null)
			{
				Lane3.spriteName = GetSprite(deck.GetLandscape(2));
			}
			if (Lane4 != null)
			{
				Lane4.spriteName = GetSprite(deck.GetLandscape(3));
			}
		}
	}

	private string GetSprite(LandscapeType land)
	{
		switch (land)
		{
		case LandscapeType.Corn:
			return sprite_Corn;
		case LandscapeType.Cotton:
			return sprite_Cotton;
		case LandscapeType.Plains:
			return sprite_Plains;
		case LandscapeType.Swamp:
			return sprite_Swamp;
		case LandscapeType.Sand:
			return sprite_Sand;
		default:
			return "CW_Grey";
		}
	}
}
