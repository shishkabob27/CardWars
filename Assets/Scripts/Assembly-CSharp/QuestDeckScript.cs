using UnityEngine;

public class QuestDeckScript : MonoBehaviour
{
	private void Update()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		UILabel componentInChildren = base.gameObject.GetComponentInChildren<UILabel>();
		if (instance != null && componentInChildren != null)
		{
			componentInChildren.text = string.Format(KFFLocalization.Get("!!FORMAT_QUESTDECKSCRIPT"), instance.SelectedDeck + 1);
		}
	}
}
