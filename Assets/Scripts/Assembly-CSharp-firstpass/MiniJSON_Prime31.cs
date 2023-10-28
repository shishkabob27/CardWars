using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MiniJSON_Prime31
{
	private const int TOKEN_NONE = 0;

	private const int TOKEN_CURLY_OPEN = 1;

	private const int TOKEN_CURLY_CLOSE = 2;

	private const int TOKEN_SQUARED_OPEN = 3;

	private const int TOKEN_SQUARED_CLOSE = 4;

	private const int TOKEN_COLON = 5;

	private const int TOKEN_COMMA = 6;

	private const int TOKEN_STRING = 7;

	private const int TOKEN_NUMBER = 8;

	private const int TOKEN_TRUE = 9;

	private const int TOKEN_FALSE = 10;

	private const int TOKEN_NULL = 11;

	private const int BUILDER_CAPACITY = 2000;

	protected static int lastErrorIndex = -1;

	protected static string lastDecode = string.Empty;

	public static object jsonDecode(string json)
	{
		lastDecode = json;
		if (json != null)
		{
			char[] json2 = json.ToCharArray();
			int index = 0;
			bool success = true;
			object result = parseValue(json2, ref index, ref success);
			if (success)
			{
				lastErrorIndex = -1;
			}
			else
			{
				lastErrorIndex = index;
			}
			return result;
		}
		return null;
	}

	public static string jsonEncode(object json)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		return (!serializeValue(json, stringBuilder)) ? null : stringBuilder.ToString();
	}

	public static bool lastDecodeSuccessful()
	{
		return lastErrorIndex == -1;
	}

	public static int getLastErrorIndex()
	{
		return lastErrorIndex;
	}

	public static string getLastErrorSnippet()
	{
		if (lastErrorIndex == -1)
		{
			return string.Empty;
		}
		int num = lastErrorIndex - 5;
		int num2 = lastErrorIndex + 15;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= lastDecode.Length)
		{
			num2 = lastDecode.Length - 1;
		}
		return lastDecode.Substring(num, num2 - num + 1);
	}

	protected static Hashtable parseObject(char[] json, ref int index)
	{
		Hashtable hashtable = new Hashtable();
		nextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (lookAhead(json, index))
			{
			case 0:
				return null;
			case 6:
				nextToken(json, ref index);
				continue;
			case 2:
				nextToken(json, ref index);
				return hashtable;
			}
			string text = parseString(json, ref index);
			if (text == null)
			{
				return null;
			}
			int num = nextToken(json, ref index);
			if (num != 5)
			{
				return null;
			}
			bool success = true;
			object value = parseValue(json, ref index, ref success);
			if (!success)
			{
				return null;
			}
			hashtable[text] = value;
		}
		return hashtable;
	}

	protected static ArrayList parseArray(char[] json, ref int index)
	{
		ArrayList arrayList = new ArrayList();
		nextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (lookAhead(json, index))
			{
			case 0:
				return null;
			case 6:
				nextToken(json, ref index);
				continue;
			case 4:
				break;
			default:
			{
				bool success = true;
				object value = parseValue(json, ref index, ref success);
				if (!success)
				{
					return null;
				}
				arrayList.Add(value);
				continue;
			}
			}
			nextToken(json, ref index);
			break;
		}
		return arrayList;
	}

	protected static object parseValue(char[] json, ref int index, ref bool success)
	{
		switch (lookAhead(json, index))
		{
		case 7:
			return parseString(json, ref index);
		case 8:
			return parseNumber(json, ref index);
		case 1:
			return parseObject(json, ref index);
		case 3:
			return parseArray(json, ref index);
		case 9:
			nextToken(json, ref index);
			return bool.Parse("TRUE");
		case 10:
			nextToken(json, ref index);
			return bool.Parse("FALSE");
		case 11:
			nextToken(json, ref index);
			return null;
		default:
			success = false;
			return null;
		}
	}

	protected static string parseString(char[] json, ref int index)
	{
		string text = string.Empty;
		eatWhitespace(json, ref index);
		char c = json[index++];
		bool flag = false;
		while (!flag && index != json.Length)
		{
			c = json[index++];
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
				if (index != json.Length)
				{
					switch (json[index++])
					{
					case '"':
						text += '"';
						continue;
					case '\\':
						text += '\\';
						continue;
					case '/':
						text += '/';
						continue;
					case 'b':
						text += '\b';
						continue;
					case 'f':
						text += '\f';
						continue;
					case 'n':
						text += '\n';
						continue;
					case 'r':
						text += '\r';
						continue;
					case 't':
						text += '\t';
						continue;
					case 'u':
						break;
					default:
						continue;
					}
					int num = json.Length - index;
					if (num >= 4)
					{
						char[] array = new char[4];
						Array.Copy(json, index, array, 0, 4);
						text = text + "&#x" + new string(array) + ";";
						index += 4;
						continue;
					}
				}
				break;
			default:
				text += c;
				continue;
			}
			break;
		}
		if (!flag)
		{
			return null;
		}
		return text;
	}

	protected static double parseNumber(char[] json, ref int index)
	{
		eatWhitespace(json, ref index);
		int lastIndexOfNumber = getLastIndexOfNumber(json, index);
		int num = lastIndexOfNumber - index + 1;
		char[] array = new char[num];
		Array.Copy(json, index, array, 0, num);
		index = lastIndexOfNumber + 1;
		return double.Parse(new string(array));
	}

	protected static int getLastIndexOfNumber(char[] json, int index)
	{
		int i;
		for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	protected static void eatWhitespace(char[] json, ref int index)
	{
		while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1)
		{
			index++;
		}
	}

	protected static int lookAhead(char[] json, int index)
	{
		int index2 = index;
		return nextToken(json, ref index2);
	}

	protected static int nextToken(char[] json, ref int index)
	{
		eatWhitespace(json, ref index);
		if (index == json.Length)
		{
			return 0;
		}
		char c = json[index];
		index++;
		switch (c)
		{
		case '{':
			return 1;
		case '}':
			return 2;
		case '[':
			return 3;
		case ']':
			return 4;
		case ',':
			return 6;
		case '"':
			return 7;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return 8;
		case ':':
			return 5;
		default:
		{
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return 10;
			}
			if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
			{
				index += 4;
				return 9;
			}
			if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
			{
				index += 4;
				return 11;
			}
			return 0;
		}
		}
	}

	protected static bool serializeObjectOrArray(object objectOrArray, StringBuilder builder)
	{
		if (objectOrArray is Hashtable)
		{
			return serializeObject((Hashtable)objectOrArray, builder);
		}
		if (objectOrArray is ArrayList)
		{
			return serializeArray((ArrayList)objectOrArray, builder);
		}
		return false;
	}

	protected static bool serializeObject(Hashtable anObject, StringBuilder builder)
	{
		builder.Append("{");
		IDictionaryEnumerator enumerator = anObject.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			string aString = enumerator.Key.ToString();
			object value = enumerator.Value;
			if (!flag)
			{
				builder.Append(", ");
			}
			serializeString(aString, builder);
			builder.Append(":");
			if (!serializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected static bool serializeDictionary(Dictionary<string, string> dict, StringBuilder builder)
	{
		builder.Append("{");
		bool flag = true;
		foreach (KeyValuePair<string, string> item in dict)
		{
			if (!flag)
			{
				builder.Append(", ");
			}
			serializeString(item.Key, builder);
			builder.Append(":");
			serializeString(item.Value, builder);
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected static bool serializeArray(ArrayList anArray, StringBuilder builder)
	{
		builder.Append("[");
		bool flag = true;
		for (int i = 0; i < anArray.Count; i++)
		{
			object value = anArray[i];
			if (!flag)
			{
				builder.Append(", ");
			}
			if (!serializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("]");
		return true;
	}

	protected static bool serializeValue(object value, StringBuilder builder)
	{
		if (value == null)
		{
			builder.Append("null");
		}
		else if (value.GetType().IsArray)
		{
			serializeArray(new ArrayList((ICollection)value), builder);
		}
		else if (value is string)
		{
			serializeString((string)value, builder);
		}
		else if (value is char)
		{
			serializeString(Convert.ToString((char)value), builder);
		}
		else if (value is Hashtable)
		{
			serializeObject((Hashtable)value, builder);
		}
		else if (value is Dictionary<string, string>)
		{
			serializeDictionary((Dictionary<string, string>)value, builder);
		}
		else if (value is ArrayList)
		{
			serializeArray((ArrayList)value, builder);
		}
		else if (value is bool && (bool)value)
		{
			builder.Append("true");
		}
		else if (value is bool && !(bool)value)
		{
			builder.Append("false");
		}
		else
		{
			if (!value.GetType().IsPrimitive)
			{
				return false;
			}
			serializeNumber(Convert.ToDouble(value), builder);
		}
		return true;
	}

	protected static void serializeString(string aString, StringBuilder builder)
	{
		builder.Append("\"");
		char[] array = aString.ToCharArray();
		foreach (char c in array)
		{
			switch (c)
			{
			case '"':
				builder.Append("\\\"");
				continue;
			case '\\':
				builder.Append("\\\\");
				continue;
			case '\b':
				builder.Append("\\b");
				continue;
			case '\f':
				builder.Append("\\f");
				continue;
			case '\n':
				builder.Append("\\n");
				continue;
			case '\r':
				builder.Append("\\r");
				continue;
			case '\t':
				builder.Append("\\t");
				continue;
			}
			int num = Convert.ToInt32(c);
			if (num >= 32 && num <= 126)
			{
				builder.Append(c);
			}
			else
			{
				builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
			}
		}
		builder.Append("\"");
	}

	protected static void serializeNumber(double number, StringBuilder builder)
	{
		builder.Append(Convert.ToString(number));
	}
}
