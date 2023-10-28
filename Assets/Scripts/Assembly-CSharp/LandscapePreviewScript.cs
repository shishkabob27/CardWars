using UnityEngine;

public class LandscapePreviewScript : MonoBehaviour
{
	public int Lane;

	public UISprite icon;

	public UILabel label;

	public UISprite frame;

	private LandscapeType type;

	private PlayerInfoScript pInfo;

	private Deck deck;

	private string sprite_Corn = "Landscape_Corn1";

	private string sprite_Cotton = "Landscape_Cotton1";

	private string sprite_Plains = "Landscape_Plains1";

	private string sprite_Swamp = "Landscape_Swamp1";

	private string sprite_Sand = "Landscape_Sand1";

	public bool IsOpponent;

	public static string name_Corn
	{
		get
		{
			return KFFLocalization.Get("!!LANDSCAPE_CORN");
		}
	}

	public static string name_Cotton
	{
		get
		{
			return KFFLocalization.Get("!!LANDSCAPE_NICELANDS");
		}
	}

	public static string name_Swamp
	{
		get
		{
			return KFFLocalization.Get("!!LANDSCAPE_SWAMP");
		}
	}

	public static string name_Plains
	{
		get
		{
			return KFFLocalization.Get("!!LANDSCAPE_PLAINS");
		}
	}

	public static string name_Sand
	{
		get
		{
			return KFFLocalization.Get("!!LANDSCAPE_SANDYLAND");
		}
	}

	private void OnEnable()
	{
		pInfo = PlayerInfoScript.GetInstance();
		deck = pInfo.GetSelectedDeck();
		if (deck != null)
		{
			type = deck.GetLandscape(Lane);
		}
		UpdatePreview();
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
			LandscapeType landscape = deck.GetLandscape(Lane);
			type = landscape;
			if (icon != null)
			{
				icon.spriteName = GetArtSprite();
			}
			if (label != null)
			{
				label.text = GetName();
			}
			if (frame != null)
			{
				frame.spriteName = GetFrameSprite();
			}
		}
	}

	private string GetArtSprite()
	{
		switch (type)
		{
		case LandscapeType.Corn:
			return sprite_Corn;
		case LandscapeType.Cotton:
			return sprite_Cotton;
		case LandscapeType.Swamp:
			return sprite_Swamp;
		case LandscapeType.Plains:
			return sprite_Plains;
		case LandscapeType.Sand:
			return sprite_Sand;
		default:
			return string.Empty;
		}
	}

	private string GetFrameSprite()
	{
		string text = null;
		switch (type)
		{
		case LandscapeType.Corn:
			text = "Corn";
			break;
		case LandscapeType.Cotton:
			text = "Cotton";
			break;
		case LandscapeType.Swamp:
			text = "Swamp";
			break;
		case LandscapeType.Plains:
			text = "Plains";
			break;
		case LandscapeType.Sand:
			text = "Sand";
			break;
		}
		if (text == null)
		{
			return "Frame_Landscape";
		}
		return "Frame_" + text + "_Landscape";
	}

	private string GetName()
	{
		switch (type)
		{
		case LandscapeType.Corn:
			return name_Corn;
		case LandscapeType.Cotton:
			return name_Cotton;
		case LandscapeType.Swamp:
			return name_Swamp;
		case LandscapeType.Plains:
			return name_Plains;
		case LandscapeType.Sand:
			return name_Sand;
		default:
			return string.Empty;
		}
	}
}
