using UnityEngine;

internal class SteamManager : MonoBehaviour
{
	public static bool Initialized
	{
		get
		{
			return false;
		}
	}

	public static bool IsUsingSteam()
	{
		return false;
	}
}
