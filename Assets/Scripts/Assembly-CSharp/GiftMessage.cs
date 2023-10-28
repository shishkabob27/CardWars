using System;
using System.Collections.Generic;
using JsonFx.Json;
using MiniJSON;

public class GiftMessage
{
	public Action ConfirmAction { get; private set; }

	public string MessageText { get; private set; }

	public int Gems { get; private set; }

	public int Coins { get; private set; }

	public GiftMessage(string input)
	{
		object obj = Json.Deserialize(input);
		Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
		MessageText = (string)dictionary["message"];
		Gems = TFUtils.LoadInt(dictionary, "hard_currency", 0);
		Coins = TFUtils.LoadInt(dictionary, "soft_currency", 0);
		if (Gems > 0 || Coins > 0)
		{
			ConfirmAction = delegate
			{
				PlayerInfoScript instance = PlayerInfoScript.GetInstance();
				instance.Coins += Coins;
				instance.Gems += Gems;
				instance.Save();
			};
		}
	}

	public static List<string> ProcessMessageListData(string response)
	{
		string[] array = JsonReader.Deserialize<string[]>(response);
		return (array != null) ? new List<string>(array) : null;
	}
}
