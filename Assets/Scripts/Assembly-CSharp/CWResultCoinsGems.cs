using UnityEngine;

public class CWResultCoinsGems : MonoBehaviour
{
	public UILabel CoinsLabel;

	public UILabel GemsLabel;

	public UILabel XPLabel;

	private void OnEnable()
	{
		UpdateText();
	}

	public void UpdateText()
	{
		GameDataScript instance = GameDataScript.GetInstance();
		QuestData activeQuest = GameState.Instance.ActiveQuest;
		CoinsLabel.text = instance.P1_CoinsEarned.ToString();
		GemsLabel.text = "0";
		XPLabel.text = activeQuest.XPRewarded.ToString();
	}
}
