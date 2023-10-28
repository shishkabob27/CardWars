using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Range
{
	private static readonly char[] Separators = new char[1] { ',' };

	public static List<int> Explode(int from, int to)
	{
		return Enumerable.Range(from, to - from + 1).ToList();
	}

	public static List<int> Interpret(string input)
	{
		List<int> list = new List<int>();
		string[] array = input.Split(Separators);
		string pattern = "(?<range>(?<from>\\d+)-(?<to>\\d+))";
		Regex regex = new Regex(pattern);
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.Trim();
			Match match = regex.Match(text2);
			if (match.Success)
			{
				int from = Parse(match.Groups["from"].Value);
				int to = Parse(match.Groups["to"].Value);
				list.AddRange(Explode(from, to));
			}
			else
			{
				list.Add(Parse(text2));
			}
		}
		return list;
	}

	private static int Parse(string value)
	{
		int result;
		if (!int.TryParse(value, out result))
		{
			throw new FormatException(string.Format("Failed to parse '{0}' as an integer", value));
		}
		return result;
	}
}
