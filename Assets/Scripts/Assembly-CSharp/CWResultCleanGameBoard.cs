using System.Collections;
using UnityEngine;

public class CWResultCleanGameBoard : MonoBehaviour
{
	public GameObject battleLane;

	public GameObject[] gameBoards;

	public string textureName;

	private CreatureManagerScript creatureMgr;

	private GameState GameInstance;

	private Texture boardTexture;

	private void Start()
	{
		creatureMgr = CreatureManagerScript.GetInstance();
		GameInstance = GameState.Instance;
	}

	private void OnClick()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					if (GameInstance.LaneHasCard(i, j, (CardType)k))
					{
						creatureMgr.RemoveInstance(i, j, (CardType)k);
					}
				}
			}
		}
		CWCreatureStatsFloorDisplay[] componentsInChildren = battleLane.GetComponentsInChildren<CWCreatureStatsFloorDisplay>();
		CWCreatureStatsFloorDisplay[] array = componentsInChildren;
		foreach (CWCreatureStatsFloorDisplay cWCreatureStatsFloorDisplay in array)
		{
			cWCreatureStatsFloorDisplay.gameObject.SetActive(false);
		}
		Transform[] componentsInChildren2 = battleLane.GetComponentsInChildren<Transform>();
		Transform[] array2 = componentsInChildren2;
		foreach (Transform transform in array2)
		{
			if (transform.name.EndsWith("mass"))
			{
				transform.gameObject.SetActive(false);
			}
		}
		CWLootingSequenceTrigger[] componentsInChildren3 = battleLane.GetComponentsInChildren<CWLootingSequenceTrigger>(true);
		CWLootingSequenceTrigger[] array3 = componentsInChildren3;
		foreach (CWLootingSequenceTrigger cWLootingSequenceTrigger in array3)
		{
			Object.DestroyImmediate(cWLootingSequenceTrigger.transform.parent.gameObject);
		}
		CWFloopActionManager.GetInstance().RemovePersistantVFX("BlockSpell");
		CWFloopActionManager.GetInstance().RemovePersistantVFX("BlockBuilding");
		CWFloopActionManager.GetInstance().RemovePersistantVFX("BlockCreature");
		CWFloopActionManager instance = CWFloopActionManager.GetInstance();
		for (int num = 0; num < 2; num++)
		{
			for (int num2 = 0; num2 < 4; num2++)
			{
				instance.RemovePersistantVFX(num, num2);
			}
		}
		for (int num3 = 0; num3 < 2; num3++)
		{
			for (int num4 = 0; num4 < 4; num4++)
			{
				for (int num5 = 0; num5 < 2; num5++)
				{
					if (GameInstance.LaneHasCard(num3, num4, (CardType)num5))
					{
						CardScript script = GameInstance.GetScript(num3, num4, (CardType)num5);
						script.FloopBlocked = false;
					}
				}
			}
		}
		boardTexture = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Textures/Board/" + textureName) as Texture;
		StartCoroutine(FadeBoard());
	}

	private IEnumerator FadeBoard()
	{
		yield return new WaitForSeconds(0.3f);
		GameObject[] array = gameBoards;
		foreach (GameObject obj in array)
		{
			if (boardTexture != null)
			{
				obj.GetComponent<Renderer>().material.mainTexture = boardTexture;
			}
		}
	}

	private void Update()
	{
	}
}
