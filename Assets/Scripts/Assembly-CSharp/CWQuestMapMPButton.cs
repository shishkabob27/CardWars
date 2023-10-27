using Multiplayer;
using UnityEngine;

public class CWQuestMapMPButton : AsyncData<MultiplayerData>
{
	public string daggerAnimation;
	public string idleAnimation;
	public Animation battleMapAnimation;
	public GameObject enterMapEvents;
	public Camera mainMenuCamera;
	public MultiAnimationScript AnimationScripts;
	public UIButtonTween ShowEnterMPNameUI;
	public UIButtonTween HideEnterMPNameUI;
	public UIButtonTween HideBottonInfo;
	public UIButtonTween NotEnoughMoney;
	public UIButtonTween UnderMaintenance;
	public UIButtonTween LoadingActivityShow;
	public UIButtonTween LoadingActivityHide;
	public UIButtonTween ConnectionFailedShow;
	public UIButtonTween ConnectionFailedHide;
}
