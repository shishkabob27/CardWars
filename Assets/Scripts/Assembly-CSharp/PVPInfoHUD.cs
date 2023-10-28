using UnityEngine;

public class PVPInfoHUD : MonoBehaviour
{
	public GameObject ParentObject;

	public UILabel PVPPlayerName;

	public UILabel PVPOpponentName;

	public UILabel PVPTrophyWin;

	public UILabel PVPTrophyLoss;

	private void OnEnable()
	{
		if (GlobalFlags.Instance.InMPMode)
		{
			if ((bool)ParentObject)
			{
				ParentObject.SetActive(true);
			}
			if ((bool)PVPPlayerName)
			{
				PVPPlayerName.text = CWMPMapController.GetInstance().mLastMPData.PlayerPVPName;
			}
			if ((bool)PVPOpponentName)
			{
				PVPOpponentName.text = CWMPMapController.GetInstance().mLastMPData.OpponentPVPName;
			}
			if ((bool)PVPTrophyWin)
			{
				PVPTrophyWin.text = CWMPMapController.GetInstance().mLastMPData.TrophyWin.ToString();
			}
			if ((bool)PVPTrophyLoss)
			{
				PVPTrophyLoss.text = CWMPMapController.GetInstance().mLastMPData.TrophyLoss.ToString();
			}
		}
	}
}
