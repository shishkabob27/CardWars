using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace JsonFx.Json
{
	public class JsonReader
	{
		internal const string LiteralFalse = "false";

		internal const string LiteralTrue = "true";

		internal const string LiteralNull = "null";

		internal const string LiteralUndefined = "undefined";

		internal const string LiteralNotANumber = "NaN";

		internal const string LiteralPositiveInfinity = "Infinity";

		internal const string LiteralNegativeInfinity = "-Infinity";

		internal const char OperatorNegate = '-';

		internal const char OperatorUnaryPlus = '+';

		internal const char OperatorArrayStart = '[';

		internal const char OperatorArrayEnd = ']';

		internal const char OperatorObjectStart = '{';

		internal const char OperatorObjectEnd = '}';

		internal const char OperatorStringDelim = '"';

		internal const char OperatorStringDelimAlt = '\'';

		internal const char OperatorValueDelim = ',';

		internal const char OperatorNameDelim = ':';

		internal const char OperatorCharEscape = '\\';

		private const string CommentStart = "/*";

		private const string CommentEnd = "*/";

		private const string CommentLine = "//";

		private const string LineEndings = "\r\n";

		internal const string TypeGenericIDictionary = "System.Collections.Generic.IDictionary`2";

		private const string ErrorUnrecognizedToken = "Illegal JSON sequence.";

		private const string ErrorUnterminatedComment = "Unterminated comment block.";

		private const string ErrorUnterminatedObject = "Unterminated JSON object.";

		private const string ErrorUnterminatedArray = "Unterminated JSON array.";

		private const string ErrorUnterminatedString = "Unterminated JSON string.";

		private const string ErrorIllegalNumber = "Illegal JSON number.";

		private const string ErrorExpectedString = "Expected JSON string.";

		private const string ErrorExpectedObject = "Expected JSON object.";

		private const string ErrorExpectedArray = "Expected JSON array.";

		private const string ErrorExpectedPropertyName = "Expected JSON object property name.";

		private const string ErrorExpectedPropertyNameDelim = "Expected JSON object property name delimiter.";

		private const string ErrorGenericIDictionary = "Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})";

		private const string ErrorGenericIDictionaryKeys = "Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})";

		private readonly JsonReaderSettings Settings = new JsonReaderSettings();

		private readonly string Source;

		private readonly int SourceLength;

		private int index;

		[Obsolete("This has been deprecated in favor of JsonReaderSettings object")]
		public bool AllowNullValueTypes
		{
			get
			{
				return Settings.AllowNullValueTypes;
			}
			set
			{
				Settings.AllowNullValueTypes = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonReaderSettings object")]
		public string TypeHintName
		{
			get
			{
				return Settings.TypeHintName;
			}
			set
			{
				Settings.TypeHintName = value;
			}
		}

		public JsonReader(TextReader input)
			: this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(TextReader input, JsonReaderSettings settings)
		{
			Settings = settings;
			Source = input.ReadToEnd();
			SourceLength = Source.Length;
		}

		public JsonReader(Stream input)
			: this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(Stream input, JsonReaderSettings settings)
		{
			Settings = settings;
			using (StreamReader streamReader = new StreamReader(input, true))
			{
				Source = streamReader.ReadToEnd();
			}
			SourceLength = Source.Length;
		}

		public JsonReader(string input)
			: this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(string input, JsonReaderSettings settings)
		{
			Settings = settings;
			Source = input;
			SourceLength = Source.Length;
		}

		public JsonReader(StringBuilder input)
			: this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(StringBuilder input, JsonReaderSettings settings)
		{
			Settings = settings;
			Source = input.ToString();
			SourceLength = Source.Length;
		}

		public object Deserialize()
		{
			return Deserialize((Type)null);
		}

		public object Deserialize(int start)
		{
			index = start;
			return Deserialize((Type)null);
		}

		public object Deserialize(Type type)
		{
			return Read(type, false);
		}

		public T Deserialize<T>()
		{
			return (T)Read(typeof(T), false);
		}

		public object Deserialize(int start, Type type)
		{
			index = start;
			return Read(type, false);
		}

		public T Deserialize<T>(int start)
		{
			index = start;
			return (T)Read(typeof(T), false);
		}

		private object Read(Type expectedType, bool typeIsHint)
		{
			if (expectedType == typeof(object))
			{
				expectedType = null;
			}
			switch (Tokenize())
			{
			case JsonToken.ObjectStart:
				return ReadObject((!typeIsHint) ? expectedType : null);
			case JsonToken.ArrayStart:
				return ReadArray((!typeIsHint) ? expectedType : null);
			case JsonToken.String:
				return ReadString((!typeIsHint) ? expectedType : null);
			case JsonToken.Number:
				return ReadNumber((!typeIsHint) ? expectedType : null);
			case JsonToken.False:
				index += "false".Length;
				return false;
			case JsonToken.True:
				index += "true".Length;
				return true;
			case JsonToken.Null:
				index += "null".Length;
				return null;
			case JsonToken.NaN:
				index += "NaN".Length;
				return double.NaN;
			case JsonToken.PositiveInfinity:
				index += "Infinity".Length;
				return double.PositiveInfinity;
			case JsonToken.NegativeInfinity:
				index += "-Infinity".Length;
				return double.NegativeInfinity;
			case JsonToken.Undefined:
				index += "undefined".Length;
				return null;
			default:
				return null;
			}
		}

		private object ReadObject(Type objectType)
		{
			if (Source[index] != '{')
			{
				throw new JsonDeserializationException("Expected JSON object.", index);
			}
			Type type = null;
			Dictionary<string, MemberInfo> memberMap = null;
			object obj;
			if (objectType != null)
			{
				obj = Settings.Coercion.InstantiateObject(objectType, out memberMap);
				if (memberMap == null)
				{
					Type @interface = objectType.GetInterface("System.Collections.Generic.IDictionary`2");
					if (@interface != null)
					{
						Type[] genericArguments = @interface.GetGenericArguments();
						if (genericArguments.Length == 2)
						{
							if (genericArguments[0] != typeof(string))
							{
								throw new JsonDeserializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})", objectType), index);
							}
							if (genericArguments[1] != typeof(object))
							{
								type = genericArguments[1];
							}
						}
					}
				}
			}
			else
			{
				obj = new Dictionary<string, object>();
			}
			JsonToken jsonToken;
			do
			{
				index++;
				if (index >= SourceLength)
				{
					throw new JsonDeserializationException("Unterminated JSON object.", index);
				}
				jsonToken = Tokenize(Settings.AllowUnquotedObjectKeys);
				switch (jsonToken)
				{
				default:
					throw new JsonDeserializationException("Expected JSON object property name.", index);
				case JsonToken.String:
				case JsonToken.UnquotedName:
				{
					string text = ((jsonToken != JsonToken.String) ? ReadUnquotedKey() : ((string)ReadString(null)));
					Type type2;
					MemberInfo memberInfo;
					if (type == null && memberMap != null)
					{
						type2 = TypeCoercionUtility.GetMemberInfo(memberMap, text, out memberInfo);
					}
					else
					{
						type2 = type;
						memberInfo = null;
					}
					jsonToken = Tokenize();
					if (jsonToken != JsonToken.NameDelim)
					{
						throw new JsonDeserializationException("Expected JSON object property name delimiter.", index);
					}
					index++;
					if (index >= SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON object.", index);
					}
					object obj2 = Read(type2, false);
					if (obj is IDictionary)
					{
						if (objectType == null && Settings.IsTypeHintName(text))
						{
							obj = Settings.Coercion.ProcessTypeHint((IDictionary)obj, obj2 as string, out objectType, out memberMap);
						}
						else
						{
							((IDictionary)obj)[text] = obj2;
						}
					}
					else
					{
						if (objectType.GetInterface("System.Collections.Generic.IDictionary`2") != null)
						{
							throw new JsonDeserializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})", objectType), index);
						}
						Settings.Coercion.SetMemberValue(obj, type2, memberInfo, obj2);
					}
					goto IL_0270;
				}
				case JsonToken.ObjectEnd:
					break;
				}
				break;
				IL_0270:
				jsonToken = Tokenize();
			}
			while (jsonToken == JsonToken.ValueDelim);
			if (jsonToken != JsonToken.ObjectEnd)
			{
				throw new JsonDeserializationException("Unterminated JSON object.", index);
			}
			index++;
			return obj;
		}

		private IEnumerable ReadArray(Type arrayType)
		{
			if (Source[index] != '[')
			{
				throw new JsonDeserializationException("Expected JSON array.", index);
			}
			bool flag = arrayType != null;
			bool typeIsHint = !flag;
			Type type = null;
			if (flag)
			{
				if (arrayType.HasElementType)
				{
					type = arrayType.GetElementType();
				}
				else if (arrayType.IsGenericType)
				{
					Type[] genericArguments = arrayType.GetGenericArguments();
					if (genericArguments.Length == 1)
					{
						type = genericArguments[0];
					}
				}
			}
			ArrayList arrayList = new ArrayList();
			JsonToken jsonToken;
			do
			{
				index++;
				if (index >= SourceLength)
				{
					throw new JsonDeserializationException("Unterminated JSON array.", index);
				}
				jsonToken = Tokenize();
				if (jsonToken == JsonToken.ArrayEnd)
				{
					break;
				}
				object obj = Read(type, typeIsHint);
				arrayList.Add(obj);
				if (obj == null)
				{
					if (type != null && type.IsValueType)
					{
						type = null;
					}
					flag = true;
				}
				else if (type != null && !type.IsAssignableFrom(obj.GetType()))
				{
					if (obj.GetType().IsAssignableFrom(type))
					{
						type = obj.GetType();
					}
					else
					{
						type = null;
						flag = true;
					}
				}
				else if (!flag)
				{
					type = obj.GetType();
					flag = true;
				}
				jsonToken = Tokenize();
			}
			while (jsonToken == JsonToken.ValueDelim);
			if (jsonToken != JsonToken.ArrayEnd)
			{
				throw new JsonDeserializationException("Unterminated JSON array.", index);
			}
			index++;
			if (type != null && type != typeof(object))
			{
				return arrayList.ToArray(type);
			}
			return arrayList.ToArray();
		}

		private string ReadUnquotedKey()
		{
			int num = index;
			do
			{
				index++;
			}
			while (Tokenize(true) == JsonToken.UnquotedName);
			return Source.Substring(num, index - num);
		}

		private object ReadString(Type expectedType)
		{
			if (Source[index] != '"' && Source[index] != '\'')
			{
				throw new JsonDeserializationException("Expected JSON string.", index);
			}
			char c = Source[index];
			index++;
			if (index >= SourceLength)
			{
				throw new JsonDeserializationException("Unterminated JSON string.", index);
			}
			int num = index;
			StringBuilder stringBuilder = new StringBuilder();
			while (Source[index] != c)
			{
				if (Source[index] == '\\')
				{
					stringBuilder.Append(Source, num, index - num);
					index++;
					if (index >= SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON string.", index);
					}
					switch (Source[index])
					{
					case 'b':
						stringBuilder.Append('\b');
						break;
					case 'f':
						stringBuilder.Append('\f');
						break;
					case 'n':
						stringBuilder.Append('\n');
						break;
					case 'r':
						stringBuilder.Append('\r');
						break;
					case 't':
						stringBuilder.Append('\t');
						break;
					case 'u':
					{
						int result;
						if (index + 4 < SourceLength && int.TryParse(Source.Substring(index + 1, 4), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out result))
						{
							stringBuilder.Append(char.ConvertFromUtf32(result));
							index += 4;
						}
						else
						{
							stringBuilder.Append(Source[index]);
						}
						break;
					}
					default:
						stringBuilder.Append(Source[index]);
						break;
					case '0':
						break;
					}
					index++;
					if (index >= SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON string.", index);
					}
					num = index;
				}
				else
				{
					index++;
					if (index >= SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON string.", index);
					}
				}
			}
			stringBuilder.Append(Source, num, index - num);
			index++;
			if (expectedType != null && expectedType != typeof(string))
			{
				return Settings.Coercion.CoerceType(expectedType, stringBuilder.ToString());
			}
			return stringBuilder.ToString();
		}

		private object ReadNumber(Type expectedType)
		{
			bool flag = false;
			bool flag2 = false;
			int num = index;
			int num2 = 0;
			int result = 0;
			if (Source[index] == '-')
			{
				index++;
				if (index >= SourceLength || !char.IsDigit(Source[index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", index);
				}
			}
			while (index < SourceLength && char.IsDigit(Source[index]))
			{
				index++;
			}
			if (index < SourceLength && Source[index] == '.')
			{
				flag = true;
				index++;
				if (index >= SourceLength || !char.IsDigit(Source[index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", index);
				}
				while (index < SourceLength && char.IsDigit(Source[index]))
				{
					index++;
				}
			}
			num2 = index - num - (flag ? 1 : 0);
			if (index < SourceLength && (Source[index] == 'e' || Source[index] == 'E'))
			{
				flag2 = true;
				index++;
				if (index >= SourceLength)
				{
					throw new JsonDeserializationException("Illegal JSON number.", index);
				}
				int num3 = index;
				if (Source[index] == '-' || Source[index] == '+')
				{
					index++;
					if (index >= SourceLength || !char.IsDigit(Source[index]))
					{
						throw new JsonDeserializationException("Illegal JSON number.", index);
					}
				}
				else if (!char.IsDigit(Source[index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", index);
				}
				while (index < SourceLength && char.IsDigit(Source[index]))
				{
					index++;
				}
				int.TryParse(Source.Substring(num3, index - num3), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result);
			}
			string s = Source.Substring(num, index - num);
			if (!flag && !flag2 && num2 < 19)
			{
				decimal num4 = decimal.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
				if (expectedType != null)
				{
					return Settings.Coercion.CoerceType(expectedType, num4);
				}
				if (num4 >= -2147483648m && num4 <= 2147483647m)
				{
					return (int)num4;
				}
				if (num4 >= -9223372036854775808m && num4 <= 9223372036854775807m)
				{
					return (long)num4;
				}
				return num4;
			}
			if (expectedType == typeof(decimal))
			{
				return decimal.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
			double num5 = double.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			if (expectedType != null)
			{
				return Settings.Coercion.CoerceType(expectedType, num5);
			}
			return num5;
		}

		public static object Deserialize(string value)
		{
			return Deserialize(value, 0, null);
		}

		public static T Deserialize<T>(string value)
		{
			return (T)Deserialize(value, 0, typeof(T));
		}

		public static object Deserialize(string value, int start)
		{
			return Deserialize(value, start, null);
		}

		public static T Deserialize<T>(string value, int start)
		{
			return (T)Deserialize(value, start, typeof(T));
		}

		public static object Deserialize(string value, Type type)
		{
			return Deserialize(value, 0, type);
		}

		public static object Deserialize(string value, int start, Type type)
		{
			return new JsonReader(value).Deserialize(start, type);
		}

		private JsonToken Tokenize()
		{
			return Tokenize(false);
		}

		private JsonToken Tokenize(bool allowUnquotedString)
		{
			if (index >= SourceLength)
			{
				return JsonToken.End;
			}
			while (char.IsWhiteSpace(Source[index]))
			{
				index++;
				if (index >= SourceLength)
				{
					return JsonToken.End;
				}
			}
			if (Source[index] == "/*"[0])
			{
				if (index + 1 >= SourceLength)
				{
					throw new JsonDeserializationException("Illegal JSON sequence.", index);
				}
				index++;
				bool flag = false;
				if (Source[index] == "/*"[1])
				{
					flag = true;
				}
				else if (Source[index] != "//"[1])
				{
					throw new JsonDeserializationException("Illegal JSON sequence.", index);
				}
				index++;
				if (flag)
				{
					int num = index - 2;
					if (index + 1 >= SourceLength)
					{
						throw new JsonDeserializationException("Unterminated comment block.", num);
					}
					while (Source[index] != "*/"[0] || Source[index + 1] != "*/"[1])
					{
						index++;
						if (index + 1 >= SourceLength)
						{
							throw new JsonDeserializationException("Unterminated comment block.", num);
						}
					}
					index += 2;
					if (index >= SourceLength)
					{
						return JsonToken.End;
					}
				}
				else
				{
					while ("\r\n".IndexOf(Source[index]) < 0)
					{
						index++;
						if (index >= SourceLength)
						{
							return JsonToken.End;
						}
					}
				}
				while (char.IsWhiteSpace(Source[index]))
				{
					index++;
					if (index >= SourceLength)
					{
						return JsonToken.End;
					}
				}
			}
			if (Source[index] == '+')
			{
				index++;
				if (index >= SourceLength)
				{
					return JsonToken.End;
				}
			}
			switch (Source[index])
			{
			case '[':
				return JsonToken.ArrayStart;
			case ']':
				return JsonToken.ArrayEnd;
			case '{':
				return JsonToken.ObjectStart;
			case '}':
				return JsonToken.ObjectEnd;
			case '"':
			case '\'':
				return JsonToken.String;
			case ',':
				return JsonToken.ValueDelim;
			case ':':
				return JsonToken.NameDelim;
			default:
				if (char.IsDigit(Source[index]) || (Source[index] == '-' && index + 1 < SourceLength && char.IsDigit(Source[index + 1])))
				{
					return JsonToken.Number;
				}
				if (MatchLiteral("false"))
				{
					return JsonToken.False;
				}
				if (MatchLiteral("true"))
				{
					return JsonToken.True;
				}
				if (MatchLiteral("null"))
				{
					return JsonToken.Null;
				}
				if (MatchLiteral("NaN"))
				{
					return JsonToken.NaN;
				}
				if (MatchLiteral("Infinity"))
				{
					return JsonToken.PositiveInfinity;
				}
				if (MatchLiteral("-Infinity"))
				{
					return JsonToken.NegativeInfinity;
				}
				if (MatchLiteral("undefined"))
				{
					return JsonToken.Undefined;
				}
				if (allowUnquotedString)
				{
					return JsonToken.UnquotedName;
				}
				throw new JsonDeserializationException("Illegal JSON sequence.", index);
			}
		}

		private bool MatchLiteral(string literal)
		{
			int length = literal.Length;
			int num = 0;
			int num2 = index;
			while (num < length && num2 < SourceLength)
			{
				if (literal[num] != Source[num2])
				{
					return false;
				}
				num++;
				num2++;
			}
			return true;
		}

		public static T CoerceType<T>(object value, T typeToMatch)
		{
			return (T)new TypeCoercionUtility().CoerceType(typeof(T), value);
		}

		public static T CoerceType<T>(object value)
		{
			return (T)new TypeCoercionUtility().CoerceType(typeof(T), value);
		}

		public static object CoerceType(Type targetType, object value)
		{
			return new TypeCoercionUtility().CoerceType(targetType, value);
		}
	}
}
