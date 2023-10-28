using UnityEngine;

public class CWCommandCardFill : MonoBehaviour
{
	public CardItem card;

	public int playerType;

	public int lane;

	public bool creatureFlag;

	public bool canFloop;

	public bool canAct;

	public GameObject creatureObj;

	public GameObject floopButton;

	public GameObject actionButton;

	public GameObject closeButton;

	private PanelManagerBattle pMgrBtl;

	public int bonusATK;

	public int bonusDEF;

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		pMgrBtl = PanelManagerBattle.GetInstance();
		pMgrBtl.ShowBlankCard(base.gameObject, card);
		if (card != null)
		{
			PanelManagerBattle.FillCardInfo(base.gameObject, card, bonusATK, bonusDEF);
		}
		if (creatureFlag)
		{
			pMgrBtl.HPBarUpdate(base.gameObject, playerType, lane - 1);
		}
		UITexture[] componentsInChildren = base.gameObject.GetComponentsInChildren<UITexture>(true);
		UITexture[] array = componentsInChildren;
		foreach (UITexture uITexture in array)
		{
			switch (uITexture.name)
			{
			case "Card_Glimmer":
				if (card != null && card.Form != null && card.Form.HasGlimmer())
				{
					uITexture.gameObject.SetActive(true);
				}
				else
				{
					uITexture.gameObject.SetActive(false);
				}
				break;
			}
		}
		UISprite[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<UISprite>(true);
		UISprite[] array2 = componentsInChildren2;
		foreach (UISprite uISprite in array2)
		{
			switch (uISprite.name)
			{
			case "BG_Gold":
				if (card != null && card.Form != null)
				{
					uISprite.spriteName = card.Form.GetCardQualitySummonPanelSprite();
					uISprite.gameObject.SetActive(uISprite.spriteName.Length > 0);
				}
				else
				{
					uISprite.gameObject.SetActive(false);
				}
				break;
			case "BG":
				if (card != null && card.Form != null)
				{
					if (card.Form.Faction == Faction.Corn)
					{
						uISprite.spriteName = "SummonPanelCorn";
					}
					else if (card.Form.Faction == Faction.Plains)
					{
						uISprite.spriteName = "SummonPanelPlains";
					}
					else if (card.Form.Faction == Faction.Cotton)
					{
						uISprite.spriteName = "SummonPanelCotton";
					}
					else if (card.Form.Faction == Faction.Sand)
					{
						uISprite.spriteName = "SummonPanelSand";
					}
					else if (card.Form.Faction == Faction.Swamp)
					{
						uISprite.spriteName = "SummonPanelSwamp";
					}
					else if (card.Form.Faction == Faction.Universal)
					{
						uISprite.spriteName = "SummonPanelColorless";
					}
					else
					{
						uISprite.spriteName = "SummonPanelColorless";
					}
				}
				break;
			case "CardType_Icon":
				if (card != null && card.Form != null)
				{
					if (card.Form.Type == CardType.Creature)
					{
						uISprite.spriteName = "IconCreatures";
					}
					else if (card.Form.Type == CardType.Building)
					{
						uISprite.spriteName = "IconBuildings";
					}
					else if (card.Form.Type == CardType.Spell)
					{
						uISprite.spriteName = "IconSpells";
					}
					else
					{
						uISprite.spriteName = "IconLeaders";
					}
				}
				else
				{
					uISprite.spriteName = "IconLeaders";
				}
				uISprite.color = Color.white;
				break;
			}
		}
		UILabel[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<UILabel>(true);
		UILabel[] array3 = componentsInChildren3;
		foreach (UILabel uILabel in array3)
		{
			if (!(uILabel.name == "CardType_Label"))
			{
				continue;
			}
			if (card != null && card.Form != null)
			{
				if (card.Form.Type == CardType.Creature)
				{
					uILabel.text = KFFLocalization.Get("!!CREATURE");
				}
				else if (card.Form.Type == CardType.Building)
				{
					uILabel.text = KFFLocalization.Get("!!BUILDING");
				}
				else if (card.Form.Type == CardType.Spell)
				{
					uILabel.text = KFFLocalization.Get("!!SPELL");
				}
				else
				{
					uILabel.text = string.Empty;
				}
			}
			else
			{
				uILabel.text = string.Empty;
			}
		}
		if (floopButton != null)
		{
			floopButton.SetActive(false);
			SetCreatureAnimation();
		}
		if (closeButton != null)
		{
			if (GameState.Instance.IsSummoning(playerType))
			{
				closeButton.SetActive(false);
			}
			else
			{
				closeButton.SetActive(true);
			}
		}
	}

	private void SetCreatureAnimation()
	{
		CWPlayCreatureAnimation component = floopButton.GetComponent<CWPlayCreatureAnimation>();
		if ((bool)component)
		{
			component.animTarget = creatureObj;
			AnimationState animationState = creatureObj.GetComponent<Animation>()["Floop"];
			if ((bool)animationState)
			{
				component.animName = "Floop";
			}
		}
	}

	private void SetFloopButtonState()
	{
		floopButton.SetActive(playerType == 0);
		UISprite[] componentsInChildren = floopButton.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			uISprite.color = ((!canFloop) ? Color.gray : Color.white);
		}
		floopButton.GetComponent<Collider>().enabled = canFloop;
	}

	private void SetActionButtonState()
	{
		NGUITools.SetActive(actionButton, card.Form.AbilityType == AbilityType.Action);
		UISprite componentInChildren = actionButton.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.color = ((!canAct) ? Color.gray : Color.white);
		}
		actionButton.GetComponent<Collider>().enabled = canAct;
	}
}
