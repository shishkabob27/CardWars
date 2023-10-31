using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MapControllerBase))]
public class SideQuestController : MonoBehaviour
{
	public string SideQuestGroup;

	public GameObject HudCanvas;

	public GameObject PanelCanvas;

	public string SQPanelPrefabPath;

	protected GameObject SQPanelObj;

	protected SideQuestPanel SQPanel;

	public string SQHudPrefabPath;

	protected GameObject SQHudObj;

	protected SideQuestHud SQHud;

	private UGuiSpriteMap UIAtlas;

	private string UIAtlasName;

	private MapControllerBase mapController;

	private bool updating;

	public SideQuestData ActiveSideQuest { get; private set; }

	public bool IsPanelVisible { get; private set; }

	public bool IsHudVisible { get; private set; }

	private void Awake()
	{
		ActiveSideQuest = null;
		IsPanelVisible = false;
		IsHudVisible = false;
	}

	private void Start()
	{
		mapController = GetComponent<MapControllerBase>();
		if (mapController != null)
		{
			mapController.OnShowMap += OnShowMap;
			mapController.OnHideMap += OnHideMap;
			mapController.OnQuestCleared += OnMapQuestCleared;
		}
	}

	private void OnDestroy()
	{
		if (mapController != null)
		{
			mapController.OnShowMap -= OnShowMap;
			mapController.OnHideMap -= OnHideMap;
			mapController.OnQuestCleared -= OnMapQuestCleared;
		}
	}

	private SideQuestData DetermineActiveSideQuest(List<SideQuestData> quests)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		SideQuestData sideQuestData = null;
		for (int i = 0; i < quests.Count; i++)
		{
			bool flag = false;
			SideQuestData sideQuestData2 = quests[i];
			if (!sideQuestData2.IsPlayable || !(sideQuestData2.Group == SideQuestGroup))
			{
				continue;
			}
			switch (instance.GetSideQuestState(sideQuestData2))
			{
			case SideQuestProgress.SideQuestState.InProgress:
			case SideQuestProgress.SideQuestState.Expired:
			case SideQuestProgress.SideQuestState.Accomplished:
				sideQuestData = sideQuestData2;
				flag = true;
				break;
			case SideQuestProgress.SideQuestState.Inactive:
				if (sideQuestData == null)
				{
					sideQuestData = sideQuestData2;
				}
				break;
			case SideQuestProgress.SideQuestState.Pending:
				sideQuestData = sideQuestData2;
				break;
			}
			if (flag)
			{
				break;
			}
		}
		return sideQuestData;
	}

	public void UpdateActiveSideQuest()
	{
		if (GlobalFlags.Instance.InMPMode)
		{
			return;
		}
		ActiveSideQuest = SideQuestManager.Instance.GetActiveSideQuest(mapController.MapQuestType, SideQuestGroup);
		if (ActiveSideQuest == null)
		{
			string mapQuestType = mapController.MapQuestType;
			List<SideQuestData> sideQuests = SideQuestManager.Instance.GetSideQuests(mapQuestType);
			SideQuestData sideQuestData = DetermineActiveSideQuest(sideQuests);
			if (sideQuestData != null)
			{
				PlayerInfoScript instance = PlayerInfoScript.GetInstance();
				SideQuestProgress.SideQuestState sideQuestState = instance.GetSideQuestState(sideQuestData);
				if (sideQuestData.State == SideQuestProgress.SideQuestState.Inactive)
				{
					int wins = instance.GetQuestMatchStats(mapQuestType).Wins;
					int wins2 = instance.GetSideQuestMatchStats(mapQuestType).Wins;
					int num = wins - wins2;
					int sideQuestRequiredMatchLapse = instance.GetSideQuestRequiredMatchLapse(mapQuestType);
					if (num < sideQuestRequiredMatchLapse)
					{
						sideQuestData = null;
					}
				}
			}
			if (sideQuestData != null)
			{
				ActiveSideQuest = sideQuestData;
				SideQuestManager.Instance.ActivateSideQuest(sideQuestData);
			}
		}
		StartCoroutine(UpdateUI());
	}

	private SideQuestProgress.SideQuestState UpdateActiveSideQuestState()
	{
		if (ActiveSideQuest == null)
		{
			return SideQuestProgress.SideQuestState.Inactive;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		SideQuestProgress.SideQuestState sideQuestState = instance.GetSideQuestState(ActiveSideQuest);
		switch (sideQuestState)
		{
		case SideQuestProgress.SideQuestState.Inactive:
			sideQuestState = SideQuestManager.Instance.IntroduceSideQuest(ActiveSideQuest);
			break;
		case SideQuestProgress.SideQuestState.Pending:
			if (ActiveSideQuest.IsEventEnded)
			{
				OnFailSideQuest();
			}
			break;
		case SideQuestProgress.SideQuestState.InProgress:
			if (SideQuestManager.Instance.IsSideQuestAccomplished(ActiveSideQuest))
			{
				sideQuestState = SideQuestManager.Instance.AccomplishSideQuest(ActiveSideQuest);
			}
			else if (ActiveSideQuest.IsEventEnded)
			{
				sideQuestState = SideQuestManager.Instance.ExpireSideQuest(ActiveSideQuest);
			}
			break;
		}
		return sideQuestState;
	}

	private IEnumerator UpdateUI()
	{
		if (updating)
		{
			yield break;
		}
		updating = true;
		UpdateActiveSideQuestState();
		if (ActiveSideQuest != null)
		{
			if (ShouldInstantiateUI())
			{
				yield return StartCoroutine(InstantiateUI());
			}
			ShowHud();
			HidePanel();
		}
		else
		{
			DestroyUI();
		}
		updating = false;
	}

	private bool ShouldInstantiateUI()
	{
		if (SQHudObj == null || SQPanelObj == null)
		{
			return true;
		}
		if (ShouldLoadAtlas())
		{
			return true;
		}
		return false;
	}

	private IEnumerator InstantiateUI()
	{
		IsPanelVisible = false;
		IsHudVisible = false;
		if (ShouldLoadAtlas())
		{
			yield return StartCoroutine(LoadAtlas());
		}
		if (!(UIAtlas != null))
		{
			yield break;
		}
		if (SQHudObj == null)
		{
			GameObject prefab2 = Resources.Load(SQHudPrefabPath, typeof(GameObject)) as GameObject;
			SQHudObj = Instantiate(prefab2);
			SQHudObj.name = "SideQuest_Hud";
			SQHudObj.transform.SetParent(HudCanvas.transform, false);
			Button hudButton = SQHudObj.GetComponentInChildren<Button>();
			if (hudButton != null)
			{
				hudButton.onClick.AddListener(OnClickHUD);
			}
			SQHud = SQHudObj.GetComponent<SideQuestHud>();
			SQHud.SQController = this;
		}
		SQHud.UIAtlas = UIAtlas;
		if (SQPanelObj == null)
		{
			GameObject prefab = Resources.Load(SQPanelPrefabPath, typeof(GameObject)) as GameObject;
			SQPanelObj = Instantiate(prefab);
			SQPanelObj.name = "SideQuest_Panel";
			SQPanelObj.transform.SetParent(PanelCanvas.transform, false);
			SQPanel = SQPanelObj.GetComponent<SideQuestPanel>();
			SQPanel.SQController = this;
			SQPanel.panelButton.onClick.AddListener(OnClickPanel);
		}
		SQPanel.UIAtlas = UIAtlas;
	}

	private void DestroyUI()
	{
		if (SQHudObj != null)
		{
			Button componentInChildren = SQHudObj.GetComponentInChildren<Button>();
			if (componentInChildren != null)
			{
				componentInChildren.onClick.RemoveListener(OnClickHUD);
			}
			SQHud.UIAtlas = null;
			SQHud.SQController = null;
			SQHud = null;
			UnityEngine.Object.Destroy(SQHudObj);
			SQHudObj = null;
		}
		if (SQPanelObj != null)
		{
			SQPanel.UIAtlas = null;
			SQPanel.SQController = null;
			SQPanel = null;
			UnityEngine.Object.Destroy(SQPanelObj);
			SQPanelObj = null;
		}
		UIAtlas = null;
		UIAtlasName = string.Empty;
	}

	private void Update()
	{
		if (SQHud != null && SQPanel != null)
		{
			GameObject gameObject = GameObject.Find("F_QuestInfo");
			GameObject gameObject2 = GameObject.Find("G_MapHeartPanel");
			GameObject gameObject3 = GameObject.Find("H_NewRegion");
			GameObject gameObject4 = GameObject.Find("I_LeaderCardAcquire");
			GameObject gameObject5 = GameObject.Find("J_TooManyCards");
			GameObject gameObject6 = GameObject.Find("K_NewRecipe");
			GameObject gameObject7 = GameObject.Find("L_FreeGem");
			GameObject gameObject8 = GameObject.Find("M_Locked");
			GameObject gameObject9 = GameObject.Find("I_3_YouGotThis");
			GameObject gameObject10 = GameObject.Find("ElfistoPanel_Defeat");
			GameObject gameObject11 = GameObject.Find("NisAsyncPlayer(Clone)");
			if (gameObject != null || gameObject2 != null || gameObject3 != null || gameObject4 != null || gameObject5 != null || gameObject6 != null || gameObject7 != null || gameObject8 != null || gameObject9 != null || gameObject10 != null || gameObject11 != null)
			{
				HideHud();
				HidePanel();
			}
			else if (!IsHudVisible && !IsPanelVisible)
			{
				StartCoroutine(UpdateUI());
			}
		}
	}

	private void OnClickHUD()
	{
		if (IsHudVisible)
		{
			HideHud();
			ShowPanel();
		}
	}

	public void OnClickPanel()
	{
		if (!IsPanelVisible)
		{
			return;
		}
		HidePanel();
		SideQuestProgress activeSideQuestProgress = GetActiveSideQuestProgress();
		if (activeSideQuestProgress != null)
		{
			switch (activeSideQuestProgress.State)
			{
			case SideQuestProgress.SideQuestState.Pending:
				StartCoroutine(OnStartSideQuest());
				break;
			case SideQuestProgress.SideQuestState.InProgress:
				ShowHud();
				break;
			case SideQuestProgress.SideQuestState.Accomplished:
				StartCoroutine(OnCompleteSideQuest());
				break;
			case SideQuestProgress.SideQuestState.Expired:
				OnFailSideQuest();
				break;
			}
		}
	}

	private SideQuestProgress GetActiveSideQuestProgress()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		return instance.GetSideQuestProgress(ActiveSideQuest, true);
	}

	private void OnShowMap(object sender, EventArgs eventArgs)
	{
		UpdateActiveSideQuest();
	}

	private void OnHideMap(object sender, EventArgs eventArgs)
	{
		DestroyUI();
	}

	private void OnMapQuestCleared(object sender, QuestData qd)
	{
		SideQuestManager.Instance.UpdatePlayableSideQuests(qd, SideQuestGroup);
		UpdateActiveSideQuest();
	}

	private IEnumerator OnStartSideQuest()
	{
		MenuController menu = MenuController.GetInstance();
		if (menu != null && !string.IsNullOrEmpty(ActiveSideQuest.InitialCardID))
		{
			yield return StartCoroutine(menu.AwardCard(ActiveSideQuest.InitialCardID));
		}
		SideQuestManager.Instance.StartSideQuest(ActiveSideQuest);
		ShowHud();
	}

	private IEnumerator OnCompleteSideQuest()
	{
		MenuController menu = MenuController.GetInstance();
		if (!string.IsNullOrEmpty(ActiveSideQuest.InitialCardID))
		{
			PlayerInfoScript pInfo = PlayerInfoScript.GetInstance();
			pInfo.DeckManager.RemoveCard(ActiveSideQuest.InitialCardID);
		}
		if (menu != null && !string.IsNullOrEmpty(ActiveSideQuest.RewardID))
		{
			string rewardID = SideQuestManager.Instance.GetRewardCardID(ActiveSideQuest);
			yield return StartCoroutine(menu.AwardCard(rewardID));
		}
		SideQuestManager.Instance.CompleteSideQuest(ActiveSideQuest);
		UpdateActiveSideQuest();
	}

	private void OnFailSideQuest()
	{
		SideQuestManager.Instance.FailSideQuest(ActiveSideQuest);
		UpdateActiveSideQuest();
	}

	private bool ShouldLoadAtlas()
	{
		if (ActiveSideQuest != null && (UIAtlas == null || UIAtlasName != ActiveSideQuest.TextureAtlas))
		{
			return true;
		}
		return false;
	}

	public IEnumerator LoadAtlas()
	{
		if (ShouldLoadAtlas())
		{
			SLOTResoureRequest req = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResourceAsync(ActiveSideQuest.TextureAtlas, typeof(UGuiSpriteMap));
			yield return req.asyncOp;
			UIAtlas = (UGuiSpriteMap)req.asset;
			UIAtlasName = ActiveSideQuest.TextureAtlas;
		}
	}

	private void HideHud()
	{
		IsHudVisible = false;
		SQHud.Hide();
	}

	private void ShowHud()
	{
		if (!IsHudVisible)
		{
			IsHudVisible = true;
			SQHud.Show();
		}
	}

	private void HidePanel()
	{
		IsPanelVisible = false;
		SQPanel.Hide();
	}

	private void ShowPanel()
	{
		if (!IsPanelVisible)
		{
			IsPanelVisible = true;
			SQPanel.Show();
		}
	}
}
