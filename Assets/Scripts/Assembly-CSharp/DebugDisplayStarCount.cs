using UnityEngine;

public class DebugDisplayStarCount : MonoBehaviour
{
	private UILabel displayLabel;

	private PlayerInfoScript pInfo;

	private CWMapQuestInfoSet questInfoSet;

	private void Start()
	{
		displayLabel = GetComponent<UILabel>();
		pInfo = PlayerInfoScript.GetInstance();
		questInfoSet = base.transform.parent.GetComponent<CWMapQuestInfoSet>();
	}

	private void Update()
	{
		displayLabel.text = pInfo.GetQuestProgress(questInfoSet.questData).ToString();
	}
}
