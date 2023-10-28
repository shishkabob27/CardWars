using System;
using UnityEngine;

public class CWBattleEndPlayerStats : MonoBehaviour
{
	private PlayerInfoScript pInfo;

	private GameState GameInstance;

	public UILabel nameLabel;

	public UILabel staminaLabel;

	public UIFilledSprite staminaBar;

	public UILabel staminaTimer;

	public UILabel rankLabel;

	public UILabel coinLabel;

	public UILabel gemLabel;

	public UILabel inventoryLabel;

	public UILabel inventoryMaxLabel;

	public UIFilledSprite xpBar;

	public UILabel xpLabel;

	public UILabel xpToNextLabel;

	public LeaderItem ssleaderCard;

	public int ssCoin;

	public int ssGem;

	public int ssNumStars;

	public UILabel NumTrophies;

	private static CWBattleEndPlayerStats g_battlEndPlayerStats;

	public string ssSchemeID;

	public int ssRank;

	public int ssXP;

	public int ssHP;

	private void Start()
	{
		GameInstance = GameState.Instance;
	}

	private void Awake()
	{
		g_battlEndPlayerStats = this;
	}

	public static CWBattleEndPlayerStats GetInstance()
	{
		return g_battlEndPlayerStats;
	}

	private void Initialize()
	{
		int stamina_Max = pInfo.Stamina_Max;
		int stamina = pInfo.Stamina;
		staminaBar.fillAmount = stamina / stamina_Max;
		staminaLabel.text = stamina + "/" + pInfo.Stamina_Max;
		if (pInfo.Stamina < pInfo.Stamina_Max)
		{
			TimeSpan timeSpanRemaining = StaminaTimerController.GetInstance().GetTimeSpanRemaining();
			staminaTimer.text = string.Format("{0:d}:{1:d2}", timeSpanRemaining.Minutes, timeSpanRemaining.Seconds);
		}
		else
		{
			staminaTimer.text = KFFLocalization.Get("!!G_STAMINA_FULL");
		}
		try
		{
			Deck currentDeck = GetCurrentDeck();
			LeaderItem leaderItem = null;
			if (currentDeck != null)
			{
				leaderItem = currentDeck.Leader;
			}
			if (leaderItem != null)
			{
				rankLabel.text = leaderItem.Rank.ToString();
			}
			if (nameLabel != null && leaderItem != null)
			{
				nameLabel.text = leaderItem.Form.Name;
			}
			if (xpBar != null && leaderItem != null)
			{
				xpBar.fillAmount = (float)leaderItem.XP / (float)(leaderItem.ToNextRank + leaderItem.XP);
			}
			if (xpLabel != null && leaderItem != null)
			{
				xpLabel.text = leaderItem.XP.ToString();
			}
			xpToNextLabel.text = (leaderItem.ToNextRank + leaderItem.XP).ToString();
		}
		catch (ArgumentOutOfRangeException)
		{
		}
		if (inventoryLabel != null)
		{
			inventoryLabel.text = pInfo.DeckManager.GetSortedInventory().Count.ToString();
		}
		inventoryMaxLabel.text = pInfo.MaxInventory.ToString();
		coinLabel.text = pInfo.Coins.ToString();
		gemLabel.text = pInfo.Gems.ToString();
		NumTrophies.text = pInfo.TotalTrophies.ToString();
	}

	private void StorePrevData()
	{
		ssleaderCard = GameState.Instance.GetLeader(PlayerType.User);
		ssSchemeID = ssleaderCard.Form.LvUpSchemeID;
		ssXP = ssleaderCard.XP;
		ssHP = ssleaderCard.HP;
		ssRank = ssleaderCard.Rank;
		ssCoin = pInfo.Coins;
		ssGem = pInfo.Gems;
		ssNumStars = ((GameState.Instance.BattleResolver == null) ? pInfo.GetQuestProgress(pInfo.GetCurrentQuest()) : GameState.Instance.BattleResolver.questStars);
	}

	private void OnEnable()
	{
		pInfo = PlayerInfoScript.GetInstance();
		GameInstance = GameState.Instance;
		StorePrevData();
		Initialize();
	}

	private void Update()
	{
		NumTrophies.text = pInfo.TotalTrophies.ToString();
	}

	private Deck GetCurrentDeck()
	{
		Deck result = null;
		if (GameInstance != null)
		{
			result = GameInstance.GetDeck(PlayerType.User);
		}
		return result;
	}
}
