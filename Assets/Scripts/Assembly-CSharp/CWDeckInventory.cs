using System.Collections.Generic;
using UnityEngine;

public class CWDeckInventory : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public GameObject LeaderCardPrefab;

	public CardType panelType;

	private bool showHeroes;

	public void OnEnable()
	{
		PlayerDeckManager.ResetSort();
		Sort();
	}

	public void Sort()
	{
		FillTable();
	}

	public void ShowHeroes()
	{
		showHeroes = true;
	}

	public bool GetShowHeroes()
	{
		return showHeroes;
	}

	public void ShowCards()
	{
		showHeroes = false;
	}

	private void FillTable()
	{
		UIFastGrid component = base.gameObject.GetComponent<UIFastGrid>();
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		List<CardItem> sortedInventory = deckManager.GetSortedInventory();
		deckManager.DetermineMembership();
		List<object> list = new List<object>();
		if (!showHeroes)
		{
			foreach (CardItem item in sortedInventory)
			{
				if (item.Form.Type == panelType)
				{
					list.Add(item);
				}
			}
		}
		else
		{
			foreach (LeaderItem leader in LeaderManager.Instance.leaders)
			{
				list.Add(leader);
			}
		}
		component.Initialize(list, pickPrefab, fillCard);
	}

	private GameObject pickPrefab(object data)
	{
		if (data is LeaderItem)
		{
			return LeaderCardPrefab;
		}
		return DeckCardPrefab;
	}

	private void fillCard(GameObject cardObj, object data)
	{
		if (data is CardItem)
		{
			CWDeckDeckCards.FillCard(data as CardItem, cardObj, 0.73f, false, false);
			return;
		}
		cardObj.transform.localScale = new Vector3(0.73f, 0.73f, 1f);
		LeaderItem leader = data as LeaderItem;
		CWDeckHero component = cardObj.GetComponent<CWDeckHero>();
		if ((bool)component)
		{
			component.leader = leader;
			component.RadioParentObject = base.gameObject;
			component.NonSelectable = true;
		}
		FillCardInfo(cardObj, leader);
	}

	public static void FillCardInfo(GameObject obj, LeaderItem leader)
	{
		UISprite[] componentsInChildren = obj.GetComponentsInChildren<UISprite>(true);
		UISprite[] array = componentsInChildren;
		foreach (UISprite uISprite in array)
		{
			switch (uISprite.name)
			{
			case "Card_Art":
				uISprite.enabled = true;
				SQUtils.SetIcon(uISprite, leader.Form.IconAtlas, leader.Form.SpriteName, Color.white);
				break;
			case "Card_Frame":
				uISprite.enabled = true;
				uISprite.spriteName = leader.Form.FrameSpriteName;
				break;
			}
		}
		UILabel[] componentsInChildren2 = obj.GetComponentsInChildren<UILabel>(true);
		UILabel[] array2 = componentsInChildren2;
		foreach (UILabel uILabel in array2)
		{
			switch (uILabel.name)
			{
			case "Desc_Label":
				uILabel.enabled = true;
				uILabel.text = leader.Description;
				break;
			case "Name_Label":
				uILabel.enabled = true;
				uILabel.text = leader.Form.Name;
				break;
			case "HP_Label":
				uILabel.enabled = true;
				uILabel.text = leader.HP.ToString();
				break;
			case "ToNext_Label":
				uILabel.enabled = true;
				uILabel.text = leader.ToNextRank.ToString();
				break;
			case "LevelNum_Label":
				uILabel.enabled = true;
				uILabel.text = leader.Rank.ToString();
				break;
			case "Cost_Label":
				uILabel.enabled = true;
				uILabel.text = string.Empty;
				break;
			case "Type_Label":
				uILabel.enabled = true;
				uILabel.text = string.Empty;
				break;
			}
		}
	}
}
