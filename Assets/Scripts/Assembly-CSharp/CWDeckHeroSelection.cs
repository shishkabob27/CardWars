using System.Collections;
using UnityEngine;

public class CWDeckHeroSelection : MonoBehaviour
{
	public GameObject DeckCardPrefab;

	public void OnEnable()
	{
		PlayerDeckManager.ResetSort();
		Sort();
	}

	public void Sort()
	{
		reset();
		StartCoroutine(FillTable());
	}

	public void reset()
	{
		StopAllCoroutines();
		UIDraggablePanel component = base.transform.parent.GetComponent<UIDraggablePanel>();
		if (component != null)
		{
			component.ResetPosition();
		}
		foreach (Transform item in base.gameObject.transform)
		{
			Object.Destroy(item.gameObject);
		}
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
				SQUtils.SetIcon(uISprite, leader.Form.IconAtlas, leader.Form.SpriteNameHero, Color.white);
				break;
			case "Card_Frame":
				uISprite.enabled = true;
				SQUtils.SetIcon(uISprite, "CardFrameAtlas", "Frame_Hero", Color.white);
				break;
			case "XPBar_BG":
				uISprite.enabled = true;
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

	private GameObject AddCard(LeaderItem leader, GameObject parent, GameObject prefab, float scale, int curItemNum)
	{
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		SQUtils.SetLayer(gameObject, parent.layer);
		gameObject.name = string.Format("DeckCard{0:D3}", curItemNum);
		gameObject.transform.localScale = new Vector3(scale, scale, 1f);
		gameObject.GetComponent<Collider>().enabled = true;
		bool flag = true;
		if (leader.Form.FCWorld)
		{
			PlayerInfoScript instance = PlayerInfoScript.GetInstance();
			int currentDeck = CWDeckBuildDeckController.GetInstance().currentDeck;
			flag = instance.SelectedMPDeck != currentDeck;
		}
		gameObject.GetComponent<Collider>().enabled = flag;
		UIPanel component = gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		FillCardInfo(gameObject, leader);
		CWDeckHero component2 = gameObject.GetComponent<CWDeckHero>();
		if ((bool)component2)
		{
			PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
			int currentDeck2 = CWDeckController.GetInstance().currentDeck;
			Deck deck = deckManager.GetDeck(currentDeck2);
			component2.leader = leader;
			component2.RadioParentObject = base.gameObject;
			if (deck.Leader == leader)
			{
				component2.Selected = true;
			}
			component2.Grayed = !flag;
		}
		return gameObject;
	}

	private IEnumerator FillTable()
	{
		UIGrid grid = base.gameObject.GetComponent<UIGrid>();
		UIDraggablePanel drag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		if (drag != null)
		{
			drag.ResetPosition();
		}
		int curItemNum = 0;
		foreach (LeaderItem leader in LeaderManager.Instance.leaders)
		{
			AddCard(leader, base.gameObject, DeckCardPrefab, 0.63f, curItemNum);
			grid.Reposition();
			curItemNum++;
			yield return null;
		}
		if (drag != null)
		{
			drag.ResetPosition();
		}
	}
}
