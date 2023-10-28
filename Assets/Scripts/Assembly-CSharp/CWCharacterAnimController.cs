using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWCharacterAnimController : MonoBehaviour
{
	public List<GameObject> playerCharacters = new List<GameObject>();

	public CharacterData[] playerData = new CharacterData[2];

	public List<GameObject> holdHandBones = new List<GameObject>();

	public List<GameObject> playHandBones = new List<GameObject>();

	public List<CWPlayCardsController> playCardControllers = new List<CWPlayCardsController>();

	public float cardReleaseTimePlayCard;

	public float cardReleaseTimeRareCard;

	public string playerID;

	public string opponentID;

	private BattlePhaseManager phaseMgr;

	private static CWCharacterAnimController g_charAnimController;

	private Animation anim;

	public string P1FaceAnim;

	public string P2FaceAnim;

	public float P1FaceTime;

	public float P2FaceTime;

	private bool printFlag;

	public GameObject dweebCup;

	private float timer;

	private void Awake()
	{
		g_charAnimController = this;
	}

	public static CWCharacterAnimController GetInstance()
	{
		return g_charAnimController;
	}

	private void OnEnable()
	{
		SetupCharacters();
	}

	private void Start()
	{
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public void SetupCharacters()
	{
		if (playerCharacters.Count != 0)
		{
			for (int i = 0; i < playerCharacters.Count; i++)
			{
				GameObject item = FindInChildren(playerCharacters[i], "Puppet_CardPlay");
				playHandBones.Add(item);
				GameObject item2 = FindInChildren(playerCharacters[i], playerData[i].Hand);
				holdHandBones.Add(item2);
			}
			int num = 0;
			for (int j = 0; j < holdHandBones.Count; j++)
			{
				GameObject gameObject = SpawnCardObjects("HoldCards", holdHandBones[j]);
				playCardControllers.Add(gameObject.GetComponent<CWPlayCardsController>());
				playCardControllers[j].player = j;
				gameObject = SpawnCardObjects("PlayingCard", playHandBones[j]);
				num++;
			}
		}
	}

	private GameObject SpawnCardObjects(string prefabName, GameObject parentBone)
	{
		//UnityEngine.Object @object = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResource("Props/" + prefabName);
        GameObject original = Resources.Load("Props/" + prefabName, typeof(GameObject)) as GameObject;
        GameObject gameObject = null;
		if (original != null)
		{
			gameObject = Instantiate(original, parentBone.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = parentBone.transform;
			gameObject.transform.localRotation = ((!(prefabName == "HoldCards")) ? Quaternion.identity : Quaternion.Euler(new Vector3(270f, 0f, 0f)));
		}
		return gameObject;
	}

	private GameObject FindInChildren(GameObject parentObj, string childName)
	{
		Transform[] componentsInChildren = parentObj.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == childName)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public string GetAnimFromJson(CharAnimType animType, CharacterData data)
	{
		string result = string.Empty;
		switch (animType)
		{
		case CharAnimType.Happy:
			result = data.HappyAnim;
			break;
		case CharAnimType.Sad:
			result = data.SadAnim;
			break;
		case CharAnimType.Idle:
			result = data.IdleAnim;
			break;
		case CharAnimType.Fidget:
			result = data.FidgetAnim;
			break;
		case CharAnimType.CardIdle:
			result = data.CardIdleAnim;
			break;
		case CharAnimType.LastCard:
			result = data.LastCardAnim;
			break;
		case CharAnimType.PlayCard:
			result = data.PlayCardAnim;
			break;
		case CharAnimType.PlayRare:
			result = data.PlayRareCardAnim;
			break;
		case CharAnimType.PlayRareLastCard:
			result = data.PlayRareLastCardAnim;
			break;
		case CharAnimType.DweebDrink:
			result = data.DweebDrinkAnim;
			break;
		case CharAnimType.IntroP1:
			result = data.IntroP1Anim;
			break;
		case CharAnimType.IntroP2:
			result = data.IntroP2Anim;
			break;
		case CharAnimType.Defeated:
			result = data.DefeatedAnim;
			break;
		}
		return result;
	}

	public void playAnim(PlayerType player, CharAnimType currentAnim, WrapMode currentWrap, CharAnimType nextAnim, WrapMode nextWrap, bool clampForever)
	{
		anim = playerCharacters[player].GetComponent<Animation>();
		if ((phaseMgr.Phase == BattlePhase.P1SetupAction || phaseMgr.Phase == BattlePhase.P2SetupAction || phaseMgr.Phase == BattlePhase.P1SetupActionSpell || phaseMgr.Phase == BattlePhase.P2SetupActionSpell) && (phaseMgr.prevPhase == BattlePhase.P1SetupActionRareCard || phaseMgr.prevPhase == BattlePhase.P2SetupActionRareCard))
		{
			return;
		}
		string animFromJson = GetAnimFromJson(currentAnim, playerData[(int)player]);
		string animFromJson2 = GetAnimFromJson(nextAnim, playerData[(int)player]);
		if (anim != null && animFromJson != string.Empty)
		{
			anim.wrapMode = currentWrap;
			anim.Play(animFromJson);
			if (!clampForever)
			{
				anim.Rewind();
				anim.PlayQueued(animFromJson2);
			}
		}
		Transform transform = playerCharacters[player].transform.Find("M_MOUTH");
		Transform transform2 = playerCharacters[player].transform.Find("M_EYES");
		if (player == PlayerType.User)
		{
			P1FaceAnim = animFromJson;
		}
		else
		{
			P2FaceAnim = animFromJson;
		}
		if (!animFromJson.Contains("LadyRainicorn"))
		{
			if (transform != null)
			{
				StartCoroutine(PlayFaceAnimation(player, "CNT_MOUTH", transform.gameObject, animFromJson, animFromJson2, true));
			}
			if (transform2 != null)
			{
				StartCoroutine(PlayFaceAnimation(player, "CNT_EYES", transform2.gameObject, animFromJson, animFromJson2, true));
			}
		}
	}

	private void SetFaceIdle(string targetController, GameObject targetMesh, string animName)
	{
		try
		{
			Dictionary<string, object>[] array = SQUtils.ReadJSONData(animName + ".json", "CharacterFaceAnim");
			Dictionary<string, object>[] array2 = array;
			foreach (Dictionary<string, object> dictionary in array2)
			{
				string text = (string)dictionary["CNT"];
				if (text == targetController)
				{
					float x = float.Parse((string)dictionary["x"]);
					float y = float.Parse((string)dictionary["y"]);
					Renderer component = targetMesh.GetComponent<Renderer>();
					component.material.SetTextureOffset("_MainTex", new Vector2(x, y));
					break;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public IEnumerator PlayFaceAnimation(PlayerType player, string targetController, GameObject targetMesh, string currentAnimName, string nextAnimName, bool clampForever)
	{
		Dictionary<string, object>[] data = null;
		try
		{
			data = SQUtils.ReadJSONData(currentAnimName + ".json", "CharacterFaceAnim");
		}
		catch (Exception ex)
		{
			Exception e = ex;
		}
		if (data != null)
		{
			float totalTime = 0f;
			float startTime = float.Parse((string)data[0]["time"]);
			float prevTime = startTime;
			Dictionary<string, object>[] array = data;
			foreach (Dictionary<string, object> dt in array)
			{
				string controller = (string)dt["CNT"];
				if (controller == targetController)
				{
					float time = float.Parse((string)dt["time"]) * 2f - prevTime;
					yield return new WaitForSeconds(time);
					totalTime += time;
					prevTime += time;
					if (player == PlayerType.User)
					{
						P1FaceTime = totalTime;
					}
					else
					{
						P2FaceTime = totalTime;
					}
					float x = float.Parse((string)dt["x"]);
					float y = float.Parse((string)dt["y"]);
					Renderer targetRenderer = targetMesh.GetComponent<Renderer>();
					targetRenderer.material.SetTextureOffset("_MainTex", new Vector2(x, y));
				}
			}
		}
		yield return null;
		if (!clampForever)
		{
			SetFaceIdle(targetController, targetMesh, nextAnimName);
		}
	}

	public void DoEffectPlayCardSpell(PlayerType player, int lane, CardItem card)
	{
		StartCoroutine(JustWaitSpell(2f, player, lane, card));
	}

	private IEnumerator JustWaitSpell(float waitTime, PlayerType player, int lane, CardItem card)
	{
		yield return new WaitForSeconds(waitTime);
		CWPlayerHandsController.GetInstance().PreCastSpell(player, card.Form.Rarity >= 5, card);
	}

	public void DoEffectPlayCard(PlayerType player, int lane, CardItem card)
	{
		StartCoroutine(JustWait(2f, player, lane, card));
	}

	private IEnumerator JustWait(float waitTime, PlayerType player, int lane, CardItem card)
	{
		yield return new WaitForSeconds(waitTime);
		GameState.Instance.DoResultSummon(player, lane, card);
	}

	public void P1DweebAction()
	{
		DweebCupSetup(PlayerType.User);
	}

	private void DweebCupSetup(PlayerType player)
	{
		playCardControllers[player].gameObject.SetActive(false);
		GameObject gameObject = SpawnCardObjects("DweebCup", holdHandBones[player]);
		gameObject.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		phaseMgr.ActivateLoserTween();
	}

	private void Update()
	{
	}
}
