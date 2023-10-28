using System.Collections;
using UnityEngine;

public class CodeRedemptionScript : MonoBehaviour
{
	private const string PigCode = "P1GGY1";

	private const string CerebralCode = "C3R3BL";

	private const string BardCode = "MINSTR3L";

	private const string PajamaFinnCode = "UND3RPANT5";

	private const string BriefPowerCode = "BRIEF";

	private const string DocFinnCode = "DRKUNGFU";

	private const string JakeGhostCode = "GHOST DOG";

	private const string XmasBmoCode = "WINTER BMO";

	private const string LSPSweaterCode = "LUMPS";

	private const string SnuggleTreeCode = "CUTIE PIE";

	private const string MusicMallardCode = "123224";

	private const string SpikedMaceStumpCode = "NICE ONE";

	private const string FreezyJCode = "COOLDUDE";

	private const string AwesomatudeCode = "AWESOMATUDE";

	private const string MeMowCode = "ME-MOW";

	public UILabel Label;

	public AudioClip fanfare;

	public AudioClip success;

	public AudioClip failure;

	public AudioClip pig;

	public AudioClip cerebral;

	public UIButtonTween ShowSuccess;

	public UIButtonTween Showfail;

	public UIButtonTween ShowDupe;

	public UILabel CreatureName;

	public void RedeemCode(string input)
	{
		if ("P1GGY1" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotCreaturePig, "Creature_Pig");
			return;
		}
		if ("CUTIE PIE" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotCreatureSnuggleTree, "Creature_SnuggleTree");
			return;
		}
		if ("MINSTR3L" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotCreatureEmbarassingBard, "Creature_EmbarrassingBard");
			return;
		}
		if ("C3R3BL" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotSpellCerebral, "Spell_CerebralBloodstorm");
			return;
		}
		if ("BRIEF" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotSpellBriefPower, "Spell_BriefPower");
			return;
		}
		if ("123224" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotCreatureMusicMallard, "Creature_MusicMallard");
			return;
		}
		if ("NICE ONE" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotSpikedMaceStump, "Creature_SpikedMaceStump");
			return;
		}
		if ("COOLDUDE" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotFreezyJ, "Creature_FreezyJ");
			return;
		}
		if ("AWESOMATUDE" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotAwesomatude, "Creature_SpikedMaceStump");
			return;
		}
		if ("ME-MOW" == input)
		{
			DoCardRedeem(PlayerInfoScript.Tag.GotMeMow, "Creature_FreezyJ");
			return;
		}
		if ("WINTER BMO" == input)
		{
			DoLeaderRedeem("Leader_BMOSweater");
			return;
		}
		if ("GHOST DOG" == input && ScheduleDataManager.Instance.IsItemAvailableAndUnlocked("chest_premium", "halloween", TFUtils.ServerTime.Ticks))
		{
			DoLeaderRedeem("Leader_JakeGhost");
			return;
		}
		if ("LUMPS" == input && ScheduleDataManager.Instance.IsItemAvailableAndUnlocked("chest_premium", "christmas", TFUtils.ServerTime.Ticks))
		{
			DoLeaderRedeem("Leader_LumpySweater");
			return;
		}
		if ("UND3RPANT5" == input && Singleton<CodeRedemptionManager>.Instance.GetCurrentScheme() != null)
		{
			DoLeaderRedeem("Leader_FinnPajama");
			return;
		}
		if ("DRKUNGFU" == input && Singleton<CodeRedemptionManager>.Instance.GetCurrentScheme() != null)
		{
			DoLeaderRedeem("Leader_FinnDoctor");
			return;
		}
		if (Showfail != null)
		{
			Showfail.Play(true);
		}
		NGUITools.PlaySound(failure);
	}

	private void DoCardRedeem(PlayerInfoScript.Tag tag, string cardName)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (!instance.HasTag(tag))
		{
			NGUITools.PlaySound(fanfare);
			CardItem cardItem = new CardItem(CardDataManager.Instance.GetCard(cardName));
			if (cardItem != null)
			{
				instance.DeckManager.AddCard(cardItem);
				instance.SetTag(tag);
				if (CreatureName != null)
				{
					CreatureName.text = cardItem.Form.Name;
				}
				if (ShowSuccess != null)
				{
					ShowSuccess.Play(true);
				}
				StartCoroutine(CardRewardSfxCoroutine(cerebral));
			}
			instance.Save();
		}
		else
		{
			if (ShowDupe != null)
			{
				ShowDupe.Play(true);
			}
			NGUITools.PlaySound(success);
		}
	}

	private void DoLeaderRedeem(string leaderName)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		NGUITools.PlaySound(fanfare);
		LeaderItem leaderItem = LeaderManager.Instance.AddNewLeaderIfUnique(leaderName);
		if (leaderItem != null)
		{
			instance.Save();
			if (CreatureName != null)
			{
				CreatureName.text = leaderItem.Form.Name;
			}
			if (ShowSuccess != null)
			{
				ShowSuccess.Play(true);
			}
			StartCoroutine(CardRewardSfxCoroutine(cerebral));
		}
		else if (ShowDupe != null)
		{
			ShowDupe.Play(true);
		}
	}

	protected IEnumerator CardRewardSfxCoroutine(AudioClip clip)
	{
		yield return new WaitForSeconds(1.5f);
		NGUITools.PlaySound(clip);
	}

	private void OnClick()
	{
		if (Label != null)
		{
			string text = Label.text;
			RedeemCode(text);
			Label.text = string.Empty;
		}
	}
}
