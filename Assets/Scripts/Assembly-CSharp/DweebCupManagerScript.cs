using System.Collections.Generic;
using UnityEngine;

public class DweebCupManagerScript : MonoBehaviour
{
	private CardDataManager CardManager;

	public P2AnteScript TotalSoda;

	public P2AnteScript P2_Ante;

	public GameObject ContinueButton;

	public GameObject BlankCard;

	public GameObject PreMatch;

	public GameObject P1_Ante;

	public GameObject Cards;

	public UILabel Stakes1;

	public UILabel Stakes2;

	public float Timer;

	public int SpawnPosition = -640;

	public int Card;

	public CardItem PlayerAnte;

	public CardItem OpponentAnte;

	public bool DweebCupUpdated;

	public bool FingerDown;

	public bool CardGrown;

	public bool Ante;

	public float Momentum;

	public bool MoveLeft;

	public bool MoveRight;

	private void Start()
	{
		CardManager = CardDataManager.Instance;
		Cards.transform.localPosition = new Vector3(0f, -705f, 0f);
	}

	private void Update()
	{
		if (!FingerDown && !Ante)
		{
			if (Cards.transform.localPosition.x > 0f)
			{
				Cards.transform.localPosition = new Vector3(Mathf.Lerp(Cards.transform.localPosition.x, 0f, Time.deltaTime * 5f), Cards.transform.localPosition.y, 0f);
				MoveRight = false;
			}
			if (Cards.transform.localPosition.x < (float)((Cards.transform.childCount - 8) * -180))
			{
				Cards.transform.localPosition = new Vector3(Mathf.Lerp(Cards.transform.localPosition.x, (Cards.transform.childCount - 8) * -180, Time.deltaTime * 5f), Cards.transform.localPosition.y, 0f);
				MoveLeft = false;
			}
		}
		if (Ante)
		{
			Timer += Time.deltaTime * 2f;
			if (Timer < 2f)
			{
			}
			if (Timer > 2f && Timer < 3f)
			{
				P2_Ante.Fall = true;
				OpponentAnte = new CardItem(CardManager.GetCard(CardType.Dweeb, "Dweeb_ApplePie"));
			}
			if (Timer > 3f && Timer < 4f)
			{
				TotalSoda.Fall = true;
			}
			if (Timer > 4f)
			{
				Cards.transform.localPosition = new Vector3(Cards.transform.localPosition.x, Mathf.Lerp(Cards.transform.localPosition.y, -705f, Time.deltaTime * 5f), 0f);
				if (!DweebCupUpdated)
				{
					DweebCupUpdated = true;
					GameState.Instance.GetDweebCup().Add(PlayerAnte);
					GameState.Instance.GetDweebCup().Add(OpponentAnte);
					Stakes1.enabled = true;
					Stakes2.enabled = true;
				}
			}
		}
		else
		{
			Cards.transform.localPosition = new Vector3(Cards.transform.localPosition.x, Mathf.Lerp(Cards.transform.localPosition.y, -195f, Time.deltaTime * 5f), 0f);
		}
		if (MoveLeft)
		{
			Cards.transform.localPosition = new Vector3(Cards.transform.localPosition.x - Momentum, Cards.transform.localPosition.y, Cards.transform.localPosition.z);
			Momentum -= Time.deltaTime * Momentum;
			if (Momentum < 1f)
			{
				Momentum = 0f;
				MoveLeft = false;
			}
		}
		if (MoveRight)
		{
			Cards.transform.localPosition = new Vector3(Cards.transform.localPosition.x + Momentum, Cards.transform.localPosition.y, Cards.transform.localPosition.z);
			Momentum -= Time.deltaTime * Momentum;
			if (Momentum < 1f)
			{
				Momentum = 0f;
				MoveRight = false;
			}
		}
		if (!Input.GetKeyDown("space"))
		{
		}
	}

	public void Populate()
	{
		List<CardItem> list = new List<CardItem>();
		Cards.transform.localPosition = new Vector3(0f, -705f, 0f);
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = SLOTGame.InstantiateFX(BlankCard, base.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = Cards.transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localPosition = new Vector3(SpawnPosition, 0f, 0f);
			MovingDweebCardScript component = gameObject.GetComponent<MovingDweebCardScript>();
			component.CardData = list[i];
			SpawnPosition += 180;
		}
	}

	public void Reset()
	{
		Object.Destroy(P1_Ante);
		P2_Ante.transform.position = new Vector3(4.5f, 65f, -33.5f);
		P2_Ante.Splashed = false;
		P2_Ante.enabled = true;
		P2_Ante.Fall = false;
		P2_Ante.Bob = false;
		TotalSoda.transform.position = new Vector3(-6.5f, 65f, -33.5f);
		TotalSoda.Splashed = false;
		TotalSoda.enabled = true;
		TotalSoda.Fall = false;
		TotalSoda.Bob = false;
		Cards.transform.localPosition = new Vector3(0f, -705f, 0f);
		Stakes1.enabled = false;
		Stakes2.enabled = false;
		GameState.Instance.GetDweebCup().Clear();
		while (Cards.transform.childCount > 0)
		{
			Object.DestroyImmediate(Cards.transform.GetChild(0).gameObject);
		}
		SpawnPosition = -640;
		OpponentAnte = null;
		PlayerAnte = null;
		Timer = 0f;
		Card = 0;
		DweebCupUpdated = false;
		Ante = false;
	}

	public void HideStakes()
	{
		Stakes1.enabled = false;
		Stakes2.enabled = false;
	}
}
