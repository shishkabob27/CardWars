using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : ILoadable
{
	public Dictionary<Faction, FactionData> factions;

	private static FactionManager instance;

	public static FactionManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new FactionManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Faction.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		factions = new Dictionary<Faction, FactionData>();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> row in array)
		{
			FactionData fdata = new FactionData
			{
				FactionID = (Faction)(int)Enum.Parse(typeof(Faction), TFUtils.LoadString(row, "FactionID")),
				LandscapeName = TFUtils.LoadString(row, "LandscapeName"),
				CardFrameAtlas = TFUtils.LoadString(row, "CardFrameAtlas"),
				CardFrameSprite = TFUtils.LoadString(row, "CardFrameSprite"),
				LandscapeIcon = TFUtils.LoadString(row, "LandscapeIcon")
			};
			fdata.HighlightFX_Lane[0] = TFUtils.LoadString(row, "HighlightFX_Lane1");
			fdata.HighlightFX_Lane[1] = TFUtils.LoadString(row, "HighlightFX_Lane2");
			fdata.HighlightFX_Lane[2] = TFUtils.LoadString(row, "HighlightFX_Lane3");
			fdata.HighlightFX_Lane[3] = TFUtils.LoadString(row, "HighlightFX_Lane4");
			fdata.InvalidFX_Lane[0] = TFUtils.LoadString(row, "InvalidFX_Lane1");
			fdata.InvalidFX_Lane[1] = TFUtils.LoadString(row, "InvalidFX_Lane2");
			fdata.InvalidFX_Lane[2] = TFUtils.LoadString(row, "InvalidFX_Lane3");
			fdata.InvalidFX_Lane[3] = TFUtils.LoadString(row, "InvalidFX_Lane4");
			string colorRGB2 = TFUtils.LoadString(row, "FactionColor");
			string[] ar2 = colorRGB2.Split(',');
			fdata.FactionColor = new Color((float)int.Parse(ar2[0]) / 255f, (float)int.Parse(ar2[1]) / 255f, (float)int.Parse(ar2[2]) / 255f);
			string colorRGB = TFUtils.LoadString(row, "CardFrameColor", string.Empty);
			string[] ar = colorRGB.Split(',');
			if (ar.Length >= 3)
			{
				fdata.CardFrameColor = new Color((float)int.Parse(ar[0]) / 255f, (float)int.Parse(ar[1]) / 255f, (float)int.Parse(ar[2]) / 255f);
			}
			else
			{
				fdata.CardFrameColor = fdata.FactionColor;
			}
			factions.Add(fdata.FactionID, fdata);
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

	public FactionData GetFactionData(Faction fact)
	{
		if (factions.ContainsKey(fact))
		{
			return factions[fact];
		}
		return null;
	}

	public FactionData GetFactionData(string FactionName)
	{
		Faction fact = Faction.Universal;
		try
		{
			fact = (Faction)(int)Enum.Parse(typeof(Faction), FactionName);
		}
		catch
		{
		}
		return GetFactionData(fact);
	}

	public FactionData GetFactionData(LandscapeType type)
	{
		return GetFactionData(type.ToString());
	}
}
