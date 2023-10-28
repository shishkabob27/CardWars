using System;
using System.Reflection;

public abstract class CardForm
{
	public string ID { get; set; }

	public string Name { get; set; }

	public CardType Type { get; set; }

	public string IconAtlas { get; set; }

	public string FrameAtlas { get; set; }

	public string SpriteName { get; set; }

	public string RawDescription { get; set; }

	public string CostDescription { get; set; }

	public abstract int BaseATK { get; set; }

	public abstract int BaseDEF { get; set; }

	public int BaseVal1 { get; set; }

	public int BaseVal2 { get; set; }

	public int BaseSalePrice { get; set; }

	public string FrameSpriteName { get; set; }

	public string ObjectName { get; set; }

	public string ScriptName { get; set; }

	public string ScriptVizName { get; set; }

	public Faction Faction { get; set; }

	public Quality Quality { get; set; }

	public AbilityType AbilityType { get; set; }

	public int Rarity { get; set; }

	public bool CanFuse { get; set; }

	public int Cost { get; set; }

	public int FloopCost { get; set; }

	public string ShortHand { get; set; }

	public Type GetScriptClass()
	{
		Type type = System.Type.GetType(ScriptName);
		if (type == null)
		{
			switch (Type)
			{
			case CardType.Creature:
				type = System.Type.GetType("CreatureScript");
				break;
			case CardType.Building:
				type = System.Type.GetType("BuildingScript");
				break;
			case CardType.Spell:
				type = System.Type.GetType("SpellScript");
				break;
			default:
				type = System.Type.GetType("CardScript");
				break;
			}
		}
		return type;
	}

	public bool CanPlay(PlayerType player, int lane)
	{
		Type scriptClass = GetScriptClass();
		MethodInfo method = scriptClass.GetMethod("CanPlay", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return (bool)method.Invoke(null, new object[3] { player, lane, this });
	}

	public int DetermineCost(PlayerType player)
	{
		Type scriptClass = GetScriptClass();
		MethodInfo method = scriptClass.GetMethod("DetermineCost", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return (int)method.Invoke(null, new object[2] { player, this });
	}

	public CardScript InstanceScript()
	{
		Type scriptClass = GetScriptClass();
		return Activator.CreateInstance(scriptClass) as CardScript;
	}

	public string GetCardQualityFrameSprite()
	{
		switch (Quality)
		{
		case Quality.Obsidian:
			return "Frame_Obsidian";
		case Quality.Halloween:
			return "Frame_Halloween";
		case Quality.Christmas:
			return "Frame_Christmas";
		case Quality.Gold:
			return "Frame_Gold";
		case Quality.FionnaCake:
			return "Frame_FC";
		default:
			return string.Empty;
		}
	}

	public string GetCardQualitySummonPanelSprite()
	{
		switch (Quality)
		{
		case Quality.Obsidian:
			return "SummonPanelObsidian";
		case Quality.Halloween:
			return "SummonPanelHalloween";
		case Quality.Christmas:
			return "SummonPanelChristmas";
		case Quality.Gold:
			return "SummonPanelGold";
		case Quality.FionnaCake:
			return "SummonPanelFC";
		default:
			return string.Empty;
		}
	}

	public bool HasGlimmer()
	{
		return Quality == Quality.Obsidian || Quality == Quality.Gold;
	}
}
