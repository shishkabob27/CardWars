using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DungeonStageItem : MonoBehaviour
{
	public GameObject ButtonStart;

	public GameObject WidgetHighlight;

	public GameObject WidgetBlackout;

	public GameObject ButtonLocked;

	public UILabel LabelGemCost;

	public UILabel LabelIndex;

	public UILabel LabelStatus;

	public UISprite Icon;

	public AudioClip LockedAudio;

	public AudioClip UnlockedAudio;

	public DungeonData.Quest Stage { get; private set; }

	public QuestData QuestData { get; private set; }

	public bool StageUnavailable { get; private set; }

	[method: MethodImpl(32)]
	public event Action<DungeonStageItem> OnSelectEvent;

	public void SetData(DungeonData.Quest stage)
	{
		Stage = stage;
		QuestData = QuestManager.Instance.GetDungeonQuest(stage.ID);
		LabelGemCost.text = stage.HeartCost.ToString();
		string text = KFFLocalization.Get("!!AREA_INDEX");
		text = text.Split()[0];
		LabelIndex.text = text + " " + stage.Index;
		Icon.spriteName = LeaderManager.Instance.GetMPLeaderPortrait(QuestData.LeaderID);
		UpdateStatus();
	}

	public void SetHighlighted(bool highlighted)
	{
		WidgetHighlight.SetActive(highlighted);
	}

	private void UpdateStatus()
	{
		bool flag = Stage.HeartCost > PlayerInfoScript.GetInstance().Stamina;
		bool flag2 = LeaderManager.Instance.IsLeaderFromFC(QuestData.LeaderID);
		Deck selectedDeckCopy = PlayerInfoScript.GetInstance().GetSelectedDeckCopy();
		bool flag3 = selectedDeckCopy.Leader.Form.FCWorld != flag2;
		StageUnavailable = Stage.Locked || flag || flag3;
		ButtonStart.SetActive(!Stage.Locked);
		ButtonLocked.SetActive(Stage.Locked);
		string key = "!!STATUS_READY";
		if (Stage.Locked)
		{
			key = string.Empty;
		}
		else if (flag)
		{
			key = "!!_NOT_ENOUGH_MAX_STAMINA";
		}
		else if (flag3)
		{
			key = "!!QM_P_FCLEADER_POPUP_DESC";
		}
		else if (Stage.Completed)
		{
			key = "!!CLEAR";
		}
		LabelStatus.text = KFFLocalization.Get(key);
		WidgetBlackout.SetActive(StageUnavailable);
		UIButtonSound uIButtonSound = GetComponent(typeof(UIButtonSound)) as UIButtonSound;
		uIButtonSound.audioClip = ((!StageUnavailable) ? UnlockedAudio : LockedAudio);
	}

	private void OnClick()
	{
		if (this.OnSelectEvent != null)
		{
			this.OnSelectEvent(this);
		}
	}
}
