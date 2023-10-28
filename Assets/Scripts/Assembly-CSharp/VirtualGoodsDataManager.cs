using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonFx.Json;

public class VirtualGoodsDataManager : ILoadable
{
	private const string kVirtualGoodsFileName = "db_VirtualGoods.json";

	private static readonly VirtualGoods kEmptyTemplate = new VirtualGoods
	{
		ProductID = null,
		Gems = 0,
		Hearts = 0,
		Coins = 0,
		Inventory = 0,
		OccuranceStrings = new List<string>(),
		Cards = new List<string>()
	};

	private static VirtualGoodsDataManager instance = null;

	public Dictionary<string, VirtualGoods> VirtualGoodsData = new Dictionary<string, VirtualGoods>();

	public static VirtualGoodsDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new VirtualGoodsDataManager();
			}
			return instance;
		}
	}

	public bool Loaded { get; private set; }

	public VirtualGoodsDataManager()
	{
		Loaded = false;
	}

	private Dictionary<string, object>[] LoadData()
	{
		string text = Path.Combine("Blueprints", "db_VirtualGoods.json");
		TFUtils.DebugLog("Loading VirtualGoods data from path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = LoadData();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			VirtualGoods vg;
			try
			{
				vg = new VirtualGoods
				{
					ProductID = TFUtils.LoadString(dict, "ProductID_Android", null),
					Gems = TFUtils.LoadInt(dict, "Gems", 0),
					Hearts = TFUtils.LoadInt(dict, "Hearts", 0),
					Coins = TFUtils.LoadInt(dict, "Coins", 0),
					Inventory = TFUtils.LoadInt(dict, "Inventory", 0)
				};
				int ithCard = 1;
				string cardName = string.Empty;
				while (true)
				{
					string value;
					cardName = (value = TFUtils.LoadString(dict, "Card" + ithCard++, null));
					if (string.IsNullOrEmpty(value))
					{
						break;
					}
					vg.Cards.Add(cardName);
				}
				int ithOccuranceString = 1;
				string occuranceString2 = string.Empty;
				while (true)
				{
					string value;
					occuranceString2 = (value = TFUtils.LoadString(dict, "OccuranceString" + ithOccuranceString++, null));
					if (!string.IsNullOrEmpty(value))
					{
						vg.OccuranceStrings.Add(occuranceString2);
						continue;
					}
					break;
				}
			}
			catch (Exception)
			{
				TFUtils.WarnLog("Error parsing VirtualGoods data: " + dict.ToString());
				continue;
			}
			string key = vg.ProductID;
			if (!string.IsNullOrEmpty(key))
			{
				VirtualGoodsData.Add(key, vg);
			}
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		Loaded = true;
	}

	public VirtualGoods GetVirtualGoods(string purchaseID)
	{
		VirtualGoods value = null;
		return (!VirtualGoodsData.TryGetValue(purchaseID, out value)) ? null : value;
	}

	public List<string> GetPurchaseIds()
	{
		return (VirtualGoodsData == null) ? new List<string>() : VirtualGoodsData.Keys.ToList();
	}

	public void Destroy()
	{
		instance = null;
	}
}
