using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

public class SQServer
{
	public const string SECRET_KEY = "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-";

	private const string FB_LOGIN_URL = "account/fbTokenAuth/";

	private const string GC_LOGIN_URL = "account/gcAuth/";

	private const string PRE_AUTH_URL = "account/preAuth/";

	private const string CONFIRM_PURCHASE_URL = "billing/confirm_purchase/";

	private const string GET_UNCONFIRMED_PURCHASE_URL = "billing/get_unconfirmed_hard_currency/";

	private const string RECORD_PURCHASE_URL = "billing/record_purchase/";

	private const string GET_PURCHASES_URL = "billing/getPurchases/";

	private const string GET_SERVER_TIME = "time/";

	private const int MP_VERSION = 0;

	private const string POST_NEW_PLAYER_URL = "multiplayer/new_player/";

	private const string GET_PLAYER_URL = "multiplayer/player/";

	private const string POST_UPDATE_PLAYER_URL = "multiplayer/update_player/";

	private const string POST_UPDATE_DECK_URL = "multiplayer/update_deck/";

	private const string GET_PLAYER_RECORD_URL = "multiplayer/player_record/";

	private const string POST_PERSONAL_RECORD_URL = "multiplayer/record/";

	private const string POST_TOURNAMENT_PLAYER_RESULT = "multiplayer/tournament/player/";

	private const string TOURNAMENT_END_DATE = "multiplayer/tournament/expiration/";

	private const string GET_LEADERBOARD_URL = "multiplayer/active_leaderboard/";

	private const string POST_MATCHMAKE_URL = "multiplayer/matchmake/";

	private const string POST_TOURNAMENT_REDEMPTION = "multiplayer/tournament/complete/";

	private TFServer tfServer;

	private string nonce = string.Empty;

	public static string IAP_VERIFICATION_SERVER_URL
	{
		get
		{
			return "http://cardwars.retroretreat.net/";
		}
	}

	public SQServer(CookieContainer cookies)
	{
		tfServer = new TFServer(cookies, SQSettings.PATCHING_FILE_LIMIT);
	}

	public void SetLoggedOut()
	{
		tfServer.ShortCircuitAllRequests();
	}

	public bool IsNetworkError(Dictionary<string, object> response)
	{
		return response.ContainsKey("error") && "Network error".Equals(response["error"]);
	}

	public void PreAuth(TFServer.JsonResponseHandler callback)
	{
		GetToJSON(SQSettings.SERVER_URL + "account/preAuth/", callback);
	}

	public void GetToJSON(string url, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(url, callback);
	}

	public void GetPurchases(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, string.Empty, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "billing/getPurchases/", dictionary, callback);
	}

	public void GetUnconfirmedPurchases(string playerId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["signature"] = SignDictionary(dictionary, string.Empty, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "billing/get_unconfirmed_hard_currency/", dictionary, callback);
	}

	public void SavePurchase(string store_id, string store, string sandbox, string bundle_id, string productId, string playerId, string receipt, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["store_id"] = store_id;
		dictionary["store"] = store;
		dictionary["environment"] = sandbox;
		dictionary["bundle_id"] = bundle_id;
		dictionary["product_id"] = productId;
		dictionary["receipt"] = receipt;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(IAP_VERIFICATION_SERVER_URL + "billing/record_purchase/", dictionary, callback);
	}

	public void ConfirmPurchase(string playerId, string transaction_id, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["transaction_id"] = transaction_id;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(IAP_VERIFICATION_SERVER_URL + "billing/confirm_purchase/", dictionary, callback);
	}

	public void FbLogin(string fbAccessToken, string fbExpirationDate, string nonce, TFServer.JsonResponseHandler callback)
	{
		this.nonce = nonce;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["fb_access_token"] = fbAccessToken;
		dictionary["fb_expiration_date"] = fbExpirationDate;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "account/fbTokenAuth/", dictionary, callback);
	}

	public void GcLogin(string playerId, string alias, string nonce, TFServer.JsonResponseHandler callback)
	{
		this.nonce = nonce;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["player_id"] = playerId;
		dictionary["alias"] = alias;
		dictionary["alternate"] = alias;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "account/gcAuth/", dictionary, callback);
	}

	public void GetTime(TFServer.JsonResponseHandler handler)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "time/", handler);
	}

	public void MultiplayerPlayerInfo(string playerId, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/player/" + playerId, callback);
	}

	public void MultiplayerNewPlayer(string name, string icon, string deck, float deckRank, string landscapes, string leader, int leaderLevel, int maxLevel, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["name"] = name;
		dictionary["icon"] = icon;
		dictionary["deck"] = deck;
		dictionary["deck_rank"] = deckRank;
		dictionary["landscapes"] = landscapes;
		dictionary["leader"] = leader;
		dictionary["leader_level"] = leaderLevel;
		dictionary["level"] = maxLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/new_player/", dictionary, callback);
	}

	public void MultiplayerUpdateDeck(string name,string deck, float deckRank, string landscapes, string leader, int leaderLevel, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["name"] = name;
		dictionary["deck"] = deck;
		dictionary["deck_rank"] = deckRank;
		dictionary["landscapes"] = landscapes;
		dictionary["leader"] = leader;
		dictionary["leader_level"] = leaderLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/update_deck/", dictionary, callback);
	}

	public void MultiplayerUpdatePlayer(string name, string icon, int maxLevel, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["name"] = name;
		dictionary["icon"] = icon;
		dictionary["level"] = maxLevel;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/update_player/", dictionary, callback);
	}

	public void MultiplayerExtendedRecord(string playerId, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/player_record/" + playerId, callback);
	}

	public void MultiplayerLeaderboardPlayer(string playerId, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/" + playerId, callback);
	}

	public void MultiplayerLeaderboardTop(TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/", callback);
	}

	public void MultiplayerGetRank(bool global, TFServer.JsonResponseHandler callback)
	{
		if (global)
		{
			tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/globalrank/?username="+ SessionManager.GetInstance().theSession.ThePlayer.playerId, callback);
		}
		else
		{
			tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/active_leaderboard/rank/?username="+ SessionManager.GetInstance().theSession.ThePlayer.playerId, callback);
		}
	}

	public void MultiplayerPersonalRecord(string target, TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/record/" + target, callback);
	}

	public void MultiplayerNotification(TFServer.JsonResponseHandler callback)
	{
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/record/recent/", new Dictionary<string, object>(), callback);
	}

	public void MultiplayerTournamentPlayerResult(TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/player/?username=" + SessionManager.GetInstance().theSession.ThePlayer.playerId, callback);
	}

	public void MultiplayerFindMatch(string playerId, int maxLevel, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["name"] = playerId;
		dictionary["level"] = maxLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/matchmake/find/", dictionary, callback);
	}

	public void MultiplayerStartMatch(string matchId, float deckRank, string leader, int leaderLevel, TFServer.JsonStringHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["match_id"] = matchId;
		dictionary["deck_rank"] = deckRank;
		dictionary["leader"] = leader;
		dictionary["leader_level"] = leaderLevel;
		dictionary["version"] = 0;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToString(SQSettings.SERVER_URL + "multiplayer/matchmake/start/", dictionary, callback);
	}

	public void MultiplayerEndMatch(string matchId, bool loss, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["match_id"] = matchId;
		dictionary["name"] = SessionManager.GetInstance().theSession.ThePlayer.playerId;
        dictionary["loss"] = loss;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/matchmake/complete/", dictionary, callback);
	}

	public void MultiplayerCheaterTournamentEnd(TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["sync"] = true;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/expiration/", dictionary, callback);
	}

	public void MultiplayerGetTournamentEnd(TFServer.JsonResponseHandler callback)
	{
		tfServer.GetToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/expiration/", callback);
	}

	public void MultiplayerRedeemReward(int tournamentId, TFServer.JsonResponseHandler callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["username"] = SessionManager.GetInstance().theSession.ThePlayer.playerId;
		dictionary["tournament_id"] = tournamentId;
		dictionary["signature"] = SignDictionary(dictionary, nonce, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-");
		tfServer.PostToJSON(SQSettings.SERVER_URL + "multiplayer/tournament/complete/", dictionary, callback);
	}

	public bool SessionValid()
	{
		Cookie cookie = tfServer.GetCookie(new Uri(SQSettings.SERVER_URL), "sessionid");
		return cookie != null && !cookie.Expired;
	}

	private string SignDictionary(Dictionary<string, object> data, string nonce, string secret)
	{
		string text = string.Empty;
		List<string> list = new List<string>(data.Keys);
		list.Sort();
		foreach (string item in list)
		{
			text = text + item + data[item];
		}
		text += nonce;
		using (HMACSHA256 hMACSHA = new HMACSHA256())
		{
			hMACSHA.Key = Encoding.ASCII.GetBytes(secret);
			byte[] inArray = hMACSHA.ComputeHash(Encoding.ASCII.GetBytes(text));
			return Convert.ToBase64String(inArray).Replace('+', '-').Replace('/', '_');
		}
	}
}
