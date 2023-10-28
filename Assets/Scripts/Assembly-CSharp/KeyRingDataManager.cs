using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class KeyRingDataManager : ILoadable
{
	private const string KeyRingFileName = "db_KeyRing.json";

	private static KeyRingDataManager instance;

	private Dictionary<string, KeyRingItem> KeyRingItems;

	public bool Loaded;

	public static KeyRingDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new KeyRingDataManager();
			}
			return instance;
		}
	}

	public Dictionary<string, KeyRingItem> GetKeyRingItems()
	{
		return KeyRingItems;
	}

	public KeyRingItem GetKeyRingItem(string Type)
	{
		KeyRingItem value;
		if (KeyRingItems.TryGetValue(Type, out value))
		{
			return value;
		}
		return null;
	}

	public Dictionary<string, object>[] LoadKeyRingData()
	{
		string filename = Path.Combine("Blueprints", "db_KeyRing.json");
		string jsonFileContent = TFUtils.GetJsonFileContent(filename);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		KeyRingItems = new Dictionary<string, KeyRingItem>();
		KeyRingItem CurrentItem = null;
		Dictionary<string, object>[] array = LoadKeyRingData();
		foreach (Dictionary<string, object> dict in array)
		{
			CurrentItem = new KeyRingItem
			{
				Type = TFUtils.LoadString(dict, "Type"),
				Name = TFUtils.LoadString(dict, "Name"),
				Info = TFUtils.LoadString(dict, "Info"),
				Icon = TFUtils.LoadString(dict, "Icon"),
				GachaColumn = TFUtils.LoadString(dict, "GachaColumn")
			};
			string specialCards = TFUtils.TryLoadString(dict, "SpecialCards");
			if (!string.IsNullOrEmpty(specialCards))
			{
				CurrentItem.SpecialCards = specialCards.Split(',');
				for (int i = 0; i < CurrentItem.SpecialCards.Length; i++)
				{
					CurrentItem.SpecialCards[i] = CurrentItem.SpecialCards[i].Trim();
				}
			}
			KeyRingItems.Add(CurrentItem.Type, CurrentItem);
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
