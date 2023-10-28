using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestConditionManager : ILoadable
{
	public QuestStats currentStats;

	private Dictionary<string, QuestCondition> conditions;

	private static QuestConditionManager instance;

	public static QuestConditionManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new QuestConditionManager();
			}
			return instance;
		}
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_QuestConditions.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		conditions = new Dictionary<string, QuestCondition>();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			string key = TFUtils.LoadNullableString(dict, "ID");
			if (string.IsNullOrEmpty(key))
			{
				continue;
			}
			QuestCondition cond = new QuestCondition
			{
				ID = key,
				Description = TFUtils.LoadNullableString(dict, "Description")
			};
			if (cond.Description != null)
			{
				cond.Description = KFFLocalization.Get(cond.Description);
			}
			cond.QuestStatus = TFUtils.LoadNullableString(dict, "QuestStatus");
			cond.MaxTurns = TFUtils.TryLoadNullableInt(dict, "MaxTurns");
			cond.MinCreaturesDefeated = TFUtils.TryLoadNullableInt(dict, "MinCreaturesDefeated");
			cond.MaxActionPointsPerCard = TFUtils.TryLoadNullableInt(dict, "MaxActionPointsPerCard");
			cond.MaxActionPointsTotal = TFUtils.TryLoadNullableInt(dict, "MaxActionPointsTotal");
			cond.MaxHPLost = TFUtils.TryLoadNullableInt(dict, "MaxHPLost");
			cond.MaxNumCreaturesLost = TFUtils.TryLoadNullableInt(dict, "MaxNumCreaturesLost");
			cond.MinLandscapesUsed = TFUtils.TryLoadNullableInt(dict, "MinLandscapesUsed");
			cond.MaxDeckCost = TFUtils.TryLoadNullableInt(dict, "MaxDeckCost");
			cond.MaxFloopsUsed = TFUtils.TryLoadNullableInt(dict, "MaxFloopsUsed");
			char[] delimiterChars = new char[3] { ' ', ',', ';' };
			string tmp2 = TFUtils.LoadNullableString(dict, "AllowedTypes");
			if (!string.IsNullOrEmpty(tmp2))
			{
				string[] words2 = tmp2.Split(delimiterChars);
				cond.AllowedTypes = new bool[Enum.GetNames(typeof(CardType)).Length];
				string[] array2 = words2;
				foreach (string s2 in array2)
				{
					if (!string.IsNullOrEmpty(s2))
					{
						CardType val2 = (CardType)(int)Enum.Parse(typeof(CardType), s2, true);
						cond.AllowedTypes[(int)val2] = true;
					}
				}
			}
			tmp2 = TFUtils.LoadNullableString(dict, "AllowedFactions");
			if (!string.IsNullOrEmpty(tmp2))
			{
				string[] words = tmp2.Split(delimiterChars);
				cond.AllowedFactions = new bool[Enum.GetNames(typeof(Faction)).Length];
				string[] array3 = words;
				foreach (string s in array3)
				{
					if (!string.IsNullOrEmpty(s))
					{
						Faction val = (Faction)(int)Enum.Parse(typeof(Faction), s, true);
						cond.AllowedFactions[(int)val] = true;
					}
				}
			}
			conditions.Add(key, cond);
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

	public void ResetStats()
	{
		currentStats = new QuestStats();
	}

	public string ConditionDescription(string conditionID)
	{
		if (!conditions.ContainsKey(conditionID))
		{
			return string.Empty;
		}
		return conditions[conditionID].Description;
	}

	public void QuestConditionToStatus(string condId, out string status, out string passFail, out string condition, out Color fontColor)
	{
		if (condId == null)
		{
			status = string.Empty;
			condition = KFFLocalization.Get("!!QUESTCONDITION_CLEARED_MESSAGE");
			passFail = KFFLocalization.Get("!!QUESTCONDITION_PASS");
			fontColor = new Color(0f, 1f, 0f);
			return;
		}
		int amount;
		if (StatsMeetCondition(condId, out amount))
		{
			passFail = KFFLocalization.Get("!!QUESTCONDITION_PASS");
			fontColor = new Color(0f, 1f, 0f);
		}
		else
		{
			passFail = KFFLocalization.Get("!!QUESTCONDITION_FAIL");
			fontColor = new Color(1f, 0f, 0f);
		}
		if (conditions.ContainsKey(condId) && conditions[condId].QuestStatus != null)
		{
			status = string.Format(KFFLocalization.Get(conditions[condId].QuestStatus), amount);
		}
		else
		{
			status = string.Empty;
		}
		condition = ConditionDescription(condId);
	}

	public bool StatsMeetCondition(string conditionID)
	{
		int amount;
		return StatsMeetCondition(conditionID, out amount);
	}

	public bool StatsMeetCondition(string conditionID, out int amount)
	{
		amount = 0;
		if (!conditions.ContainsKey(conditionID))
		{
			return false;
		}
		if (currentStats == null)
		{
			return false;
		}
		QuestCondition questCondition = conditions[conditionID];
		if (questCondition.AllowedTypes != null)
		{
			for (int i = 0; i < questCondition.AllowedTypes.Length; i++)
			{
				if (!questCondition.AllowedTypes[i] && currentStats.UsedTypes[i] > 0)
				{
					amount += currentStats.UsedTypes[i];
				}
			}
		}
		if (amount > 0)
		{
			return false;
		}
		if (questCondition.AllowedFactions != null)
		{
			for (int j = 0; j < questCondition.AllowedFactions.Length; j++)
			{
				if (!questCondition.AllowedFactions[j] && currentStats.UsedFactions[j] > 0)
				{
					amount += currentStats.UsedFactions[j];
				}
			}
		}
		if (amount > 0)
		{
			return false;
		}
		bool result = true;
		int? maxTurns = questCondition.MaxTurns;
		if (maxTurns.HasValue)
		{
			int? maxTurns2 = questCondition.MaxTurns;
			if (maxTurns2.HasValue && currentStats.NumTurns > maxTurns2.Value)
			{
				result = false;
			}
			amount = currentStats.NumTurns;
		}
		int? minCreaturesDefeated = questCondition.MinCreaturesDefeated;
		if (minCreaturesDefeated.HasValue)
		{
			int? minCreaturesDefeated2 = questCondition.MinCreaturesDefeated;
			if (minCreaturesDefeated2.HasValue && currentStats.NumCreaturesDefeated < minCreaturesDefeated2.Value)
			{
				result = false;
			}
			amount = questCondition.MinCreaturesDefeated.Value;
		}
		int? maxActionPointsPerCard = questCondition.MaxActionPointsPerCard;
		if (maxActionPointsPerCard.HasValue)
		{
			int? maxActionPointsPerCard2 = questCondition.MaxActionPointsPerCard;
			if (maxActionPointsPerCard2.HasValue && currentStats.NumActionPointsPerCard > maxActionPointsPerCard2.Value)
			{
				result = false;
			}
			amount = questCondition.MaxActionPointsPerCard.Value;
		}
		int? maxActionPointsTotal = questCondition.MaxActionPointsTotal;
		if (maxActionPointsTotal.HasValue)
		{
			int? maxActionPointsTotal2 = questCondition.MaxActionPointsTotal;
			if (maxActionPointsTotal2.HasValue && currentStats.NumActionPointsTotal > maxActionPointsTotal2.Value)
			{
				result = false;
			}
			amount = questCondition.MaxActionPointsTotal.Value;
		}
		int? maxHPLost = questCondition.MaxHPLost;
		if (maxHPLost.HasValue)
		{
			int? maxHPLost2 = questCondition.MaxHPLost;
			if (maxHPLost2.HasValue && currentStats.HPLost > maxHPLost2.Value)
			{
				result = false;
				amount = questCondition.MaxHPLost.Value;
			}
			else
			{
				amount = currentStats.HPLost;
			}
		}
		int? maxNumCreaturesLost = questCondition.MaxNumCreaturesLost;
		if (maxNumCreaturesLost.HasValue)
		{
			int? maxNumCreaturesLost2 = questCondition.MaxNumCreaturesLost;
			if (maxNumCreaturesLost2.HasValue && currentStats.NumCreaturesLost > maxNumCreaturesLost2.Value)
			{
				result = false;
			}
			amount = questCondition.MaxNumCreaturesLost.Value;
		}
		int? minLandscapesUsed = questCondition.MinLandscapesUsed;
		if (minLandscapesUsed.HasValue)
		{
			int? minLandscapesUsed2 = questCondition.MinLandscapesUsed;
			if (minLandscapesUsed2.HasValue && currentStats.NumLandscapesUsed < minLandscapesUsed2.Value)
			{
				result = false;
				amount = currentStats.NumLandscapesUsed;
			}
			else
			{
				amount = questCondition.MinLandscapesUsed.Value;
			}
		}
		int? maxDeckCost = questCondition.MaxDeckCost;
		if (maxDeckCost.HasValue)
		{
			int? maxDeckCost2 = questCondition.MaxDeckCost;
			if (maxDeckCost2.HasValue && currentStats.DeckCost > maxDeckCost2.Value)
			{
				result = false;
			}
			amount = questCondition.MaxDeckCost.Value;
		}
		int? maxFloopsUsed = questCondition.MaxFloopsUsed;
		if (maxFloopsUsed.HasValue)
		{
			int? maxFloopsUsed2 = questCondition.MaxFloopsUsed;
			if (maxFloopsUsed2.HasValue && currentStats.NumFloopsUsed > maxFloopsUsed2.Value)
			{
				result = false;
			}
			amount = questCondition.MaxFloopsUsed.Value;
		}
		return result;
	}
}
