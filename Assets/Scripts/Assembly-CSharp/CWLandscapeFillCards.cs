using UnityEngine;

public class CWLandscapeFillCards : MonoBehaviour
{
	private LandscapeManagerScript landscapeMgr;

	private GameState GameInstance;

	private void OnEnable()
	{
		landscapeMgr = LandscapeManagerScript.GetInstance();
		GameInstance = GameState.Instance;
		FillLandscapeCards();
	}

	private void FillLandscapeCards()
	{
		for (int i = 0; i < 4; i++)
		{
			LandscapeType landscapeInDeck = GameInstance.GetLandscapeInDeck(PlayerType.User, i);
			FillCard(landscapeInDeck, i, landscapeMgr.CardScripts[i].gameObject);
		}
	}

	private void FillCard(LandscapeType t, int index, GameObject obj)
	{
		string factionName = GetFactionName(t);
		string spriteName = "Landscape_" + t.ToString() + "1";
		string spriteName2 = "Frame_Base_Landscape";
		UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			if (uISprite.name == "Card_Art")
			{
				uISprite.spriteName = spriteName;
			}
			if (uISprite.name == "Card_Frame")
			{
				uISprite.spriteName = spriteName2;
				uISprite.color = GetFactionColor(t);
			}
		}
		UILabel[] componentsInChildren2 = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			if (uILabel.name == "Card_Label")
			{
				uILabel.text = factionName;
			}
		}
		CWLandscapeCardDragOld component = obj.GetComponent<CWLandscapeCardDragOld>();
		if (component != null)
		{
			component.CurrentType = t;
			component.currentTypeName = t.ToString();
		}
	}

	private Color GetFactionColor(LandscapeType t)
	{
		return FactionManager.Instance.GetFactionData(t).FactionColor;
	}

	private string GetFactionName(LandscapeType t)
	{
		switch (t)
		{
		case LandscapeType.Corn:
			return KFFLocalization.Get("!!FACTION_CORN");
		case LandscapeType.Cotton:
			return KFFLocalization.Get("!!FACTION_COTTON");
		case LandscapeType.Plains:
			return KFFLocalization.Get("!!FACTION_PLAINS");
		case LandscapeType.Sand:
			return KFFLocalization.Get("!!FACTION_SAND");
		case LandscapeType.Swamp:
			return KFFLocalization.Get("!!FACTION_SWAMP");
		default:
			return t.ToString();
		}
	}
}
