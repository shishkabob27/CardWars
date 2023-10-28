using System.Collections;
using UnityEngine;

public class CWOutOfCardsPopup : MonoBehaviour
{
	public UIButtonTween showOutOfCards;

	public UIButtonTween hideOutOfCards;

	private static CWOutOfCardsPopup g_OutOfCardsPopup;

	private PlayerType WhichPlayer;

	private DiscardTarget DiscardEffect;

	public static CWOutOfCardsPopup Instance
	{
		get
		{
			return g_OutOfCardsPopup;
		}
	}

	private void Awake()
	{
		if (g_OutOfCardsPopup == null)
		{
			g_OutOfCardsPopup = this;
		}
		DiscardEffect = new DiscardTarget();
		CardForm cardForm = new SpellCard();
		cardForm.ScriptName = "DiscardTarget";
		DiscardEffect.Data = new CardItem(cardForm);
		DiscardEffect.GameInstance = GameState.Instance;
	}

	public void Show(PlayerType player)
	{
		if (showOutOfCards != null)
		{
			WhichPlayer = player;
			showOutOfCards.Play(true);
		}
	}

	public void StartProcess()
	{
		StartCoroutine(ProcessBoard());
	}

	public PlayerType GetPlayer()
	{
		return WhichPlayer;
	}

	private IEnumerator ProcessBoard()
	{
		bool WaitingToDie = true;
		while (WaitingToDie)
		{
			WaitingToDie = false;
			for (int i = 0; i < 4; i++)
			{
				if (GameState.Instance.IsMarkedForDeath(WhichPlayer, i))
				{
					WaitingToDie = true;
				}
			}
			yield return null;
		}
		DiscardEffect.Owner = WhichPlayer;
		DiscardEffect.Cast();
		yield return null;
		BattlePhase Phase = BattlePhaseManager.GetInstance().Phase;
		while (Phase == BattlePhase.P1FloopAction || Phase == BattlePhase.P2FloopAction)
		{
			yield return null;
			Phase = BattlePhaseManager.GetInstance().Phase;
		}
		yield return new WaitForSeconds(1f);
		Hide();
	}

	public void Hide()
	{
		if (hideOutOfCards != null)
		{
			hideOutOfCards.Play(true);
		}
	}
}
