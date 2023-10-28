using UnityEngine;

public class UserIDScript : MonoBehaviour
{
	public UILabel IDLabel;

	private void OnEnable()
	{
		if (IDLabel != null)
		{
			IDLabel.text = PlayerInfoScript.GetInstance().GetPlayerCode();
		}
	}
}
