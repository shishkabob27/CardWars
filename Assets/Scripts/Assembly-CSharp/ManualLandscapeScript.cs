using System;
using System.Collections.Generic;
using UnityEngine;

public class ManualLandscapeScript : MonoBehaviour
{
	public List<ManualLandscapeToggle> LandscapeCard;

	private static ManualLandscapeScript ml_instance;

	private string sprite_Corn = "Landscape_Corn1";

	private string sprite_Cotton = "Landscape_Cotton1";

	private string sprite_Plains = "Landscape_Plains1";

	private string sprite_Swamp = "Landscape_Swamp1";

	private string sprite_Sand = "Landscape_Sand1";

	private LandscapeType lane1;

	private LandscapeType lane2;

	private LandscapeType lane3;

	private LandscapeType lane4;

	private void Awake()
	{
		ml_instance = this;
	}

	private void OnEnable()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck selectedDeck = instance.GetSelectedDeck();
		try
		{
			lane1 = selectedDeck.GetLandscape(0);
			lane2 = selectedDeck.GetLandscape(1);
			lane3 = selectedDeck.GetLandscape(2);
			lane4 = selectedDeck.GetLandscape(3);
		}
		catch (NullReferenceException)
		{
			lane1 = LandscapeType.Corn;
			lane2 = LandscapeType.Corn;
			lane3 = LandscapeType.Corn;
			lane4 = LandscapeType.Corn;
		}
		foreach (ManualLandscapeToggle item in LandscapeCard)
		{
			Init(item);
		}
	}

	public void Init(ManualLandscapeToggle toggle)
	{
		LandscapeType landscapeType = ((toggle.LaneID == 1) ? lane1 : ((toggle.LaneID == 2) ? lane2 : ((toggle.LaneID == 3) ? lane3 : ((toggle.LaneID != 4) ? LandscapeType.None : lane4))));
		switch (landscapeType)
		{
		case LandscapeType.Corn:
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Corn;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Corn;
			}
			break;
		case LandscapeType.Cotton:
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Cotton;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Cotton;
			}
			break;
		case LandscapeType.Swamp:
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Swamp;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Swamp;
			}
			break;
		case LandscapeType.Plains:
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Plains;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Plains;
			}
			break;
		case LandscapeType.Sand:
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Sand;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Sand;
			}
			break;
		}
		if (toggle.LandscapeFrame != null)
		{
			toggle.LandscapeFrame.color = GetFactionColor(landscapeType);
		}
	}

	public static ManualLandscapeScript GetInstance()
	{
		return ml_instance;
	}

	public void ToggleLandscape(ManualLandscapeToggle toggle)
	{
		LandscapeType landscapeType = ((toggle.LaneID == 1) ? lane1 : ((toggle.LaneID == 2) ? lane2 : ((toggle.LaneID == 3) ? lane3 : ((toggle.LaneID != 4) ? LandscapeType.None : lane4))));
		switch (landscapeType)
		{
		case LandscapeType.Corn:
			landscapeType = LandscapeType.Cotton;
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Cotton;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Cotton;
			}
			break;
		case LandscapeType.Cotton:
			landscapeType = LandscapeType.Swamp;
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Swamp;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Swamp;
			}
			break;
		case LandscapeType.Swamp:
			landscapeType = LandscapeType.Plains;
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Plains;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Plains;
			}
			break;
		case LandscapeType.Plains:
			landscapeType = LandscapeType.Sand;
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Sand;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Sand;
			}
			break;
		case LandscapeType.Sand:
			landscapeType = LandscapeType.Corn;
			if (toggle.LandscapeArt != null)
			{
				toggle.LandscapeArt.spriteName = sprite_Corn;
			}
			if (toggle.LandscapeName != null)
			{
				toggle.LandscapeName.text = LandscapePreviewScript.name_Corn;
			}
			break;
		}
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		Deck selectedDeck = instance.GetSelectedDeck();
		if (toggle.LaneID == 1)
		{
			lane1 = landscapeType;
			selectedDeck.SetLandscape(0, lane1);
		}
		else if (toggle.LaneID == 2)
		{
			lane2 = landscapeType;
			selectedDeck.SetLandscape(1, lane2);
		}
		else if (toggle.LaneID == 3)
		{
			lane3 = landscapeType;
			selectedDeck.SetLandscape(2, lane3);
		}
		else if (toggle.LaneID == 4)
		{
			lane4 = landscapeType;
			selectedDeck.SetLandscape(3, lane4);
		}
		if (toggle.LandscapeFrame != null)
		{
			toggle.LandscapeFrame.color = GetFactionColor(landscapeType);
		}
	}

	private Color GetFactionColor(LandscapeType t)
	{
		return FactionManager.Instance.GetFactionData(t).FactionColor;
	}
}
