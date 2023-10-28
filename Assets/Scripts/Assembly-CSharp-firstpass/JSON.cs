using System.Collections;
using UnityEngine;

public class JSON : MonoBehaviour
{
	public static Hashtable ParseJSON(string json)
	{
		Hashtable hashtable = new Hashtable();
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		char c = "\""[0];
		bool flag = false;
		bool flag2 = false;
		string text = string.Empty;
		string key = string.Empty;
		bool flag3 = false;
		for (int i = 1; i < json.Length - 1; i++)
		{
			char c2 = json[i];
			if (c2 == c)
			{
				if (flag)
				{
					flag = false;
					flag2 = true;
				}
				else
				{
					flag = true;
					text = string.Empty;
				}
			}
			else if (flag)
			{
				text += c2;
			}
			else if (c2 == "{"[0])
			{
				if (flag3)
				{
					hashtable = new Hashtable();
					arrayList2[arrayList2.Count] = hashtable;
					arrayList.Add(arrayList2);
					flag3 = false;
				}
				else
				{
					hashtable[key] = new Hashtable();
					arrayList.Add(hashtable);
					hashtable = (Hashtable)hashtable[key];
					key = string.Empty;
				}
			}
			else if (c2 == "["[0])
			{
				if (flag3)
				{
					arrayList2[arrayList2.Count] = new ArrayList();
					arrayList.Add(arrayList2);
					arrayList2 = (ArrayList)arrayList2[arrayList2.Count - 1];
				}
				else
				{
					arrayList2 = new ArrayList();
					arrayList.Add(hashtable);
					hashtable[key] = arrayList2;
					key = string.Empty;
				}
				flag3 = true;
			}
			else if (c2 == "]"[0] || c2 == "}"[0])
			{
				if (flag2 || text != null)
				{
					if (flag3)
					{
						arrayList2.Add(text);
					}
					else
					{
						hashtable[key] = text;
						key = string.Empty;
					}
					flag2 = false;
					text = string.Empty;
				}
				object obj = arrayList[arrayList.Count - 1];
				arrayList.RemoveAt(arrayList.Count - 1);
				if (obj is Hashtable)
				{
					hashtable = (Hashtable)obj;
					flag3 = false;
				}
				else
				{
					arrayList2 = (ArrayList)obj;
					flag3 = true;
				}
			}
			else if (c2 == ":"[0])
			{
				key = text;
				flag2 = false;
				text = string.Empty;
			}
			else if (c2 == ","[0])
			{
				if (flag2 || text != null)
				{
					if (flag3)
					{
						arrayList2.Add(text);
					}
					else
					{
						hashtable[key] = text;
						key = string.Empty;
					}
					flag2 = false;
					text = string.Empty;
				}
			}
			else
			{
				text += c2;
			}
		}
		if (flag2 || text != null)
		{
			if (flag3)
			{
				arrayList2.Add(text);
			}
			else
			{
				hashtable[key] = text;
				key = string.Empty;
			}
			flag2 = false;
		}
		return hashtable;
	}
}
