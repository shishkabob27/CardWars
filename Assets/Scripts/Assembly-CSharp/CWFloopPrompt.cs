using UnityEngine;

public class CWFloopPrompt : MonoBehaviour
{
	public UILabel Prompt;

	public GameObject CurrentQuestInfo;

	private void OnEnable()
	{
		if (Prompt != null)
		{
			Prompt.text = KFFLocalization.Get("!!FLOOP");
		}
	}

	public void SetFloopPrompt(CardItem card)
	{
		if (Prompt != null && card != null)
		{
			Prompt.text = KFFLocalization.Get("!!BS_Q_FLOOPTHE") + " " + card.Form.ShortHand + "?";
		}
		else if (Prompt != null)
		{
			Prompt.text = KFFLocalization.Get("!!FLOOP");
		}
	}

	private void Update()
	{
		if (PauseMenu.pauseMenuShown || CurrentQuestInfo.activeInHierarchy)
		{
			GameObject gameObject = base.gameObject.transform.Find("No").gameObject;
			if ((bool)gameObject)
			{
				gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
