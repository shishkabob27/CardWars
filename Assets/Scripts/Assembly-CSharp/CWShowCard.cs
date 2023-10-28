using AnimationOrTween;
using UnityEngine;

public class CWShowCard : MonoBehaviour
{
	public CardItem card;

	public GameObject tweenToObj;

	public CWRevealCard revealScript;

	public GameObject RevealAnimation;

	public GameObject tweenToHide;

	public GameObject FlipVFX;

	public bool DebugVFX;

	private bool hasBeenShown;

	private bool currDelay;

	private float counter;

	public float Delay = 0.5f;

	private void OnClick()
	{
		ShowCard();
	}

	public void ShowCard()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (card != null)
		{
			if (instance.DeckManager.HasCard(card.Form.ID))
			{
				if (FlipVFX != null)
				{
					GameObject gameObject = (GameObject)SLOTGame.InstantiateFX(FlipVFX);
					if (gameObject != null)
					{
						gameObject.transform.parent = base.gameObject.transform;
						gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
						gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					}
					counter = 0f;
					currDelay = true;
				}
				else
				{
					PanelManagerBattle.FillCardInfo(base.gameObject, card);
				}
			}
			else
			{
				SetupReveal();
				CWResultsVFXSwitchboard instance2 = CWResultsVFXSwitchboard.GetInstance();
				if (instance2 != null)
				{
					if (card.Form.Rarity < 3)
					{
						instance2.SetRarityLow();
					}
					else if (card.Form.Rarity >= 3 && card.Form.Rarity <= 4)
					{
						instance2.SetRarityMed();
					}
					else if (card.Form.Rarity >= 5)
					{
						instance2.SetRarityHigh();
					}
				}
				UIButtonTween uIButtonTween = base.gameObject.GetComponent<UIButtonTween>();
				if (uIButtonTween == null)
				{
					uIButtonTween = base.gameObject.AddComponent<UIButtonTween>();
				}
				if (tweenToObj != null)
				{
					uIButtonTween.tweenTarget = tweenToHide;
					uIButtonTween.disableWhenFinished = DisableCondition.DoNotDisable;
					uIButtonTween.playDirection = Direction.Reverse;
					if (RevealAnimation != null)
					{
						uIButtonTween.eventReceiver = RevealAnimation;
						uIButtonTween.callWhenFinished = "OnClick";
					}
				}
			}
			instance.DeckManager.AddCard(card);
		}
		hasBeenShown = true;
	}

	public bool CanForceShowCard()
	{
		if (card != null)
		{
			bool flag = PlayerInfoScript.GetInstance().DeckManager.HasCard(card.Form.ID);
			base.gameObject.SendMessage("OnClick");
			return !flag;
		}
		return false;
	}

	public void ForceShowCard()
	{
		if (card != null)
		{
			PanelManagerBattle.FillCardInfo(base.gameObject, card);
		}
		UIButtonTween component = base.gameObject.GetComponent<UIButtonTween>();
		if (component != null)
		{
			component.enabled = false;
		}
		hasBeenShown = true;
	}

	public void SetupReveal()
	{
		if (tweenToObj != null && revealScript != null)
		{
			revealScript.SetCard(card, this);
		}
	}

	public bool HasBeenShown()
	{
		return hasBeenShown;
	}

	private void Update()
	{
		if (currDelay)
		{
			counter += Time.deltaTime;
			if (counter >= Delay)
			{
				currDelay = false;
				PanelManagerBattle.FillCardInfo(base.gameObject, card);
			}
		}
		if (!DebugVFX)
		{
			return;
		}
		DebugVFX = false;
		if (FlipVFX != null)
		{
			GameObject gameObject = (GameObject)SLOTGame.InstantiateFX(FlipVFX);
			if (gameObject != null)
			{
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}
}
