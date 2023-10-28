using UnityEngine;
using UnityEngine.UI;

public class SideQuestHud : MonoBehaviour
{
	private const int BUTTON_STATE_PENDING = 0;

	private const int BUTTON_STATE_IN_PROGRESS = 1;

	private const int BUTTON_STATE_ACCOMPLISHED = 2;

	private const int BUTTON_STATE_EXPIRED = 3;

	public Image ItemSlot;

	public Text ItemCount;

	public Animator anim;

	public SideQuestController SQController { get; set; }

	public UGuiSpriteMap UIAtlas { get; set; }

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		if (!(UIAtlas != null))
		{
			return;
		}
		SideQuestData activeSideQuest = SQController.ActiveSideQuest;
		SideQuestProgress sideQuestProgress = PlayerInfoScript.GetInstance().GetSideQuestProgress(activeSideQuest);
		if (sideQuestProgress != null)
		{
			if (ItemSlot != null)
			{
				ItemSlot.sprite = UIAtlas.GetSprite(activeSideQuest.CollectibleIcon);
			}
			if (ItemCount != null)
			{
				ItemCount.text = string.Format("{0}/{1}", sideQuestProgress.Collected, activeSideQuest.NumCollectibles);
			}
			switch (sideQuestProgress.State)
			{
			case SideQuestProgress.SideQuestState.Pending:
				anim.SetInteger("State", 0);
				break;
			case SideQuestProgress.SideQuestState.InProgress:
				anim.SetInteger("State", 1);
				break;
			case SideQuestProgress.SideQuestState.Accomplished:
				anim.SetInteger("State", 2);
				break;
			case SideQuestProgress.SideQuestState.Expired:
				anim.SetInteger("State", 3);
				break;
			default:
				Hide();
				break;
			}
		}
	}

	public bool IsVisible()
	{
		return anim.GetBool("Visible");
	}

	public void Hide()
	{
		anim.SetBool("Visible", false);
	}

	public void Show()
	{
		anim.SetBool("Visible", true);
		Refresh();
	}
}
