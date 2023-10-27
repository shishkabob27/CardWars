using UnityEngine;

public class DailyGiftController : MonoBehaviour
{
	public GameObject[] Wheel;
	public UIButtonTween ShowGiftTween;
	public UIButtonTween AreYouSureTween;
	public UIButtonTween NotEnoughGemsTween;
	public GameObject RefreshTimeBox;
	public UILabel RefreshTime;
	public UILabel GiftLabel;
	public UISprite GiftIcon;
	public UILabel ButtonText;
	public UILabel RetryCost;
	public AudioClip WheelClip;
	public AudioClip RewardClip;
	public float WheelClipPitchRange;
	public float startSpeed;
	public float endSpeed;
	public float speedDuration;
	public int numCells;
	public GameObject CardObj;
	public GameObject ActivateButton;
}
