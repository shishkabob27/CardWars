using UnityEngine;

public class DweebManagerScript : MonoBehaviour
{
	public GameObject ResultsScreen;

	public GameObject TableDweebCup;

	public GameObject PlayerCharacter;

	public Renderer PlayerMouth;

	public Transform PlayerHand;

	public GameObject OpponentCharacter;

	public Renderer OpponentMouth;

	public Transform OpponentHand;

	public Renderer Eyes;

	public Renderer Mouth;

	public GameObject DweebCup;

	public string OpponentDweebDrink;

	public string PlayerDweebDrink;

	public bool AnimateMouth;

	public bool Animate;

	private GameState GameInstance;

	private BattlePhaseManager phaseMgr;

	public GameObject GameOver;

	private static DweebManagerScript g_dweebManager;

	private void Awake()
	{
		g_dweebManager = this;
	}

	public static DweebManagerScript GetInstance()
	{
		return g_dweebManager;
	}

	private void Start()
	{
		GameInstance = GameState.Instance;
		phaseMgr = BattlePhaseManager.GetInstance();
	}

	public void DetermineDweeb()
	{
		if (TableDweebCup != null)
		{
			NGUITools.SetActive(TableDweebCup, false);
		}
		if (GameInstance.GetHealth(PlayerType.User) <= 0)
		{
			Mouth = PlayerMouth;
			if (PlayerCharacter != null && PlayerDweebDrink != null)
			{
				PlayerCharacter.GetComponent<Animation>().Play(PlayerDweebDrink);
			}
			if (PlayerHand != null)
			{
				DweebCup.transform.parent = PlayerHand;
				Transform transform = PlayerHand.Find("HoldCards(Clone)");
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
			}
			DweebCup.transform.localPosition = new Vector3(0f, 0f, 0f);
			DweebCup.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			phaseMgr.ActivateLoserTween();
		}
		else
		{
			Mouth = OpponentMouth;
			if (OpponentCharacter != null && OpponentDweebDrink != null)
			{
				OpponentCharacter.GetComponent<Animation>().Play(OpponentDweebDrink);
			}
			if (OpponentHand != null)
			{
				DweebCup.transform.parent = OpponentHand;
			}
			DweebCup.transform.localPosition = new Vector3(0f, 0f, 0f);
			DweebCup.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			phaseMgr.ActivateWinnerTween();
		}
		Animate = true;
	}

	private void Update()
	{
	}
}
