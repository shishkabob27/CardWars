using System;
using System.Collections.Generic;
using System.Text;

namespace Prime31
{
	public class JsonFormatter
	{
		private enum JsonContextType
		{
			Object,
			Array
		}

		private const int defaultIndent = 0;

		private const string indent = "\t";

		private const string space = " ";

		private bool inDoubleString = false;

		private bool inSingleString = false;

		private bool inVariableAssignment = false;

		private char prevChar = '\0';

		private Stack<JsonContextType> context = new Stack<JsonContextType>();

		public static string prettyPrint(string input)
		{
			try
			{
				return new JsonFormatter().print(input);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static void buildIndents(int indents, StringBuilder output)
		{
			for (indents = indents; indents > 0; indents--)
			{
				output.Append("\t");
			}
		}

		private bool inString()
		{
			return inDoubleString || inSingleString;
		}

		public string print(string input)
		{
			StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
			foreach (char c in input)
			{
				switch (c)
				{
				case '{':
					if (!inString())
					{
						if (inVariableAssignment || (context.Count > 0 && context.Peek() != JsonContextType.Array))
						{
							stringBuilder.Append(Environment.NewLine);
							buildIndents(context.Count, stringBuilder);
						}
						stringBuilder.Append(c);
						context.Push(JsonContextType.Object);
						stringBuilder.Append(Environment.NewLine);
						buildIndents(context.Count, stringBuilder);
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '}':
					if (!inString())
					{
						stringBuilder.Append(Environment.NewLine);
						context.Pop();
						buildIndents(context.Count, stringBuilder);
						stringBuilder.Append(c);
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '[':
					stringBuilder.Append(c);
					if (!inString())
					{
						context.Push(JsonContextType.Array);
					}
					break;
				case ']':
					if (!inString())
					{
						stringBuilder.Append(c);
						context.Pop();
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '=':
					stringBuilder.Append(c);
					break;
				case ',':
					stringBuilder.Append(c);
					if (!inString())
					{
						stringBuilder.Append(" ");
					}
					if (!inString() && context.Peek() != JsonContextType.Array)
					{
						buildIndents(context.Count, stringBuilder);
						stringBuilder.Append(Environment.NewLine);
						buildIndents(context.Count, stringBuilder);
						inVariableAssignment = false;
					}
					break;
				case '\'':
					if (!inDoubleString && prevChar != '\\')
					{
						inSingleString = !inSingleString;
					}
					stringBuilder.Append(c);
					break;
				case ':':
					if (!inString())
					{
						inVariableAssignment = true;
						stringBuilder.Append(c);
						stringBuilder.Append(" ");
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '"':
					if (!inSingleString && prevChar != '\\')
					{
						inDoubleString = !inDoubleString;
					}
					stringBuilder.Append(c);
					break;
				case ' ':
					if (inString())
					{
						stringBuilder.Append(c);
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				prevChar = c;
			}
			return stringBuilder.ToString();
		}
	}
}
