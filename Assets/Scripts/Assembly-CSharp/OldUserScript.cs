using UnityEngine;

public class OldUserScript : MonoBehaviour
{
	public UILabel Label;

	private void Start()
	{
		Label.text = string.Format(KFFLocalization.Get("!!FORMAT_OLD_USER"), PlayerInfoScript.GetInstance().PlayerName);
	}
}
