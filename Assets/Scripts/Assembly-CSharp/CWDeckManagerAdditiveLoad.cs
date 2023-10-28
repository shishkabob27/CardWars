using System.Collections;
using UnityEngine;

public class CWDeckManagerAdditiveLoad : AsyncLoader
{
	public string deckManagerScene;

	private PanelManagerDeck pManagerDeck;

	private Camera uiCameraMap;

	private PanelManager pManager;

	public GameObject backToMenuPanel;

	public Transform menuCamPosition;

	public Transform menuCamTargetPosition;

	private static CWDeckManagerAdditiveLoad instance;

	private void Awake()
	{
		instance = this;
	}

	public static CWDeckManagerAdditiveLoad GetInstance()
	{
		return instance;
	}

	private void OnEnable()
	{
		pManagerDeck = PanelManagerDeck.GetInstance();
	}

	private IEnumerator Start()
	{
		yield return SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevelAdditiveAsync(deckManagerScene);
		CWMenuEnvironmentAdditiveLoad environmentLoader = CWMenuEnvironmentAdditiveLoad.Instance;
		if (environmentLoader != null)
		{
			while (!environmentLoader.IsReady)
			{
				yield return null;
			}
		}
		base.IsReady = true;
	}

	public void NavigateToDeckManager()
	{
		GlobalFlags globalFlags = GlobalFlags.Instance;
		pManager = PanelManager.GetInstance();
		if (globalFlags != null && globalFlags.ReturnToBuildDeck)
		{
			pManager.newCamera.transform.position = menuCamPosition.position;
			pManager.newCameraTarget.transform.position = menuCamTargetPosition.position;
			CWMenuCameraTarget component = pManager.newCameraTarget.GetComponent<CWMenuCameraTarget>();
			component.followFlag = true;
			globalFlags.ReturnToBuildDeck = false;
			BuildDeckNavigationController buildDeckNavigationController = BuildDeckNavigationController.GetInstance();
			if (buildDeckNavigationController != null)
			{
				buildDeckNavigationController.LaunchBuildDeck();
			}
		}
	}

	public void DeckManagerEnable()
	{
		DeckCameraSwitch(true);
	}

	private void DeckManagerDisable()
	{
		DeckCameraSwitch(false);
	}

	public void DeckCameraSwitch(bool enable)
	{
		if (pManager == null)
		{
			pManager = PanelManager.GetInstance();
		}
		if (pManagerDeck == null)
		{
			pManagerDeck = PanelManagerDeck.GetInstance();
		}
		CameraManager.SetCameraActive(pManagerDeck.uiCamera, enable);
		CameraManager.SetCameraActive(pManager.newCamera.GetComponent<Camera>(), !enable);
	}

	private void Update()
	{
	}
}
