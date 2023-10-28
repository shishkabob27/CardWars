namespace JsonFx.Json
{
	public interface IJsonSerializable
	{
		void ReadJson(JsonReader reader);

		void WriteJson(JsonWriter writer);
	}
}
