using System;
using UnityEngine;

public class CWUpdatePlayerStats : MonoBehaviour
{
	private PlayerInfoScript pInfo;

	private GameState GameInstance;

	public UILabel nameLabel;

	public UILabel staminaLabel;

	public UILabel staminaMaxLabel;

	public UIFilledSprite staminaBar;

	public UILabel staminaTimer;

	public UILabel rankLabel;

	public UILabel coinLabel;

	public UILabel gemLabel;

	public GameObject gemSprite;

	public GameObject heartSprite;

	public UILabel NumTrophies;

	public UILabel inventoryLabel;

	public UIFilledSprite xpBar;

	public UILabel xpLabel;

	private GlobalFlags gflags;

	private static CWUpdatePlayerStats g_updatePlayerStats;

	public bool holdUpdateFlag;

	private float prevSTM;

	private float currentSTM;

	private float maxSTM;

	private void Awake()
	{
		g_updatePlayerStats = this;
	}

	public static CWUpdatePlayerStats GetInstance()
	{
		return g_updatePlayerStats;
	}

	private void Start()
	{
		pInfo = PlayerInfoScript.GetInstance();
		nameLabel.text = pInfo.name;
		GameInstance = GameState.Instance;
		gflags = GlobalFlags.Instance;
		if (gflags.ReturnToMainMenu && gflags.Cleared)
		{
			staminaMaxLabel.text = gflags.lastStaminaMax.ToString();
			gemLabel.text = gflags.lastGem.ToString();
			staminaLabel.text = gflags.lastStamina.ToString();
			inventoryLabel.text = pInfo.DeckManager.GetSortedInventory().Count + "/" + pInfo.MaxInventory;
			NumTrophies.text = pInfo.TotalTrophies.ToString();
			holdUpdateFlag = true;
		}
	}

	public int GetStaminaCountdown()
	{
		pInfo = PlayerInfoScript.GetInstance();
		if ((bool)pInfo && pInfo.Stamina < pInfo.Stamina_Max && (bool)StaminaTimerController.GetInstance())
		{
			int num = 0;
			int num2 = 0;
			if ((bool)pInfo && pInfo.Stamina < pInfo.Stamina_Max)
			{
				num = pInfo.Stamina_Max - pInfo.Stamina;
				if (num > 1)
				{
					num2 = (num - 1) * StaminaTimerController.GetInstance().Interval * 60;
				}
			}
			TimeSpan timeSpanRemaining = StaminaTimerController.GetInstance().GetTimeSpanRemaining();
			return timeSpanRemaining.Minutes * 60 + timeSpanRemaining.Seconds + num2;
		}
		return 0;
	}

	private void Update()
	{
		if (pInfo == null)
		{
			pInfo = PlayerInfoScript.GetInstance();
		}
		maxSTM = pInfo.Stamina_Max;
		currentSTM = pInfo.Stamina;
		if (prevSTM != currentSTM)
		{
			prevSTM = Mathf.Lerp(prevSTM, currentSTM, Time.deltaTime * 2f);
			staminaBar.fillAmount = prevSTM / maxSTM;
			if (prevSTM > currentSTM)
			{
				staminaBar.fillAmount = prevSTM / maxSTM;
			}
			else
			{
				staminaBar.fillAmount = currentSTM / maxSTM;
			}
		}
		if ((double)Mathf.Abs(currentSTM - prevSTM) < 0.1)
		{
			prevSTM = currentSTM;
		}
		if (!holdUpdateFlag)
		{
			staminaLabel.text = ((int)prevSTM).ToString();
			staminaMaxLabel.text = pInfo.Stamina_Max.ToString();
		}
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
				xpLabel.text = leaderItem.XP + "/" + (leaderItem.ToNextRank + leaderItem.XP);
			}
		}
		catch (ArgumentOutOfRangeException)
		{
		}
		if (!holdUpdateFlag && inventoryLabel != null)
		{
			inventoryLabel.text = pInfo.DeckManager.GetSortedInventory().Count + "/" + pInfo.MaxInventory;
		}
		coinLabel.text = pInfo.Coins.ToString();
		if (!holdUpdateFlag)
		{
			CWGachaController instance = CWGachaController.GetInstance();
			gemLabel.text = (pInfo.Gems + instance.GetBonusGemCount()).ToString();
		}
		NumTrophies.text = pInfo.TotalTrophies.ToString();
	}

	private Deck GetCurrentDeck()
	{
		Deck result = null;
		if (CWDeckController.GetInstance() != null)
		{
			int currentDeck = CWDeckController.GetInstance().currentDeck;
			result = pInfo.DeckManager.Decks[currentDeck];
		}
		else if (GameInstance != null)
		{
			result = GameInstance.GetDeck(PlayerType.User);
		}
		return result;
	}
}
