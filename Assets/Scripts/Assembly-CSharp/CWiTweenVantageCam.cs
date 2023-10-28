using UnityEngine;

public class CWiTweenVantageCam : MonoBehaviour
{
	public float CAMERA_Y_OFFSET;

	public bool CAMERA_Y_OFFSET_FlAG;

	public float CAMERA_DISTANCE_OFFSET;

	public bool CAMERA_DISTANCE_OFFSET_FLAG;

	public GameObject gameCamera;

	public GameObject gameCameraTarget;

	public Transform transformTo;

	public Transform transformTargetTo;

	public string theLastTweenEvent;

	public float time;

	public Transform[] creatureVantagePoints;

	public Transform[] buildingVantagePoints;

	public Transform[,,] vantageTargets = new Transform[2, 4, 2];

	public float summonTimer;

	public GameObject target;

	public Vector3 targetOffset;

	public Vector3 camOffset;

	private GameState GameInstance;

	private CWiTweenCamTrigger triggerScript;

	private int currentCardLane;

	private static CWiTweenVantageCam g_vantageCam;

	private void Awake()
	{
		g_vantageCam = this;
	}

	public static CWiTweenVantageCam GetInstance()
	{
		return g_vantageCam;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		PanelManagerBattle instance = PanelManagerBattle.GetInstance();
		if (instance != null)
		{
			gameCamera = instance.newCamera;
			gameCameraTarget = instance.newCameraTarget;
		}
		FindVantageTargets();
	}

	public void CreatureSpawnCamera(PlayerType player)
	{
		iTween.StopByName(gameCamera, "ToP2Setup");
		CardScript summoningCard = GetSummoningCard(player);
		SetCamTargets(summoningCard, player, currentCardLane);
		if (summoningCard.Data.Form.Type == CardType.Spell || !(target != null))
		{
			return;
		}
		Vector3 position = transformTargetTo.transform.position;
		Vector3 position2 = transformTo.transform.position;
		if (summoningCard.Data.Form.Type != CardType.Building)
		{
			targetOffset = Vector3.zero;
			camOffset = Vector3.zero;
			if (CAMERA_Y_OFFSET_FlAG)
			{
				targetOffset = gameCameraTarget.transform.up * CAMERA_Y_OFFSET;
				position += targetOffset;
				camOffset += gameCamera.transform.up * CAMERA_Y_OFFSET;
			}
			if (CAMERA_DISTANCE_OFFSET_FLAG)
			{
				Bounds rendererBoundsRecursive = SLOTGame.GetRendererBoundsRecursive(target.transform);
				Vector3 vector = position2 + camOffset - (position + targetOffset);
				float a = Mathf.Max(rendererBoundsRecursive.size.x, rendererBoundsRecursive.size.y);
				a = Mathf.Max(a, rendererBoundsRecursive.size.z);
				float f = a * 0.5f / Mathf.Tan(GetComponent<Camera>().fieldOfView * 0.5f);
				camOffset += vector * (Mathf.Abs(f) + CAMERA_DISTANCE_OFFSET);
			}
			position2 += camOffset;
		}
		iTween.MoveTo(gameCamera, iTween.Hash("position", position2, "time", time));
		iTween.MoveTo(gameCameraTarget, iTween.Hash("position", position, "time", time));
	}

	private CardScript GetSummoningCard(PlayerType player)
	{
		for (int i = 0; i < 4; i++)
		{
			CardScript summoning = GameInstance.GetSummoning(player, i);
			if (summoning != null)
			{
				currentCardLane = i;
				return summoning;
			}
		}
		return null;
	}

	private void FindVantageTargets()
	{
		string[] array = new string[2] { "Creature", "Building" };
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					string text = "P" + (k + 1) + "Lane" + (i + 1) + array[j] + "VantageTarget";
					vantageTargets[k, i, j] = GameObject.Find(text).transform;
				}
			}
		}
	}

	private void SetCamTargets(CardScript script, PlayerType player, int n)
	{
		switch (script.Data.Form.Type)
		{
		case CardType.Creature:
			transformTo = ((player != PlayerType.User) ? creatureVantagePoints[3 - n] : creatureVantagePoints[n]);
			transformTargetTo = vantageTargets[(int)player, n, 0];
			break;
		case CardType.Building:
			transformTo = ((player != PlayerType.User) ? buildingVantagePoints[3 - n] : buildingVantagePoints[n]);
			transformTargetTo = vantageTargets[(int)player, n, 1];
			break;
		}
	}
}
