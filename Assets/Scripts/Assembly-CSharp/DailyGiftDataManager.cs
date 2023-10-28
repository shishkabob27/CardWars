using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class DailyGiftDataManager : ILoadable
{
	private const string DailyGiftFileName = "db_DailyGift.json";

	private const int kMaxDailyGifts = 12;

	private static DailyGiftDataManager instance;

	private DailyGiftWeightTable dailyGiftWeightTable = new DailyGiftWeightTable();

	private DailyGift[] DailyGifts = new DailyGift[12];

	private int numDailyGifts;

	private int _GemCost;

	public bool Loaded;

	public int RetyGemCost
	{
		get
		{
			return _GemCost;
		}
	}

	public static DailyGiftDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DailyGiftDataManager();
			}
			return instance;
		}
	}

	public Dictionary<string, object>[] LoadDailyGiftsData()
	{
		string filename = Path.Combine("Blueprints", "db_DailyGift.json");
		string jsonFileContent = TFUtils.GetJsonFileContent(filename);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		DailyGift CurrentGift2 = null;
		Dictionary<string, object>[] array = LoadDailyGiftsData();
		foreach (Dictionary<string, object> dict in array)
		{
			CurrentGift2 = new DailyGift
			{
				Type = TFUtils.LoadString(dict, "Type"),
				Quantity = TFUtils.LoadInt(dict, "Quantity"),
				Rarity = TFUtils.LoadInt(dict, "Rarity"),
				Name = TFUtils.LoadString(dict, "Name"),
				Icon = TFUtils.LoadString(dict, "Icon")
			};
			DailyGifts[numDailyGifts] = CurrentGift2;
			dailyGiftWeightTable.Add(numDailyGifts, CurrentGift2.Rarity);
			numDailyGifts++;
			if (numDailyGifts >= 12)
			{
				_GemCost = TFUtils.LoadInt(dict, "RetryGemCost");
				break;
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

	public int PickGift()
	{
		return dailyGiftWeightTable.Pick();
	}

	public DailyGift GetGift(int id)
	{
		if (id < numDailyGifts)
		{
			return DailyGifts[id];
		}
		return DailyGifts[0];
	}
}
