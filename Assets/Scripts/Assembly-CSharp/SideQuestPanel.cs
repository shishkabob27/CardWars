using UnityEngine;
using UnityEngine.UI;

public class SideQuestPanel : MonoBehaviour
{
	public Animator anim;

	public Button panelButton;

	public Image NPCSlot;

	public Text Dialog;

	public Image ItemSlot;

	public UGuiSpriteMap UIAtlas { get; set; }

	public SideQuestController SQController { get; set; }

	public void Show()
	{
		anim.SetBool("Visible", true);
		Reresh();
	}

	public void Hide()
	{
		anim.SetBool("Visible", false);
	}

	private void Reresh()
	{
		SideQuestData activeSideQuest = SQController.ActiveSideQuest;
		if (activeSideQuest != null)
		{
			string spriteName;
			string text;
			switch (PlayerInfoScript.GetInstance().GetSideQuestState(activeSideQuest))
			{
			case SideQuestProgress.SideQuestState.Accomplished:
				spriteName = activeSideQuest.NPCRewardIcon;
				text = activeSideQuest.RewardMessage;
				break;
			case SideQuestProgress.SideQuestState.Expired:
				spriteName = activeSideQuest.NPCFailIcon;
				text = activeSideQuest.FailMessage;
				break;
			default:
				spriteName = activeSideQuest.NPCIntroIcon;
				text = activeSideQuest.IntroMessage;
				break;
			}
			NPCSlot.sprite = UIAtlas.GetSprite(spriteName);
			Dialog.text = text;
			ItemSlot.sprite = UIAtlas.GetSprite(activeSideQuest.CollectibleIcon);
			UGuiLocalizedText component = Dialog.GetComponent<UGuiLocalizedText>();
			if (component != null)
			{
				component.Refresh();
			}
		}
	}
}
