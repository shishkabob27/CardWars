using UnityEngine;

public class CreatureBattleTest : MonoBehaviour
{
	public GameObject Creature1;

	public GameObject Creature2;

	public GameObject HuskerKnightP1_1;

	public GameObject HuskerKnightP1_2;

	public GameObject HuskerKnightP1_3;

	public GameObject HuskerKnightP1_4;

	public GameObject HuskerKnightP2_1;

	public GameObject HuskerKnightP2_2;

	public GameObject HuskerKnightP2_3;

	public GameObject HuskerKnightP2_4;

	public GameObject DustCloud;

	public float Position;

	public float Timer;

	public int Lane;

	private void Start()
	{
		DustCloud.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
	}

	private void Update()
	{
		Timer += Time.deltaTime;
		if (Timer > 2f && Timer < 4f)
		{
			if (Position < 6.1f)
			{
				Position += Time.deltaTime * 20f;
				if (Position > 6.1f)
				{
					Position = 6.1f;
				}
			}
			Creature1.transform.localPosition = new Vector3(Position, 0f, 0f);
			Creature2.transform.localPosition = new Vector3(Position * -1f, 0f, 0f);
			DustCloud.transform.localScale = new Vector3(Position, Position, Position);
		}
		if (Timer > 4f)
		{
			if (Position > 0f)
			{
				Position -= Time.deltaTime * 20f;
				if (Position < 0.01f)
				{
					Position = 0.01f;
				}
			}
			Creature1.transform.localPosition = new Vector3(Position, 0f, 0f);
			Creature2.transform.localPosition = new Vector3(Position * -1f, 0f, 0f);
			DustCloud.transform.localScale = new Vector3(Position, Position, Position);
		}
		if (Timer > 5f)
		{
			Timer = 0f;
			Lane++;
		}
		if (Lane == 1)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 0f, 10.5f), Time.deltaTime * 5f);
			Creature1 = HuskerKnightP1_1;
			Creature2 = HuskerKnightP2_4;
		}
		if (Lane == 2)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 0f, -0.5f), Time.deltaTime * 5f);
			Creature1 = HuskerKnightP1_2;
			Creature2 = HuskerKnightP2_3;
		}
		if (Lane == 3)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 0f, -12.5f), Time.deltaTime * 5f);
			Creature1 = HuskerKnightP1_3;
			Creature2 = HuskerKnightP2_2;
		}
		if (Lane == 4)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 0f, -23.5f), Time.deltaTime * 5f);
			Creature1 = HuskerKnightP1_4;
			Creature2 = HuskerKnightP2_1;
		}
	}
}
