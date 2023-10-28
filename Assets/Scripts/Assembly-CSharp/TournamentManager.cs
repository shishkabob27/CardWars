using System;
using System.Collections;
using System.Collections.Generic;

public class TournamentManager : Singleton<TournamentManager>, ILoadable
{
	public enum RewardType
	{
		Coin,
		Gem,
		Card
	}

	public class Reward
	{
		public int Rank;

		public RewardType Type;

		public string CardName;

		public int Amount;
	}

	public class Tournament
	{
		public int ID;

		public string Name;

		public List<Reward> Rewards = new List<Reward>();

		public List<Reward> GetReward(int aRank)
		{
			List<Reward> list = new List<Reward>();
			foreach (Reward reward in Rewards)
			{
				if (reward.Rank >= aRank)
				{
					list.Add(reward);
				}
			}
			return list;
		}
	}

	public List<Tournament> Tournaments = new List<Tournament>();

	public Tournament GetCurrentTournament(int aID)
	{
		return Tournaments.Find((Tournament s) => aID == s.ID);
	}

	public List<Reward> GetRewards(int aTournamentID, int aRank)
	{
		Tournament currentTournament = GetCurrentTournament(aTournamentID);
		if (currentTournament != null)
		{
			return currentTournament.GetReward(aRank);
		}
		return null;
	}

	public IEnumerator Load()
	{
		Dictionary<string, object>[] data = SQUtils.ReadJSONData("db_Tournament.json");
		if (LoadingManager.ShouldYield())
		{
			yield return null;
		}
		Dictionary<string, object>[] array = data;
		foreach (Dictionary<string, object> dict in array)
		{
			Tournament NewTournament = new Tournament
			{
				ID = TFUtils.LoadInt(dict, "TournamentID", 0),
				Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty)
			};
			Reward NewReward5 = new Reward();
			try
			{
				NewReward5.Type = (RewardType)(int)Enum.Parse(typeof(RewardType), TFUtils.LoadString(dict, "Reward1Type"), true);
			}
			catch
			{
				NewReward5.Type = RewardType.Card;
				NewReward5.CardName = TFUtils.LoadString(dict, "Reward1Type");
			}
			NewReward5.Amount = TFUtils.LoadInt(dict, "Reward1Amount", 0);
			NewReward5.Rank = TFUtils.LoadInt(dict, "Reward1Rank", 0);
			NewTournament.Rewards.Add(NewReward5);
			NewReward5 = new Reward();
			try
			{
				NewReward5.Type = (RewardType)(int)Enum.Parse(typeof(RewardType), TFUtils.LoadString(dict, "Reward2Type"), true);
			}
			catch
			{
				NewReward5.Type = RewardType.Card;
				NewReward5.CardName = TFUtils.LoadString(dict, "Reward2Type");
			}
			NewReward5.Amount = TFUtils.LoadInt(dict, "Reward2Amount", 0);
			NewReward5.Rank = TFUtils.LoadInt(dict, "Reward2Rank", 0);
			NewTournament.Rewards.Add(NewReward5);
			NewReward5 = new Reward();
			try
			{
				NewReward5.Type = (RewardType)(int)Enum.Parse(typeof(RewardType), TFUtils.LoadString(dict, "Reward3Type"), true);
			}
			catch
			{
				NewReward5.Type = RewardType.Card;
				NewReward5.CardName = TFUtils.LoadString(dict, "Reward3Type");
			}
			NewReward5.Amount = TFUtils.LoadInt(dict, "Reward3Amount", 0);
			NewReward5.Rank = TFUtils.LoadInt(dict, "Reward3Rank", 0);
			NewTournament.Rewards.Add(NewReward5);
			NewReward5 = new Reward();
			try
			{
				NewReward5.Type = (RewardType)(int)Enum.Parse(typeof(RewardType), TFUtils.LoadString(dict, "Reward4Type"), true);
			}
			catch
			{
				NewReward5.Type = RewardType.Card;
				NewReward5.CardName = TFUtils.LoadString(dict, "Reward4Type");
			}
			NewReward5.Amount = TFUtils.LoadInt(dict, "Reward4Amount", 0);
			NewReward5.Rank = TFUtils.LoadInt(dict, "Reward4Rank", 0);
			NewTournament.Rewards.Add(NewReward5);
			NewReward5 = new Reward();
			try
			{
				NewReward5.Type = (RewardType)(int)Enum.Parse(typeof(RewardType), TFUtils.LoadString(dict, "Reward5Type"), true);
			}
			catch
			{
				NewReward5.Type = RewardType.Card;
				NewReward5.CardName = TFUtils.LoadString(dict, "Reward5Type");
			}
			NewReward5.Amount = TFUtils.LoadInt(dict, "Reward5Amount", 0);
			NewReward5.Rank = TFUtils.LoadInt(dict, "Reward5Rank", 0);
			NewTournament.Rewards.Add(NewReward5);
			Tournaments.Add(NewTournament);
			if (LoadingManager.ShouldYield())
			{
				yield return null;
			}
		}
	}

	void ILoadable.Destroy()
	{
		Destroy();
	}
}
