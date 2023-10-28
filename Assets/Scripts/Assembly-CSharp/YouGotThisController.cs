#define ASSERTS_ON
using System.Collections;
using UnityEngine;

public class YouGotThisController : MonoBehaviour
{
	public const string CardMarker = "Card";

	public const string LeaderMarker = "Leader_";

	public const string GemMarker = "Gems";

	public const string CoinMarker = "Coins";

	public const string HeartMarker = "Hearts";

	private const string youGotThisLocKey = "!!I_3_YOUGOTTHIS";

	public GameObject Card;

	public GameObject LeaderCardObj;

	public UIButtonTween RewardShow;

	public AudioClip RewardClip;

	public UILabel RewardText;

	public GameObject IconReward;

	public UILabel GiftRewardText;

	private void Start()
	{
	}

	private void Reset()
	{
		if (null != LeaderCardObj)
		{
			LeaderCardObj.SetActive(false);
		}
		if (null != Card)
		{
			Card.SetActive(false);
		}
		if (null != IconReward)
		{
			IconReward.SetActive(false);
		}
		base.gameObject.SetActive(true);
	}

	private void EnableInput(bool enable)
	{
		UICamera.useInputEnabler = !enable;
	}

	public IEnumerator AwardLeader(string cardId)
	{
		Reset();
		EnableInput(false);
		LeaderItem newLeader = LeaderManager.Instance.AddNewLeaderIfUnique(cardId);
		if (newLeader == null)
		{
			newLeader = LeaderManager.Instance.GetLeader(cardId);
			TFUtils.DebugLog("Already own Leader " + cardId + ", not awarding it again.", GetType().ToString());
		}
		TFUtils.Assert(newLeader != null, "newLeader is null, validate cardId: " + cardId);
		if (newLeader != null && LeaderCardObj != null)
		{
			LeaderCardObj.SetActive(true);
			CWDeckInventory.FillCardInfo(LeaderCardObj, newLeader);
			if (null != RewardText)
			{
				RewardText.text = KFFLocalization.Get("!!I_3_YOUGOTTHIS", "<Val1>", newLeader.Form.Name + "!");
			}
			if (null != RewardShow)
			{
				if (null != RewardClip)
				{
					SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), RewardClip);
				}
				RewardShow.Play(true);
			}
		}
		yield return new WaitForSeconds(2.5f);
		EnableInput(true);
	}

	public IEnumerator AwardCard(string cardId)
	{
		Reset();
		EnableInput(false);
		cardId = cardId.Trim();
		PlayerInfoScript pInfo = PlayerInfoScript.GetInstance();
		CardItem cardItem = pInfo.DeckManager.AddCardAward(cardId);
		if (cardItem != null)
		{
			TFUtils.DebugLog(string.Format("Awarded Card: card: {0} - {1}", cardItem.Form.ID, cardItem.Form.Name), GetType().ToString());
			pInfo.Save();
			if (null != Card)
			{
				Card.SetActive(true);
				PanelManagerBattle.FillCardInfo(Card, cardItem);
				if (null != RewardText)
				{
					RewardText.text = KFFLocalization.Get("!!I_3_YOUGOTTHIS", "<Val1>", cardItem.Form.Name + "!");
				}
				CWDeckCard script = Card.GetComponent<CWDeckCard>();
				if (null != script)
				{
					script.card = cardItem;
				}
				if (null != RewardShow)
				{
					if (null != RewardClip)
					{
						SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), RewardClip);
					}
					RewardShow.Play(true);
				}
			}
		}
		else
		{
			TFUtils.WarnLog("Couldn't reward card ID: " + cardId);
		}
		yield return new WaitForSeconds(2.5f);
		EnableInput(true);
	}

	public IEnumerator AwardItem(string item, string locKey = null, string icon = null, int quantity = 1)
	{
		Reset();
		EnableInput(false);
		if (item.StartsWith("Card"))
		{
			Card.SetActive(true);
			string cardId = item.Substring("Card".Length);
			cardId = cardId.TrimStart(' ').TrimEnd(' ');
			if (cardId.StartsWith("Leader_"))
			{
				yield return StartCoroutine(AwardLeader(cardId));
			}
			else
			{
				yield return StartCoroutine(AwardCard(cardId));
			}
		}
		else
		{
			if (null != GiftRewardText && !string.IsNullOrEmpty(locKey))
			{
				GiftRewardText.text = quantity + " " + KFFLocalization.Get(locKey);
			}
			if (null != IconReward)
			{
				IconReward.SetActive(true);
				if (!string.IsNullOrEmpty(icon))
				{
					UISprite sprite = IconReward.GetComponent<UISprite>();
					sprite.spriteName = icon;
				}
			}
			PlayerInfoScript pinfo = PlayerInfoScript.GetInstance();
			switch (item)
			{
			case "Hearts":
				pinfo.Stamina += quantity;
				break;
			case "Gems":
				pinfo.Gems += quantity;
				break;
			case "Coins":
				pinfo.Coins += quantity;
				break;
			}
			if (null != RewardShow)
			{
				if (null != RewardClip)
				{
					SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), RewardClip);
				}
				RewardShow.Play(true);
				yield return new WaitForSeconds(2.5f);
			}
		}
		EnableInput(true);
	}
}
