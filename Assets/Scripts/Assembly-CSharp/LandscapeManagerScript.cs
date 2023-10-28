using UnityEngine;

public class LandscapeManagerScript : MonoBehaviour
{
	public struct LandscapePool
	{
		public LandscapeType LandscapeIdx;

		public int Players;
	}

	public CWLandscapeCardDragOld[] CardScripts;

	public GameObject ReadyButton;

	public GameObject HexGrid1;

	public GameObject HexGrid2;

	public FlippedLandscapeScript[,] FlippedScripts;

	public LandscapeScript[,,] LandscapeScripts = new LandscapeScript[2, 4, 5];

	private GameObject[,] Landscapes = new GameObject[2, 4];

	private GameState GameInstance;

	private static LandscapeManagerScript g_landscapeManager;

	public GameObject[] player1LandscapeLanes;

	public GameObject[] player2LandscapeLanes;

	public GameObject[] corns;

	public GameObject[] plains;

	public GameObject[] cottons;

	public GameObject[] sands;

	public GameObject[] swamps;

	public int populatedLandscapeCount;

	public GameObject[] landDamageFXprefabs;

	public bool enableDragLandscapeCard { get; set; }

	private void Awake()
	{
		g_landscapeManager = this;
	}

	public static LandscapeManagerScript GetInstance()
	{
		return g_landscapeManager;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		GameInstance.LandscapeManager = this;
		enableDragLandscapeCard = true;
		string[] array = new string[4] { "1", "2", "3", "4" };
		FlippedScripts = new FlippedLandscapeScript[2, 4];
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				string text = "P" + (i + 1) + "FlippedLandscape" + array[j];
				FlippedScripts[i, j] = GameObject.Find(text).GetComponent<FlippedLandscapeScript>();
				FlippedScripts[i, j].Index = j;
			}
		}
	}

	public void PoolLandscape()
	{
		Deck deck = GameInstance.GetDeck(PlayerType.User);
		Deck deck2 = GameInstance.GetDeck(PlayerType.Opponent);
		int num = 0;
		int num2 = deck.GetUniqueLandscapeCount() + deck2.GetUniqueLandscapeCount();
		LandscapePool[] array = new LandscapePool[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i].LandscapeIdx = LandscapeType.None;
			array[i].Players = 0;
		}
		for (int j = 0; j < deck.GetLandscapeCount(); j++)
		{
			LandscapeType landscape = deck.GetLandscape(j);
			bool flag = false;
			for (int k = 0; k < num2; k++)
			{
				if (landscape == array[k].LandscapeIdx)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				array[num].LandscapeIdx = landscape;
				array[num].Players |= 1;
				num++;
			}
		}
		for (int l = 0; l < deck2.GetLandscapeCount(); l++)
		{
			LandscapeType landscape = deck2.GetLandscape(l);
			bool flag2 = false;
			for (int m = 0; m < num2; m++)
			{
				if (landscape == array[m].LandscapeIdx)
				{
					flag2 = true;
					array[m].Players |= 16;
					break;
				}
			}
			if (!flag2)
			{
				array[num].LandscapeIdx = landscape;
				array[num].Players |= 16;
				num++;
			}
		}
		for (int n = 0; n < num; n++)
		{
			GameObject gameObject = null;
			for (int num3 = 0; num3 < 4; num3++)
			{
				int count = 1;
				if (array[n].Players == 17)
				{
					count = 2;
				}
				switch (array[n].LandscapeIdx)
				{
				case LandscapeType.Corn:
					gameObject = corns[num3];
					break;
				case LandscapeType.Plains:
					gameObject = plains[num3];
					break;
				case LandscapeType.Cotton:
					gameObject = cottons[num3];
					break;
				case LandscapeType.Sand:
					gameObject = sands[num3];
					break;
				case LandscapeType.Swamp:
					gameObject = swamps[num3];
					break;
				}
				if ((bool)gameObject)
				{
					PoolManager.PopulateStore(gameObject, count);
				}
			}
		}
	}

	public void FlipLandscape(PlayerType player, int lane, bool flip)
	{
		NGUITools.SetActive(Landscapes[(int)player, lane].transform.parent.gameObject, !flip);
	}

	public void HighlightLandscape(PlayerType player, int lane)
	{
		FlippedLandscapeScript flippedLandscapeScript = FlippedScripts[(int)player, lane];
		flippedLandscapeScript.LandType = GameInstance.GetLandscapeType(player, lane);
		flippedLandscapeScript.Flash = true;
	}

	public void UnhighlightLandscape(PlayerType player, int lane)
	{
		FlippedScripts[(int)player, lane].StopHighlight();
	}

	public void RemoveLandscape(PlayerType player, int lane)
	{
		if (Landscapes[(int)player, lane] != null)
		{
			Object.Destroy(Landscapes[(int)player, lane]);
			Landscapes[(int)player, lane] = null;
		}
	}

	public void UpdateLandscapes()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				GameObject gameObject = null;
				switch (GameInstance.GetLandscapeType(i, j))
				{
				case LandscapeType.Corn:
					gameObject = corns[j];
					break;
				case LandscapeType.Plains:
					gameObject = plains[j];
					break;
				case LandscapeType.Cotton:
					gameObject = cottons[j];
					break;
				case LandscapeType.Sand:
					gameObject = sands[j];
					break;
				case LandscapeType.Swamp:
					gameObject = swamps[j];
					break;
				}
				if (!(gameObject != null) || !(Landscapes[i, j] == null))
				{
					continue;
				}
				GameObject gameObject2 = SLOTGame.InstantiateGO(gameObject);
				Landscapes[i, j] = gameObject2;
				gameObject2.transform.parent = ((i != 0) ? player2LandscapeLanes[j].transform : player1LandscapeLanes[j].transform);
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localRotation = Quaternion.identity;
				Component[] componentsInChildren = gameObject2.GetComponentsInChildren(typeof(TutorialAnimTrigger));
				Component[] array = componentsInChildren;
				foreach (Component component in array)
				{
					TutorialAnimTrigger tutorialAnimTrigger = component as TutorialAnimTrigger;
					if (tutorialAnimTrigger != null && tutorialAnimTrigger.WillTriggerTutorial())
					{
						UICamera.useInputEnabler = true;
						break;
					}
				}
			}
		}
	}

	public void AssignOpponentLandscapes()
	{
		for (int i = 0; i < 4; i++)
		{
			LandscapeType landscapeInDeck = GameInstance.GetLandscapeInDeck(PlayerType.Opponent, i);
			GameInstance.SetLandscape(PlayerType.Opponent, i, landscapeInDeck);
		}
	}

	public void SpawnLandDamageFX(PlayerType player, int lane)
	{
		GameObject gameObject = landDamageFXprefabs[3 - lane];
		GameObject gameObject2 = FlippedScripts[(int)player, 3 - lane].gameObject;
		if (gameObject != null)
		{
			Vector3 position = gameObject2.transform.position;
			float num = (((int)player != 0) ? 0.1f : 0f);
			position.y += num;
			SLOTGame.InstantiateFX(gameObject, position, ((int)player != 0) ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity);
		}
	}
}
