using UnityEngine;

public class DebugButtonScript : MonoBehaviour
{
	private GameDataScript GameData;

	public GameObject DebugWindow;

	public GameObject BlankCard;

	private GameObject NewCard;

	public UILabel Label;

	private int CardNumber;

	public int ID;

	private GameState GameInstance;

	private void Start()
	{
		GameData = GameDataScript.GetInstance();
		GameInstance = GameState.Instance;
		if (Label != null && ID == 6)
		{
			Label.text = "Spawn Card " + 1;
		}
	}

	private void OnClick()
	{
		if (ID == 1)
		{
			NGUITools.SetActive(DebugWindow, true);
		}
		else if (ID == 2)
		{
			NGUITools.SetActive(DebugWindow, false);
		}
		else if (ID == 3)
		{
			NGUITools.SetActive(DebugWindow, false);
		}
		else if (ID == 4)
		{
			NGUITools.SetActive(DebugWindow, false);
		}
		else if (ID == 5)
		{
			NGUITools.SetActive(base.transform.parent.gameObject, false);
			GameInstance.SetHealth(PlayerType.Opponent, 0);
			GameData.UpdateText();
			GameData.Timer = 1f;
		}
		else if (ID == 6)
		{
			if (NewCard != null)
			{
				Object.Destroy(NewCard);
				Resources.UnloadUnusedAssets();
			}
			NewCard = Object.Instantiate(BlankCard, base.transform.position, Quaternion.identity) as GameObject;
			NewCard.transform.parent = base.transform.parent.transform;
			NewCard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			NewCard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			NewCard.transform.localPosition = new Vector3(Random.Range(-515, 516), Random.Range(-270, 341), 100f);
			CardNumber++;
			Label.text = "Spawn Card " + (CardNumber + 1);
		}
		else if (ID != 7)
		{
		}
	}
}
