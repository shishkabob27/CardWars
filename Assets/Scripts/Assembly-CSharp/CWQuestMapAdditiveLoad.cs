using System;
using System.Collections;
using UnityEngine;

public class CWQuestMapAdditiveLoad : AsyncLoader
{
	public string questMapScene;

	public string fcQuestMapScene;

	public GameObject mapBoardButton;

	public GameObject playerStats;

	public Transform mapCamPosition;

	public Transform mapCamTargetPosition;

	private CWPlayerMoveController pController;

	private void OnEnable()
	{
	}

	private IEnumerator Start()
	{
		yield return SLOTGameSingleton<SLOTSceneManager>.GetInstance().LoadLevelAdditiveAsync(questMapScene);
		yield return null;
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

	public void NavigateToMapScene()
	{
		SetupCameraPos();
		NGUITools.SetActive(mapBoardButton.transform.parent.gameObject, true);
		mapBoardButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		playerStats.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
	}

	private IEnumerator ShowDungeonExtras()
	{
		MenuController menuController = MenuController.GetInstance();
		if (null != menuController && GlobalFlags.Instance.BattleResult != null)
		{
			menuController.SwitchDungeon();
			yield return new WaitForSeconds(0.5f);
			menuController.SwitchToDungeonExtras();
		}
	}

	private void SetupCameraPos()
	{
		PanelManager instance = PanelManager.GetInstance();
		instance.newCamera.transform.position = mapCamPosition.position;
		instance.newCameraTarget.transform.position = mapCamTargetPosition.position;
		CWMenuCameraTarget component = instance.newCameraTarget.GetComponent<CWMenuCameraTarget>();
		component.followFlag = true;
	}

	public void OnClick()
	{
		MapControllerBase instance = MapControllerBase.GetInstance();
		PanelManager instance2 = PanelManager.GetInstance();
		try
		{
			instance.ShowMap();
			CameraManager.DeactivateCamera(instance2.newCamera.GetComponent<Camera>());
		}
		catch (NullReferenceException)
		{
			if (instance2 == null)
			{
			}
			if (instance == null)
			{
			}
			if (instance2 != null && !(instance2.newCamera == null))
			{
			}
		}
		CWMPMapController instance3 = CWMPMapController.GetInstance();
		if (instance3 != null)
		{
			instance3.MPMapRefresh();
		}
	}
}
