using UnityEngine;

public class CWiTweenCamTrigger : MonoBehaviour
{
	public GameObject gameCamera;

	public GameObject gameCameraTarget;

	public string tweenName;

	public bool followFlag = true;

	private CWiTweenVantageCam vantageCam;

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		vantageCam = CWiTweenVantageCam.GetInstance();
		if (!(gameCamera != null) || !(gameCameraTarget != null))
		{
			PanelManager instance = PanelManager.GetInstance();
			PanelManagerBattle instance2 = PanelManagerBattle.GetInstance();
			if (instance != null)
			{
				gameCamera = instance.newCamera;
				gameCameraTarget = instance.newCameraTarget;
			}
			else
			{
				gameCamera = instance2.newCamera;
				gameCameraTarget = instance2.newCameraTarget;
			}
		}
	}

	private void OnClick()
	{
		if (base.enabled)
		{
			PlayCam();
			if (vantageCam != null)
			{
				vantageCam.theLastTweenEvent = tweenName;
			}
		}
	}

	public void PlayCam()
	{
		Initialize();
		if (gameCamera != null && tweenName != string.Empty)
		{
			iTweenEvent @event = iTweenEvent.GetEvent(gameCamera, tweenName);
			if (@event != null)
			{
				@event.Play();
			}
		}
		if (!(gameCameraTarget != null))
		{
			return;
		}
		gameCameraTarget.GetComponent<CWMenuCameraTarget>().followFlag = followFlag;
		if (tweenName != string.Empty)
		{
			iTweenEvent event2 = iTweenEvent.GetEvent(gameCameraTarget, tweenName);
			if (event2 != null)
			{
				event2.Play();
			}
		}
	}
}
