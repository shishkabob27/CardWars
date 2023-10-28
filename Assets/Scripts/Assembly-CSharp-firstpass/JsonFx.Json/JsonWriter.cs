using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace JsonFx.Json
{
	public class JsonWriter : IDisposable
	{
		public const string JsonMimeType = "application/json";

		public const string JsonFileExtension = ".json";

		private const string AnonymousTypePrefix = "<>f__AnonymousType";

		private const string ErrorMaxDepth = "The maxiumum depth of {0} was exceeded. Check for cycles in object graph.";

		private const string ErrorIDictionaryEnumerator = "Types which implement Generic IDictionary<TKey, TValue> must have an IEnumerator which implements IDictionaryEnumerator. ({0})";

		private readonly TextWriter Writer;

		private JsonWriterSettings settings;

		private int depth;

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public string TypeHintName
		{
			get
			{
				return settings.TypeHintName;
			}
			set
			{
				settings.TypeHintName = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public bool PrettyPrint
		{
			get
			{
				return settings.PrettyPrint;
			}
			set
			{
				settings.PrettyPrint = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public string Tab
		{
			get
			{
				return settings.Tab;
			}
			set
			{
				settings.Tab = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public string NewLine
		{
			get
			{
				return settings.NewLine;
			}
			set
			{
				TextWriter writer = Writer;
				settings.NewLine = value;
				writer.NewLine = value;
			}
		}

		protected int Depth
		{
			get
			{
				return depth;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public int MaxDepth
		{
			get
			{
				return settings.MaxDepth;
			}
			set
			{
				settings.MaxDepth = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public bool UseXmlSerializationAttributes
		{
			get
			{
				return settings.UseXmlSerializationAttributes;
			}
			set
			{
				settings.UseXmlSerializationAttributes = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonWriterSettings object")]
		public WriteDelegate<DateTime> DateTimeSerializer
		{
			get
			{
				return settings.DateTimeSerializer;
			}
			set
			{
				settings.DateTimeSerializer = value;
			}
		}

		public TextWriter TextWriter
		{
			get
			{
				return Writer;
			}
		}

		public JsonWriterSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				if (value == null)
				{
					value = new JsonWriterSettings();
				}
				settings = value;
			}
		}

		public JsonWriter(TextWriter output)
			: this(output, new JsonWriterSettings())
		{
		}

		public JsonWriter(TextWriter output, JsonWriterSettings settings)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Writer = output;
			this.settings = settings;
			Writer.NewLine = this.settings.NewLine;
		}

		public JsonWriter(Stream output)
			: this(output, new JsonWriterSettings())
		{
		}

		public JsonWriter(Stream output, JsonWriterSettings settings)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Writer = new StreamWriter(output, Encoding.UTF8);
			this.settings = settings;
			Writer.NewLine = this.settings.NewLine;
		}

		public JsonWriter(string outputFileName)
			: this(outputFileName, new JsonWriterSettings())
		{
		}

		public JsonWriter(string outputFileName, JsonWriterSettings settings)
		{
			if (outputFileName == null)
			{
				throw new ArgumentNullException("outputFileName");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Stream stream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
			Writer = new StreamWriter(stream, Encoding.UTF8);
			this.settings = settings;
			Writer.NewLine = this.settings.NewLine;
		}

		public JsonWriter(StringBuilder output)
			: this(output, new JsonWriterSettings())
		{
		}

		public JsonWriter(StringBuilder output, JsonWriterSettings settings)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Writer = new StringWriter(output, CultureInfo.InvariantCulture);
			this.settings = settings;
			Writer.NewLine = this.settings.NewLine;
		}

		void IDisposable.Dispose()
		{
			if (Writer != null)
			{
				Writer.Dispose();
			}
		}

		public static string Serialize(object value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (JsonWriter jsonWriter = new JsonWriter(stringBuilder))
			{
				jsonWriter.Write(value);
			}
			return stringBuilder.ToString();
		}

		public void Write(object value)
		{
			Write(value, false);
		}

		protected virtual void Write(object value, bool isProperty)
		{
			if (isProperty && settings.PrettyPrint)
			{
				Writer.Write(' ');
			}
			if (value == null)
			{
				Writer.Write("null");
				return;
			}
			if (value is IJsonSerializable)
			{
				try
				{
					if (isProperty)
					{
						depth++;
						if (depth > settings.MaxDepth)
						{
							throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
						}
						WriteLine();
					}
					((IJsonSerializable)value).WriteJson(this);
					return;
				}
				finally
				{
					if (isProperty)
					{
						depth--;
					}
				}
			}
			if (value is Enum)
			{
				Write((Enum)value);
				return;
			}
			Type type = value.GetType();
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				Write((bool)value);
				return;
			case TypeCode.Byte:
				Write((byte)value);
				return;
			case TypeCode.Char:
				Write((char)value);
				return;
			case TypeCode.DateTime:
				Write((DateTime)value);
				return;
			case TypeCode.Empty:
			case TypeCode.DBNull:
				Writer.Write("null");
				return;
			case TypeCode.Decimal:
				Write((decimal)value);
				return;
			case TypeCode.Double:
				Write((double)value);
				return;
			case TypeCode.Int16:
				Write((short)value);
				return;
			case TypeCode.Int32:
				Write((int)value);
				return;
			case TypeCode.Int64:
				Write((long)value);
				return;
			case TypeCode.SByte:
				Write((sbyte)value);
				return;
			case TypeCode.Single:
				Write((float)value);
				return;
			case TypeCode.String:
				Write((string)value);
				return;
			case TypeCode.UInt16:
				Write((ushort)value);
				return;
			case TypeCode.UInt32:
				Write((uint)value);
				return;
			case TypeCode.UInt64:
				Write((ulong)value);
				return;
			}
			if (value is Guid)
			{
				Write((Guid)value);
				return;
			}
			if (value is Uri)
			{
				Write((Uri)value);
				return;
			}
			if (value is TimeSpan)
			{
				Write((TimeSpan)value);
				return;
			}
			if (value is Version)
			{
				Write((Version)value);
				return;
			}
			if (value is IDictionary)
			{
				try
				{
					if (isProperty)
					{
						depth++;
						if (depth > settings.MaxDepth)
						{
							throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
						}
						WriteLine();
					}
					WriteObject((IDictionary)value);
					return;
				}
				finally
				{
					if (isProperty)
					{
						depth--;
					}
				}
			}
			if (type.GetInterface("System.Collections.Generic.IDictionary`2") != null)
			{
				try
				{
					if (isProperty)
					{
						depth++;
						if (depth > settings.MaxDepth)
						{
							throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
						}
						WriteLine();
					}
					WriteDictionary((IEnumerable)value);
					return;
				}
				finally
				{
					if (isProperty)
					{
						depth--;
					}
				}
			}
			if (value is IEnumerable)
			{
				if (value is XmlNode)
				{
					Write((XmlNode)value);
					return;
				}
				try
				{
					if (isProperty)
					{
						depth++;
						if (depth > settings.MaxDepth)
						{
							throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
						}
						WriteLine();
					}
					WriteArray((IEnumerable)value);
					return;
				}
				finally
				{
					if (isProperty)
					{
						depth--;
					}
				}
			}
			try
			{
				if (isProperty)
				{
					depth++;
					if (depth > settings.MaxDepth)
					{
						throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
					}
					WriteLine();
				}
				WriteObject(value, type);
			}
			finally
			{
				if (isProperty)
				{
					depth--;
				}
			}
		}

		public virtual void WriteBase64(byte[] value)
		{
			Write(Convert.ToBase64String(value));
		}

		public virtual void WriteHexString(byte[] value)
		{
			if (value == null || value.Length == 0)
			{
				Write(string.Empty);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				stringBuilder.Append(value[i].ToString("x2"));
			}
			Write(stringBuilder.ToString());
		}

		public virtual void Write(DateTime value)
		{
			if (settings.DateTimeSerializer != null)
			{
				settings.DateTimeSerializer(this, value);
				return;
			}
			DateTimeKind kind = value.Kind;
			if (kind != DateTimeKind.Utc)
			{
				if (kind != DateTimeKind.Local)
				{
					Write(string.Format("{0:s}", value));
					return;
				}
				value = value.ToUniversalTime();
			}
			Write(string.Format("{0:s}Z", value));
		}

		public virtual void Write(Guid value)
		{
			Write(value.ToString("D"));
		}

		public virtual void Write(Enum value)
		{
			string text = null;
			Type type = value.GetType();
			if (type.IsDefined(typeof(FlagsAttribute), true) && !Enum.IsDefined(type, value))
			{
				Enum[] flagList = GetFlagList(type, value);
				string[] array = new string[flagList.Length];
				for (int i = 0; i < flagList.Length; i++)
				{
					array[i] = JsonNameAttribute.GetJsonName(flagList[i]);
					if (string.IsNullOrEmpty(array[i]))
					{
						array[i] = flagList[i].ToString("f");
					}
				}
				text = string.Join(", ", array);
			}
			else
			{
				text = JsonNameAttribute.GetJsonName(value);
				if (string.IsNullOrEmpty(text))
				{
					text = value.ToString("f");
				}
			}
			Write(text);
		}

		public virtual void Write(string value)
		{
			if (value == null)
			{
				Writer.Write("null");
				return;
			}
			int num = 0;
			int length = value.Length;
			Writer.Write('"');
			for (int i = num; i < length; i++)
			{
				char c = value[i];
				if (c <= '\u001f' || c >= '\u007f' || c == '<' || c == '"' || c == '\\')
				{
					if (i > num)
					{
						Writer.Write(value.Substring(num, i - num));
					}
					num = i + 1;
					switch (c)
					{
					case '"':
					case '\\':
						Writer.Write('\\');
						Writer.Write(c);
						break;
					case '\b':
						Writer.Write("\\b");
						break;
					case '\f':
						Writer.Write("\\f");
						break;
					case '\n':
						Writer.Write("\\n");
						break;
					case '\r':
						Writer.Write("\\r");
						break;
					case '\t':
						Writer.Write("\\t");
						break;
					default:
						Writer.Write("\\u");
						Writer.Write(char.ConvertToUtf32(value, i).ToString("X4"));
						break;
					}
				}
			}
			if (length > num)
			{
				Writer.Write(value.Substring(num, length - num));
			}
			Writer.Write('"');
		}

		public virtual void Write(bool value)
		{
			Writer.Write((!value) ? "false" : "true");
		}

		public virtual void Write(byte value)
		{
			Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(sbyte value)
		{
			Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(short value)
		{
			Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(ushort value)
		{
			Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(int value)
		{
			Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(uint value)
		{
			if (InvalidIeee754(value))
			{
				Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
			else
			{
				Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(long value)
		{
			if (InvalidIeee754(value))
			{
				Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
			else
			{
				Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(ulong value)
		{
			if (InvalidIeee754(value))
			{
				Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
			else
			{
				Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				Writer.Write("null");
			}
			else
			{
				Writer.Write(value.ToString("r", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(double value)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
			{
				Writer.Write("null");
			}
			else
			{
				Writer.Write(value.ToString("r", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(decimal value)
		{
			if (InvalidIeee754(value))
			{
				Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
			else
			{
				Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(char value)
		{
			Write(new string(value, 1));
		}

		public virtual void Write(TimeSpan value)
		{
			Write(value.Ticks);
		}

		public virtual void Write(Uri value)
		{
			Write(value.ToString());
		}

		public virtual void Write(Version value)
		{
			Write(value.ToString());
		}

		public virtual void Write(XmlNode value)
		{
			Write(value.OuterXml);
		}

		protected internal virtual void WriteArray(IEnumerable value)
		{
			bool flag = false;
			Writer.Write('[');
			depth++;
			if (depth > settings.MaxDepth)
			{
				throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
			}
			try
			{
				foreach (object item in value)
				{
					if (flag)
					{
						WriteArrayItemDelim();
					}
					else
					{
						flag = true;
					}
					WriteLine();
					WriteArrayItem(item);
				}
			}
			finally
			{
				depth--;
			}
			if (flag)
			{
				WriteLine();
			}
			Writer.Write(']');
		}

		protected virtual void WriteArrayItem(object item)
		{
			Write(item, false);
		}

		protected virtual void WriteObject(IDictionary value)
		{
			WriteDictionary(value);
		}

		protected virtual void WriteDictionary(IEnumerable value)
		{
			IDictionaryEnumerator dictionaryEnumerator = value.GetEnumerator() as IDictionaryEnumerator;
			if (dictionaryEnumerator == null)
			{
				throw new JsonSerializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> must have an IEnumerator which implements IDictionaryEnumerator. ({0})", value.GetType()));
			}
			bool flag = false;
			Writer.Write('{');
			depth++;
			if (depth > settings.MaxDepth)
			{
				throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
			}
			try
			{
				while (dictionaryEnumerator.MoveNext())
				{
					if (flag)
					{
						WriteObjectPropertyDelim();
					}
					else
					{
						flag = true;
					}
					WriteObjectProperty(Convert.ToString(dictionaryEnumerator.Entry.Key), dictionaryEnumerator.Entry.Value);
				}
			}
			finally
			{
				depth--;
			}
			if (flag)
			{
				WriteLine();
			}
			Writer.Write('}');
		}

		private void WriteObjectProperty(string key, object value)
		{
			WriteLine();
			WriteObjectPropertyName(key);
			Writer.Write(':');
			WriteObjectPropertyValue(value);
		}

		protected virtual void WriteObjectPropertyName(string name)
		{
			Write(name);
		}

		protected virtual void WriteObjectPropertyValue(object value)
		{
			Write(value, true);
		}

		protected virtual void WriteObject(object value, Type type)
		{
			bool flag = false;
			Writer.Write('{');
			depth++;
			if (depth > settings.MaxDepth)
			{
				throw new JsonSerializationException(string.Format("The maxiumum depth of {0} was exceeded. Check for cycles in object graph.", settings.MaxDepth));
			}
			try
			{
				if (!string.IsNullOrEmpty(settings.TypeHintName))
				{
					if (flag)
					{
						WriteObjectPropertyDelim();
					}
					else
					{
						flag = true;
					}
					WriteObjectProperty(settings.TypeHintName, type.FullName + ", " + type.Assembly.GetName().Name);
				}
				bool flag2 = type.IsGenericType && type.Name.StartsWith("<>f__AnonymousType");
				PropertyInfo[] properties = type.GetProperties();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (!propertyInfo.CanRead || (!propertyInfo.CanWrite && !flag2) || IsIgnored(type, propertyInfo, value))
					{
						continue;
					}
					object value2 = propertyInfo.GetValue(value, null);
					if (!IsDefaultValue(propertyInfo, value2))
					{
						if (flag)
						{
							WriteObjectPropertyDelim();
						}
						else
						{
							flag = true;
						}
						string text = JsonNameAttribute.GetJsonName(propertyInfo);
						if (string.IsNullOrEmpty(text))
						{
							text = propertyInfo.Name;
						}
						WriteObjectProperty(text, value2);
					}
				}
				FieldInfo[] fields = type.GetFields();
				FieldInfo[] array2 = fields;
				foreach (FieldInfo fieldInfo in array2)
				{
					if (!fieldInfo.IsPublic || fieldInfo.IsStatic || IsIgnored(type, fieldInfo, value))
					{
						continue;
					}
					object value3 = fieldInfo.GetValue(value);
					if (!IsDefaultValue(fieldInfo, value3))
					{
						if (flag)
						{
							WriteObjectPropertyDelim();
							WriteLine();
						}
						else
						{
							flag = true;
						}
						string text2 = JsonNameAttribute.GetJsonName(fieldInfo);
						if (string.IsNullOrEmpty(text2))
						{
							text2 = fieldInfo.Name;
						}
						WriteObjectProperty(text2, value3);
					}
				}
			}
			finally
			{
				depth--;
			}
			if (flag)
			{
				WriteLine();
			}
			Writer.Write('}');
		}

		protected virtual void WriteArrayItemDelim()
		{
			Writer.Write(',');
		}

		protected virtual void WriteObjectPropertyDelim()
		{
			Writer.Write(',');
		}

		protected virtual void WriteLine()
		{
			if (settings.PrettyPrint)
			{
				Writer.WriteLine();
				for (int i = 0; i < depth; i++)
				{
					Writer.Write(settings.Tab);
				}
			}
		}

		private bool IsIgnored(Type objType, MemberInfo member, object obj)
		{
			if (JsonIgnoreAttribute.IsJsonIgnore(member))
			{
				return true;
			}
			string jsonSpecifiedProperty = JsonSpecifiedPropertyAttribute.GetJsonSpecifiedProperty(member);
			if (!string.IsNullOrEmpty(jsonSpecifiedProperty))
			{
				PropertyInfo property = objType.GetProperty(jsonSpecifiedProperty);
				if (property != null)
				{
					object value = property.GetValue(obj, null);
					if (value is bool && !Convert.ToBoolean(value))
					{
						return true;
					}
				}
			}
			if (settings.UseXmlSerializationAttributes)
			{
				if (JsonIgnoreAttribute.IsXmlIgnore(member))
				{
					return true;
				}
				PropertyInfo property2 = objType.GetProperty(member.Name + "Specified");
				if (property2 != null)
				{
					object value2 = property2.GetValue(obj, null);
					if (value2 is bool && !Convert.ToBoolean(value2))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsDefaultValue(MemberInfo member, object value)
		{
			DefaultValueAttribute defaultValueAttribute = Attribute.GetCustomAttribute(member, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
			if (defaultValueAttribute == null)
			{
				return false;
			}
			if (defaultValueAttribute.Value == null)
			{
				return value == null;
			}
			return defaultValueAttribute.Value.Equals(value);
		}

		private static Enum[] GetFlagList(Type enumType, object value)
		{
			ulong num = Convert.ToUInt64(value);
			Array values = Enum.GetValues(enumType);
			List<Enum> list = new List<Enum>(values.Length);
			if (num == 0L)
			{
				list.Add((Enum)Convert.ChangeType(value, enumType));
				return list.ToArray();
			}
			for (int num2 = values.Length - 1; num2 >= 0; num2--)
			{
				ulong num3 = Convert.ToUInt64(values.GetValue(num2));
				if ((num2 != 0 || num3 != 0L) && (num & num3) == num3)
				{
					num -= num3;
					list.Add(values.GetValue(num2) as Enum);
				}
			}
			if (num != 0L)
			{
				list.Add(Enum.ToObject(enumType, num) as Enum);
			}
			return list.ToArray();
		}

		protected virtual bool InvalidIeee754(decimal value)
		{
			try
			{
				return (decimal)(double)value != value;
			}
			catch
			{
				return true;
			}
		}
	}
}
