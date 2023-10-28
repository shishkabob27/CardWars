using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWDiscardPileFillTable : MonoBehaviour
{
	public GameObject CardPrefab;

	private GameState GameInstance;

	public PlayerType type;

	public UILabel headerLabel;

	private CardScript FilterScript;

	public void OnEnable()
	{
		reset();
		GameInstance = GameState.Instance;
		headerLabel.text = KFFLocalization.Get((type != PlayerType.User) ? "!!DISCARD_PILE_OPPONENT" : "!!DISCARD_PILE_PLAYER");
		StartCoroutine(FillTable(type));
	}

	private void OnDisable()
	{
		DebugFlagsScript instance = DebugFlagsScript.GetInstance();
		instance.cardSelection = false;
		FilterScript = null;
	}

	public void reset()
	{
		foreach (Transform item in base.gameObject.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void SetFilterScript(CardScript script)
	{
		FilterScript = script;
	}

	public static GameObject AddCard(CardItem card, CardScript filterScript, GameObject parent, GameObject prefab, float scale, int curItemNum, bool selectShowsX, PlayerType player)
	{
		GameObject gameObject = NGUITools.AddChild(parent, prefab);
		SQUtils.SetLayer(gameObject, parent.layer);
		gameObject.name = string.Format("DeckCard{0:D3}", curItemNum);
		gameObject.transform.localScale = new Vector3(scale, scale, 1f);
		gameObject.GetComponent<Collider>().enabled = true;
		UIPanel component = gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		CWDiscardCard component2 = gameObject.GetComponent<CWDiscardCard>();
		if ((bool)component2)
		{
			component2.card = card;
			component2.SelectShowsX = selectShowsX;
			component2.filterScript = filterScript;
		}
		PanelManagerBattle.FillCardInfo(gameObject, card, player);
		return gameObject;
	}

	private IEnumerator FillTable(PlayerType type)
	{
		DebugFlagsScript dbg = DebugFlagsScript.GetInstance();
		UIGrid grid = base.gameObject.GetComponent<UIGrid>();
		List<CardItem> discardPile2 = null;
		if (!dbg.cardSelection)
		{
			discardPile2 = GameInstance.GetDiscardPile((int)type);
		}
		else
		{
			discardPile2 = new List<CardItem>();
			List<CardForm> formList = CardDataManager.Instance.GetCards();
			foreach (CardForm form in formList)
			{
				CardItem item = new CardItem(form);
				discardPile2.Add(item);
			}
		}
		if (discardPile2 == null)
		{
			yield break;
		}
		int curItemNum = 0;
		for (int i = 0; i < discardPile2.Count; i++)
		{
			if (FilterScript == null || FilterScript.CardFilter(discardPile2[i]))
			{
				AddCard(discardPile2[i], FilterScript, base.gameObject, CardPrefab, 0.7f, curItemNum, true, type);
				grid.Reposition();
				curItemNum++;
				yield return null;
			}
		}
	}
}
