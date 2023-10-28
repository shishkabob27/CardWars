#define ASSERTS_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class ElFistoDataManager : ILoadable
{
	private const string kElFistoFileName = "db_ElFisto.json";

	private static readonly ElFisto kEmptyTemplate = new ElFisto
	{
		Round = -1,
		QuestID = string.Empty,
		RewardType = string.Empty,
		RewardName = string.Empty,
		RewardQuantity = 0,
		RewardIcon = string.Empty,
		Portrait = string.Empty,
		ChanceToAppear = 0f,
		BattlesNeeded = 0
	};

	private List<ElFisto> elFistos;

	private static ElFistoDataManager instance = null;

	public static ElFistoDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ElFistoDataManager();
			}
			return instance;
		}
	}

	public bool Loaded { get; private set; }

	public ElFistoDataManager()
	{
		Loaded = false;
	}

	private ElFisto GetElFistoByIndex(int index)
	{
		if (elFistos != null && index < elFistos.Count && index >= 0)
		{
			return elFistos[index];
		}
		return null;
	}

	public ElFisto GetElFistoByRound(int round)
	{
		return GetElFistoByIndex(round - 1);
	}

	public int GetNumRounds()
	{
		return (elFistos != null) ? elFistos.Count : 0;
	}

	private Dictionary<string, object>[] LoadData()
	{
		string text = Path.Combine("Blueprints", "db_ElFisto.json");
		TFUtils.DebugLog("Loading ElFisto data from path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		elFistos = new List<ElFisto>();
		Dictionary<string, object>[] data = LoadData();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			int lastRound2 = 0;
			ElFisto ef2 = null;
			try
			{
				ef2 = new ElFisto
				{
					Round = TFUtils.LoadInt(dict, "Round", -1),
					QuestID = TFUtils.LoadString(dict, "QuestID", string.Empty),
					RewardType = TFUtils.LoadString(dict, "RewardType", string.Empty),
					RewardName = TFUtils.LoadString(dict, "RewardName", string.Empty),
					RewardQuantity = TFUtils.LoadInt(dict, "RewardQuantity", 0),
					RewardIcon = TFUtils.LoadString(dict, "RewardIcon", string.Empty),
					Portrait = TFUtils.LoadString(dict, "Portrait", string.Empty),
					ChanceToAppear = TFUtils.LoadFloat(dict, "ChanceToAppear", 0f),
					BattlesNeeded = TFUtils.LoadInt(dict, "BattlesNeeded", 0)
				};
				TFUtils.Assert(ef2.Round > lastRound2, "Out of order entries in db_ElFisto.json, make sure the Rounds are sorted ascending");
				lastRound2 = ef2.Round;
				elFistos.Add(ef2);
			}
			catch (Exception)
			{
				TFUtils.WarnLog("Error parsing DropProfile data: " + dict.ToString());
				continue;
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Loaded = true;
	}

	public void Destroy()
	{
		instance = null;
	}
}
