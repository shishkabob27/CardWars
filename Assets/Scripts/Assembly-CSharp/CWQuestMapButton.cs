using UnityEngine;

public class CWQuestMapButton : MonoBehaviour
{
	public string daggerAnimation = "BattleMap_Activation(copy)";

	public string idleAnimation = "BattleMap_Idle(copy)";

	public Animation battleMapAnimation;

	public GameObject enterMapEvents;

	public Camera mainMenuCamera;

	public MultiAnimationScript AnimationScripts;

	private bool daggerAnimationPlaying;

	private void OnClick()
	{
		if ((mainMenuCamera != null && (!mainMenuCamera.gameObject.activeInHierarchy || !mainMenuCamera.enabled)) || (AnimationScripts != null && AnimationScripts.IsPlayingStartAnimRevert()))
		{
			return;
		}
		GlobalFlags instance = GlobalFlags.Instance;
		instance.InMPMode = false;
		instance.BattleResult = null;
		if (daggerAnimation != null)
		{
			if (battleMapAnimation != null)
			{
				UICamera.useInputEnabler = true;
				battleMapAnimation.CrossFade(daggerAnimation);
				daggerAnimationPlaying = true;
			}
		}
		else
		{
			EnterMap();
		}
	}

	private void Update()
	{
		if (daggerAnimationPlaying && battleMapAnimation != null && !battleMapAnimation.isPlaying)
		{
			daggerAnimationPlaying = false;
			UICamera.useInputEnabler = false;
			EnterMap();
			battleMapAnimation.Play(idleAnimation);
		}
	}

	private void EnterMap()
	{
		CWMapController.Activate(false);
		if (enterMapEvents != null)
		{
			enterMapEvents.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}
}
