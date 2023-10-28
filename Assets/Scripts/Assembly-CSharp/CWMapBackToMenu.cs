using UnityEngine;

public class CWMapBackToMenu : MonoBehaviour
{
	public bool toDeckManager;

	public GameObject backToMainMenuButton;

	public GameObject deckManagerButton;

	private void Start()
	{
	}

	private void OnClick()
	{
		PanelManager instance = PanelManager.GetInstance();
		MapControllerBase instance2 = MapControllerBase.GetInstance();
		instance2.HideMap();
		CameraManager.ActivateCamera(instance.uiCamera);
		CameraManager.ActivateCamera(instance.newCamera.GetComponent<Camera>());
		if (toDeckManager)
		{
			TutorialMonitor.Instance.ToDeckManager();
		}
	}

	private void Update()
	{
	}
}
