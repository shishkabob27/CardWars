using UnityEngine;

public class CWCachaFlashAnimation : MonoBehaviour
{
	public GameObject FadeInAnimationStart;
	public GameObject FadeOutAnimationStart;
	public GatchaRevealScript RevealScript;
	public bool RevealAfterFadeIn;
	public float RevealDelay;
	public TriggerVFX AnimTrigger;
	public GameObject VFX_RarityLow;
	public GameObject VFX_RarityMed;
	public GameObject VFX_RarityHigh;
	public GameObject BackShadow;
	public float RarityDelay_Low;
	public float RarityDelay_Med;
	public float RarityDelay_High;
	public Transform NormalRarity;
	public Transform PremiumRarity;
}
