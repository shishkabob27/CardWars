using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LeaderManager : ILoadable
{
	public Dictionary<string, LeaderForm> leaderForms = new Dictionary<string, LeaderForm>();

	public List<LeaderItem> leaders = new List<LeaderItem>();

	private Dictionary<string, UIAtlas> uiAtlasMap = new Dictionary<string, UIAtlas>();

	private static LeaderManager instance;

	public static LeaderManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new LeaderManager();
			}
			return instance;
		}
	}

	public UIAtlas GetUiAtlas(string id)
	{
		return uiAtlasMap[id];
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Leaders.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		leaderForms.Clear();
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			string key = TFUtils.LoadString(dict, "ID");
			if (!string.IsNullOrEmpty(key))
			{
				LeaderForm newLeader = new LeaderForm
				{
					ID = key,
					Name = TFUtils.LoadLocalizedString(dict, "Name"),
					CharacterID = TFUtils.LoadString(dict, "CharacterID"),
					IconAtlas = TFUtils.LoadString(dict, "IconAtlas")
				};
				if (!uiAtlasMap.ContainsKey(newLeader.IconAtlas))
				{
					SLOTResoureRequest req = SLOTGameSingleton<SLOTResourceManager>.GetInstance().LoadResourceAsync(newLeader.IconAtlas, typeof(GameObject));
					yield return req.asyncOp;
					GameObject atlasGo = (GameObject)req.asset;
					UIAtlas uiAtlas = atlasGo.GetComponent<UIAtlas>();
					uiAtlasMap.Add(newLeader.IconAtlas, uiAtlas);
				}
				newLeader.SpriteName = TFUtils.LoadString(dict, "SpriteName");
				newLeader.SpriteNameHero = TFUtils.LoadString(dict, "SpriteNameHero");
				newLeader.FrameSpriteName = TFUtils.LoadString(dict, "FrameSpriteName");
				newLeader.Desc = TFUtils.LoadLocalizedString(dict, "Desc");
				newLeader.LvUpSchemeID = TFUtils.LoadString(dict, "LvUpSchemeID");
				newLeader.BaseHP = TFUtils.LoadInt(dict, "BaseHP", 23);
				newLeader.MaxXP = XPManager.Instance.FindRequiredXP(newLeader.LvUpSchemeID, ParametersManager.Instance.Max_Leader_Level);
				newLeader.ScriptName = TFUtils.LoadString(dict, "ScriptName", string.Empty);
				newLeader.Cooldown = TFUtils.LoadInt(dict, "Cooldown", 0);
				string str3 = TFUtils.LoadNullableString(dict, "forFaction");
				if (!string.IsNullOrEmpty(str3))
				{
					newLeader.forFaction = (Faction)(int)Enum.Parse(typeof(Faction), str3, true);
				}
				str3 = TFUtils.LoadNullableString(dict, "forLandscape");
				if (!string.IsNullOrEmpty(str3))
				{
					newLeader.forLandscape = (LandscapeType)(int)Enum.Parse(typeof(LandscapeType), str3, true);
				}
				str3 = TFUtils.LoadNullableString(dict, "forCardType");
				if (!string.IsNullOrEmpty(str3))
				{
					newLeader.forCardType = (CardType)(int)Enum.Parse(typeof(CardType), str3, true);
				}
				newLeader.BaseVal1 = TFUtils.LoadInt(dict, "Val1", 0);
				newLeader.BaseVal2 = TFUtils.LoadInt(dict, "Val2", 0);
				newLeader.Ring_P1_HitAreaRange = TFUtils.TryLoadNullableFloat(dict, "Ring_P1_HitAreaRange");
				newLeader.Ring_P1_CritAreaRange = TFUtils.TryLoadNullableFloat(dict, "Ring_P1_CritAreaRange");
				newLeader.Ring_P2_HitAreaRange = TFUtils.TryLoadNullableFloat(dict, "Ring_P2_HitAreaRange");
				newLeader.Ring_P2_CritAreaRange = TFUtils.TryLoadNullableFloat(dict, "Ring_P2_CritAreaRange");
				newLeader.Ring_P1_HitColor = TFUtils.LoadString(dict, "Ring_P1_HitColor");
				newLeader.Ring_P1_CritColor = TFUtils.LoadString(dict, "Ring_P1_CritColor");
				newLeader.Ring_P2_HitColor = TFUtils.LoadString(dict, "Ring_P2_HitColor");
				newLeader.Ring_P2_CritColor = TFUtils.LoadString(dict, "Ring_P2_CritColor");
				newLeader.TimeFor1SpinMin = TFUtils.TryLoadNullableFloat(dict, "TimeFor1SpinMin");
				newLeader.TimeFor1SpinMax = TFUtils.TryLoadNullableFloat(dict, "TimeFor1SpinMax");
				newLeader.Ring_BGSpriteAtlas = TFUtils.LoadString(dict, "Ring_BGSpriteAtlas");
				newLeader.Ring_P1_BGSprite = TFUtils.LoadString(dict, "Ring_P1_BGSprite");
				newLeader.Ring_P1_BGColor = TFUtils.LoadString(dict, "Ring_P1_BGColor");
				newLeader.Ring_P2_BGSprite = TFUtils.LoadString(dict, "Ring_P2_BGSprite");
				newLeader.Ring_P2_BGColor = TFUtils.LoadString(dict, "Ring_P2_BGColor");
				newLeader.Ring_P1_BarSprite = TFUtils.LoadString(dict, "Ring_P1_BarSprite");
				newLeader.Ring_P2_BarSprite = TFUtils.LoadString(dict, "Ring_P2_BarSprite");
				newLeader.CritDamageMod = TFUtils.TryLoadNullableFloat(dict, "CritDamageMod");
				newLeader.FCWorld = TFUtils.LoadBoolAsInt(dict, "FCWorld");
				newLeader.StartLevel = TFUtils.LoadInt(dict, "StartLevel", 1);
				leaderForms.Add(key, newLeader);
				if (LoadingManager.ShouldYield())
				{
					yield return null;
				}
			}
		}
	}

	public void Destroy()
	{
		instance = null;
	}

	public bool IsLeaderFromFC(string a_leaderID)
	{
		LeaderForm value;
		leaderForms.TryGetValue(a_leaderID, out value);
		return value != null && value.FCWorld;
	}

	public int GetMPLeaderHP(string a_leaderID, int a_rank)
	{
		LeaderForm value;
		leaderForms.TryGetValue(a_leaderID, out value);
		if (value != null)
		{
			return a_rank * 5 + value.BaseHP;
		}
		value = leaderForms["Leader_Jake"];
		return a_rank * 5 + value.BaseHP;
	}

	public string GetMPLeaderDesc(string a_leaderID)
	{
		LeaderForm value;
		leaderForms.TryGetValue(a_leaderID, out value);
		if (value != null)
		{
			string text = value.Desc.Replace("<val1>", value.BaseVal1.ToString());
			return text.Replace("<val2>", value.BaseVal2.ToString());
		}
		value = leaderForms["Leader_Jake"];
		string text2 = value.Desc.Replace("<val1>", value.BaseVal1.ToString());
		return text2.Replace("<val2>", value.BaseVal2.ToString());
	}

	public string GetMPLeaderPortrait(string a_leaderID)
	{
		LeaderForm value;
		leaderForms.TryGetValue(a_leaderID, out value);
		if (value != null)
		{
			return value.SpriteName;
		}
		value = leaderForms["Leader_Jake"];
		return value.SpriteName;
	}

	public string GetMPCharacterID(string a_leaderID)
	{
		LeaderForm value;
		leaderForms.TryGetValue(a_leaderID, out value);
		if (value != null)
		{
			return value.CharacterID;
		}
		value = leaderForms["Leader_Jake"];
		return value.CharacterID;
	}

	public LeaderItem CreateLeader(string leaderID, int Rank)
	{
		if (!leaderForms.ContainsKey(leaderID))
		{
			LeaderForm leaderForm = leaderForms["Leader_Jake"];
			LeaderItem leaderItem = new LeaderItem(leaderForm);
			leaderItem.XP = XPManager.Instance.FindRequiredXP(leaderForm.LvUpSchemeID, Rank);
			return leaderItem;
		}
		LeaderForm leaderForm2 = leaderForms[leaderID];
		LeaderItem leaderItem2 = new LeaderItem(leaderForm2);
		leaderItem2.XP = XPManager.Instance.FindRequiredXP(leaderForm2.LvUpSchemeID, Rank);
		return leaderItem2;
	}

	public bool AlreadyOwned(string leaderID)
	{
		return leaders.Exists((LeaderItem ldr) => ldr.Form.ID == leaderID);
	}

	public LeaderItem GetLeader(string leaderID)
	{
		return leaders.FindAll((LeaderItem ldr) => ldr.Form.ID == leaderID).FirstOrDefault();
	}

	public LeaderItem AddNewLeader(string leaderID)
	{
		LeaderItem leaderItem = CreateLeader(leaderID, GetStartLevel(leaderID));
		if (leaderItem == null)
		{
			return null;
		}
		leaders.Add(leaderItem);
		if (leaders.Count >= leaderForms.Count)
		{
			SocialManager.Instance.ReportAchievement(SocialManager.AchievementIDs.AT_ALL_HEROS);
		}
		return leaderItem;
	}

	public LeaderItem AddNewLeaderIfUnique(string leaderID)
	{
		if (AlreadyOwned(leaderID))
		{
			return null;
		}
		return AddNewLeader(leaderID);
	}

	private int GetStartLevel(string leaderID)
	{
		LeaderForm value;
		leaderForms.TryGetValue(leaderID, out value);
		return (value == null) ? 1 : value.StartLevel;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append("\"Leaders\":[");
		bool flag = true;
		foreach (LeaderItem leader in leaders)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("{");
			stringBuilder.Append(PlayerInfoScript.MakeJS("form", leader.Form.ID) + ",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("XP", leader.XP));
			stringBuilder.Append("}");
		}
		stringBuilder.Append("],");
		stringBuilder.Append("\"DeckAssignments\":[");
		flag = true;
		foreach (Deck deck in PlayerInfoScript.GetInstance().DeckManager.Decks)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append(deck.GetLeaderIndex().ToString());
		}
		stringBuilder.Append("]");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public void InventoryFromDict(Dictionary<string, object> dict)
	{
		if (!dict.ContainsKey("Leaders"))
		{
			return;
		}
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		object[] array = (object[])dict["Leaders"];
		if (array.Length > 0)
		{
			leaders.Clear();
			foreach (Deck deck in deckManager.Decks)
			{
				deck.ClearLeader();
			}
			object[] array2 = array;
			foreach (object obj in array2)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
				string text = (string)dictionary["form"];
				if (!leaderForms.ContainsKey(text))
				{
					Singleton<AnalyticsManager>.Instance.LogDebug("badleader_" + text);
					CrashAnalytics.LogException(new Exception("Leader not found - " + text));
				}
				else
				{
					LeaderItem leaderItem = new LeaderItem(leaderForms[text]);
					leaderItem.XP = (int)dictionary["XP"];
					leaders.Add(leaderItem);
				}
			}
		}
		if (leaders.Count < 1)
		{
			FillLeadersWithDummyData();
		}
		if (!dict.ContainsKey("DeckAssignments"))
		{
			return;
		}
		try
		{
			int num = 0;
			int[] array3 = (int[])dict["DeckAssignments"];
			int[] array4 = array3;
			foreach (int leaderForPlayer in array4)
			{
				deckManager.Decks[num++].SetLeaderForPlayer(leaderForPlayer);
			}
		}
		catch (InvalidCastException e)
		{
			Singleton<AnalyticsManager>.Instance.LogDebug("exception_inventoryfromdict");
			CrashAnalytics.LogException(e);
		}
	}

	public void FillLeadersWithDummyData()
	{
		leaders.Clear();
		PlayerDeckManager deckManager = PlayerInfoScript.GetInstance().DeckManager;
		LeaderItem item = CreateLeader("Leader_Jake", 1);
		leaders.Add(item);
		foreach (Deck deck in deckManager.Decks)
		{
			deck.SetLeaderForPlayer(0);
		}
	}
}
