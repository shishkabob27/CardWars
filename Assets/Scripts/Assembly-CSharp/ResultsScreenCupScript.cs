using UnityEngine;

public class ResultsScreenCupScript : MonoBehaviour
{
	public GameObject DweebCard;

	private GameObject NewCard;

	private ResultsDweebCardScript NewCardScript;

	public Transform ResultsCards;

	public Transform Destination;

	public bool SpawnedCards;

	public float Rotation;

	public float Timer;

	public int CupID;

	private void Start()
	{
	}

	private void Update()
	{
		if (GameState.Instance.GetHealth(PlayerType.Opponent) > 0)
		{
			return;
		}
		Timer += Time.deltaTime;
		if (!(Timer > 1f))
		{
			return;
		}
		if (CupID == 1)
		{
			Rotation = Mathf.Lerp(Rotation, -135f, Time.deltaTime * 10f);
		}
		else if (CupID == 2)
		{
			Rotation = Mathf.Lerp(Rotation, 135f, Time.deltaTime * 10f);
		}
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, Rotation);
		if (CupID == 1)
		{
			if (!SpawnedCards)
			{
				SpawnedCards = true;
				NewCard = SLOTGame.InstantiateFX(DweebCard, base.transform.position, Quaternion.identity) as GameObject;
				NewCard.transform.parent = ResultsCards;
				NewCard.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
				NewCard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				NewCard.transform.position = NewCard.transform.position;
				NewCardScript = NewCard.GetComponent<ResultsDweebCardScript>();
				NewCardScript.Destination = Destination.transform.localPosition;
				NewCardScript.UpdateCard();
			}
		}
		else if (CupID == 2 && !SpawnedCards)
		{
			SpawnedCards = true;
			NewCard = SLOTGame.InstantiateFX(DweebCard, base.transform.position, Quaternion.identity) as GameObject;
			NewCard.transform.parent = ResultsCards;
			NewCard.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			NewCard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			NewCard.transform.position = NewCard.transform.position;
			NewCardScript = NewCard.GetComponent<ResultsDweebCardScript>();
			NewCardScript.Destination = new Vector3(Destination.transform.localPosition.x, Destination.transform.localPosition.y, Destination.transform.localPosition.z);
			NewCardScript.CardData = null;
			NewCardScript.UpdateCard();
			NewCard = SLOTGame.InstantiateFX(DweebCard, base.transform.position, Quaternion.identity) as GameObject;
			NewCard.transform.parent = ResultsCards;
			NewCard.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			NewCard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			NewCard.transform.position = NewCard.transform.position;
			NewCardScript = NewCard.GetComponent<ResultsDweebCardScript>();
			NewCardScript.Destination = new Vector3(Destination.transform.localPosition.x + 225f, Destination.transform.localPosition.y, Destination.transform.localPosition.z);
			NewCardScript.CardData = null;
			NewCardScript.UpdateCard();
		}
	}
}
