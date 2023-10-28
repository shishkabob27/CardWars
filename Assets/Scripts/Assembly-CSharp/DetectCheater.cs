using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DetectCheater : MonoBehaviour
{
	public static void Detect(string json)
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		bool cheater = false;
		if (instance.Gems >= 0 && instance.GemsAccumulated < instance.Gems)
		{
			cheater = true;
		}
		if (instance.Coins >= 0 && instance.CoinsAccumulated < instance.Coins)
		{
			cheater = true;
		}
		if (instance.Gems >= 1000000)
		{
			cheater = true;
		}
		if (instance.Coins >= 1000000)
		{
			cheater = true;
		}
		if (instance.MaxInventory > CardBoxManager.Instance.MaxBoxCapacity || (instance.MaxInventory > ParametersManager.Instance.New_Player_Max_Inventory && instance.GemsAccumulated - instance.Gems == 0))
		{
			cheater = true;
		}
		if (instance.Checksum != " " && !VerifyChecksum(MD5Input()))
		{
			cheater = true;
		}
		instance.Cheater = cheater;
	}

	public static string CreateChecksum(string aPayload)
	{
		using (MD5 md5Hash = MD5.Create())
		{
			return GetMd5Hash(md5Hash, aPayload);
		}
	}

	private static bool VerifyChecksum(string aPayload)
	{
		using (MD5 md5Hash = MD5.Create())
		{
			string md5Hash2 = GetMd5Hash(md5Hash, aPayload);
			return VerifyMd5Hash(md5Hash, aPayload, md5Hash2);
		}
	}

	public static string MD5Input()
	{
		PlayerInfoScript instance = PlayerInfoScript.GetInstance();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(instance.PlayerName);
		stringBuilder.Append(instance.Coins.ToString());
		stringBuilder.Append(instance.CoinsAccumulated.ToString());
		stringBuilder.Append(instance.Gems.ToString());
		stringBuilder.Append(instance.GemsAccumulated.ToString());
		stringBuilder.Append(instance.Stamina_Max.ToString());
		stringBuilder.Append(instance.MaxInventory.ToString());
		stringBuilder.Append(instance.DeckManager.Serialize());
		stringBuilder.Append(LeaderManager.Instance.Serialize());
		return stringBuilder.ToString();
	}

	private static string GetMd5Hash(MD5 md5Hash, string input)
	{
		byte[] array = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
	{
		string md5Hash2 = GetMd5Hash(md5Hash, input);
		StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
		if (ordinalIgnoreCase.Compare(md5Hash2, hash) == 0)
		{
			return true;
		}
		return false;
	}
}
