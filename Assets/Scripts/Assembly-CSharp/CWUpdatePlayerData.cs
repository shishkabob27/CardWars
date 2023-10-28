using UnityEngine;

public class CWUpdatePlayerData : MonoBehaviour
{
	public UILabel healthLabel;

	public UIFilledSprite healthBar;

	public UIFilledSprite healthBarDamage;

	public UILabel deckLabel;

	public UILabel discardLabel;

	public UILabel magicLabel;

	public UILabel lootLabel;

	public UILabel coinsLabel;

	public UILabel gemLabel;

	public UIFilledSprite magicBar;

	public UISprite portrait;

	public UILabel playerName;

	public int player;

	private GameState GameInstance;

	private QuestEarningManager earningMgr;

	private CWLootingSequencer lootSqcr;

	private CWCharacterAnimController charAnimCtrlr;

	private BattlePhaseManager phaseMgr;

	public float maxHP;

	public float currentHP;

	public float prevHP;

	private bool fxSpawned;

	private bool initialized;

	private bool stop;

	private float prevMagic = -1f;

	private int currentMagic;

	public UILabel magicLabelAux;

	public UIFilledSprite magicBarAux;

	public UILabel turnCounter;

	public UISprite turnBG;

	public UILabel turnLbl;

	public GameObject leaderButton;

	public TriggerVFX magicPointUp;

	public TriggerVFX magicPointDown;

	private void Refresh()
	{
		GameDataScript instance = GameDataScript.GetInstance();
		GameInstance = GameState.Instance;
		earningMgr = QuestEarningManager.GetInstance();
		lootSqcr = CWLootingSequencer.GetInstance();
		maxHP = GameInstance.GetMaxHealth(player);
		prevHP = maxHP;
		currentHP = maxHP;
		healthLabel.text = currentHP.ToString();
		charAnimCtrlr = CWCharacterAnimController.GetInstance();
		if (instance.characterUiAtlas[player] != null)
		{
			portrait.atlas = instance.characterUiAtlas[player];
		}
		portrait.spriteName = charAnimCtrlr.playerData[player].PortraitSprite;
		playerName.text = GameInstance.GetDeck(player).Leader.Form.Name;
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	private void Update()
	{
		if (!initialized)
		{
			SessionManager instance = SessionManager.GetInstance();
			if (!instance.IsReady())
			{
				return;
			}
			initialized = true;
			Refresh();
		}
		if (!GameInstance.IsSetUp || stop)
		{
			return;
		}
		deckLabel.text = GameInstance.GetCardsInDeck(player).Count.ToString();
		if (discardLabel != null)
		{
			discardLabel.text = GameInstance.GetDiscardPile(player).Count.ToString();
		}
		if (!lootSqcr.holdLootFlag && lootLabel != null)
		{
			lootLabel.text = earningMgr.earnedCards.Count.ToString();
		}
		if (coinsLabel != null)
		{
			coinsLabel.text = earningMgr.earnedCoin.ToString();
		}
		currentMagic = GameInstance.GetMagicPoints(player);
		if (prevMagic < 0f)
		{
			prevMagic = currentMagic;
		}
		if (prevMagic != (float)currentMagic)
		{
			prevMagic = Mathf.Lerp(prevMagic, currentMagic, Time.deltaTime * 2f);
			if (!fxSpawned && magicPointUp != null && magicPointDown != null)
			{
				fxSpawned = true;
				if (prevMagic > (float)currentMagic)
				{
					magicPointDown.ChildOfAnchor = true;
					magicPointDown.SpawnVFX();
				}
				else
				{
					magicPointUp.ChildOfAnchor = true;
					magicPointUp.SpawnVFX();
				}
			}
		}
		else
		{
			fxSpawned = false;
		}
		if ((double)Mathf.Abs((float)currentMagic - prevMagic) < 0.1)
		{
			prevMagic = currentMagic;
		}
		if (magicBar != null)
		{
			magicBar.fillAmount = ((currentMagic < GameState.Instance.CurrentMagicPoints - (GameDataScript.GetInstance().FirstPlayer - 1)) ? (prevMagic / (float)(GameState.Instance.CurrentMagicPoints - (GameDataScript.GetInstance().FirstPlayer - 1))) : 1f);
		}
		if (magicBarAux != null)
		{
			magicBarAux.fillAmount = ((currentMagic < GameState.Instance.CurrentMagicPoints - (GameDataScript.GetInstance().FirstPlayer - 1)) ? (prevMagic / (float)(GameState.Instance.CurrentMagicPoints - (GameDataScript.GetInstance().FirstPlayer - 1))) : 1f);
		}
		if (magicLabel != null)
		{
			magicLabel.text = ((int)prevMagic).ToString();
		}
		if (magicLabelAux != null)
		{
			magicLabelAux.text = ((int)prevMagic).ToString();
		}
		currentHP = GameInstance.GetHealth(player);
		if (currentHP == 0f && prevHP == 0f)
		{
			stop = true;
		}
		if (prevHP != currentHP)
		{
			prevHP = Mathf.Lerp(prevHP, currentHP, Time.deltaTime * 2f);
			healthBarDamage.fillAmount = prevHP / maxHP;
			if (prevHP < currentHP)
			{
				healthBar.fillAmount = prevHP / maxHP;
				healthLabel.text = ((int)prevHP).ToString();
			}
			else
			{
				healthBar.fillAmount = currentHP / maxHP;
				healthLabel.text = currentHP.ToString();
			}
			if ((double)currentHP <= (double)maxHP * 0.25)
			{
				TriggerAlphaTween(true);
			}
			else
			{
				TriggerAlphaTween(false);
			}
		}
		if ((double)Mathf.Abs(currentHP - prevHP) < 0.01)
		{
			prevHP = currentHP;
			healthLabel.text = currentHP.ToString();
		}
		if (turnCounter != null)
		{
			try
			{
				turnCounter.text = GameState.Instance.GetLeaderCooldown(player).ToString();
			}
			catch
			{
				turnCounter.text = string.Empty;
			}
		}
		LeaderItem leader = GameState.Instance.GetLeader(player);
		if (phaseMgr.Phase == BattlePhase.P1Setup)
		{
			if (player == (int)PlayerType.User && GameState.Instance.GetLeaderCooldown(PlayerType.User) <= 0)
			{
				if (leader.CanPlay(PlayerType.User))
				{
					SetLeaderAbilityButton(AbilityButtonState.READY);
				}
				else
				{
					SetLeaderAbilityButton(AbilityButtonState.READY_UNSUABLE);
				}
			}
		}
		else
		{
			SetLeaderAbilityButton(AbilityButtonState.COUNTDOWN);
		}
	}

	private void EnableLeaderAbilityButton(bool enable, bool isGrey)
	{
		if (leaderButton != null && leaderButton.activeInHierarchy != enable)
		{
			leaderButton.SetActive(enable);
			if (isGrey)
			{
				SQUtils.SetGray(leaderButton, 0.4f);
				Collider[] componentsInChildren = leaderButton.GetComponentsInChildren<Collider>();
				Collider[] array = componentsInChildren;
				foreach (Collider collider in array)
				{
					collider.enabled = false;
				}
			}
			else
			{
				SQUtils.SetGray(leaderButton, 1f);
				Collider[] componentsInChildren2 = leaderButton.GetComponentsInChildren<Collider>();
				Collider[] array2 = componentsInChildren2;
				foreach (Collider collider2 in array2)
				{
					collider2.enabled = true;
				}
			}
			TweenScale component = leaderButton.GetComponent<TweenScale>();
			if (component != null && component.enabled != !isGrey)
			{
				component.enabled = !isGrey;
			}
		}
		if (turnLbl != null && turnLbl.gameObject.activeInHierarchy == enable)
		{
			turnLbl.gameObject.SetActive(!enable);
		}
		if (turnCounter != null && turnCounter.gameObject.activeInHierarchy == enable)
		{
			turnCounter.gameObject.SetActive(!enable);
		}
		if (turnBG != null && !turnBG.gameObject.activeInHierarchy == enable)
		{
			turnBG.gameObject.SetActive(!enable);
		}
	}

	private void SetLeaderAbilityButton(AbilityButtonState state)
	{
		switch (state)
		{
		case AbilityButtonState.COUNTDOWN:
			EnableLeaderAbilityButton(false, false);
			break;
		case AbilityButtonState.READY_UNSUABLE:
			EnableLeaderAbilityButton(true, true);
			break;
		case AbilityButtonState.READY:
			EnableLeaderAbilityButton(true, false);
			TutorialMonitor.Instance.TriggerTutorial(TutorialTrigger.LeaderAbilityReady);
			break;
		}
	}

	private void TriggerAlphaTween(bool enable)
	{
		TweenAlpha component = healthBar.GetComponent<TweenAlpha>();
		if (!(component == null))
		{
			if (enable)
			{
				component.Play(enable);
			}
			if (!enable)
			{
				component.enabled = false;
				healthBar.color = Color.white;
			}
		}
	}

	public void Resurrect()
	{
		stop = false;
	}
}
