using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Multiplayer
{
	public class Multiplayer
	{
		public static void GetMultiplayerStatus(Session session, MultiplayerDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else if (status == HttpStatusCode.OK)
				{
                    callback(new MultiplayerData(data), ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(null, ResponseFlag.None);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerPlayerInfo(session.ThePlayer.playerId, callback2);
		}

		public static void CreateMultiplayerUser(Session session, string name, string icon, string deck, float deckRank, string landscapes, string leader, int leaderLevel, int maxLevel, MultiplayerDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data == null)
				{
					callback(null, ResponseFlag.Error);
				}
				else
				{
					switch (status)
					{
					case HttpStatusCode.OK:
						if ((bool)data["success"])
						{
							Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
							callback(new MultiplayerData(dict), ResponseFlag.Success);
						}
						else
						{
							callback(null, ResponseFlag.Error);
						}
						break;
					case HttpStatusCode.Forbidden:
						callback(null, ResponseFlag.Invalid);
						break;
					default:
						callback(null, ResponseFlag.Error);
						break;
					}
				}
			};
			string deck2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(deck));
			session.Server.MultiplayerNewPlayer(name, icon, deck2, deckRank, landscapes, leader, leaderLevel, maxLevel, callback2);
		}

		public static void PlayerRecord(Session session, ExtendedRecordCallback callback)
		{
			PlayerRecord(session, session.ThePlayer.playerId, callback);
		}

		public static void PlayerRecord(Session session, string playerId, ExtendedRecordCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK)
				{
					bool flag = (bool)data["success"];
					List<object> data2 = (List<object>)data["data"];
					if (flag)
					{
						callback(ExtendedRecordData.ProcessList(data2));
						return;
					}
				}
				callback(null);
			};
			session.Server.MultiplayerExtendedRecord(playerId, callback2);
		}

		public static void RecentBattles(Session session, NotificationCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK && (bool)data["success"])
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new RecentNotification(dict));
				}
				else
				{
					callback(null);
				}
			};
			session.Server.MultiplayerNotification(callback2);
		}

		public static void AttackRecord(Session session, RecordCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK)
				{
					bool flag = (bool)data["success"];
					List<object> data2 = (List<object>)data["data"];
					if (flag)
					{
						callback(RecordData.ProcessList(data2));
						return;
					}
				}
				callback(null);
			};
			session.Server.MultiplayerPersonalRecord("attack", callback2);
		}

		public static void DefenseRecord(Session session, RecordCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (data != null && status == HttpStatusCode.OK)
				{
					bool flag = (bool)data["success"];
					List<object> data2 = (List<object>)data["data"];
					if (flag)
					{
						callback(RecordData.ProcessList(data2));
						return;
					}
				}
				callback(null);
			};
			session.Server.MultiplayerPersonalRecord("defense", callback2);
		}

		public static void TournamentReward(Session session, RewardsCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					List<object> data2 = (List<object>)data["data"];
					callback(RewardData.ProcessList(data2), ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(null, ResponseFlag.None);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerTournamentPlayerResult(callback2);
		}

		public static void TopLeaderboard(Session session, LeaderboardCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					List<object> data2 = (List<object>)data["data"];
					callback(LeaderboardData.ProcessList(data2));
				}
				else
				{
					callback(null);
				}
			};
			session.Server.MultiplayerLeaderboardTop(callback2);
		}

		public static void NearbyLeaderboard(Session session, LeaderboardCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					List<object> data2 = (List<object>)data["data"];
					callback(LeaderboardData.ProcessList(data2));
				}
				else
				{
					callback(null);
				}
			};
			session.Server.MultiplayerLeaderboardPlayer(session.ThePlayer.playerId, callback2);
		}

		public static void MatchMake(Session session, int maxLevel, MatchDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null)
				{
					//Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new MatchData(data), ResponseFlag.Success);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerFindMatch(session.ThePlayer.playerId ,maxLevel, callback2);
		}

		public static void MatchGetDeck(Session session, string matchid, float deckRank, string leader, int leaderLevel, StringCallback callback)
		{
			TFServer.JsonStringHandler callback2 = delegate(string data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null)
				{
					byte[] bytes = Convert.FromBase64String(data);
					string @string = Encoding.UTF8.GetString(bytes);
					callback(@string, ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(null, ResponseFlag.None);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerStartMatch(matchid, deckRank, leader, leaderLevel, callback2);
		}

		public static void MatchFinish(Session session, string matchId, bool loss, StringCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					callback(dictionary["trophies"].ToString(), ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(null, ResponseFlag.None);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerEndMatch(matchId, loss, callback2);
		}

		public static void UpdateMultiplayerUser(Session session, string name, string icon, int maxLevel, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.Forbidden)
				{
					callback(ResponseFlag.Invalid);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerUpdatePlayer(name, icon, maxLevel, callback2);
		}

		public static void UpdateDeck(Session session, string deck, float deckRank, string landscapes, string leader, int leaderLevel, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			string deck2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(deck));
			session.Server.MultiplayerUpdateDeck(session.ThePlayer.playerId, deck2, deckRank, landscapes, leader, leaderLevel, callback2);
		}

		public static void GetTournamentEndDate(Session session, bool cheater, TournamentDataCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					Dictionary<string, object> dict = (Dictionary<string, object>)data["data"];
					callback(new TournamentData(dict), ResponseFlag.Success);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			if (cheater)
			{
				session.Server.MultiplayerCheaterTournamentEnd(callback2);
			}
			else
			{
				session.Server.MultiplayerGetTournamentEnd(callback2);
			}
		}

		public static void CompleteTournamentReward(Session session, int tournamentId, SuccessCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					callback(ResponseFlag.Success);
				}
				else
				{
					callback(ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerRedeemReward(tournamentId, callback2);
		}

		public static void GetRank(Session session, bool global, StringCallback callback)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status == HttpStatusCode.OK && data != null && (bool)data["success"])
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					callback(dictionary["rank"].ToString(), ResponseFlag.Success);
				}
				else if (status == HttpStatusCode.NotFound)
				{
					callback(null, ResponseFlag.None);
				}
				else
				{
					callback(null, ResponseFlag.Error);
				}
			};
			session.Server.MultiplayerGetRank(global, callback2);
		}
	}
}
