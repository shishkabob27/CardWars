using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager
{
	public delegate void OnFinishedDelegate();

	private const float PROCESSTIME_MIN = 0.015f;

	private static LoadingManager instance;

	private static float lastYieldTime;

	private static float lastProcessTime = -1f;

	private List<ILoadable> LoadableList;

	public static LoadingManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new LoadingManager();
			}
			return instance;
		}
	}

	private LoadingManager()
	{
		LoadableList = new List<ILoadable>();
		LoadableList.Add(ParametersManager.Instance);
		LoadableList.Add(FactionManager.Instance);
		LoadableList.Add(CharacterDataManager.Instance);
		LoadableList.Add(CardDataManager.Instance);
		LoadableList.Add(CardBoxManager.Instance);
		LoadableList.Add(AIDeckManager.Instance);
		LoadableList.Add(QuestManager.Instance);
		LoadableList.Add(SideQuestManager.Instance);
		LoadableList.Add(XPManager.Instance);
		LoadableList.Add(RankManager.Instance);
		LoadableList.Add(FusionManager.Instance);
		LoadableList.Add(GachaManager.Instance);
		LoadableList.Add(LeaderManager.Instance);
		LoadableList.Add(QuestConditionManager.Instance);
		LoadableList.Add(RPSMatrix.Instance);
		LoadableList.Add(TutorialManager.Instance);
		LoadableList.Add(VOManager.Instance);
		LoadableList.Add(TipManager.Instance);
		LoadableList.Add(Singleton<TournamentManager>.Instance);
		LoadableList.Add(Singleton<CodeRedemptionManager>.Instance);
		LoadableList.Add(DungeonDataManager.Instance);
		LoadableList.Add(DailyGiftDataManager.Instance);
		LoadableList.Add(CalendarGiftDataManager.Instance);
		LoadableList.Add(ScheduleDataManager.Instance);
		LoadableList.Add(KeyRingDataManager.Instance);
		LoadableList.Add(VirtualGoodsDataManager.Instance);
		LoadableList.Add(DropProfileDataManager.Instance);
		LoadableList.Add(ElFistoDataManager.Instance);
	}

	public static bool ShouldYield()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (lastProcessTime < 0f)
		{
			lastProcessTime = realtimeSinceStartup;
			return false;
		}
		float num = realtimeSinceStartup - lastProcessTime;
		float num2 = ((!KFFLODManager.IsLowEndDevice()) ? 0.03f : 0.1f);
		bool flag = num >= 0.015f && realtimeSinceStartup - lastYieldTime > num2;
		if (flag)
		{
			lastYieldTime = realtimeSinceStartup;
			lastProcessTime = -1f;
		}
		return flag;
	}

	public void Add(ILoadable item)
	{
		LoadableList.Add(item);
	}

	public IEnumerator LoadAll(OnFinishedDelegate callback)
	{
		foreach (ILoadable ldr in LoadableList)
		{
			if (ldr != null)
			{
				IEnumerator current = ldr.Load();
				while (current.MoveNext())
				{
					yield return current.Current;
				}
			}
		}
		if (callback != null)
		{
			callback();
		}
	}

	public void Clear()
	{
		foreach (ILoadable loadable in LoadableList)
		{
			loadable.Destroy();
		}
		instance = null;
	}
}
