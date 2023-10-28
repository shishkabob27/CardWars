using System;
using System.IO;

namespace JsonFx.Json
{
	public class JsonDataReader : IDataReader
	{
		public const string JsonMimeType = "application/json";

		public const string JsonFileExtension = ".json";

		private readonly JsonReaderSettings Settings;

		public string ContentType
		{
			get
			{
				return "application/json";
			}
		}

		public JsonDataReader(JsonReaderSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Settings = settings;
		}

		public object Deserialize(TextReader input, Type type)
		{
			return new JsonReader(input, Settings).Deserialize(type);
		}

		public static JsonReaderSettings CreateSettings(bool allowNullValueTypes)
		{
			JsonReaderSettings jsonReaderSettings = new JsonReaderSettings();
			jsonReaderSettings.AllowNullValueTypes = allowNullValueTypes;
			return jsonReaderSettings;
		}
	}
}
