using UnityEngine;

public class CWBattleEndWinnerStats : MonoBehaviour
{
	public UILabel questIDLabel;
	public UILabel cardCountLabel;
	public UILabel earnedCoinLabel;
	public UILabel statCoinLabel;
	public UILabel earnedXPLabel;
	public UILabel currentXPLabel;
	public UILabel xpToNextLabel;
	public UISprite xpBar;
	public UILabel statCurrentXPLabel;
	public UILabel statXPToNextLabel;
	public UISprite statXPBar;
	public UILabel statRankLabel;
	public float DelayInterval;
	public GameObject[] TweenControllers;
	public GameObject lvlUpTween;
	public GameObject lvlUpDismissTween;
	public GameObject questConditionTween;
	public UISprite[] stars;
	public Transform targetTr;
	public GameObject starFX;
	public UILabel descOnPanel;
	public int currentHP;
	public int currentRank;
	public float XPBarInterval;
	public UILabel debugConterLabel;
	public UILabel debugXPAnimateFlag;
}
