using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class CharacterDataManager : ILoadable
{
	private const string CharacterFileName = "db_Characters.json";

	private static CharacterDataManager instance;

	private Dictionary<string, CharacterData> Characters = new Dictionary<string, CharacterData>();

	public bool Loaded;

	public static CharacterDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CharacterDataManager();
			}
			return instance;
		}
	}

	public Dictionary<string, object>[] LoadCharacterData()
	{
		string text = Path.Combine("Blueprints", "db_Characters.json");
		TFUtils.DebugLog("CharacterData path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] array = LoadCharacterData();
		foreach (Dictionary<string, object> dict in array)
		{
			CharacterData Data = new CharacterData
			{
				ID = TFUtils.LoadString(dict, "ID"),
				Name = TFUtils.LoadLocalizedString(dict, "Name"),
				Prefab = TFUtils.LoadString(dict, "Prefab"),
				ObjectName = TFUtils.LoadString(dict, "ObjectName"),
				PortraitSprite = TFUtils.LoadString(dict, "Portrait"),
				PortraitAtlas = TFUtils.LoadString(dict, "PortraitAtlas", null),
				Mouth = TFUtils.LoadString(dict, "Mouth"),
				Hand = TFUtils.LoadString(dict, "Hand"),
				IdleAnim = TFUtils.LoadString(dict, "Idle"),
				FidgetAnim = TFUtils.LoadString(dict, "Fidget"),
				CardIdleAnim = TFUtils.LoadString(dict, "CardIdle"),
				PlayCardAnim = TFUtils.LoadString(dict, "PlayCard"),
				PlayRareCardAnim = TFUtils.LoadString(dict, "PlayRare"),
				PlayRareLastCardAnim = TFUtils.LoadString(dict, "PlayRareLastCard"),
				LastCardAnim = TFUtils.LoadString(dict, "LastCard"),
				HappyAnim = TFUtils.LoadString(dict, "Happy"),
				SadAnim = TFUtils.LoadString(dict, "Sad"),
				DweebDrinkAnim = TFUtils.LoadString(dict, "DweebDrink"),
				IntroP1Anim = TFUtils.LoadString(dict, "IntroP1"),
				IntroP2Anim = TFUtils.LoadString(dict, "IntroP2"),
				DefeatedAnim = TFUtils.LoadString(dict, "Defeated"),
				UseChair = TFUtils.LoadBool(dict, "UseChair", true),
				ChairOffsetX = TFUtils.LoadFloat(dict, "ChairOffsetX"),
				ChairOffsetY = TFUtils.LoadFloat(dict, "ChairOffsetY"),
				CharacterOffsetX = TFUtils.LoadFloat(dict, "CharacterOffsetX"),
				CharacterOffsetY = TFUtils.LoadFloat(dict, "CharacterOffsetY")
			};
			try
			{
				Data.BattleMusic = TFUtils.LoadString(dict, "BattleMusic");
			}
			catch (Exception)
			{
				Data.BattleMusic = string.Empty;
			}
			Data.GoVO = TFUtils.LoadString(dict, "GoVO");
			Characters.Add(Data.ID, Data);
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

	public CharacterData GetCharacterData(string id)
	{
		return Characters[id];
	}
}
