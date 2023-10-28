using System;
using UnityEngine;

namespace Prime31
{
	public static class Utils
	{
		private static System.Random _random;

		public static void logObject(object obj)
		{
			string json = Json.encode(obj);
			prettyPrintJson(json);
		}

		public static void prettyPrintJson(string json)
		{
			string text = string.Empty;
			if (json != null)
			{
				text = JsonFormatter.prettyPrint(json);
			}
			try
			{
				Debug.Log(text);
			}
			catch (Exception)
			{
				Console.WriteLine(text);
			}
		}
	}
}
