using UnityEngine;
using System;
using System.Collections.Generic;

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
	public List<CWGachaChestModelPicker.ModelVariation> Variations;
}
