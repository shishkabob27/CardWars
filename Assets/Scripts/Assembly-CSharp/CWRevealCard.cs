using UnityEngine;

public class CWRevealCard : MonoBehaviour
{
	public bool Reveal;
	public GameObject CardObj;
	public float Rotation;
	public bool Premium;
	public TweenTransform prizeTween;
	public GameObject ContinueButton;
	public CWResultBannerAnimation BannerAnims;
	public float LowDelay;
	public float MedDelay;
	public float HighDelay;
	public bool tweenPlayed;
}
