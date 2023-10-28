using System.Collections;

public class XMLNode : Hashtable
{
	public XMLNodeList GetNodeList(string path)
	{
		return (XMLNodeList)GetObject(path);
	}

	public XMLNode GetNode(string path)
	{
		return (XMLNode)GetObject(path);
	}

	public string GetValue(string path)
	{
		return GetObject(path) as string;
	}

	private object GetObject(string path)
	{
		string[] array = path.Split(">"[0]);
		XMLNode xMLNode = this;
		XMLNodeList xMLNodeList = null;
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (flag)
			{
				xMLNode = (XMLNode)xMLNodeList[int.Parse(array[i])];
				flag = false;
				continue;
			}
			object obj = xMLNode[array[i]];
			if (obj is ArrayList)
			{
				xMLNodeList = (XMLNodeList)obj;
				flag = true;
				continue;
			}
			if (i != array.Length - 1)
			{
				string text = string.Empty;
				for (int j = 0; j <= i; j++)
				{
					text = text + ">" + array[j];
				}
			}
			return obj;
		}
		if (flag)
		{
			return xMLNodeList;
		}
		return xMLNode;
	}
}
