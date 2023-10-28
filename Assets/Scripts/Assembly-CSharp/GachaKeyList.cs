using System.Collections.Generic;
using System.Text;

public class GachaKeyList
{
	public List<GachaKey> _GachaKeys;

	public void AddKey(string Type)
	{
		if (_GachaKeys == null)
		{
			_GachaKeys = new List<GachaKey>();
		}
		GachaKey gachaKey = _GachaKeys.Find((GachaKey x) => x.Type == Type);
		if (gachaKey == null)
		{
			_GachaKeys.Add(new GachaKey(Type, 1));
		}
		else
		{
			gachaKey.Count++;
		}
	}

	public void ConsumeKey(string Type)
	{
		if (_GachaKeys == null)
		{
			return;
		}
		GachaKey gachaKey = _GachaKeys.Find((GachaKey x) => x.Type == Type);
		if (gachaKey != null)
		{
			gachaKey.Count--;
			if (gachaKey.Count == 0)
			{
				_GachaKeys.Remove(gachaKey);
			}
		}
	}

	public string Serialize()
	{
		if (_GachaKeys == null || _GachaKeys.Count == 0)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (GachaKey gachaKey in _GachaKeys)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append("{");
			stringBuilder.Append(PlayerInfoScript.MakeJS("Type", gachaKey.Type) + ",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("Count", gachaKey.Count));
			stringBuilder.Append("}");
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public void Deserialize(object[] arr)
	{
		if (_GachaKeys == null)
		{
			_GachaKeys = new List<GachaKey>();
		}
		foreach (object obj in arr)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
			string t = (string)dictionary["Type"];
			int c = (int)dictionary["Count"];
			_GachaKeys.Add(new GachaKey(t, c));
		}
	}
}
