using System.Collections;
using UnityEngine;

public class NgConvert
{
	public static string GetTabSpace(int nTab)
	{
		string text = "    ";
		string text2 = string.Empty;
		for (int i = 0; i < nTab; i++)
		{
			text2 += text;
		}
		return text2;
	}

	public static string[] GetIntStrings(int start, int count)
	{
		string[] array = new string[count];
		for (int i = start; i < count; i++)
		{
			array[i] = i.ToString();
		}
		return array;
	}

	public static int[] GetIntegers(int start, int count)
	{
		int[] array = new int[count];
		for (int i = start; i < count; i++)
		{
			array[i] = i;
		}
		return array;
	}

	public static ArrayList ToArrayList<TT>(TT[] data)
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < data.Length; i++)
		{
			arrayList.Add(data[i]);
		}
		return arrayList;
	}

	public static TT[] ToArray<TT>(ArrayList data)
	{
		TT[] array = new TT[data.Count];
		int num = 0;
		foreach (TT datum in data)
		{
			if (datum != null)
			{
				array[num] = datum;
			}
			num++;
		}
		return array;
	}

	public static TT[] ResizeArray<TT>(TT[] src, int nResize)
	{
		TT[] array = new TT[nResize];
		for (int i = 0; i < src.Length && i < nResize; i++)
		{
			array[i] = src[i];
		}
		return array;
	}

	public static TT[] ResizeArray<TT>(TT[] src, int nResize, TT defaultValue)
	{
		TT[] array = new TT[nResize];
		int i;
		for (i = 0; i < src.Length && i < nResize; i++)
		{
			array[i] = src[i];
		}
		for (; i < array.Length; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	public static string[] ResizeArray(string[] src, int nResize)
	{
		string[] array = new string[nResize];
		for (int i = 0; i < src.Length && i < nResize; i++)
		{
			array[i] = src[i];
		}
		return array;
	}

	public static GameObject[] ResizeArray(GameObject[] src, int nResize)
	{
		GameObject[] array = new GameObject[nResize];
		for (int i = 0; i < src.Length && i < nResize; i++)
		{
			array[i] = src[i];
		}
		return array;
	}

	public static GUIContent[] ResizeArray(GUIContent[] src, int nResize)
	{
		GUIContent[] array = new GUIContent[nResize];
		for (int i = 0; i < src.Length && i < nResize; i++)
		{
			array[i] = src[i];
		}
		return array;
	}

	public static GUIContent[] StringsToContents(string[] strings)
	{
		if (strings == null)
		{
			return null;
		}
		GUIContent[] array = new GUIContent[strings.Length];
		for (int i = 0; i < strings.Length; i++)
		{
			array[i] = new GUIContent(strings[i], strings[i]);
		}
		return array;
	}

	public static string[] ContentsToStrings(GUIContent[] contents)
	{
		if (contents == null)
		{
			return null;
		}
		string[] array = new string[contents.Length];
		for (int i = 0; i < contents.Length; i++)
		{
			array[i] = contents[i].text;
		}
		return array;
	}

	public static uint ToUint(string value, uint nDefaultValue)
	{
		value = value.Trim();
		if (value == string.Empty)
		{
			value = "0";
		}
		uint result;
		if (uint.TryParse(value, out result))
		{
			return result;
		}
		return nDefaultValue;
	}

	public static int ToInt(string value, int nDefaultValue)
	{
		value = value.Trim();
		if (value == string.Empty)
		{
			value = "0";
		}
		int result;
		if (int.TryParse(value, out result))
		{
			return result;
		}
		return nDefaultValue;
	}

	public static float ToFloat(string value, float fDefaultValue)
	{
		value = value.Trim();
		if (value == string.Empty)
		{
			value = "0";
		}
		float result;
		if (float.TryParse(value, out result))
		{
			return result;
		}
		return fDefaultValue;
	}

	public static string GetVaildFloatString(string strInput, ref float fCompleteValue)
	{
		int num = 0;
		int num2 = 0;
		string text = "0123456789";
		strInput = strInput.Trim();
		while (num < strInput.Length)
		{
			if (text.Contains(strInput[num].ToString()))
			{
				num++;
			}
			else if (strInput[num] == '+' || strInput[num] == '-')
			{
				if (num == 0)
				{
					num++;
				}
				else
				{
					strInput = strInput.Remove(num, 1);
				}
			}
			else if (strInput[num] == '.')
			{
				num2++;
				num++;
				if (num2 != 1)
				{
					strInput = strInput.Remove(num - 1, 1);
				}
			}
			else
			{
				num++;
			}
		}
		float result;
		if (strInput == string.Empty || !float.TryParse(strInput, out result))
		{
			return strInput;
		}
		if (strInput[strInput.Length - 1] == '.')
		{
			return strInput;
		}
		fCompleteValue = result;
		return null;
	}
}
