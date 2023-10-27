using UnityEngine;

public class CWQuestLoad : MonoBehaviour
{
	public UIButtonTween ShowStaminaError;
	public UIButtonTween ShowTooManyCardsError;
	public UIButtonTween ShowFCLeaderPopup;
	public AudioClip errorSound;
	public AudioClip okSound;
	public string questContext;
	public LeaderSelectController LeaderSelect;
}
