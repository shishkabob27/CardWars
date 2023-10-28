using UnityEngine;

public class TotalTrophies : MonoBehaviour
{
	public UILabel TrophyCount;

	private void Start()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		if ((bool)instance && (bool)TrophyCount)
		{
			TrophyCount.text = instance.TotalTrophies.ToString();
		}
	}

	private void Update()
	{
	}
}
