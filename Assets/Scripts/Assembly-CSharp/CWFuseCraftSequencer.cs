using UnityEngine;

public class CWFuseCraftSequencer : MonoBehaviour
{
	public AudioClip craftStartSound;
	public AudioClip craftSound;
	public TweenAlpha blackPanelTween;
	public TweenAlpha fuseAnimPanel;
	public GameObject[] recipeCards;
	public Transform[] recipeCardsStartPos;
	public Transform craftAnimationTr;
	public float recipeFlyTime;
	public AudioClip recipeFlySound;
	public float burstFXStartDelay;
	public GameObject burstFX;
	public float whitePanelStartDelay;
	public TweenAlpha whitePanelTween;
	public AudioClip whiteFXSound;
	public GameObject cardBefore;
	public GameObject[] vfxRarity;
	public GameObject normalChestFX;
	public GameObject premiumChestFX;
	public GameObject[] bannerObjects;
	public AudioClip[] raritySounds;
	public GameObject resultCard;
	public GameObject resultCardObj;
	public float[] fxTimes;
	public float[] cardTimes;
	public GameObject tapAnywherePanel;
	public GameObject flyingCard;
	public float flyingTime;
	public Transform destTr;
	public AudioClip earnSound;
	public Transform bannerParentTr;
	public Transform effectParentTr;
}
