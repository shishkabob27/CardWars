using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using UnityEngine;

public class VOManager : ILoadable
{
	private const string VOFileName = "db_CharacterVO.json";

	private static VOManager instance;

	private Dictionary<VOEvent, VOControl> VOControls = new Dictionary<VOEvent, VOControl>();

	private VOController[] VOControllers = new VOController[2];

	public bool IsPlaying
	{
		get
		{
			VOController[] vOControllers = VOControllers;
			foreach (VOController vOController in vOControllers)
			{
				if (vOController != null && vOController.IsPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static VOManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new VOManager();
			}
			return instance;
		}
	}

	public Dictionary<string, object>[] LoadVOData()
	{
		string text = Path.Combine("Blueprints", "db_CharacterVO.json");
		TFUtils.DebugLog("VOManager VOData path: " + text);
		string jsonFileContent = TFUtils.GetJsonFileContent(text);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] array = LoadVOData();
		foreach (Dictionary<string, object> dict in array)
		{
			string EventID = TFUtils.LoadString(dict, "EventID");
			VOEvent voe;
			try
			{
				voe = (VOEvent)(int)Enum.Parse(typeof(VOEvent), EventID, true);
			}
			catch
			{
				continue;
			}
			VOControl Control = new VOControl
			{
				VOE = voe,
				ActivateChance = TFUtils.LoadFloat(dict, "ActivateChance", 0f)
			};
			Control.IsActive[(int)PlayerType.User] = TFUtils.LoadBool(dict, "Player", true);
			Control.IsActive[(int)PlayerType.Opponent] = TFUtils.LoadBool(dict, "Opponent", false);
			VOControls.Add(Control.VOE, Control);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	public void Destroy()
	{
		instance = null;
	}

	public VOControl GetVOControl(VOEvent voe)
	{
		return VOControls[voe];
	}

	public void AddVOController(PlayerType Owner, VOController voc)
	{
		VOControllers[(int)Owner] = voc;
	}

	public void PlayEvent(PlayerType Owner, VOEvent VOE)
	{
		VOController vOController = VOControllers[(int)Owner];
		if (!(vOController != null) || IsPlaying || !VOControls.ContainsKey(VOE))
		{
			return;
		}
		VOControl vOControl = VOControls[VOE];
		if (vOControl.IsActive[(int)Owner])
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			if (num < vOControl.ActivateChance)
			{
				vOController.PlayEvent(VOE);
			}
		}
	}
}
