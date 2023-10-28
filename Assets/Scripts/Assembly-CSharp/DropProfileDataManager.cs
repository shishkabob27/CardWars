using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class DropProfileDataManager : ILoadable
{
	private const string kDropProfileFileName = "db_DropProfile.json";

	private static readonly DropProfile kEmptyTemplate = new DropProfile
	{
		DropProfileID = string.Empty,
		ChestDropPercentages = new List<float>(),
		CardWeights = new List<int>(),
		CoinDropPercentages = new List<float>(),
		CoinAmounts = new List<int>(),
		CoinWeights = new List<int>(),
		ItemDropPercentages = new List<float>()
	};

	private Dictionary<string, DropProfile> dropProfiles;

	private static DropProfileDataManager instance = null;

	public static DropProfileDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DropProfileDataManager();
			}
			return instance;
		}
	}

	public bool Loaded { get; private set; }

	public DropProfileDataManager()
	{
		Loaded = false;
	}

	public DropProfile GetDropProfile(string dropProfileId)
	{
		DropProfile value = null;
		return (!dropProfiles.TryGetValue(dropProfileId, out value)) ? null : value;
	}

	private Dictionary<string, object>[] LoadData()
	{
		string text = Path.Combine("Blueprints", "db_DropProfile.json");
		TFUtils.DebugLog("Loading DropProfile data from path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		dropProfiles = new Dictionary<string, DropProfile>();
		Dictionary<string, object>[] data = LoadData();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			DropProfile dp2 = null;
			try
			{
				dp2 = new DropProfile
				{
					DropProfileID = TFUtils.LoadString(dict, "DropProfileID", string.Empty)
				};
				if (string.IsNullOrEmpty(dp2.DropProfileID))
				{
					continue;
				}
				for (int n = 1; dict.ContainsKey("ChestDropPercentage" + n); n++)
				{
					dp2.ChestDropPercentages.Add(TFUtils.LoadFloat(dict, "ChestDropPercentage" + n, 0f));
				}
				for (int m = 1; dict.ContainsKey("CardWeight" + m); m++)
				{
					dp2.CardWeights.Add(TFUtils.LoadInt(dict, "CardWeight" + m, 0));
				}
				for (int l = 1; dict.ContainsKey("CoinDropPercentage" + l); l++)
				{
					dp2.CoinDropPercentages.Add(TFUtils.LoadFloat(dict, "CoinDropPercentage" + l, 0f));
				}
				for (int k = 1; dict.ContainsKey("CoinAmount" + k); k++)
				{
					dp2.CoinAmounts.Add(TFUtils.LoadInt(dict, "CoinAmount" + k, 0));
				}
				for (int j = 1; dict.ContainsKey("CoinWeight" + j); j++)
				{
					dp2.CoinWeights.Add(TFUtils.LoadInt(dict, "CoinWeight" + j, 0));
				}
				for (int i = 1; dict.ContainsKey("ItemDropPercentage" + i); i++)
				{
					dp2.ItemDropPercentages.Add(TFUtils.LoadFloat(dict, "ItemDropPercentage" + i, 0f));
				}
				dropProfiles.Add(dp2.DropProfileID, dp2);
				goto IL_03af;
			}
			catch (Exception)
			{
				TFUtils.WarnLog("Error parsing DropProfile data: " + dict.ToString());
			}
			continue;
			IL_03af:
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
