using UnityEngine;

public class CreatureBattleTest2 : MonoBehaviour
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

	public float Position;

	public float Timer;

	public int Lane;

	private void Update()
	{
		if (Lane < 5)
		{
			Timer += Time.deltaTime;
			if (Timer > 0.5f && (double)Timer < 1.43333333333)
			{
				if (Position < 6.1f)
				{
					Position += Time.deltaTime * 10f;
					if (Position > 6.1f)
					{
						Position = 6.1f;
					}
				}
				Creature1.GetComponent<Animation>().Play("Crash");
				Creature2.GetComponent<Animation>().Play("Crash");
			}
			if ((double)Timer > 1.43333333333)
			{
				if (Position > 0f)
				{
					Position -= Time.deltaTime * 10f;
					if (Position < 0f)
					{
						Position = 0f;
					}
				}
				Creature1.GetComponent<Animation>().Play("Husker_Idle");
				Creature2.GetComponent<Animation>().Play("Husker_Idle");
			}
			if (Timer > 1.5f)
			{
				Timer = 0f;
				Lane++;
			}
		}
		if (Lane == 1)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 4f, 6.5f), Time.deltaTime * 10f);
			Creature1 = HuskerKnightP1_1;
			Creature2 = HuskerKnightP2_4;
		}
		if (Lane == 2)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 4f, -4.5f), Time.deltaTime * 10f);
			Creature1 = HuskerKnightP1_2;
			Creature2 = HuskerKnightP2_3;
		}
		if (Lane == 3)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 4f, -16.5f), Time.deltaTime * 10f);
			Creature1 = HuskerKnightP1_3;
			Creature2 = HuskerKnightP2_2;
		}
		if (Lane == 4)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(0f, 4f, -27.5f), Time.deltaTime * 10f);
			Creature1 = HuskerKnightP1_4;
			Creature2 = HuskerKnightP2_1;
		}
	}
}
