using System;

namespace JsonFx.Json
{
	public class EcmaScriptIdentifier : IJsonSerializable
	{
		private readonly string identifier;

		public string Identifier
		{
			get
			{
				return identifier;
			}
		}

		public EcmaScriptIdentifier()
			: this(null)
		{
		}

		public EcmaScriptIdentifier(string ident)
		{
			identifier = ((!string.IsNullOrEmpty(ident)) ? EnsureValidIdentifier(ident, true) : string.Empty);
		}

		void IJsonSerializable.ReadJson(JsonReader reader)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		void IJsonSerializable.WriteJson(JsonWriter writer)
		{
			if (string.IsNullOrEmpty(identifier))
			{
				writer.TextWriter.Write("null");
			}
			else
			{
				writer.TextWriter.Write(identifier);
			}
		}

		public static string EnsureValidIdentifier(string varExpr, bool nested)
		{
			return EnsureValidIdentifier(varExpr, nested, true);
		}

		public static string EnsureValidIdentifier(string varExpr, bool nested, bool throwOnEmpty)
		{
			if (string.IsNullOrEmpty(varExpr))
			{
				if (throwOnEmpty)
				{
					throw new ArgumentException("Variable expression is empty.");
				}
				return string.Empty;
			}
			varExpr = varExpr.Replace(" ", string.Empty);
			if (!IsValidIdentifier(varExpr, nested))
			{
				throw new ArgumentException("Variable expression \"" + varExpr + "\" is not supported.");
			}
			return varExpr;
		}

		public static bool IsValidIdentifier(string varExpr, bool nested)
		{
			if (string.IsNullOrEmpty(varExpr))
			{
				return false;
			}
			if (nested)
			{
				string[] array = varExpr.Split('.');
				string[] array2 = array;
				foreach (string varExpr2 in array2)
				{
					if (!IsValidIdentifier(varExpr2, false))
					{
						return false;
					}
				}
				return true;
			}
			if (IsReservedWord(varExpr))
			{
				return false;
			}
			bool flag = false;
			foreach (char c in varExpr)
			{
				if (!flag || !char.IsDigit(c))
				{
					if (!char.IsLetter(c) && c != '_' && c != '$')
					{
						return false;
					}
					flag = true;
				}
			}
			return true;
		}

		private static bool IsReservedWord(string varExpr)
		{
			switch (varExpr)
			{
			case "null":
			case "false":
			case "true":
			case "break":
			case "case":
			case "catch":
			case "continue":
			case "debugger":
			case "default":
			case "delete":
			case "do":
			case "else":
			case "finally":
			case "for":
			case "function":
			case "if":
			case "in":
			case "instanceof":
			case "new":
			case "return":
			case "switch":
			case "this":
			case "throw":
			case "try":
			case "typeof":
			case "var":
			case "void":
			case "while":
			case "with":
			case "abstract":
			case "boolean":
			case "byte":
			case "char":
			case "class":
			case "const":
			case "double":
			case "enum":
			case "export":
			case "extends":
			case "final":
			case "float":
			case "goto":
			case "implements":
			case "import":
			case "int":
			case "interface":
			case "long":
			case "native":
			case "package":
			case "private":
			case "protected":
			case "public":
			case "short":
			case "static":
			case "super":
			case "synchronized":
			case "throws":
			case "transient":
			case "volatile":
			case "let":
			case "yield":
				return true;
			default:
				return false;
			}
		}

		public static EcmaScriptIdentifier Parse(string value)
		{
			return new EcmaScriptIdentifier(value);
		}

		public override bool Equals(object obj)
		{
			EcmaScriptIdentifier ecmaScriptIdentifier = obj as EcmaScriptIdentifier;
			if (ecmaScriptIdentifier == null)
			{
				return base.Equals(obj);
			}
			if (string.IsNullOrEmpty(identifier) && string.IsNullOrEmpty(ecmaScriptIdentifier.identifier))
			{
				return true;
			}
			return StringComparer.Ordinal.Equals(identifier, ecmaScriptIdentifier.identifier);
		}

		public override string ToString()
		{
			return identifier;
		}

		public override int GetHashCode()
		{
			if (identifier == null)
			{
				return 0;
			}
			return identifier.GetHashCode();
		}

		public static implicit operator string(EcmaScriptIdentifier ident)
		{
			if (ident == null)
			{
				return string.Empty;
			}
			return ident.identifier;
		}

		public static implicit operator EcmaScriptIdentifier(string ident)
		{
			return new EcmaScriptIdentifier(ident);
		}
	}
}
