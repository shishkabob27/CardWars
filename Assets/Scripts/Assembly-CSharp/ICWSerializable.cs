public interface ICWSerializable
{
	string Serialize();

	void Deserialize(object json);
}
