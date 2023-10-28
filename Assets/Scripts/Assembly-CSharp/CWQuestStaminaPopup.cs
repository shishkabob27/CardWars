using UnityEngine;

public class CWQuestStaminaPopup : MonoBehaviour
{
	public GameObject ButtonOK;

	private void OnEnable()
	{
		if (!(ButtonOK == null))
		{
			bool flag = PlayerInfoScript.GetInstance().Gems > 0;
			float val = ((!flag) ? 0.4f : 1f);
			SQUtils.SetGray(ButtonOK, val);
			ButtonOK.GetComponent<Collider>().enabled = flag;
		}
	}
}
