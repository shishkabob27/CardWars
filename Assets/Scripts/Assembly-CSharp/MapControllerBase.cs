using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class MapControllerBase : MonoBehaviour
{
	public delegate void QuestEventHandler(object sender, QuestData e);

	private const float SoundDelayInterval = 0.2f;

	private const float HIDDENROAD_ALPHA_SECS = 1f;

	public string dbMapJsonFileName = "db_Map.json";

	public GameObject QuestMapRoot;

	public string QuestMapPrefabPath;

	public MapInfo QuestMapInfo;

	public GameObject playerIcon;

	public Camera uiCameraMap;

	public GameObject vfx;

	public GameObject vfxPrefab;

	public GameObject vfxStar;

	public GameObject vfxStarPrefab;

	public GameObject bonusQuestPrefab;

	public AudioClip bonusQuestAlertSound;

	public UISprite regionSprite;

	public GameObject regionPanel;

	public AudioClip regionUnlockSound;

	public AudioClip regionUnlockQuestSound;

	public UISprite recipeSprite;

	public UILabel recipeLabel;

	public UILabel recipeTypeLabel;

	public UISprite leaderSprite;

	public UILabel leaderLabel;

	public UILabel leaderAbility;

	public GameObject heartEarnedPanel;

	public AudioClip heartEarnedSound;

	public GameObject leaderCardPanel;

	public AudioClip leaderAquiredSound;

	public GameObject recipeAquiredPanel;

	public AudioClip recipeAquiredSound;

	public GameObject gemEarnedPanel;

	public AudioClip gemEarnedSound;

	public GameObject endOfRoadPanel;

	public AudioClip endOfRoadSound;

	public AudioClip[] playerMoveSounds;

	public AudioClip starSound;

	public GameObject questPanel;

	protected PlayerInfoScript pInfo;

	protected GlobalFlags gflags;

	protected CWMapTap mapTap;

	public bool resumeFlag;

	protected QuestManager qMgr;

	private static WeakReference ms_activeMapController = new WeakReference(null);

	public string LoadingScreenTextureName;

	private WeakReference lastMusicSourceRef = new WeakReference(null);

	private bool initialized;

	private bool isRegionsValidated;

	public GameObject mainCamera
	{
		get
		{
			return (!(QuestMapInfo == null)) ? QuestMapInfo.MainCameraObj : null;
		}
	}

	public Camera mainCameraMap
	{
		get
		{
			return (!(QuestMapInfo == null)) ? QuestMapInfo.MainCamera : null;
		}
	}

	public Camera MPCameraMap
	{
		get
		{
			return (!(QuestMapInfo == null)) ? QuestMapInfo.MPCamera : null;
		}
	}

	public TBPan tbPanScript
	{
		get
		{
			return (!(QuestMapInfo == null)) ? QuestMapInfo.MainCameraPanScript : null;
		}
	}

	public string MapQuestType { get; protected set; }

	[method: MethodImpl(32)]
	public event EventHandler OnShowMap;

	[method: MethodImpl(32)]
	public event EventHandler OnHideMap;

	[method: MethodImpl(32)]
	public event QuestEventHandler OnQuestCleared;

	public virtual int GetSideQuestFirstQuestID()
	{
		return 0;
	}

	public virtual int GetSideQuestMatchlapse()
	{
		return 0;
	}

	public abstract bool IsCameraPosSaved();

	public abstract Vector3 GetSavedCameraPos();

	public virtual void SaveCameraPos()
	{
		if (mainCamera != null)
		{
			SaveCameraPos(mainCamera.transform.position, mainCamera.GetComponent<Camera>().orthographicSize);
		}
	}

	public abstract void SaveCameraPos(Vector3 camPos, float orthoSize);

	public abstract float GetSavedCameraOrthoSize();

	private void Awake()
	{
		gflags = GlobalFlags.Instance;
		if (pInfo == null)
		{
			pInfo = PlayerInfoScript.GetInstance();
		}
		pInfo = PlayerInfoScript.GetInstance();
		qMgr = QuestManager.Instance;
		HideMap();
	}

	private void OnDestroy()
	{
		MapControllerBase mapControllerBase = ms_activeMapController.Target as MapControllerBase;
		if (mapControllerBase == this)
		{
			ms_activeMapController.Target = null;
		}
	}

	protected static void SetActiveInstance(MapControllerBase instance, bool showMap)
	{
		MapControllerBase mapControllerBase = ms_activeMapController.Target as MapControllerBase;
		if (ms_activeMapController.IsAlive && mapControllerBase != instance)
		{
			mapControllerBase.HideMap();
		}
		ms_activeMapController.Target = instance;
		if (instance != null)
		{
			if (showMap)
			{
				instance.ShowMap();
			}
			else
			{
				instance.HideMap();
			}
		}
	}

	public static MapControllerBase GetInstance()
	{
		if (ms_activeMapController.IsAlive)
		{
			return ms_activeMapController.Target as MapControllerBase;
		}
		return null;
	}

	public GameObject FindDescendantGameObject(string goName)
	{
		if (base.gameObject != null && base.gameObject.transform != null)
		{
			Transform transform = base.gameObject.transform.FindDescendant(goName);
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private IEnumerator _refresh()
	{
		pInfo = PlayerInfoScript.GetInstance();
		qMgr = QuestManager.Instance;
		if (mapTap == null)
		{
			mapTap = UnityEngine.Object.FindObjectOfType(typeof(CWMapTap)) as CWMapTap;
		}
		if (gflags.InMPMode)
		{
			yield break;
		}
		if (!IsCameraPosSaved())
		{
			SaveCameraPos(tbPanScript.idealPos, 22f);
		}
		UpdateMapRegions();
		QuestData quest = pInfo.GetCurrentQuest(MapQuestType);
		if (pInfo.CurrentQuestType == "tcat" && pInfo.BonusQuests.ContainsKey(MapQuestType))
		{
			int qID = pInfo.BonusQuests[MapQuestType].ReplacedQuestID;
			QuestData rQuest = QuestManager.Instance.GetQuestByID(MapQuestType, qID);
			if (rQuest != null)
			{
				quest = rQuest;
			}
		}
		PlacePlayerIcon(quest);
		yield return 0;
		float duration = FocusCameraOnPlayer();
		yield return new WaitForSeconds(duration);
		if ((bool)mapTap)
		{
			mapTap.InvalidateQuests();
		}
		yield return StartCoroutine(QuestUnlockFlow());
	}

	public virtual void QuestMapRefresh()
	{
		StartCoroutine(_refresh());
	}

	protected virtual IEnumerator QuestUnlockFlow()
	{
		QuestData qd = pInfo.GetLastPlayedQuest();
		yield return StartCoroutine(QuestUnlockSequence(qd));
	}

	private IEnumerator ValidateRegions()
	{
		PlayerInfoScript pInfo = PlayerInfoScript.GetInstance();
		bool modified = false;
		for (int regionIndex = 0; regionIndex < QuestMapInfo.Regions.Count; regionIndex++)
		{
			MapRegionInfo region = QuestMapInfo.Regions[regionIndex];
			if (!region.IsLocked())
			{
				continue;
			}
			if (regionIndex == 0)
			{
				modified = true;
				region.Unlock();
				UpdateRegion(region);
				continue;
			}
			bool unlockRegion = false;
			foreach (QuestData qd in region.Quests)
			{
				if (qd.GetState() == QuestData.QuestState.PLAYABLE)
				{
					int progress = pInfo.GetQuestProgress(qd);
					if (progress > 0)
					{
						unlockRegion = false;
						modified = true;
						region.Unlock();
						UpdateRegion(region);
						break;
					}
					unlockRegion = true;
				}
			}
			if (unlockRegion)
			{
				modified = true;
				yield return StartCoroutine(RegionUnlockSequence(region));
			}
		}
		if (modified)
		{
			pInfo.Save();
		}
	}

	protected virtual IEnumerator QuestUnlockSequence(QuestData qd)
	{
		if (!isRegionsValidated)
		{
			isRegionsValidated = true;
			yield return StartCoroutine(ValidateRegions());
		}
		if (qd == null || !gflags.ReturnToMainMenu || !gflags.Cleared)
		{
			yield break;
		}
		UICamera.useInputEnabler = true;
		yield return StartCoroutine(QuestClearAction(qd));
		if (qd.StaminaAwarded != 0 && gflags.NewlyCleared)
		{
			yield return StartCoroutine(HeartEarningSequence());
		}
		if (qd.LeaderAwarded != string.Empty && gflags.NewlyCleared)
		{
			yield return StartCoroutine(LeaderCardSequence(qd));
		}
		CardForm cd = FusionManager.Instance.GetCardFormByQuestUnlock(qd.QuestID);
		if (cd != null && gflags.NewlyCleared)
		{
			yield return StartCoroutine(RecipeUnlockSequence(cd));
		}
		List<QuestData> unlockedQuests = GetQuestListToUnlock(qd);
		if (unlockedQuests.Count > 0)
		{
			for (int iQuest2 = 0; iQuest2 < unlockedQuests.Count; iQuest2++)
			{
				unlockedQuests[iQuest2].SetState(QuestData.QuestState.PLAYABLE);
			}
			for (int iQuest = 0; iQuest < unlockedQuests.Count; iQuest++)
			{
				int regionID = unlockedQuests[iQuest].RegionID;
				MapRegionInfo regionToUnlock = QuestMapInfo.GetRegionByID(regionID);
				if (regionToUnlock != null && regionToUnlock.IsLocked())
				{
					yield return StartCoroutine(RegionUnlockSequence(regionToUnlock));
				}
				else
				{
					yield return StartCoroutine(QuestNodeUnlockSequence(unlockedQuests[iQuest], true));
				}
			}
		}
		yield return StartCoroutine(QuestNodeAnimateHiddenPaths(qd));
		yield return StartCoroutine(UpdatePlayerPosition(qd));
		gflags.NewlyCleared = false;
		gflags.ReturnToMainMenu = false;
		gflags.Cleared = false;
		CWUpdatePlayerStats playerStats = CWUpdatePlayerStats.GetInstance();
		if (playerStats != null)
		{
			playerStats.holdUpdateFlag = false;
		}
		QuestEventHandler tmpEvent = this.OnQuestCleared;
		if (tmpEvent != null)
		{
			tmpEvent(this, qd);
		}
		UICamera.useInputEnabler = false;
	}

	public void UpdateQuestNodeStars(CWMapQuestInfoSet questInfoSet, QuestData.QuestState questState)
	{
		if (questInfoSet == null)
		{
			return;
		}
		QuestData questData = questInfoSet.questData;
		int questProgress = pInfo.GetQuestProgress(questData);
		if (questData.QuestType == MapQuestType && questProgress > 0 && questState == QuestData.QuestState.PLAYABLE)
		{
			for (int i = 0; i < questInfoSet.stars.Length; i++)
			{
				questInfoSet.stars[i].gameObject.SetActive(true);
				if (questProgress > i)
				{
					questInfoSet.stars[i].spriteName = "UI_Star_Full";
				}
				else
				{
					questInfoSet.stars[i].spriteName = "UI_Star_Empty";
				}
			}
		}
		else
		{
			for (int j = 0; j < questInfoSet.stars.Length; j++)
			{
				questInfoSet.stars[j].gameObject.SetActive(false);
			}
		}
	}

	public void UpdateQuestNodeOpponentIconAndFrame(CWMapQuestInfoSet questInfoSet, QuestData.QuestState questState)
	{
		if (questState == QuestData.QuestState.PLAYABLE)
		{
			questInfoSet.charIcon.enabled = true;
			questInfoSet.lockedFrameIcon.enabled = false;
			questInfoSet.unlockedFrameIcon.enabled = true;
			CharacterData characterData = CharacterDataManager.Instance.GetCharacterData(questInfoSet.questData.Opponent.ID.ToString());
			questInfoSet.charIcon.atlas = LeaderManager.Instance.GetUiAtlas(characterData.PortraitAtlas);
			string spriteName = characterData.PortraitSprite.Replace("Frame", "Round");
			questInfoSet.charIcon.spriteName = spriteName;
			SpriteSizeReset(questInfoSet.unlockedFrameIcon);
		}
		else
		{
			questInfoSet.charIcon.enabled = false;
			questInfoSet.lockedFrameIcon.enabled = true;
			questInfoSet.unlockedFrameIcon.enabled = false;
			SpriteSizeReset(questInfoSet.lockedFrameIcon);
		}
	}

	protected void SpriteSizeReset(UISprite sprite)
	{
		if (sprite.GetAtlasSprite() == null)
		{
			return;
		}
		Rect rect = sprite.GetAtlasSprite().outer;
		if (sprite.atlas != null && sprite.atlas.coordinates != 0)
		{
			Texture mainTexture = sprite.mainTexture;
			if (mainTexture != null)
			{
				rect = NGUIMath.ConvertToPixels(rect, (int)((float)mainTexture.width * sprite.atlas.pixelSize), (int)((float)mainTexture.height * sprite.atlas.pixelSize), true);
			}
		}
		float num = ((!KFFLODManager.IsLowEndDevice()) ? 1f : 2f);
		sprite.transform.localScale = new Vector3(rect.width * num, rect.height * num, 1f);
	}

	public virtual void ShowMap()
	{
		InstantiateMapPrefab();
		if (QuestMapRoot != null)
		{
			QuestMapRoot.SetActive(true);
		}
		if (GlobalFlags.Instance.InMPMode)
		{
			CameraManager.DeactivateCamera(mainCameraMap);
			CameraManager.ActivateCamera(MPCameraMap);
		}
		else
		{
			CameraManager.ActivateCamera(mainCameraMap);
			CameraManager.DeactivateCamera(MPCameraMap);
		}
		CameraManager.ActivateCamera(uiCameraMap);
		pInfo.SetCurrentMap(GetInstance().MapQuestType);
		pInfo.SelectDeckForMap();
		EventHandler onShowMap = this.OnShowMap;
		if (onShowMap != null)
		{
			onShowMap(this, new EventArgs());
		}
		StartMapMusic();
		QuestMapRefresh();
	}

	public virtual void HideMap()
	{
		EventHandler onHideMap = this.OnHideMap;
		if (onHideMap != null)
		{
			onHideMap(this, new EventArgs());
		}
		SaveCameraPos();
		if (QuestMapRoot != null)
		{
			QuestMapRoot.SetActive(false);
		}
		CameraManager.DeactivateCamera(uiCameraMap);
		CameraManager.DeactivateCamera(mainCameraMap);
		CameraManager.DeactivateCamera(MPCameraMap);
		pInfo.SetCurrentMap(null);
		StopMapMusic();
		ReleaseMapPrefab();
	}

	protected void StartMapMusic()
	{
		SLOTAudioManager instance = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		AudioSource audioSource = ((!(QuestMapInfo != null)) ? null : QuestMapInfo.MapMusic);
		if (audioSource != null && !audioSource.isPlaying)
		{
			lastMusicSourceRef.Target = instance.LastMusicAudioSource;
			instance.StopMusic(instance.LastMusicAudioSource);
			instance.PlayMusic(audioSource, audioSource.clip, 0f, 1f);
		}
	}

	protected void StopMapMusic()
	{
		SLOTAudioManager instance = SLOTGameSingleton<SLOTAudioManager>.GetInstance();
		AudioSource audioSource = ((!(QuestMapInfo != null)) ? null : QuestMapInfo.MapMusic);
		if (audioSource != null && audioSource.isPlaying)
		{
			instance.StopMusic(audioSource);
		}
		AudioSource audioSource2 = lastMusicSourceRef.Target as AudioSource;
		if (audioSource2 != null && !audioSource2.isPlaying)
		{
			instance.PlayMusic(audioSource2, audioSource2.clip, 0f, 1f);
		}
	}

	protected void ReleaseMapPrefab()
	{
		if (QuestMapRoot != null)
		{
			UnityEngine.Object.Destroy(QuestMapRoot);
		}
		QuestMapInfo = null;
		QuestMapRoot = null;
		initialized = false;
	}

	public GameObject CreateQuestMapNode(Transform rootTransform, QuestData quest)
	{
		GameObject questNodePrefab = QuestMapInfo.GetQuestNodePrefab(quest);
		GameObject gameObject = null;
		if (questNodePrefab != null)
		{
			gameObject = SLOTGame.InstantiateFX(questNodePrefab, rootTransform.position, rootTransform.rotation) as GameObject;
			gameObject.name = string.Format("{0}_{1}", rootTransform.name, gameObject.name);
		}
		return gameObject;
	}

	protected IEnumerator WaitForKeyInput()
	{
		resumeFlag = false;
		while (!resumeFlag)
		{
			yield return null;
		}
		yield return null;
	}

	protected IEnumerator ReachedEndOfTheRoad()
	{
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), leaderAquiredSound);
		endOfRoadPanel.SetActive(true);
		yield return StartCoroutine(WaitForKeyInput());
	}

	protected IEnumerator RecipeUnlockSequence(CardForm cd)
	{
		recipeSprite.spriteName = cd.SpriteName;
		recipeLabel.text = cd.Name;
		recipeTypeLabel.text = KFFLocalization.Get("!!" + cd.Type.ToString().ToUpper());
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), leaderAquiredSound);
		recipeAquiredPanel.SetActive(true);
		yield return StartCoroutine(WaitForKeyInput());
	}

	protected IEnumerator HeartEarningSequence()
	{
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), heartEarnedSound);
		heartEarnedPanel.SetActive(true);
		yield return StartCoroutine(WaitForKeyInput());
	}

	protected IEnumerator LeaderCardSequence(QuestData qd)
	{
		LeaderForm leader = LeaderManager.Instance.leaderForms[qd.LeaderAwarded];
		SQUtils.SetIcon(leaderSprite, leader.IconAtlas, leader.SpriteName);
		leaderLabel.text = leader.Name;
		leaderAbility.text = leader.Desc.Replace("<val1>", leader.BaseVal1.ToString()).Replace("<val2>", leader.BaseVal2.ToString());
		SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), leaderAquiredSound);
		leaderCardPanel.SetActive(true);
		yield return StartCoroutine(WaitForKeyInput());
	}

	private void QuestClear(QuestData qd)
	{
		GameObject questMapNode = GetQuestMapNode(qd);
		if (!(questMapNode == null) && !(questMapNode.transform == null))
		{
			Transform transform = questMapNode.transform;
			CWiTweenTrigger componentInChildren = transform.GetComponentInChildren<CWiTweenTrigger>();
			if (componentInChildren != null)
			{
				componentInChildren.TriggerTweens("Pressed");
			}
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(playerIcon.GetComponent<AudioSource>(), playerMoveSounds[0], true, false, SLOTAudioManager.AudioType.SFX);
			vfxPrefab.SetActive(false);
		}
	}

	protected IEnumerator QuestClearAction(QuestData qd)
	{
		if (gflags.Cleared)
		{
			QuestClear(qd);
		}
		GameObject currentNode = GetQuestMapNode(qd);
		UISprite sp2 = null;
		if (currentNode != null)
		{
			CWMapQuestInfoSet qInfoSet = currentNode.GetComponentInChildren<CWMapQuestInfoSet>();
			for (int i = 1; i <= qInfoSet.stars.Length; i++)
			{
				sp2 = qInfoSet.stars[i - 1];
				sp2.gameObject.SetActive(true);
				if (i <= gflags.lastQuestConditionStatus)
				{
					sp2.spriteName = "UI_Star_Full";
				}
				else
				{
					sp2.spriteName = "UI_Star_Empty";
				}
			}
			yield return new WaitForSeconds(0.5f);
			for (int j = 1; j <= qInfoSet.stars.Length; j++)
			{
				sp2 = qInfoSet.stars[j - 1];
				if (j > gflags.lastQuestConditionStatus && j <= pInfo.GetQuestProgress(qd))
				{
					sp2.spriteName = "UI_Star_Full";
					iTweenEvent tweenEvent = iTweenEvent.GetEvent(sp2.gameObject, "Completed");
					if (tweenEvent != null)
					{
						tweenEvent.Play();
					}
					SLOTGame.InstantiateFX(position: new Vector3(sp2.transform.position.x, 15f, sp2.transform.position.z), original: vfxStar, rotation: Quaternion.identity);
					SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), starSound);
					yield return new WaitForSeconds(1f);
				}
			}
		}
		if (gflags.lastQuestConditionStatus == 2 && pInfo.GetQuestProgress(qd) == 3)
		{
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), gemEarnedSound);
			gemEarnedPanel.SetActive(true);
			yield return StartCoroutine(WaitForKeyInput());
		}
	}

	public virtual GameObject GetQuestMapNode(QuestData quest)
	{
		if (QuestMapInfo != null)
		{
			return QuestMapInfo.GetQuestMapNode(quest);
		}
		return null;
	}

	protected void InstantiateMapPrefab()
	{
		if (QuestMapRoot == null)
		{
            //GameObject original = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(QuestMapPrefabPath) as GameObject;
            GameObject original = Instantiate(Resources.Load(QuestMapPrefabPath, typeof(GameObject))) as GameObject;
			GameObject gameObject = original;
			if (gameObject != null)
			{
				gameObject.transform.SetParent(base.gameObject.transform.transform, false);
				QuestMapInfo = gameObject.GetComponent<MapInfo>();
				QuestMapInfo.InitializeMapData(MapQuestType);
				QuestMapRoot = gameObject;
				InitializeMapAndQuestData();
			}
			Resources.UnloadUnusedAssets();
		}
	}

	public void InitializeMapAndQuestData()
	{
		for (int i = 0; i < QuestMapInfo.Regions.Count; i++)
		{
			MapRegionInfo mapRegionInfo = QuestMapInfo.Regions[i];
			mapRegionInfo.ClearQuests();
			for (int j = mapRegionInfo.FirstQuestIndex; j < mapRegionInfo.FirstQuestIndex + mapRegionInfo.NumQuests; j++)
			{
				QuestData questByIndex = qMgr.GetQuestByIndex(MapQuestType, j);
				if (questByIndex != null)
				{
					mapRegionInfo.AddQuest(questByIndex);
					questByIndex.RegionID = mapRegionInfo.RegionID;
				}
			}
		}
		InitializeHiddenPaths();
	}

	protected virtual void InitializeHiddenPaths()
	{
		int currentQuestID = pInfo.GetCurrentQuestID();
		foreach (MapRegionInfo region in QuestMapInfo.Regions)
		{
			if (region.GetState() != MapRegionInfo.RegionState.PLAYABLE)
			{
				continue;
			}
			foreach (QuestData quest in region.Quests)
			{
				if (quest.iQuestID == currentQuestID && gflags.ReturnToMainMenu && gflags.Cleared)
				{
					continue;
				}
				int questProgress = pInfo.GetQuestProgress(quest);
				for (int num = questProgress - 1; num >= 0; num--)
				{
					foreach (string item in quest.UnlockPaths[num])
					{
						HiddenPathShow(item);
					}
				}
			}
		}
	}

	protected IEnumerator RegionUnlockSequence(MapRegionInfo regionToUnlock)
	{
		if (!regionToUnlock.IsLocked())
		{
			yield break;
		}
		regionToUnlock.Unlock();
		yield return null;
		mainCamera.GetComponent<Camera>().orthographicSize = 30f;
		Vector3 targetPos = QuestMapInfo.GetRegionCameraPos(regionToUnlock);
		float duration2 = TweenCameraOnFocus(targetPos, "ToRegion", true, "RegionUnlockSequence");
		GameObject bannerObj = null;
		if (!string.IsNullOrEmpty(regionToUnlock.RegionSpriteName))
		{
			bannerObj = SpawnRegionBanner(regionToUnlock.RegionSpriteName, regionPanel.transform);
			regionPanel.SetActive(true);
			TweenPosition[] tweens = regionPanel.GetComponents<TweenPosition>();
			TweenPosition[] array = tweens;
			foreach (TweenPosition tw in array)
			{
				tw.Reset();
				tw.Play(true);
			}
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), regionUnlockSound);
		}
		UpdateRegionRoadTexture(regionToUnlock, MapRegionInfo.RegionState.PLAYABLE);
		duration2 = Mathf.Max(duration2, 2f);
		yield return new WaitForSeconds(duration2);
		yield return StartCoroutine(UnlockRegionNodes(regionToUnlock, true));
		if (bannerObj != null)
		{
			UnityEngine.Object.DestroyImmediate(bannerObj);
		}
		regionPanel.SetActive(false);
		for (int iQuest = 0; iQuest < regionToUnlock.Quests.Count; iQuest++)
		{
			QuestData qd = regionToUnlock.Quests[iQuest];
			if (qd.GetState() == QuestData.QuestState.PLAYABLE)
			{
				yield return StartCoroutine(QuestNodeUnlockSequence(qd, true));
			}
		}
	}

	private GameObject SpawnRegionBanner(string bannerName, Transform parentTr)
	{
		UnityEngine.Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Props/" + bannerName);
		GameObject gameObject = null;
		if (@object != null)
		{
			gameObject = SLOTGame.InstantiateFX(@object, parentTr.position, parentTr.rotation) as GameObject;
			gameObject.transform.parent = parentTr;
			gameObject.transform.localScale = Vector3.one;
		}
		return gameObject;
	}

	public void UpdateRegionRoadTexture(MapRegionInfo region, MapRegionInfo.RegionState regionState)
	{
		GameObject gameObject = FindDescendantGameObject(region.RoadMeshName);
		if (gameObject != null)
		{
			Texture texture = null;
			texture = ((regionState != MapRegionInfo.RegionState.PLAYABLE) ? QuestMapInfo.LockedRoadTexture : QuestMapInfo.RegionRoadTextures[region.RegionIndex]);
			gameObject.GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	private IEnumerator UnlockRegionNodes(MapRegionInfo region, bool showQuestAsLocked)
	{
		for (int i = 0; i < region.Quests.Count; i++)
		{
			QuestData quest = region.Quests[i];
			QuestData.QuestState questState = ((!showQuestAsLocked) ? quest.GetState() : QuestData.QuestState.LOCKED);
			UpdateQuestNode(quest, questState, MapRegionInfo.RegionState.PLAYABLE);
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlaySound(GetComponent<AudioSource>(), regionUnlockQuestSound, true, false, SLOTAudioManager.AudioType.SFX);
			yield return new WaitForSeconds(0.2f);
		}
	}

	public void UpdateMapRegions()
	{
		for (int i = 0; i < QuestMapInfo.Regions.Count; i++)
		{
			MapRegionInfo region = QuestMapInfo.Regions[i];
			UpdateRegion(region);
		}
	}

	protected void UpdateRegion(MapRegionInfo region)
	{
		MapRegionInfo.RegionState regionState = ((!region.IsLocked()) ? MapRegionInfo.RegionState.PLAYABLE : MapRegionInfo.RegionState.HIDDEN);
		UpdateRegionRoadTexture(region, regionState);
		UpdateRegionNodes(region, regionState);
	}

	protected void UpdateRegionNodes(MapRegionInfo region, MapRegionInfo.RegionState regionState)
	{
		for (int i = 0; i < region.Quests.Count; i++)
		{
			QuestData questData = region.Quests[i];
			UpdateQuestNode(questData, questData.GetState(), regionState);
		}
	}

	public void UpdateQuestNode(QuestData quest, QuestData.QuestState questState, MapRegionInfo.RegionState regionState)
	{
		GameObject questMapNode = QuestMapInfo.GetQuestMapNode(quest);
		if (questMapNode == null || questMapNode.transform == null)
		{
			return;
		}
		bool flag = false;
		if (regionState == MapRegionInfo.RegionState.HIDDEN)
		{
			MapRegionInfo regionByID = QuestMapInfo.GetRegionByID(quest.RegionID);
			if (regionByID != null && regionByID.NodesAlwaysVisible)
			{
				questState = QuestData.QuestState.LOCKED;
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		Transform transform = questMapNode.transform;
		if (flag)
		{
			QuestData questData = quest;
			GameObject gameObject = null;
			CWMapQuestInfoSet cWMapQuestInfoSet = null;
			if (pInfo.BonusQuests.ContainsKey(MapQuestType) && pInfo.BonusQuests[MapQuestType].ReplacedQuestID == quest.iQuestID && pInfo.BonusQuests[MapQuestType].ActiveQuest != null)
			{
				questData = pInfo.BonusQuests[MapQuestType].ActiveQuest.ShallowClone();
				questData.SetState(questState);
			}
			if (transform.childCount != 0)
			{
				gameObject = transform.GetChild(0).gameObject;
				cWMapQuestInfoSet = gameObject.GetComponentInChildren<CWMapQuestInfoSet>();
				if (cWMapQuestInfoSet.questData.iQuestID != questData.iQuestID)
				{
					gameObject.SetActive(false);
					gameObject.transform.parent = transform;
					gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
					gameObject.transform.localPosition = new Vector3(10000f, 10000f, 10000f);
					UnityEngine.Object.Destroy(gameObject);
					gameObject = null;
				}
			}
			if (gameObject == null)
			{
				gameObject = CreateQuestMapNode(transform, questData);
			}
			if (gameObject != null)
			{
				gameObject.transform.parent = transform;
				gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
				gameObject.transform.localPosition = new Vector3(0f, 0f, 3f);
				UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
				componentInChildren.enabled = questState == QuestData.QuestState.PLAYABLE;
				componentInChildren.text = questData.QuestLabel;
				BoxCollider componentInChildren2 = gameObject.GetComponentInChildren<BoxCollider>();
				componentInChildren2.enabled = questState == QuestData.QuestState.PLAYABLE;
				cWMapQuestInfoSet = gameObject.GetComponentInChildren<CWMapQuestInfoSet>();
				cWMapQuestInfoSet.questData = questData;
				UpdateQuestNodeStars(cWMapQuestInfoSet, questState);
				UpdateQuestNodeOpponentIconAndFrame(cWMapQuestInfoSet, questState);
				if (questState == QuestData.QuestState.PLAYABLE)
				{
					UIButtonTween[] componentsInChildren = gameObject.GetComponentsInChildren<UIButtonTween>();
					componentsInChildren[0].tweenTarget = questPanel;
				}
			}
		}
		else if (transform.childCount != 0)
		{
			UnityEngine.Object.Destroy(transform.GetChild(0).gameObject);
		}
		if ((bool)mapTap)
		{
			mapTap.InvalidateQuests();
		}
	}

	private List<QuestData> GetQuestListToUnlock(QuestData qd)
	{
		List<QuestData> list = new List<QuestData>();
		int questProgress = pInfo.GetQuestProgress(qd);
		for (int num = questProgress - 1; num >= 0; num--)
		{
			foreach (int item in qd.UnlockQuestIDs[num])
			{
				QuestData quest = QuestManager.Instance.GetQuest(item);
				if (quest.GetState() != QuestData.QuestState.PLAYABLE)
				{
					list.Add(quest);
				}
			}
		}
		return list;
	}

	protected IEnumerator QuestNodeUnlockSequence(QuestData qd, bool focusOnNode)
	{
		if (qd == null)
		{
			yield break;
		}
		GameObject qNode = QuestMapInfo.GetQuestMapNode(qd);
		if (qNode != null)
		{
			if (focusOnNode)
			{
				float duration = TweenCameraOnFocus(qNode.transform.position, "ToRegion", true, "QuestNodeUnlockSequence");
				yield return new WaitForSeconds(duration);
			}
			else
			{
				UpdateQuestNode(qd, QuestData.QuestState.PLAYABLE, MapRegionInfo.RegionState.PLAYABLE);
			}
			CWiTweenTrigger tweenTrigger = qNode.GetComponentInChildren<CWiTweenTrigger>();
			if (tweenTrigger != null)
			{
				tweenTrigger.TriggerTweens("Spin");
				yield return new WaitForSeconds(0.5f);
				UpdateQuestNode(qd, QuestData.QuestState.PLAYABLE, MapRegionInfo.RegionState.PLAYABLE);
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				UpdateQuestNode(qd, QuestData.QuestState.PLAYABLE, MapRegionInfo.RegionState.PLAYABLE);
			}
		}
	}

	protected IEnumerator QuestNodeAnimateHiddenPaths(QuestData qd)
	{
		int qProgress = pInfo.GetQuestProgress(qd);
		for (int i = qProgress - 1; i >= 0; i--)
		{
			foreach (string path in qd.UnlockPaths[i])
			{
				Singleton<AnalyticsManager>.Instance.LogPathUnlocked(qd, path);
				yield return StartCoroutine(HiddenPathAnimateVisible(path));
			}
		}
	}

	protected GameObject HiddenPathShow(string pathName)
	{
		GameObject hiddenPath = QuestMapInfo.GetHiddenPath(pathName);
		if (hiddenPath != null && !hiddenPath.activeSelf)
		{
			hiddenPath.SetActive(true);
		}
		return hiddenPath;
	}

	protected IEnumerator HiddenPathAnimateVisible(string pathName)
	{
		GameObject pathObj = QuestMapInfo.GetHiddenPath(pathName);
		if (pathObj != null && !pathObj.activeSelf)
		{
			pathObj.SetActive(true);
			Vector3 oldCamPos = mainCamera.transform.position;
			float duration2 = TweenCameraOnFocus(pathObj.transform.position, "ToRegion", true, "Show - HiddenPathAnimateVisible");
			iTween.FadeFrom(pathObj, iTween.Hash("alpha", 0f, "delay", duration2, "time", 1f));
			yield return new WaitForSeconds(duration2);
			yield return new WaitForSeconds(1f);
			duration2 = TweenCameraOnFocus(oldCamPos, "ToRegion", true, "Return - HiddenPathAnimateVisible");
			yield return new WaitForSeconds(duration2);
		}
	}

	protected virtual IEnumerator UpdatePlayerPosition(QuestData qd)
	{
		yield return null;
	}

	protected IEnumerator MovePlayerIcon(QuestData fromQd, QuestData toQd)
	{
		GameObject fromQNode = GetQuestMapNode(fromQd);
		GameObject toQNode = GetQuestMapNode(toQd);
		if (fromQNode != null && fromQNode.transform != null && toQNode != null && toQNode.transform != null)
		{
			Transform startPos = fromQNode.transform;
			Transform endPos = toQNode.transform;
			TweenTransform tween = playerIcon.GetComponent<TweenTransform>();
			tween.from = startPos;
			tween.to = endPos;
			tween.Reset();
			tween.Play(true);
			TweenCameraOnFocus(endPos.position, "Walk", true, "MovePlayerIcon");
			SLOTGameSingleton<SLOTAudioManager>.GetInstance().PlayOneShot(GetComponent<AudioSource>(), playerMoveSounds[1]);
			yield return new WaitForSeconds(1f);
		}
	}

	protected void PlacePlayerIcon(QuestData quest)
	{
		GameObject questMapNode = QuestMapInfo.GetQuestMapNode(quest);
		if (questMapNode == null || questMapNode.transform == null)
		{
			return;
		}
		playerIcon.transform.position = questMapNode.transform.position;
		if (vfxPrefab == null)
		{
			vfxPrefab = SLOTGame.InstantiateFX(vfx) as GameObject;
			if (QuestMapRoot != null)
			{
				vfxPrefab.transform.parent = QuestMapRoot.transform;
			}
		}
		vfxPrefab.SetActive(true);
		vfxPrefab.transform.position = new Vector3(questMapNode.transform.position.x, questMapNode.transform.position.y + 3f, questMapNode.transform.position.z);
	}

	protected virtual float FocusCameraOnPlayer()
	{
		float orthographicSize = 22f;
		Vector3 targetPos;
		if (!IsCameraPosSaved() || pInfo.GetQuestProgress(QuestManager.Instance.GetFirstQuest(MapQuestType)) < 1)
		{
			targetPos = playerIcon.transform.position;
		}
		else
		{
			targetPos = GetSavedCameraPos();
			orthographicSize = GetSavedCameraOrthoSize();
		}
		if (gflags.NewlyCleared)
		{
			orthographicSize = 22f;
		}
		mainCamera.GetComponent<Camera>().orthographicSize = orthographicSize;
		return TweenCameraOnFocus(targetPos, "ToRegion", false, "FocusCameraOnPlayer");
	}

	protected virtual Vector3 GetCameraPosForTarget(Vector3 targetPos)
	{
		BoxCollider mainCameraCollider = QuestMapInfo.MainCameraCollider;
		Vector3 min = mainCameraCollider.bounds.min;
		Vector3 max = mainCameraCollider.bounds.max;
		float x = Mathf.Clamp(targetPos.x, min.x, max.x);
		float z = Mathf.Clamp(targetPos.z, min.z, max.z);
		return new Vector3(x, tbPanScript.idealPos.y, z);
	}

	protected float TweenCameraOnFocus(Vector3 targetPos, string animName, bool useTween, string debugMsg = "")
	{
		float result = 0f;
		Vector3 cameraPosForTarget = GetCameraPosForTarget(targetPos);
		QuestMapInfo.RegionCameraPos[0].transform.position = cameraPosForTarget;
		if (mainCamera.activeSelf)
		{
			if (!useTween)
			{
				mainCamera.transform.position = cameraPosForTarget;
			}
			else
			{
				iTweenEvent @event = iTweenEvent.GetEvent(mainCamera, animName);
				if (@event != null)
				{
					@event.Play();
					if (@event.Values.ContainsKey("time"))
					{
						result = Convert.ToSingle(@event.Values["time"]);
					}
					if (@event.Values.ContainsKey("speed"))
					{
						float num = Convert.ToSingle(@event.Values["speed"]);
						float num2 = Vector3.Distance(QuestMapInfo.MainCameraCollider.transform.position, cameraPosForTarget);
						result = num2 / num;
					}
				}
			}
		}
		tbPanScript.idealPos = cameraPosForTarget;
		return result;
	}
}
