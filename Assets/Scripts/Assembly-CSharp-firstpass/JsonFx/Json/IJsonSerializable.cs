// Assembly-CSharp-firstpass, Version=1.4.1003.3007, Culture=neutral, PublicKeyToken=null
// JsonFx.Json.IJsonSerializable

namespace JsonFx.Json
{
    public interface IJsonSerializable
    {
        void ReadJson(JsonReader reader);

        void WriteJson(JsonWriter writer);
    }
}
