using UnityEngine;

public class CWRevealCard : MonoBehaviour
{
	private CardItem CardData;

	private CWShowCard ShowCard;

	public bool Reveal;

	private bool DelayReveal;

	private float counter;

	public GameObject CardObj;

	public float Rotation;

	public bool Premium;

	public TweenTransform prizeTween;

	public GameObject ContinueButton;

	public CWResultBannerAnimation BannerAnims;

	public float LowDelay;

	public float MedDelay;

	public float HighDelay;

	private float delay;

	public bool tweenPlayed;

	public void SetCard(CardItem item, CWShowCard show)
	{
		CardData = item;
		ShowCard = show;
		if (item.Form.Rarity >= 5)
		{
			delay = HighDelay;
		}
		else if (item.Form.Rarity >= 3 && item.Form.Rarity <= 4)
		{
			delay = MedDelay;
		}
		else
		{
			delay = LowDelay;
		}
	}

	private void OnEnable()
	{
		Reveal = false;
		Rotation = 0f;
		tweenPlayed = false;
		if (ContinueButton != null)
		{
			NGUITools.SetActive(ContinueButton, false);
		}
		if (CardObj != null)
		{
			ShowBlankCard(CardObj, CardData);
		}
	}

	private void OnClick()
	{
		UpdateCard();
		counter = 0f;
		DelayReveal = true;
	}

	public void UpdateCard()
	{
		if (CardData == null)
		{
		}
	}

	private void Update()
	{
		if (DelayReveal)
		{
			counter += Time.deltaTime;
			if (counter >= delay)
			{
				DelayReveal = false;
				RevealCard();
			}
		}
		if (!Reveal)
		{
			return;
		}
		Rotation = Mathf.Lerp(Rotation, 3600f, Time.deltaTime * 2f);
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, Rotation, base.transform.localEulerAngles.z);
		if (Rotation > 3510f && !tweenPlayed)
		{
			if (prizeTween != null)
			{
				NGUITools.SetActive(prizeTween.gameObject, true);
				prizeTween.Play(true);
				prizeTween.Reset();
				tweenPlayed = true;
			}
			PlayerInfoScript.GetInstance().Save();
		}
		if (Rotation > 3590f)
		{
			RevealCard();
		}
	}

	private void RevealCard()
	{
		Reveal = false;
		base.transform.localEulerAngles = Vector3.zero;
		ShowCard.ForceShowCard();
		if (CardObj != null)
		{
			PanelManagerBattle.FillCardInfo(CardObj, CardData);
		}
		if (ContinueButton != null)
		{
			NGUITools.SetActive(ContinueButton, true);
		}
	}

	public void FillCardInfo(CardItem card)
	{
		UISprite[] componentsInChildren = base.gameObject.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
			case "Card_Art":
				uISprite.enabled = true;
				SQUtils.SetIcon(uISprite.gameObject, card.Form.IconAtlas, card.Form.SpriteName, Color.white);
				break;
			case "Card_Frame":
				uISprite.enabled = true;
				uISprite.spriteName = card.Form.FrameSpriteName;
				break;
			case "Card_Frame_Combat":
				if (card.Form.Type == CardType.Creature)
				{
					uISprite.enabled = true;
					uISprite.spriteName = card.Form.FrameSpriteName + "_Combat";
				}
				else
				{
					uISprite.enabled = false;
				}
				break;
			case "star1":
				uISprite.enabled = card.Form.Rarity >= 1;
				break;
			case "star2":
				uISprite.enabled = card.Form.Rarity >= 2;
				break;
			case "star3":
				uISprite.enabled = card.Form.Rarity >= 3;
				break;
			case "star4":
				uISprite.enabled = card.Form.Rarity >= 4;
				break;
			case "star5":
				uISprite.enabled = card.Form.Rarity >= 5;
				break;
			}
		}
	}

	public void ShowBlankCard(GameObject obj, CardItem card)
	{
		UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
			case "Card_Art":
				uISprite.enabled = false;
				break;
			case "Card_Frame":
				uISprite.enabled = false;
				break;
			case "Card_Frame_Combat":
				uISprite.enabled = false;
				break;
			case "star1":
				uISprite.enabled = false;
				break;
			case "star2":
				uISprite.enabled = false;
				break;
			case "star3":
				uISprite.enabled = false;
				break;
			case "star4":
				uISprite.enabled = false;
				break;
			case "star5":
				uISprite.enabled = false;
				break;
			case "ATK_BG":
				uISprite.enabled = false;
				break;
			case "HPBarFront":
				uISprite.enabled = false;
				break;
			}
		}
		UILabel[] componentsInChildren2 = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			switch (uILabel.name)
			{
			case "ATK_Label":
				uILabel.text = string.Empty;
				break;
			case "Cost_Label":
				uILabel.text = string.Empty;
				break;
			case "DEF_Label":
				uILabel.text = string.Empty;
				break;
			case "Floop_Label":
				uILabel.text = string.Empty;
				break;
			case "Desc_Label":
				uILabel.text = string.Empty;
				break;
			case "Name_Label":
				uILabel.text = string.Empty;
				break;
			case "Faction_Label":
				uILabel.text = string.Empty;
				break;
			case "Type_Label":
				uILabel.text = string.Empty;
				break;
			case "LevelNum_Label":
				uILabel.text = string.Empty;
				break;
			case "DEF_sc_Label":
				uILabel.text = string.Empty;
				break;
			case "HP_sc_Label":
				uILabel.text = string.Empty;
				break;
			case "ATK_sc_Label":
				uILabel.text = string.Empty;
				break;
			case "slash":
				uILabel.text = string.Empty;
				break;
			case "Floop_Cost_Label":
				uILabel.text = string.Empty;
				break;
			}
		}
	}
}
