using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonFx.Json
{
	public class EcmaScriptWriter : JsonWriter
	{
		private const string EcmaScriptDateCtor1 = "new Date({0})";

		private const string EcmaScriptDateCtor7 = "new Date({0:0000},{1},{2},{3},{4},{5},{6})";

		private const string EmptyRegExpLiteral = "(?:)";

		private const char RegExpLiteralDelim = '/';

		private const char OperatorCharEscape = '\\';

		private const string NamespaceDelim = ".";

		private const string RootDeclarationDebug = "\r\n/* namespace {1} */\r\nvar {0};";

		private const string RootDeclaration = "var {0};";

		private const string NamespaceCheck = "if(\"undefined\"===typeof {0}){{{0}={{}};}}";

		private const string NamespaceCheckDebug = "\r\nif (\"undefined\" === typeof {0}) {{\r\n\t{0} = {{}};\r\n}}";

		private static readonly DateTime EcmaScriptEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private static readonly char[] NamespaceDelims = new char[1] { '.' };

		private static readonly IList<string> BrowserObjects = new List<string>(new string[10] { "console", "document", "event", "frames", "history", "location", "navigator", "opera", "screen", "window" });

		public EcmaScriptWriter(TextWriter output)
			: base(output)
		{
		}

		public EcmaScriptWriter(Stream output)
			: base(output)
		{
		}

		public EcmaScriptWriter(string outputFileName)
			: base(outputFileName)
		{
		}

		public EcmaScriptWriter(StringBuilder output)
			: base(output)
		{
		}

		public new static string Serialize(object value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (EcmaScriptWriter ecmaScriptWriter = new EcmaScriptWriter(stringBuilder))
			{
				ecmaScriptWriter.Write(value);
			}
			return stringBuilder.ToString();
		}

		public static bool WriteNamespaceDeclaration(TextWriter writer, string ident, List<string> namespaces, bool isDebug)
		{
			if (string.IsNullOrEmpty(ident))
			{
				return false;
			}
			if (namespaces == null)
			{
				namespaces = new List<string>();
			}
			string[] array = ident.Split(NamespaceDelims, StringSplitOptions.RemoveEmptyEntries);
			string text = array[0];
			bool flag = false;
			for (int i = 0; i < array.Length - 1; i++)
			{
				flag = true;
				if (i > 0)
				{
					text += ".";
					text += array[i];
				}
				if (namespaces.Contains(text) || BrowserObjects.Contains(text))
				{
					continue;
				}
				namespaces.Add(text);
				if (i == 0)
				{
					if (isDebug)
					{
						writer.Write("\r\n/* namespace {1} */\r\nvar {0};", text, string.Join(".", array, 0, array.Length - 1));
					}
					else
					{
						writer.Write("var {0};", text);
					}
				}
				if (isDebug)
				{
					writer.WriteLine("\r\nif (\"undefined\" === typeof {0}) {{\r\n\t{0} = {{}};\r\n}}", text);
				}
				else
				{
					writer.Write("if(\"undefined\"===typeof {0}){{{0}={{}};}}", text);
				}
			}
			if (isDebug && flag)
			{
				writer.WriteLine();
			}
			return flag;
		}

		public override void Write(DateTime value)
		{
			WriteEcmaScriptDate(this, value);
		}

		public override void Write(float value)
		{
			base.TextWriter.Write(value.ToString("r"));
		}

		public override void Write(double value)
		{
			base.TextWriter.Write(value.ToString("r"));
		}

		protected override void Write(object value, bool isProperty)
		{
			if (value is Regex)
			{
				if (isProperty && base.Settings.PrettyPrint)
				{
					base.TextWriter.Write(' ');
				}
				WriteEcmaScriptRegExp(this, (Regex)value);
			}
			else
			{
				base.Write(value, isProperty);
			}
		}

		protected override void WriteObjectPropertyName(string name)
		{
			if (EcmaScriptIdentifier.IsValidIdentifier(name, false))
			{
				base.TextWriter.Write(name);
			}
			else
			{
				base.WriteObjectPropertyName(name);
			}
		}

		public static void WriteEcmaScriptDate(JsonWriter writer, DateTime value)
		{
			if (value.Kind == DateTimeKind.Unspecified)
			{
				writer.TextWriter.Write("new Date({0:0000},{1},{2},{3},{4},{5},{6})", value.Year, value.Month - 1, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
			}
			else
			{
				if (value.Kind == DateTimeKind.Local)
				{
					value = value.ToUniversalTime();
				}
				long num = (long)value.Subtract(EcmaScriptEpoch).TotalMilliseconds;
				writer.TextWriter.Write("new Date({0})", num);
			}
		}

		public static void WriteEcmaScriptRegExp(JsonWriter writer, Regex regex)
		{
			WriteEcmaScriptRegExp(writer, regex, false);
		}

		public static void WriteEcmaScriptRegExp(JsonWriter writer, Regex regex, bool isGlobal)
		{
			if (regex == null)
			{
				writer.TextWriter.Write("null");
				return;
			}
			string text = regex.ToString();
			if (string.IsNullOrEmpty(text))
			{
				text = "(?:)";
			}
			string text2 = ((!isGlobal) ? string.Empty : "g");
			switch (regex.Options & (RegexOptions.IgnoreCase | RegexOptions.Multiline))
			{
			case RegexOptions.IgnoreCase:
				text2 += "i";
				break;
			case RegexOptions.Multiline:
				text2 += "m";
				break;
			case RegexOptions.IgnoreCase | RegexOptions.Multiline:
				text2 += "im";
				break;
			}
			writer.TextWriter.Write('/');
			int length = text.Length;
			int num = 0;
			for (int i = num; i < length; i++)
			{
				char c = text[i];
				if (c == '/')
				{
					writer.TextWriter.Write(text.Substring(num, i - num));
					num = i + 1;
					writer.TextWriter.Write('\\');
					writer.TextWriter.Write(text[i]);
				}
			}
			writer.TextWriter.Write(text.Substring(num, length - num));
			writer.TextWriter.Write('/');
			writer.TextWriter.Write(text2);
		}
	}
}
