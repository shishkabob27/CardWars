using UnityEngine;

public class KFFRandom : MonoBehaviour
{
	public static float GetValue()
	{
		return Random.value;
	}

	public static int GetValue(int min, int max)
	{
		float f = (float)min + GetValue() * ((float)(max - min) + 1f);
		return Mathf.FloorToInt(f);
	}

	public static float GetValue(float min, float max)
	{
		return min + GetValue() * (max - min);
	}

	public static int GetRandomIndex(int count)
	{
		return (int)Mathf.Floor(GetValue() * ((float)count - 0.001f));
	}
}
