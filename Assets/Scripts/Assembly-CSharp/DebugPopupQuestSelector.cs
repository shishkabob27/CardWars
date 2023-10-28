using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(QuestLaunchHelper))]
public class DebugPopupQuestSelector : MonoBehaviour
{
	private const int kProcessButtonCountMaxYield = 50;

	public GameObject questButtonPrefab;

	public CanvasRenderer panelLevels;

	public Button buttonClose;

	private bool isReady;

	private void Start()
	{
		if (buttonClose != null)
		{
			buttonClose.onClick.AddListener(OnClose);
		}
	}

	private void OnEnable()
	{
		if (!isReady)
		{
			StartCoroutine(SetupQuestButtons());
		}
	}

	private IEnumerator SetupQuestButtons()
	{
		while (!PlayerInfoScript.GetInstance().IsReady())
		{
			yield return null;
		}
		int processCount = 0;
		foreach (KeyValuePair<string, List<QuestData>> mapEntry in QuestManager.Instance.questMap)
		{
			if (mapEntry.Key.Equals("dungeon"))
			{
				continue;
			}
			foreach (QuestData quest in mapEntry.Value)
			{
				processCount++;
				if (processCount % 50 == 0)
				{
					yield return null;
				}
				AddQuestButton(mapEntry.Key, quest.QuestID);
			}
		}
		isReady = true;
	}

	private void AddQuestButton(string questType, string questId)
	{
		GameObject gameObject = NGUITools.AddChild(panelLevels.gameObject, questButtonPrefab);
		Button componentInChildren = gameObject.GetComponentInChildren<Button>();
		componentInChildren.onClick.AddListener(delegate
		{
			LaunchQuest(questType, questId);
		});
		Text componentInChildren2 = componentInChildren.GetComponentInChildren<Text>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.text = questType + ": " + questId;
		}
	}

	private void LaunchQuest(string questType, string questId)
	{
		QuestLaunchHelper component = GetComponent<QuestLaunchHelper>();
		if (!component.isLaunching)
		{
			GlobalFlags.Instance.InMPMode = false;
			component.LaunchQuest(questId, PlayerInfoScript.GetInstance().GetSelectedDeckCopy());
		}
	}

	private void OnClose()
	{
		base.gameObject.SetActive(false);
	}
}
