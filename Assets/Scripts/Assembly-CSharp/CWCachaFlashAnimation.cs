using UnityEngine;

public class CWCachaFlashAnimation : MonoBehaviour
{
	public GameObject FadeInAnimationStart;

	public GameObject FadeOutAnimationStart;

	public GatchaRevealScript RevealScript;

	public bool RevealAfterFadeIn;

	public float RevealDelay = 1f;

	public TriggerVFX AnimTrigger;

	public GameObject VFX_RarityLow;

	public GameObject VFX_RarityMed;

	public GameObject VFX_RarityHigh;

	private GameObject vfxRarity;

	public GameObject BackShadow;

	public float RarityDelay_Low = 0.5f;

	public float RarityDelay_Med = 0.6f;

	public float RarityDelay_High = 0.5f;

	public Transform NormalRarity;

	public Transform PremiumRarity;

	private void OnEnable()
	{
		if (BackShadow != null)
		{
			TweenAlpha component = BackShadow.GetComponent<TweenAlpha>();
			if (component != null)
			{
				component.enabled = true;
			}
		}
		RarityAnim();
	}

	public void RarityAnim()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		CardForm card = CardDataManager.Instance.GetCard(instance.cardID);
		float num = 0f;
		if (card != null)
		{
			switch (card.Rarity)
			{
			case 1:
				vfxRarity = VFX_RarityLow;
				num = RarityDelay_Low;
				break;
			case 2:
				vfxRarity = VFX_RarityLow;
				num = RarityDelay_Low;
				break;
			case 3:
				vfxRarity = VFX_RarityMed;
				num = RarityDelay_Med;
				break;
			case 4:
				vfxRarity = VFX_RarityMed;
				num = RarityDelay_Med;
				break;
			case 5:
				vfxRarity = VFX_RarityHigh;
				num = RarityDelay_High;
				break;
			default:
				vfxRarity = VFX_RarityLow;
				num = RarityDelay_Low;
				break;
			}
		}
		else
		{
			vfxRarity = VFX_RarityHigh;
			num = RarityDelay_High;
		}
		if (AnimTrigger != null)
		{
			AnimTrigger.VFX = vfxRarity;
			AnimTrigger.SpawnVFX();
		}
		if (RevealScript != null)
		{
			RevealScript.RevealCardAfterDelay(num);
		}
	}

	public void FadeIn()
	{
		if (FadeInAnimationStart != null)
		{
			FadeInAnimationStart.SendMessage("OnClick");
		}
		if (RevealScript != null && RevealAfterFadeIn)
		{
			RevealScript.RevealCardAfterDelay(RevealDelay);
		}
	}

	public void FadeOut()
	{
		if (FadeOutAnimationStart != null)
		{
			FadeOutAnimationStart.SendMessage("OnClick");
		}
		if (RevealScript != null)
		{
			RevealScript.RevealCard();
		}
	}
}
