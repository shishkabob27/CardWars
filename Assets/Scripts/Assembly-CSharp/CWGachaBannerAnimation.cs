using UnityEngine;

public class CWGachaBannerAnimation : MonoBehaviour
{
	public Animation BannerRarity1;

	public Animation BannerRarity2;

	public Animation BannerRarity3;

	public Animation BannerRarity4;

	public Animation BannerRarity5;

	public Transform NormalRarity;

	public Transform PremiumRarity;

	public Transform HideRarity;

	public float NormalDelay;

	public float PremiumDelay;

	private float counter;

	private float delay;

	private bool waiting;

	private Animation bannerRarity;

	public bool debugRarityFlag;

	public int debugRarity;

	public UIButtonPlayAnimation AnimationTrigger;

	private void OnEnable()
	{
	}

	public void AnimateBanner()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		CardForm card = CardDataManager.Instance.GetCard(instance.cardID);
		int num = 0;
		num = ((!debugRarityFlag) ? card.Rarity : debugRarity);
		if (card != null)
		{
			switch (num)
			{
			case 1:
				bannerRarity = BannerRarity1;
				break;
			case 2:
				bannerRarity = BannerRarity2;
				break;
			case 3:
				bannerRarity = BannerRarity3;
				break;
			case 4:
				bannerRarity = BannerRarity4;
				break;
			case 5:
				bannerRarity = BannerRarity5;
				break;
			default:
				bannerRarity = BannerRarity1;
				break;
			}
		}
		else
		{
			bannerRarity = BannerRarity5;
		}
		delay = ((instance.activeChest != CWGachaController.ChestType.Premium) ? NormalDelay : PremiumDelay);
		counter = 0f;
		waiting = true;
	}

	private void Update()
	{
		if (waiting)
		{
			counter += Time.deltaTime;
			if (counter >= delay)
			{
				waiting = false;
				counter = 0f;
				PlayAnimation();
			}
		}
	}

	private void PlayAnimation()
	{
		CWGachaController instance = CWGachaController.GetInstance();
		if (bannerRarity != null)
		{
			if (AnimationTrigger != null)
			{
				AnimationTrigger.target = bannerRarity;
				AnimationTrigger.SendMessage("OnClick");
			}
			if (instance.activeChest == CWGachaController.ChestType.Normal && NormalRarity != null)
			{
				bannerRarity.transform.position = NormalRarity.position;
			}
			if (instance.activeChest == CWGachaController.ChestType.Premium && PremiumRarity != null)
			{
				bannerRarity.transform.position = PremiumRarity.position;
			}
		}
	}

	public void HideAnimations()
	{
		if (HideRarity != null)
		{
			if (BannerRarity1 != null)
			{
				BannerRarity1.transform.position = HideRarity.position;
			}
			if (BannerRarity2 != null)
			{
				BannerRarity2.transform.position = HideRarity.position;
			}
			if (BannerRarity3 != null)
			{
				BannerRarity3.transform.position = HideRarity.position;
			}
			if (BannerRarity4 != null)
			{
				BannerRarity4.transform.position = HideRarity.position;
			}
			if (BannerRarity5 != null)
			{
				BannerRarity5.transform.position = HideRarity.position;
			}
		}
	}
}
