using UnityEngine;

public class CWResultBannerAnimation : MonoBehaviour
{
	public Animation BannerRarity1;

	public Animation BannerRarity2;

	public Animation BannerRarity3;

	public Animation BannerRarity4;

	public Animation BannerRarity5;

	public Transform ShowRarity;

	public Transform HideRarity;

	public float Delay;

	private float counter;

	private float delay;

	private bool waiting;

	private Animation bannerRarity;

	public UIButtonPlayAnimation AnimationTrigger;

	public GameObject ContinueButton;

	public AudioClip Rarity1_SFX;

	public AudioClip Rarity2_SFX;

	public AudioClip Rarity3_SFX;

	public AudioClip Rarity4_SFX;

	public AudioClip Rarity5_SFX;

	private AudioClip rarity_sfx;

	public UIButtonSound ButtonSound;

	private int cardRarity;

	private void OnEnable()
	{
	}

	public void SetCardData(CardForm cardForm)
	{
		if (cardForm != null)
		{
			cardRarity = cardForm.Rarity;
			switch (cardRarity)
			{
			case 1:
				bannerRarity = BannerRarity1;
				rarity_sfx = Rarity1_SFX;
				break;
			case 2:
				bannerRarity = BannerRarity2;
				rarity_sfx = Rarity2_SFX;
				break;
			case 3:
				bannerRarity = BannerRarity3;
				rarity_sfx = Rarity3_SFX;
				break;
			case 4:
				bannerRarity = BannerRarity4;
				rarity_sfx = Rarity4_SFX;
				break;
			case 5:
				bannerRarity = BannerRarity5;
				rarity_sfx = Rarity5_SFX;
				break;
			default:
				bannerRarity = BannerRarity1;
				rarity_sfx = Rarity1_SFX;
				break;
			}
		}
		else
		{
			bannerRarity = BannerRarity5;
			rarity_sfx = Rarity5_SFX;
		}
	}

	public void AnimateBannerWithData(CardForm cardForm)
	{
		SetCardData(cardForm);
		AnimateBanner();
	}

	public void AnimateBanner()
	{
		if (rarity_sfx != null && ButtonSound != null)
		{
			ButtonSound.audioClip = rarity_sfx;
			ButtonSound.SendMessage("OnClick");
		}
		delay = Delay;
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
		if (bannerRarity != null)
		{
			if (AnimationTrigger != null)
			{
				AnimationTrigger.target = bannerRarity;
				AnimationTrigger.SendMessage("OnClick");
			}
			if (ShowRarity != null)
			{
				bannerRarity.transform.position = ShowRarity.position;
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
