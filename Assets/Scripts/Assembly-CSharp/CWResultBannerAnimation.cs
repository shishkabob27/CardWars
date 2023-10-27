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
	public UIButtonPlayAnimation AnimationTrigger;
	public GameObject ContinueButton;
	public AudioClip Rarity1_SFX;
	public AudioClip Rarity2_SFX;
	public AudioClip Rarity3_SFX;
	public AudioClip Rarity4_SFX;
	public AudioClip Rarity5_SFX;
	public UIButtonSound ButtonSound;
}
