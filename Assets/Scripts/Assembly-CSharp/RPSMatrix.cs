using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class RPSMatrix : ILoadable
{
	public float[,] Factors;

	private static RPSMatrix instance;

	public static RPSMatrix Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new RPSMatrix();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		string filePath = Path.Combine("Blueprints", "db_RPS.json");
		TFUtils.DebugLog("RPSMatrix: reading text from filePath");
		string json = TFUtils.GetJsonFileContent(filePath);
		Dictionary<string, object>[] data = JsonReader.Deserialize<Dictionary<string, object>[]>(json);
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		Factors = new float[6, 6];
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> row in array)
		{
			Faction f1 = (Faction)(int)Enum.Parse(typeof(Faction), TFUtils.LoadString(row, "Faction"), true);
			Faction f2 = (Faction)(int)Enum.Parse(typeof(Faction), TFUtils.LoadString(row, "Strong1"), true);
			float b = TFUtils.LoadFloat(row, "Bonus", 0f);
			Factors[(int)f1, (int)f2] = b;
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
		yield return null;
	}

	public void Destroy()
	{
		instance = null;
	}
}
