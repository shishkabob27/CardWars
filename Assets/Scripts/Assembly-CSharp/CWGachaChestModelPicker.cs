using System;
using System.Collections.Generic;
using UnityEngine;

public class CWGachaChestModelPicker : MonoBehaviour
{
	[Serializable]
	public class ModelVariation
	{
		public string ID;

		public GameObject Model;

		public string Label;
	}

	public string DefaultVariation;

	public UILabel chestLabel;

	public List<ModelVariation> Variations;

	private void OnEnable()
	{
		PartyInfo currentPartyInfo = GachaManager.Instance.GetCurrentPartyInfo();
		bool flag = false;
		foreach (ModelVariation variation in Variations)
		{
			if (variation.Model == null)
			{
				continue;
			}
			if (currentPartyInfo != null && !flag && variation.ID == currentPartyInfo.id)
			{
				variation.Model.SetActive(true);
				if (!string.IsNullOrEmpty(variation.Label))
				{
					chestLabel.text = KFFLocalization.Get(variation.Label);
				}
				flag = true;
			}
			else
			{
				variation.Model.SetActive(false);
			}
		}
		if (flag)
		{
			return;
		}
		TFUtils.DebugLog("GachaChestModelPicker: Default to default variation - " + DefaultVariation);
		try
		{
			Variations.Find((ModelVariation elem) => elem.ID == DefaultVariation).Model.SetActive(true);
		}
		catch (NullReferenceException)
		{
			TFUtils.ErrorLog("GachaChestModelPicker: Cannot find default variation '" + DefaultVariation + "'");
		}
	}
}
