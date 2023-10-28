using UnityEngine;

public class CheatScript : MonoBehaviour
{
	public int ID;

	private void OnClick()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if (ID == 1)
		{
			instance.Coins += 1000;
		}
		else if (ID == 2)
		{
			instance.Gems += 1000;
		}
	}
}
