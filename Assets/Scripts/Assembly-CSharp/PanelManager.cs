using UnityEngine;

public class PanelManager : MonoBehaviour
{
	public GameObject mainMenu;

	public GameObject[] menuButtons;

	public GameObject[] modelButtons;

	public GameObject[] subMenuPanels;

	public Camera uiCamera;

	public Camera mainCamera;

	public Camera leftCamera;

	public GameObject newCamera;

	public GameObject newCameraTarget;

	public GameObject startButton;

	public GameObject battleButton;

	private static PanelManager g_panelManager;

	private void Awake()
	{
		g_panelManager = this;
	}

	public static PanelManager GetInstance()
	{
		return g_panelManager;
	}

	private void Start()
	{
	}
}
