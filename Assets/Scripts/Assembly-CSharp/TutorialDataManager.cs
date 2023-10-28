using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

public class TutorialDataManager : ILoadable
{
	private const string TutorialFileName = "db_Tutorial.json";

	private static TutorialDataManager instance;

	private List<TutorialRule> TutorialRules = new List<TutorialRule>();

	public static TutorialDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new TutorialDataManager();
			}
			return instance;
		}
	}

	public Dictionary<string, object>[] LoadTutorialData()
	{
		string fname = Path.Combine("Blueprints", "db_Tutorial.json");
		fname = SessionManager.GetInstance().GetStreamingAssetsPath(fname);
		TFUtils.DebugLog("CardDataScript Tutorial path: " + fname);
		string jsonFileContent = TFUtils.GetJsonFileContent(fname);
		return JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFileContent);
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] array = LoadTutorialData();
		foreach (Dictionary<string, object> dict in array)
		{
			if (TFUtils.LoadInt(dict, "ID") == 0)
			{
				break;
			}
			TutorialRule Rule = new TutorialRule
			{
				Text = TFUtils.LoadString(dict, "Text"),
				Destination = TFUtils.LoadInt(dict, "Destination"),
				NextButton = TFUtils.LoadInt(dict, "NextButton"),
				ShowContinue = TFUtils.LoadInt(dict, "ShowContinue"),
				ArrowX = TFUtils.LoadInt(dict, "ArrowX"),
				ArrowY = TFUtils.LoadInt(dict, "ArrowY"),
				ArrowRotation = TFUtils.LoadInt(dict, "ArrowRotation")
			};
			TutorialRules.Add(Rule);
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

	public TutorialRule GetRule(int idx)
	{
		return TutorialRules[idx];
	}
}
