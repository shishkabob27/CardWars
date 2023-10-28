using UnityEngine;

public class CWResurrectPlayer : MonoBehaviour
{
	public GameObject noGemTween;

	public GameObject reshuffleTween;

	public CWUpdatePlayerData PlayerData;

	public BattleJukeboxScript jukeBox;

	private void OnEnable()
	{
	}

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (instance.Gems <= 0)
		{
			noGemTween.SendMessage("OnClick");
			return;
		}
		GameDataScript.GetInstance().GameOver = false;
		reshuffleTween.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		CWBattleSequenceController instance2 = CWBattleSequenceController.GetInstance();
		instance2.camAlignFlag = false;
		QuestEarningManager instance3 = QuestEarningManager.GetInstance();
		Singleton<AnalyticsManager>.Instance.LogResurrectPurchase(instance3.earnedCards, instance3.earnedCoin);
		if (jukeBox != null)
		{
			jukeBox.Refresh();
		}
	}
}
