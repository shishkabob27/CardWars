using UnityEngine;

public class BMOHidingScript : MonoBehaviour
{
	private string OpponentCharacter;

	private QuestData qd;

	private void Start()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		qd = instance.GetCurrentQuest();
		OpponentCharacter = qd.Opponent.ToString();
	}

	private void Update()
	{
		if (OpponentCharacter == "BMO")
		{
			base.transform.position = new Vector3(75f, -100f, 225f);
		}
		else
		{
			base.transform.position = new Vector3(75f, -36.5f, 225f);
		}
	}
}
