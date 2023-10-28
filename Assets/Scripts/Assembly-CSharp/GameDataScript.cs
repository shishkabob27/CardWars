using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataScript : MonoBehaviour
{
	private VoiceoverScript Voiceover;

	public GameObject BackgroundBMO;

	public GameObject[] characterObjects;

	public GameObject[] characterPositions;

	public UIAtlas[] characterUiAtlas;

	public CWUpdatePlayerData[] playerData;

	public int FirstPlayer;

	public int P1_CoinsEarned;

	public int P2_CoinsEarned;

	public int ActivePlayer;

	public int Turn;

	public bool ShowAnims;

	public bool GameOver;

	public float Timer;

	public float Spin;

	private GameState GameInstance;

	public CardItem UnlockCard;

	private BattlePhaseManager phaseMgr;

	private CWCharacterAnimController charController;

	private static GameDataScript g_gamedata;

	public bool stopUpdateFlag;

	private void Awake()
	{
		g_gamedata = this;
	}

	public static GameDataScript GetInstance()
	{
		return g_gamedata;
	}

	private void Start()
	{
		charController = CWCharacterAnimController.GetInstance();
		Voiceover = VoiceoverScript.GetInstance();
		phaseMgr = BattlePhaseManager.GetInstance();
		GameInstance = GameState.Instance;
		GameInstance.GameData = this;
		UpdateText();
	}

	public void UpdateText()
	{
		for (int i = 0; i < 2; i++)
		{
			int health = GameInstance.GetHealth(i);
			if (health <= 0)
			{
				GameOver = true;
			}
		}
	}

	private void Update()
	{
		if (stopUpdateFlag)
		{
			return;
		}
		if (Input.GetKeyDown("1"))
		{
			GameInstance.SetHealth(PlayerType.Opponent, 0);
			UpdateText();
			Timer = 1f;
		}
		if (Input.GetKeyDown("2"))
		{
			GameInstance.SetHealth(PlayerType.User, 0);
			UpdateText();
			Timer = 1f;
		}
		if (Input.GetKeyDown("0"))
		{
			GameInstance.SetMagicPoints(PlayerType.User, 100);
			UpdateText();
		}
		if (!GameOver)
		{
			return;
		}
		Timer += Time.deltaTime;
		if (Timer > 1f && (phaseMgr.Phase == BattlePhase.P1Setup || phaseMgr.Phase == BattlePhase.P1Battle || phaseMgr.Phase == BattlePhase.P2Setup || phaseMgr.Phase == BattlePhase.P2Battle))
		{
			if (GameInstance.GetHealth(PlayerType.User) <= 0)
			{
				Voiceover.P2Wins();
			}
			else if (GameInstance.GetHealth(PlayerType.Opponent) <= 0)
			{
				Voiceover.P1Wins();
			}
			EndGame();
		}
	}

	private void EndGame()
	{
		phaseMgr.Phase = ((GameInstance.GetHealth(PlayerType.User) > 0) ? BattlePhase.Result_P2Defeated : BattlePhase.Result_P1Defeated);
	}

	public void UpdateCharacters()
	{
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		for (int i = 0; i < 2; i++)
		{
			CharacterData characterData = GameInstance.GetCharacter(i);
			GameObject gameObject = null;
			string id = ((i != 0) ? "Finn" : "Jake");
			if (characterData == null)
			{
				characterData = CharacterDataManager.Instance.GetCharacterData(id);
			}
			Transform tr = characterPositions[i].transform;
			gameObject = ((i != (int)PlayerType.User) ? SpawnCharacterObject(tr, characterData, characterData.Prefab, 0f - characterData.CharacterOffsetX, characterData.CharacterOffsetY) : SpawnCharacterObject(tr, characterData, characterData.Prefab, characterData.CharacterOffsetX, characterData.CharacterOffsetY));
			characterObjects[i] = gameObject.transform.Find(characterData.ObjectName).gameObject;
			characterUiAtlas[i] = LeaderManager.Instance.GetUiAtlas(characterData.PortraitAtlas);
			GameObject gameObject2 = null;
			if (characterData.UseChair)
			{
				gameObject2 = ((i != (int)PlayerType.User) ? SpawnEnvironmentObject("battle_chair", tr, characterData, activeQuest.ChairPrefab, 0f - characterData.ChairOffsetX, characterData.ChairOffsetY) : SpawnEnvironmentObject("battle_chair", tr, characterData, activeQuest.ChairPrefab, characterData.ChairOffsetX, characterData.ChairOffsetY));
				SetShadowOffset(gameObject2);
			}
			charController.playerCharacters.Add(characterObjects[i]);
			charController.playerData[i] = characterData;
			VOController component = gameObject.GetComponent<VOController>();
			if (component != null)
			{
				component.Owner = i;
			}
		}
		charController.SetupCharacters();
		CWBattleIntroController instance = CWBattleIntroController.GetInstance();
		if (instance != null)
		{
			instance.FlagCharactersLoaded();
		}
	}

	private void SetShadowOffset(GameObject chairObj)
	{
		Transform transform = chairObj.transform.Find("Chair_Shadow");
		if (transform != null)
		{
			transform.position = new Vector3(transform.position.x, -40f, 0f);
		}
	}

	private GameObject SpawnCharacterObject(Transform tr, CharacterData charData, string prefabName, float offsetX, float offsetY)
	{
		return SpawnGameObject(tr, charData, "Characters/" + prefabName, offsetX, offsetY);
	}

	private GameObject SpawnEnvironmentObject(string scheduleCategory, Transform tr, CharacterData charData, string prefabName, float offsetX, float offsetY)
	{
		if (GameState.Instance.ActiveQuest.IsQuestType("main"))
		{
			List<ScheduleData> itemsAvailableAndUnlocked = ScheduleDataManager.Instance.GetItemsAvailableAndUnlocked(scheduleCategory, TFUtils.ServerTime.Ticks);
			foreach (ScheduleData item in itemsAvailableAndUnlocked)
			{
				GameObject gameObject = SpawnGameObject(tr, charData, "Environment/" + item.ID, offsetX, offsetY);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return SpawnGameObject(tr, charData, "Environment/" + prefabName, offsetX, offsetY);
	}

	private GameObject SpawnGameObject(Transform tr, CharacterData charData, string prefabName, float offsetX, float offsetY)
	{
		GameObject gameObject = null;
		Vector3 position = new Vector3(tr.position.x + offsetX, tr.position.y + offsetY, 0f);
        //Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource(prefabName);
        GameObject original = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        if (original != null)
		{
			gameObject = Instantiate(original, position, tr.rotation) as GameObject;
			gameObject.transform.parent = tr;
		}
		return gameObject;
	}
}
